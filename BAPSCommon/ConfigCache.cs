using System;
using System.Collections.Generic;
using System.Linq;

namespace BAPSCommon
{
    public class ConfigChange
    {
        public string Description { get; }
        public string Choice { get; }
    }

    /** The config cache is designed as a quick access to config variables,
        You tell it what config item is expected next and then ask for that
        option from the server. It will then associate the result with the
        name you asked for.
    **/
    public class ConfigCache
    {
        /// <summary>
        /// The (rather paradoxically named) integer that represents a lack of index.
        /// </summary>
        public const int NO_INDEX = -1;

        /// <summary>
        /// Non-parametric interface onto options.
        /// </summary>
        interface IOption
        {
            string Description { get; }
            bool IsIndexed { get; }
            int OptionID { get; }
            ConfigType Type { get; }
        }

        /// <summary>
        /// Base class for config option that can store one or more values.
        /// </summary>
        /// <typeparam name="T">The basic type of the values.</typeparam>
        abstract class Option<T> : IOption
        {
            public int OptionID { get; }
            public string Description { get; }
            public bool IsIndexed { get; }

            public abstract ConfigType Type { get; }        

            private Dictionary<int, T> values = new Dictionary<int, T>();

            public T ValueAt(int index) => values.TryGetValue(index, out var x) ? x : default;
            public T Value => ValueAt(-1);
            
            /// <summary>
            /// Hook for validating options before storing them.
            /// </summary>
            /// <param name="value">
            /// The value to validate; validation should raise exceptions if this value is invalid.
            /// </param>
            public abstract void ValidateValue(T value);

            public void AddValue(T value, int index = NO_INDEX)
            {
                if (IsIndexed && index == NO_INDEX)
                    throw new ArgumentOutOfRangeException(
                        nameof(index), index,
                        "Can't set a non-indexed value: this option is indexed.");

                if (!IsIndexed && index != NO_INDEX)
                    throw new ArgumentOutOfRangeException(
                        nameof(index), index,
                        "Can't set an indexed value: this option is not indexed.");

                ValidateValue(value);
                values[index] = value;
            }

            public Option(int optionID, string description, bool isIndexed)
            {
                OptionID = optionID;
                Description = description;
                IsIndexed = isIndexed;
            }
        }

        /// <summary>
        /// Describes an option where values are strings.
        /// </summary>
        class StringOption : Option<string>
        {
            public override ConfigType Type => ConfigType.STR;

            public override void ValidateValue(string value)
            {
                if (value == null) throw new ArgumentNullException(nameof(value), "Can't store a null string");
            }

            public StringOption(int optionID, string description, bool isIndexed)
                : base(optionID, description, isIndexed) { }
        }

        /// <summary>
        /// Describes an option where values are integers.
        /// </summary>
        class IntOption : Option<int>
        {
            public override ConfigType Type => ConfigType.INT;

            public override void ValidateValue(int value)
            {
                // All integers are valid.
            }

            public IntOption(int optionID, string description, bool isIndexed)
                : base(optionID, description, isIndexed) { }
        }

        /// <summary>
        /// Describes an option where values are restricted to a specific set of choices.
        /// </summary>
        class ChoiceOption : Option<int>
        {
            public override ConfigType Type => ConfigType.CHOICE;

            private Dictionary<string, int> choiceList = new Dictionary<string, int>();
            public void AddChoice(int choiceID, string description) => choiceList[description] = choiceID;
            public int ChoiceIndexFor(string description) => choiceList.TryGetValue(description, out var v) ? v : NO_INDEX;

            public override void ValidateValue(int value)
            {
                if (!choiceList.Values.Contains(value))
                    throw new ArgumentOutOfRangeException(nameof(value), value, "Value not a valid choice ID.");
            }

            public ChoiceOption(int optionID, string description, bool isIndexed)
                : base(optionID, description, isIndexed) { }
        }


        private IOption MakeOptionCacheInfo(int optionid, ConfigType type, string description, bool isIndexed)
        {
            switch (type)
            {
                case ConfigType.CHOICE:
                    return new ChoiceOption(optionid, description, isIndexed);
                case ConfigType.INT:
                    return new IntOption(optionid, description, isIndexed);
                case ConfigType.STR:
                    return new StringOption(optionid, description, isIndexed);
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, "Unsupported config type");
            }
        }

        /// <summary>
        /// Creates a bapsnet message to set an option to one of its choices.
        /// </summary>
        /// <param name="optionDesc">The option to set.</param>
        /// <param name="choiceDesc">The choice to use.</param>
        /// <param name="index">If present and valid, the index of the option to set.</param>
        /// <returns>A message that effects the described config change.</returns>
        public Message MakeConfigChoiceMessage(string optionDesc, string choiceDesc, int index = NO_INDEX)
        {
            var oci = GetOption(optionDesc);
            if (oci == null) throw new ArgumentOutOfRangeException(nameof(optionDesc), optionDesc, "Unknown option.");

            if (!(oci is ChoiceOption cci)) throw new ArgumentException("Option doesn't have choices.", nameof(optionDesc));

            var cid = cci.ChoiceIndexFor(choiceDesc);
            if (cid == NO_INDEX) throw new ArgumentOutOfRangeException(nameof(choiceDesc), choiceDesc, "Unknown choice.");

            var cmd = Command.CONFIG | Command.SETCONFIGVALUE;
            if (index != NO_INDEX) cmd |= (Command.CONFIG_USEVALUEMASK | (Command)index);

            return new Message(cmd).Add(oci.OptionID).Add((uint)cci.Type).Add(cid);
        }

        /** add an option **/
        public void AddOptionDescription(int optionid, ConfigType type, string description, bool isIndexed)
        {
            if (descLookup.ContainsKey(description)) return;
            var oci = MakeOptionCacheInfo(optionid, type, description, isIndexed);
            descLookup[description] = oci;
            idLookup[optionid] = oci;
        }
       

        public void AddOptionChoice(int optionid, int choiceid, string description)
        {
            if (!idLookup.TryGetValue(optionid, out var option)) return;
            (option as ChoiceOption)?.AddChoice(choiceid, description);
        }

        private IOption GetOption(string optionDescription) =>
         descLookup.TryGetValue(optionDescription, out var x) ? x : null;

        public int FindChoiceIndexFor(string optionDesc, string description)
        {
            if (GetOption(optionDesc) is ChoiceOption c) return c.ChoiceIndexFor(description);
            return -1;
        }

        public void AddOptionValue<T>(int optionid, T value, int index = -1)
        {
            if (idLookup.TryGetValue(optionid, out var option))
            {
                if (option is Option<T> o) o.AddValue(value, index);
            }
        }


        public T GetValue<T>(string optionDescription, int index = -1)
        {
            if (GetOption(optionDescription) is Option<T> option) return option.ValueAt(index);
            return default;
        }
        
        private Dictionary<string, IOption> descLookup = new Dictionary<string, IOption>();
	    private Dictionary<int, IOption> idLookup = new Dictionary<int, IOption>();
    }
}
