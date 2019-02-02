using System;
using System.Collections.Generic;
using System.ComponentModel;
using BAPSClientCommon.BapsNet;
using BAPSClientCommon.Events;

namespace BAPSClientCommon.ServerConfig
{
    /// <summary>
    ///     The config cache is designed as a quick access to config variables,
    ///     You tell it what config item is expected next and then ask for that
    ///     option from the server. It will then associate the result with the
    ///     name you asked for.
    /// </summary>
    public partial class Cache
    {
        /// <summary>
        ///     The (rather paradoxically named) integer that represents a lack of index.
        /// </summary>
        public const int NoIndex = -1;


        private readonly Dictionary<string, IOption> _descLookup = new Dictionary<string, IOption>();
        private readonly Dictionary<uint, IOption> _idLookup = new Dictionary<uint, IOption>();

        private IOption MakeOption(uint optionId, ConfigType type, string description, bool isIndexed)
        {
            switch (type)
            {
                case ConfigType.Choice:
                    return new ChoiceOption(this, optionId, description, isIndexed);
                case ConfigType.Int:
                    return new IntOption(this, optionId, description, isIndexed);
                case ConfigType.Str:
                    return new StringOption(this, optionId, description, isIndexed);
                default:
                    throw new InvalidEnumArgumentException(nameof(type), (int) type, typeof(ConfigType));
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
            var oci = MakeOption(optionId, type, description, isIndexed);
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

        public void SetChoice(string optionDescription, string choiceDescription, int index = NoIndex)
        {
            if (!(GetOption(optionDescription) is ChoiceOption o)) return;
            var choice = FindChoiceIndexFor(optionDescription, choiceDescription);
            if (choice != -1) o.AddValue(choice, index);
        }

        public string GetChoice(string optionDescription, int index = NoIndex)
        {
            if (GetOption(optionDescription) is ChoiceOption option) return option.ChoiceAt(index);
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
        ///     <see cref="Updates.ConfigSettingArgs" /> struct.
        /// </summary>
        /// <param name="args">The <see cref="Updates.ConfigSettingArgs" /> struct to use.</param>
        private void AddOptionValue(Updates.ConfigSettingArgs args)
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