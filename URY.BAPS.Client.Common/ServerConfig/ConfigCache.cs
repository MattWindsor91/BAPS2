using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using JetBrains.Annotations;
using URY.BAPS.Client.Common.Updaters;
using URY.BAPS.Common.Model.MessageEvents;
using URY.BAPS.Common.Model.ServerConfig;

namespace URY.BAPS.Client.Common.ServerConfig
{
    /// <summary>
    ///     The config cache is designed as a quick access to config variables,
    ///     You tell it what config item is expected next and then ask for that
    ///     option from the server. It will then associate the result with the
    ///     name you asked for.
    /// </summary>
    public partial class ConfigCache : IDisposable
    {
        /// <summary>
        ///     The (rather paradoxically named) integer that represents a lack of index.
        /// </summary>
        public const int NoIndex = -1;


        private readonly ConcurrentDictionary<string, IOption>
            _descLookup = new ConcurrentDictionary<string, IOption>();

        private readonly ConcurrentDictionary<uint, IOption> _idLookup = new ConcurrentDictionary<uint, IOption>();

        [NotNull] private readonly IList<IDisposable> _receiverSubscriptions = new List<IDisposable>();

        public void Dispose()
        {
            UnsubscribeFromReceiver();
        }

        private IOption MakeOption(uint optionId, ConfigType type, string description, bool isIndexed)
        {
            return type switch
                {
                ConfigType.Choice => (IOption) new ChoiceOption(this, optionId, description, isIndexed),
                ConfigType.Int => new IntOption(this, optionId, description, isIndexed),
                ConfigType.Str => new StringOption(this, optionId, description, isIndexed),
                _ => throw new InvalidEnumArgumentException(nameof(type), (int) type, typeof(ConfigType))
                };
        }

        public int ChoiceIndexFor(uint optionId, [ValueProvider("ChoiceKeys")] string choiceKey)
        {
            var oci = GetOption(optionId);
            if (oci == null) throw new ArgumentOutOfRangeException(nameof(optionId), optionId, "Unknown option.");

            if (!(oci is ChoiceOption cci))
                throw new ArgumentException("Option doesn't have choices.", nameof(optionId));

            var cid = cci.ChoiceIndexFor(choiceKey);
            if (cid == NoIndex)
                throw new ArgumentOutOfRangeException(nameof(choiceKey), choiceKey, "Unknown choice.");

            return cid;
        }

        /** add an option **/
        public void AddOptionDescription(uint optionId, ConfigType type, string description, bool isIndexed)
        {
            if (_descLookup.ContainsKey(description)) return;
            var oci = MakeOption(optionId, type, description, isIndexed);
            _descLookup[description] = oci;
            _idLookup[optionId] = oci;
        }


        public void AddOptionChoice(uint optionId, int choiceId, string description)
        {
            if (!_idLookup.TryGetValue(optionId, out var option)) return;
            (option as ChoiceOption)?.AddChoice(choiceId, description);
        }

        private IOption? GetOption(uint optionId)
        {
            return _idLookup.TryGetValue(optionId, out var x) ? x : null;
        }

        public int FindChoiceIndexFor(uint optionId, [ValueProvider("ChoiceKeys")] string choiceKey)
        {
            if (GetOption(optionId) is ChoiceOption c) return c.ChoiceIndexFor(choiceKey);
            return -1;
        }

        /// <summary>
        ///     Updates the value for a given option ID.
        /// </summary>
        /// <typeparam name="T">The type of the value (generally string or int).</typeparam>
        /// <param name="optionId">The ID of the option to update.</param>
        /// <param name="value">The new value to apply.</param>
        /// <param name="index">If present and non-negative, the index of the option to set.</param>
        public void AddOptionValue<T>(uint optionId, T value, int index = NoIndex)
        {
            if (_idLookup.TryGetValue(optionId, out var option)) (option as Option<T>)?.AddValue(value, index);
        }

        public void SetChoice(uint optionId, [ValueProvider("ChoiceKeys")] string choiceKey, int index = NoIndex)
        {
            if (!(GetOption(optionId) is ChoiceOption o)) return;
            var choice = FindChoiceIndexFor(optionId, choiceKey);
            if (choice != -1) o.AddValue(choice, index);
        }

        public string GetChoice(OptionKey sk, string ifNotCached, int index)
        {
            return GetChoice((uint) sk, ifNotCached, index);
        }

        public string GetChoice(uint optionId, string ifNotCached, int index)
        {
            if (GetOption(optionId) is ChoiceOption option) return option.ChoiceAt(index) ?? ifNotCached;
            return ifNotCached;
        }

        public T GetValue<T>(uint optionId, T ifNotCached, int index)
        {
            if (GetOption(optionId) is Option<T> option) return option.ValueAt(index, ifNotCached);
            return ifNotCached;
        }

        /// <summary>
        ///     Subscribes to the update observables on a given receiver.
        /// </summary>
        /// <param name="r">The <see cref="IConfigServerUpdater" /> with whose event handlers we are registering.</param>
        public void SubscribeToReceiver(IConfigServerUpdater r)
        {
            // These subscriptions are in this order so that, if the receiver's observables react to subscriptions by
            // immediately dumping a list of events, then we first fill up the options, then the choices (which depend
            // on options), then the settings (which depend on both).
            _receiverSubscriptions.Add(r.ObserveConfigOption.Subscribe(e =>
                AddOptionDescription(e.OptionId, e.Type, e.Description, e.HasIndex)));
            _receiverSubscriptions.Add(r.ObserveConfigChoice.Subscribe(e =>
                AddOptionChoice(e.OptionId, (int) e.ChoiceId, e.ChoiceDescription)));
            _receiverSubscriptions.Add(r.ObserveConfigSetting.Subscribe(AddOptionValue));
        }

        private void UnsubscribeFromReceiver()
        {
            foreach (var subscription in _receiverSubscriptions) subscription.Dispose();
        }

        /// <summary>
        ///     Updates the value for a given option ID directly from a
        ///     <see cref="ConfigSettingArgs" /> struct.
        /// </summary>
        /// <param name="args">The <see cref="ConfigSettingArgs" /> struct to use.</param>
        private void AddOptionValue(ConfigSettingArgs args)
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
    }
}