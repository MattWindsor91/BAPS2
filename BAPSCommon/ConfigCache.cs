using System;
using System.Collections.Generic;
using System.Linq;
using BAPSClientCommon.BapsNet;

namespace BAPSClientCommon
{
    /// <summary>
    ///     Arguments for a config choice change event.
    /// </summary>
    public struct ConfigChoiceChangeArgs
    {
        public string Description;
        public string Choice;
        public int Index;
    }

    public delegate void ConfigChoiceChangeHandler(object sender, ConfigChoiceChangeArgs e);


    /** The config cache is designed as a quick access to config variables,
        You tell it what config item is expected next and then ask for that
        option from the server. It will then associate the result with the
        name you asked for.
    **/
    public class ConfigCache
    {
        /// <summary>
        ///     The (rather paradoxically named) integer that represents a lack of index.
        /// </summary>
        public const int NoIndex = -1;


        private readonly Dictionary<string, IOption> _descLookup = new Dictionary<string, IOption>();
        private readonly Dictionary<uint, IOption> _idLookup = new Dictionary<uint, IOption>();

        /// <summary>
        ///     Event raised when a choice-type config option changes.
        /// </summary>
        public event ConfigChoiceChangeHandler ConfigChoiceChanged;

        private IOption MakeOptionCacheInfo(uint optionId, ConfigType type, string description, bool isIndexed)
        {
            switch (type)
            {
                case ConfigType.Choice:
                    return new ChoiceOption(optionId, description, isIndexed);
                case ConfigType.Int:
                    return new IntOption(optionId, description, isIndexed);
                case ConfigType.Str:
                    return new StringOption(optionId, description, isIndexed);
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, "Unsupported config type");
            }
        }

        /// <summary>
        ///     Creates a BAPSNet message to set an option to one of its choices.
        /// </summary>
        /// <param name="optionDesc">The option to set.</param>
        /// <param name="choiceDesc">The choice to use.</param>
        /// <param name="index">If present and valid, the index of the option to set.</param>
        /// <returns>A message that effects the described config change.</returns>
        public Message MakeConfigChoiceMessage(string optionDesc, string choiceDesc, int index = NoIndex)
        {
            var oci = GetOption(optionDesc);
            if (oci == null) throw new ArgumentOutOfRangeException(nameof(optionDesc), optionDesc, "Unknown option.");

            if (!(oci is ChoiceOption cci))
                throw new ArgumentException("Option doesn't have choices.", nameof(optionDesc));

            var cid = cci.ChoiceIndexFor(choiceDesc);
            if (cid == NoIndex)
                throw new ArgumentOutOfRangeException(nameof(choiceDesc), choiceDesc, "Unknown choice.");

            var cmd = Command.Config | Command.SetConfigValue;
            if (index != NoIndex) cmd |= Command.ConfigUseValueMask | (Command) index;

            return new Message(cmd).Add(oci.OptionId).Add((uint) cci.Type).Add((uint) cid);
        }

        /** add an option **/
        public void AddOptionDescription(uint optionId, ConfigType type, string description, bool isIndexed)
        {
            if (_descLookup.ContainsKey(description)) return;
            var oci = MakeOptionCacheInfo(optionId, type, description, isIndexed);
            _descLookup[description] = oci;
            _idLookup[optionId] = oci;
        }


        public void AddOptionChoice(uint optionId, int choiceId, string description)
        {
            if (!_idLookup.TryGetValue(optionId, out var option)) return;
            (option as ChoiceOption)?.AddChoice(choiceId, description);
        }

        private IOption GetOption(string optionDescription)
        {
            return _descLookup.TryGetValue(optionDescription, out var x) ? x : null;
        }

        public int FindChoiceIndexFor(string optionDesc, string description)
        {
            if (GetOption(optionDesc) is ChoiceOption c) return c.ChoiceIndexFor(description);
            return -1;
        }

        /// <summary>
        ///     Updates the value for a given option.
        ///     <para>
        ///         This is an internal intermediate function only.
        ///     </para>
        /// </summary>
        /// <typeparam name="T">The type of the value (generally string or int).</typeparam>
        /// <param name="option">The internal option object to update.</param>
        /// <param name="value">The new value to apply.</param>
        /// <param name="index">If present and non-negative, the index of the option to set.</param>
        private void SetValue<T>(IOption option, T value, int index = -1)
        {
            if (option is Option<T> o) o.AddValue(value, index);
            if (!(option is ChoiceOption c)) return;
            var e = new ConfigChoiceChangeArgs
            {
                Choice = c.ChoiceAt(index),
                Description = c.Description,
                Index = index
            };
            ConfigChoiceChanged?.Invoke(this, e);
        }

        /// <summary>
        ///     Updates the value for a given option ID.
        /// </summary>
        /// <typeparam name="T">The type of the value (generally string or int).</typeparam>
        /// <param name="optionId">The ID of the option to update.</param>
        /// <param name="value">The new value to apply.</param>
        /// <param name="index">If present and non-negative, the index of the option to set.</param>
        public void AddOptionValue<T>(uint optionId, T value, int index = -1)
        {
            if (_idLookup.TryGetValue(optionId, out var option)) SetValue(option, value, index);
        }

        /// <summary>
        ///     Updates the value for a given option description.
        /// </summary>
        /// <typeparam name="T">The type of the value (generally string or int).</typeparam>
        /// <param name="optionDescription">The description of the option to update.</param>
        /// <param name="value">The new value to apply.</param>
        /// <param name="index">If present and non-negative, the index of the option to set.</param>
        public void SetValue<T>(string optionDescription, T value, int index = -1)
        {
            if (_descLookup.TryGetValue(optionDescription, out var option)) SetValue(option, value, index);
        }

        public void SetChoice(string optionDescription, string choiceDescription, int index = -1)
        {
            if (!(GetOption(optionDescription) is ChoiceOption)) return;
            var choice = FindChoiceIndexFor(optionDescription, choiceDescription);
            if (choice != -1) SetValue(optionDescription, choice, index);
        }

        public string GetChoice(string optionDescription, int index = -1)
        {
            if (GetOption(optionDescription) is ChoiceOption option) return option.Choice;
            return null;
        }

        public T GetValue<T>(string optionDescription, int index = -1)
        {
            if (GetOption(optionDescription) is Option<T> option) return option.ValueAt(index);
            return default;
        }

        /// <summary>
        ///     Installs event handlers on a receiver that respond to BAPSNet configuration changes by
        ///     updating the config cache.
        /// </summary>
        /// <param name="r">The <see cref="IConfigServerUpdater" /> with whose event handlers we are registering.</param>
        public void InstallReceiverEventHandlers(IConfigServerUpdater r)
        {
            r.ConfigSetting += (sender, e) => AddOptionValue(e);
            r.ConfigOption += (sender, e) => AddOptionDescription(e.OptionId, e.Type, e.Description, e.HasIndex);
            r.ConfigChoice += (sender, e) => AddOptionChoice(e.OptionId, (int) e.ChoiceId, e.ChoiceDescription);
        }

        /// <summary>
        ///     Updates the value for a given option ID directly from a
        ///     <see cref="ServerUpdates.ConfigSettingArgs" /> struct.
        /// </summary>
        /// <param name="args">The <see cref="ServerUpdates.ConfigSettingArgs" /> struct to use.</param>
        private void AddOptionValue(ServerUpdates.ConfigSettingArgs args)
        {
            switch (args.Type)
            {
                case ConfigType.Str:
                    if (!(args.Value is string str))
                        throw new ArgumentException("value should be a string", nameof(args));
                    AddOptionValue(args.OptionId, str, args.Index);
                    break;
                case ConfigType.Choice:
                case ConfigType.Int:
                    if (!(args.Value is int i))
                        throw new ArgumentException("value should be an integer", nameof(args));
                    AddOptionValue(args.OptionId, i, args.Index);
                    break;
            }
        }

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
            private readonly Dictionary<int, T> _values = new Dictionary<int, T>();

            protected Option(uint optionId, string description, bool isIndexed)
            {
                OptionId = optionId;
                Description = description;
                IsIndexed = isIndexed;
            }

            private bool IsIndexed { get; }
            protected T Value => ValueAt(-1);
            public uint OptionId { get; }
            public string Description { get; }

            public abstract ConfigType Type { get; }

            public T ValueAt(int index)
            {
                return _values.TryGetValue(index, out var x) ? x : default;
            }

            /// <summary>
            ///     Hook for validating options before storing them.
            /// </summary>
            /// <param name="value">
            ///     The value to validate; validation should raise exceptions if this value is invalid.
            /// </param>
            public abstract void ValidateValue(T value);

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
            }
        }

        /// <summary>
        ///     Describes an option where values are strings.
        /// </summary>
        private class StringOption : Option<string>
        {
            public StringOption(uint optionId, string description, bool isIndexed)
                : base(optionId, description, isIndexed)
            {
            }

            public override ConfigType Type => ConfigType.Str;

            public override void ValidateValue(string value)
            {
                if (value == null) throw new ArgumentNullException(nameof(value), "Can't store a null string");
            }
        }

        /// <inheritdoc />
        /// <summary>
        ///     Describes an option where values are integers.
        /// </summary>
        private class IntOption : Option<int>
        {
            public IntOption(uint optionId, string description, bool isIndexed)
                : base(optionId, description, isIndexed)
            {
            }

            public override ConfigType Type => ConfigType.Int;

            public override void ValidateValue(int value)
            {
                // All integers are valid.
            }
        }

        /// <inheritdoc />
        /// <summary>
        ///     Describes an option where values are restricted to a specific set of choices.
        /// </summary>
        private class ChoiceOption : Option<int>
        {
            private readonly Dictionary<string, int> _choiceList = new Dictionary<string, int>();

            public ChoiceOption(uint optionId, string description, bool isIndexed)
                : base(optionId, description, isIndexed)
            {
            }

            public override ConfigType Type => ConfigType.Choice;
            public string Choice => ChoiceDescriptionFor(Value);

            public void AddChoice(int choiceId, string description)
            {
                _choiceList[description] = choiceId;
            }

            public int ChoiceIndexFor(string description)
            {
                return _choiceList.TryGetValue(description, out var v) ? v : NoIndex;
            }

            private string ChoiceDescriptionFor(int index)
            {
                return _choiceList.FirstOrDefault(pair => index == pair.Value).Key;
            }

            public override void ValidateValue(int value)
            {
                if (!_choiceList.Values.Contains(value))
                    throw new ArgumentOutOfRangeException(nameof(value), value, "Value not a valid choice ID.");
            }

            public string ChoiceAt(int index)
            {
                return ChoiceDescriptionFor(ValueAt(index));
            }
        }
    }
}