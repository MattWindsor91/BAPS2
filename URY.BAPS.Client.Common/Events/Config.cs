using System;
using URY.BAPS.Client.Common.Model;
using URY.BAPS.Client.Common.ServerConfig;

namespace URY.BAPS.Client.Common.Events
{
    #region Config settings and related
    
    /// <summary>
    ///     Abstract base class of event payloads over config options.
    /// </summary>
    public abstract class ConfigOptionArgsBase : ArgsBase
    {
        protected ConfigOptionArgsBase(uint optionId)
        {
            OptionId = optionId;
        }

        /// <summary>The ID of the option to update.</summary>
        public uint OptionId { get; }
    }

    /// <summary>
    ///     Abstract base class of event payloads over config options that
    ///     contain an index.
    /// </summary>
     public abstract class ConfigOptionIndexArgsBase : ConfigOptionArgsBase
     {
        protected ConfigOptionIndexArgsBase(uint optionId, int index = -1) : base(optionId)
        {
            Index = index;
        }

        /// <summary>Whether the option has an index.</summary>
        public bool HasIndex => Index != -1;

        /// <summary>If present and non-negative, the index of the option to set.</summary>
        public int Index { get; }
    }   
    
    /// <summary>
    ///     Abstract base class of event payloads over config options that
    ///     contain a type and an index.
    /// </summary>
    public abstract class ConfigOptionTypeIndexArgsBase : ConfigOptionIndexArgsBase
    {
        protected ConfigOptionTypeIndexArgsBase(uint optionId, ConfigType type, int index = -1) : base(optionId, index)
        {
            Type = type;
        }

        /// <summary>The BAPSNet type of the value.</summary>
        public ConfigType Type { get; }
    }

    /// <inheritdoc />
    /// <summary>
    ///     Event payload sent when the server declares a config option.
    /// </summary>
    public class ConfigOptionArgs : ConfigOptionTypeIndexArgsBase
    {
        public ConfigOptionArgs(uint optionId, ConfigType type, string description,
            int index = -1)
            : base(optionId, type, index)
        {
            Description = description;
        }

        /// <summary>The string description of the option.</summary>
        public string Description { get; }
    }

    /// <inheritdoc />
    /// <summary>
    ///     Event payload sent when the server declares a config setting.
    /// </summary>
    public class ConfigSettingArgs : ConfigOptionTypeIndexArgsBase
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
    ///     Event payload sent when the server declares a config choice.
    /// </summary>
    public class ConfigChoiceArgs : ConfigOptionArgsBase
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

    /// <summary>
    ///     Event payload sent when the server replies to a config change.
    /// </summary>
    public class ConfigResultArgs : ConfigOptionIndexArgsBase
    {
        /// <summary>
        ///     The result being returned.
        /// </summary>
        public ConfigResult Result { get; } 
        
        public ConfigResultArgs(uint optionId, ConfigResult result, int index = -1) : base(optionId, index)
        {
            Result = result;
        }
    }
    
    #endregion Config settings

    public class UserArgs : ArgsBase
    {
        public string Username { get; }
        public uint Permissions { get; }
        
        public UserArgs(string username, uint permissions)
        {
            Username = username;
            Permissions = permissions;
        }
    }

    public class UserResultArgs : ArgsBase
    {
        public byte ResultCode { get; }
        public string Description { get; }
        
        public UserResultArgs(byte resultCode, string description)
        {
            ResultCode = resultCode;
            Description = description;
        }
    }

    /// <summary>
    ///     Event payload sent when a server announces an IP restriction.
    /// </summary>
    public class IpRestrictionArgs : ArgsBase
    {
        public IpRestriction Restriction { get; }

        public IpRestrictionArgs(IpRestriction restriction)
        {
            Restriction = restriction;
        }
    }
}