using System;
using JetBrains.Annotations;
using URY.BAPS.Client.Common.Controllers;
using URY.BAPS.Client.Common.ServerConfig;
using URY.BAPS.Common.Model.ServerConfig;

namespace URY.BAPS.Client.Common
{
    /// <summary>
    ///     An object that, given a <see cref="ConfigCache" /> and various controllers, performs a full first-update
    ///     request on a BAPS server, including channel and directory count pre-fetches and directory refresh.
    ///     <para>
    ///         The main effect of using this object (through the <see cref="Run" /> method) is a large amount of
    ///         server updates arriving from the client.  Handling these is outside the remit of the
    ///         <see cref="InitialUpdatePerformer" />.
    ///     </para>
    /// </summary>
    public class InitialUpdatePerformer
    {
        /// <summary>
        ///     The number of milliseconds that the updater will wait for a pre-fetched count to arrive before it
        ///     gives up.
        ///     <para>
        ///         This value may include the amount of time spent setting up the update event itself.
        ///     </para>
        /// </summary>
        private const int CountPrefetchTimeoutMilliseconds = 5_000;

        /// <summary>
        ///     The cache into which we're storing any auto-update configuration.
        /// </summary>
        [NotNull] private readonly ConfigCache _cache;

        /// <summary>
        ///     The config controller we use to send count requests.
        /// </summary>
        [NotNull] private readonly ConfigController _config;

        /// <summary>
        ///     The directory controller set we use to send directory refresh requests.
        /// </summary>
        [NotNull] private readonly DirectoryControllerSet _directories;

        /// <summary>
        ///     The system controller we use to send auto-update requests.
        /// </summary>
        [NotNull] private readonly SystemController _system;

        /// <summary>
        ///     Constructs an initial update performer.
        /// </summary>
        /// <param name="cache">The configuration cache that will receive the channel and directory counts.</param>
        /// <param name="directories">The controller set for accessing directory controllers.</param>
        /// <param name="system">The system controller.</param>
        /// <param name="config">The config controller.</param>
        public InitialUpdatePerformer([CanBeNull] ConfigCache cache, [CanBeNull] DirectoryControllerSet directories,
            [CanBeNull] SystemController system, [CanBeNull] ConfigController config)
        {
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
            _directories = directories ?? throw new ArgumentNullException(nameof(directories));
            _system = system ?? throw new ArgumentNullException(nameof(system));
            _config = config ?? throw new ArgumentNullException(nameof(config));
        }

        /// <summary>
        ///     The number of channels that were available the last time this object requested an auto-update.
        /// </summary>
        public int ChannelCount { get; private set; } = -1;

        /// <summary>
        ///     The number of directories that were available the last time this object requested an auto-update.
        /// </summary>
        public int DirectoryCount { get; private set; } = -1;

        /// <summary>
        ///     Runs an initial update request.
        /// </summary>
        public void Run()
        {
            PrefetchCounts();
            RequestGeneralAutoUpdate();
            RequestDirectoryRefreshes();
        }

        /// <summary>
        ///     Pre-fetches channel and directory counts from the BAPS server.
        ///     <para>
        ///         This step mainly exists to work around a chicken-and-egg issue with the BAPS server: asking for
        ///         auto-updates sends channel information before we know how many channels are available!
        ///     </para>
        /// </summary>
        private void PrefetchCounts()
        {
            EnsureCountOptionsAvailable();
            PrefetchChannelCount();
            PrefetchDirectoryCount();
        }

        /// <summary>
        ///     Requests that the server enable auto-updates, and also send an initial such update.
        /// </summary>
        private void RequestGeneralAutoUpdate()
        {
            _system.AutoUpdate();
        }

        /// <summary>
        ///     Requests that the server send directory listings for each directory ID from 0 to
        ///     <see cref="DirectoryCount" />.
        /// </summary>
        private void RequestDirectoryRefreshes()
        {
            var count = DirectoryCount;
            for (byte i = 0; i < count; i++) _directories.ControllerFor(i).Refresh();
        }

        /// <summary>
        ///     Populates the channel and directory count config settings in the config cache.
        ///     <para>
        ///         Without these, the config cache will reject the incoming counts.
        ///     </para>
        /// </summary>
        private void EnsureCountOptionsAvailable()
        {
            _cache.AddOptionDescription((uint) OptionKey.ChannelCount, ConfigType.Int, "Number of channels",
                false);
            _cache.AddOptionDescription((uint) OptionKey.DirectoryCount, ConfigType.Int, "Number of directories",
                false);
        }

        /// <summary>
        ///     Pre-fetches the channel count, storing it in <see cref="ChannelCount" />.
        /// </summary>
        private void PrefetchChannelCount()
        {
            ChannelCount = PollWithTimeout(OptionKey.ChannelCount);
        }

        /// <summary>
        ///     Pre-fetches the directory count, storing it in <see cref="DirectoryCount" />.
        /// </summary>
        private void PrefetchDirectoryCount()
        {
            DirectoryCount = PollWithTimeout(OptionKey.DirectoryCount);
        }

        /// <summary>
        ///     Tries to retrieve the integer option with key <see cref="key" />, with a timeout of
        ///     <see cref="CountPrefetchTimeoutMilliseconds" />.
        /// </summary>
        /// <param name="key">The key to try to retrieve.</param>
        /// <returns>The value of the key if it enters the config cache before the timeout; -1 otherwise.</returns>
        private int PollWithTimeout(OptionKey key)
        {
            var timeout = TimeSpan.FromMilliseconds(CountPrefetchTimeoutMilliseconds);
            return _config.PollInt(key, timeout);
        }
    }
}