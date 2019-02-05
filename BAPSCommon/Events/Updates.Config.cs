using BAPSClientCommon.ServerConfig;

namespace BAPSClientCommon.Events
{
    public static partial class Updates
    {
        /// <inheritdoc />
        /// <summary>
        ///     Event payload sent when the server declares a config choice.
        /// </summary>
        public class ConfigChoiceArgs : ConfigArgs
        {
            public ConfigChoiceArgs(uint optionId, uint choiceId, string description)
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

        /// <inheritdoc />
        /// <summary>
        ///     Event payload sent when the server declares a config setting.
        /// </summary>
        public class ConfigSettingArgs : ConfigTypeIndexArgs
        {
            /// <summary>The new value to apply.</summary>
            public readonly object Value;

            public ConfigSettingArgs(uint optionId, ConfigType type, object value, int index = -1)
                : base(optionId, type, index)
            {
                Value = value;
            }
        }

        /// <inheritdoc />
        /// <summary>
        ///     Event payload sent when the server declares a config option.
        /// </summary>
        public class ConfigOptionArgs : ConfigTypeIndexArgs
        {
            public ConfigOptionArgs(uint optionId, ConfigType type, string description, bool hasIndex, int index = -1)
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
    }
}