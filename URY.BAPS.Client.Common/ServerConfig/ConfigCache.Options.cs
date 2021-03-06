using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using URY.BAPS.Common.Model.ServerConfig;

namespace URY.BAPS.Client.Common.ServerConfig
{
    // Internal classes for storing options in the cache.

    public partial class ConfigCache
    {
        /// <summary>
        ///     Non-parametric interface onto options.
        /// </summary>
        private interface IOption
        {
            string Description { get; }
            uint OptionId { get; }
            ConfigType Type { get; }
        }

        /// <inheritdoc />
        /// <summary>
        ///     Base class for config option that can store one or more values.
        /// </summary>
        /// <typeparam name="T">The basic type of the values.</typeparam>
        private abstract class Option<T> : IOption
        {
            [NotNull] private readonly Dictionary<int, T> _values = new Dictionary<int, T>();

            /// <summary>
            ///     The cache whose event handlers should be invoked whenever
            ///     this option changes value.
            /// </summary>
            protected readonly ConfigCache? Parent;

            protected Option(ConfigCache? parent, uint optionId, string? description,
                bool isIndexed)
            {
                Parent = parent;
                OptionId = optionId;
                Description = description ?? throw new ArgumentNullException(nameof(description));
                IsIndexed = isIndexed;
            }

            private bool IsIndexed { get; }

            /// <summary>
            ///     Gets the known BapsNet key of this setting.
            ///     <para>
            ///         If the setting's option ID isn't recognised, this returns
            ///         <see cref="OptionKey.Invalid" />.
            ///     </para>
            /// </summary>
            protected OptionKey Key =>
                Enum.IsDefined(typeof(OptionKey), OptionId) ? (OptionKey) OptionId : OptionKey.Invalid;

            public uint OptionId { get; }
            [NotNull] public string Description { get; }

            public abstract ConfigType Type { get; }

            public T ValueAt(int index, T ifNotCached)
            {
                return _values.TryGetValue(index, out var x) ? x : ifNotCached;
            }

            /// <summary>
            ///     Hook for validating options before storing them.
            /// </summary>
            /// <param name="value">
            ///     The value to validate; validation should raise exceptions if this value is invalid.
            /// </param>
            protected abstract void ValidateValue(T value);

            /// <summary>
            ///     Hook for sending a change event through <see cref="Parent" />.
            /// </summary>
            /// <param name="index">
            ///     The index, if any, that has changed.
            /// </param>
            protected abstract void SendChangeEvent(int index);

            public void AddValue(T value, int index = NoIndex)
            {
                if (IsIndexed && index == NoIndex)
                    throw new ArgumentOutOfRangeException(
                        nameof(index), index,
                        "Can't set a non-indexed value: this option is indexed.");

                if (!IsIndexed && index != NoIndex)
                    throw new ArgumentOutOfRangeException(
                        nameof(index), index,
                        "Can't set an indexed value: this option is not indexed.");

                ValidateValue(value);
                _values[index] = value;
                SendChangeEvent(index);
            }
        }

        /// <summary>
        ///     Describes an option where values are strings.
        /// </summary>
        private class StringOption : Option<string>
        {
            public StringOption(ConfigCache? parent, uint optionId, string? description, bool isIndexed)
                : base(parent, optionId, description, isIndexed)
            {
            }

            public override ConfigType Type => ConfigType.Str;

            protected override void ValidateValue(string value)
            {
                if (value == null) throw new ArgumentNullException(nameof(value), "Can't store a null string");
            }

            protected override void SendChangeEvent(int index)
            {
                Parent?.OnStringChanged(new StringChangeEventArgs(Key, ValueAt(index, ""), index));
            }
        }

        /// <inheritdoc />
        /// <summary>
        ///     Describes an option where values are integers.
        /// </summary>
        private class IntOption : Option<int>
        {
            public IntOption(ConfigCache? parent, uint optionId, string description, bool isIndexed)
                : base(parent, optionId, description, isIndexed)
            {
            }

            public override ConfigType Type => ConfigType.Int;

            protected override void ValidateValue(int value)
            {
                // All integers are valid.
            }

            protected override void SendChangeEvent(int index)
            {
                Parent?.OnIntChanged(new IntChangeEventArgs(Key, ValueAt(index, -1), index));
            }
        }

        /// <inheritdoc />
        /// <summary>
        ///     Describes an option where values are restricted to a specific set of choices.
        /// </summary>
        private class ChoiceOption : Option<int>
        {
            [NotNull] private readonly Dictionary<string, int> _choiceList = new Dictionary<string, int>();

            public ChoiceOption(ConfigCache? parent, uint optionId, string description, bool isIndexed)
                : base(parent, optionId, description, isIndexed)
            {
            }

            public override ConfigType Type => ConfigType.Choice;

            /// <summary>
            ///     Registers a possible choice with this option.
            /// </summary>
            /// <param name="choiceId">The ID of the choice to register.</param>
            /// <param name="description">The string description of the choice to register.</param>
            public void AddChoice(int choiceId, string description)
            {
                _choiceList[description] = choiceId;
            }

            public int ChoiceIndexFor(string? description)
            {
                return description != null && _choiceList.TryGetValue(description, out var v) ? v : NoIndex;
            }

            private string? ChoiceDescriptionFor(int index)
            {
                return _choiceList.FirstOrDefault(pair => index == pair.Value).Key;
            }

            protected override void ValidateValue(int value)
            {
                if (!_choiceList.Values.Contains(value))
                    throw new ArgumentOutOfRangeException(nameof(value), value, "Value not a valid choice ID.");
            }

            /// <summary>
            ///     Tries to get the description of the choice at the given index.
            /// </summary>
            /// <param name="index">The index to query; use <see cref="NoIndex" /> if this option isn't indexed.</param>
            /// <returns>
            ///     The description of the choice at <paramref name="index" />, or null if said index doesn't have a valid choice
            ///     set.
            /// </returns>
            public string? ChoiceAt(int index)
            {
                return ChoiceDescriptionFor(ValueAt(index, -1));
            }

            protected override void SendChangeEvent(int index)
            {
                Parent?.OnChoiceChanged(new ChoiceChangeEventArgs(Key, ChoiceAt(index) ?? "?", index));
            }
        }
    }
}