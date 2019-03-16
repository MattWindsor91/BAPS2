using URY.BAPS.Client.Common.ServerConfig;

namespace URY.BAPS.Client.Common.Events
{
    /// <inheritdoc />
    /// <summary>
    ///     Event payload sent when the server declares a config option.
    /// </summary>
    public class ConfigOptionEventArgs : ConfigTypeIndexEventArgsBase
    {
        public ConfigOptionEventArgs(uint optionId, ConfigType type, string description, bool hasIndex,
            int index = -1)
            : base(optionId, type, index)
        {
            Description = description;
            HasIndex = hasIndex;
        }

        /// <summary>The string description of the option.</summary>
        public string Description { get; }

        /// <summary>Whether the option has an index.</summary>
        public bool HasIndex { get; }
    }

    /// <inheritdoc />
    /// <summary>
    ///     Event payload sent when the server declares a config setting.
    /// </summary>
    public class ConfigSettingEventArgs : ConfigTypeIndexEventArgsBase
    {
        /// <summary>The new value to apply.</summary>
        public readonly object Value;

        public ConfigSettingEventArgs(uint optionId, ConfigType type, object value, int index = -1)
            : base(optionId, type, index)
        {
            Value = value;
        }
    }

    /// <inheritdoc />
    /// <summary>
    ///     Event payload sent when the server declares a config choice.
    /// </summary>
    public class ConfigChoiceEventArgs : ConfigEventArgsBase
    {
        public ConfigChoiceEventArgs(uint optionId, uint choiceId, string description)
            : base(optionId)
        {
            ChoiceId = choiceId;
            ChoiceDescription = description;
        }

        /// <summary>
        ///     The ID of the choice to add or update.
        /// </summary>
        public uint ChoiceId { get; }

        /// <summary>
        ///     The new description of the choice.
        /// </summary>
        public string ChoiceDescription { get; }
    }

    /// <summary>
    ///     Abstract base class of event payloads over config options.
    /// </summary>
    public abstract class ConfigEventArgsBase
    {
        protected ConfigEventArgsBase(uint optionId)
        {
            OptionId = optionId;
        }

        /// <summary>The ID of the option to update.</summary>
        public uint OptionId { get; }
    }

    /// <summary>
    ///     Abstract base class of event payloads over config options that
    ///     contain a type and an index.
    /// </summary>
    public abstract class ConfigTypeIndexEventArgsBase : ConfigEventArgsBase
    {
        protected ConfigTypeIndexEventArgsBase(uint optionId, ConfigType type, int index = -1) : base(optionId)
        {
            Type = type;
            Index = index;
        }

        /// <summary>The BAPSNet type of the value.</summary>
        public ConfigType Type { get; }

        /// <summary>If present and non-negative, the index of the option to set.</summary>
        public int Index { get; }
    }
}