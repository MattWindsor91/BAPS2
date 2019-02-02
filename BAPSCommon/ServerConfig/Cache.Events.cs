namespace BAPSClientCommon.ServerConfig
{
    // Events sent by the config cache when it updates config options.
    //
    // Compared to the events in Events.Updates.Config, these events abstract slightly better over the BapsNet wire
    // representation of settings.

    public partial class Cache
    {
        public delegate void ChoiceChangeEventHandler(object sender, ChoiceChangeEventArgs e);

        public delegate void StringChangeEventHandler(object sender, StringChangeEventArgs e);

        public delegate void IntChangeEventHandler(object sender, IntChangeEventArgs e);

        /// <summary>
        ///     Event raised when a choice-type config option changes.
        /// </summary>
        public event ChoiceChangeEventHandler ChoiceChanged;

        protected virtual void OnChoiceChanged(ChoiceChangeEventArgs e)
        {
            ChoiceChanged?.Invoke(this, e);
        }

        /// <summary>
        ///     Event raised when an integer-type config option changes.
        /// </summary>
        public event IntChangeEventHandler IntChanged;

        /// <summary>
        ///     Event raised when a string-type config option changes.
        /// </summary>
        public event StringChangeEventHandler StringChanged;

        protected virtual void OnIntChanged(IntChangeEventArgs e)
        {
            IntChanged?.Invoke(this, e);
        }

        protected virtual void OnStringChanged(StringChangeEventArgs e)
        {
            StringChanged?.Invoke(this, e);
        }

        public abstract class ChangeEventArgs
        {
            protected ChangeEventArgs(string key, int index)
            {
                Key = key;
                Index = index;
            }

            /// <summary>
            ///     The key of the config setting that has changed.
            /// </summary>
            public string Key { get; }

            /// <summary>
            ///     The index, if applicable, inside the config setting that has changed.
            ///     <para>
            ///         This will be <see cref="Cache.NoIndex" /> if the setting is not indexed.
            ///     </para>
            /// </summary>
            public int Index { get; }
        }

        /// <summary>
        ///     Arguments for an integer-typed config value change event.
        /// </summary>
        public class IntChangeEventArgs : ChangeEventArgs
        {
            public IntChangeEventArgs(string key, int value, int index = NoIndex) : base(key, index)
            {
                Value = value;
            }

            /// <summary>
            ///     The new integer value.
            /// </summary>
            public int Value { get; }
        }

        /// <summary>
        ///     Arguments for a string-typed config value change event.
        /// </summary>
        public class StringChangeEventArgs : ChangeEventArgs
        {
            public StringChangeEventArgs(string key, string value, int index = NoIndex) : base(key, index)
            {
                Value = value;
            }

            /// <summary>
            ///     The new string value.
            /// </summary>
            public string Value { get; }
        }

        /// <summary>
        ///     Arguments for a config choice change event.
        /// </summary>
        public class ChoiceChangeEventArgs : ChangeEventArgs
        {
            public ChoiceChangeEventArgs(string key, string choice, int index = NoIndex) : base(key, index)
            {
                Choice = choice;
            }

            /// <summary>
            ///     The key of the choice that has been selected.
            /// </summary>
            public string Choice { get; }
        }
    }
}