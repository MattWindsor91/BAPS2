using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using JetBrains.Annotations;
using URY.BAPS.Client.Common.ServerConfig;
using URY.BAPS.Client.Common.Updaters;
using URY.BAPS.Common.Model.MessageEvents;
using URY.BAPS.Common.Model.ServerConfig;
using Xunit;

namespace URY.BAPS.Client.Common.Tests.ServerConfig
{
    /// <summary>
    ///     Tests for the <see cref="ConfigCache" />.
    /// </summary>
    public class ConfigCacheTests
    {
        [NotNull] private readonly ConfigCache _configCache = new ConfigCache();

        public static TheoryData<bool> IsIndexedData => new TheoryData<bool> {true, false};

        public static TheoryData<ConfigType, bool> IntegralOptionData
        {
            get
            {
                var data = new TheoryData<ConfigType, bool>();
                var combinations =
                    from choice in new[] {ConfigType.Int, ConfigType.Choice}
                    from boolean in new[] { true, false }
                    select (choice, boolean);
                foreach (var (choice, boolean) in combinations) data.Add(choice, boolean);
                return data;
            }
        }

        [Theory, MemberData(nameof(IntegralOptionData))]
        public void TestAddIntegralOptionDescription(ConfigType type, bool isIndexed)
        {
            _configCache.AddOptionDescription(0, type, "Foo", isIndexed);
            Assert.Equal(-1, _configCache.GetValue(0, -1, isIndexed ? 0 : ConfigCache.NoIndex));
        }

        [Theory, MemberData(nameof(IsIndexedData))]
        public void TestAddStringOptionDescription(bool isIndexed)
        {
            _configCache.AddOptionDescription(0, ConfigType.Str, "Foo", isIndexed);
            Assert.Equal("Not set", _configCache.GetValue(0, "Not set", isIndexed ? 0 : ConfigCache.NoIndex));
        }

        [Theory, MemberData(nameof(IsIndexedData))]
        public void TestFindChoiceIndexFor(bool isIndexed)
        {
            _configCache.AddOptionDescription(0, ConfigType.Choice, "Foobar", isIndexed);
            _configCache.AddOptionChoice(0, 0, "Yes");
            _configCache.AddOptionChoice(0, 1, "No");
            Assert.Equal(0, _configCache.FindChoiceIndexFor(0, "Yes"));
            Assert.Equal(1, _configCache.FindChoiceIndexFor(0, "No"));
        }

        [Fact]
        public void TestGetSetNonIndexedChoiceById()
        {
            _configCache.AddOptionDescription(0, ConfigType.Choice, "Foobar", false);
            _configCache.AddOptionChoice(0, 0, "Yes");
            _configCache.AddOptionChoice(0, 1, "No");
            Assert.Equal(-1, _configCache.GetValue(0, -1, ConfigCache.NoIndex));
            _configCache.AddOptionValue(0, _configCache.FindChoiceIndexFor(0, "No"));
            Assert.Equal(1, _configCache.GetValue(0, -1, ConfigCache.NoIndex));
        }

        [Fact]
        public void TestGetSetNonIndexedChoiceByDescription()
        {
            _configCache.AddOptionDescription(0, ConfigType.Choice, "Foobar", false);
            _configCache.AddOptionChoice(0, 0, "Yes");
            _configCache.AddOptionChoice(0, 1, "No");
            Assert.Equal("?", _configCache.GetChoice((uint) 0, "?", ConfigCache.NoIndex));
            _configCache.SetChoice(0, "No");
            Assert.Equal("No", _configCache.GetChoice((uint) 0, "?", ConfigCache.NoIndex));
        }

        [Fact]
        public void TestSetIndexedChoiceIndependence()
        {
            _configCache.AddOptionDescription(0, ConfigType.Choice, "Foobar", true);
            _configCache.AddOptionChoice(0, 0, "Some");
            _configCache.AddOptionChoice(0, 1, "Most");
            _configCache.AddOptionChoice(0, 2, "All");
            _configCache.AddOptionValue(0, _configCache.FindChoiceIndexFor(0, "All"), 0);
            _configCache.AddOptionValue(0, _configCache.FindChoiceIndexFor(0, "Most"), 1);
            _configCache.AddOptionValue(0, _configCache.FindChoiceIndexFor(0, "Some"), 2);
            Assert.Equal(2, _configCache.GetValue(0, -1, 0));
            Assert.Equal(1, _configCache.GetValue(0, -1, 1));
            Assert.Equal(0, _configCache.GetValue(0, -1, 2));
        }

        public class MockConfigServerUpdater : IConfigServerUpdater
        {
            private IObservable<ConfigChoiceArgs> _observeConfigChoice;
            private IObservable<ConfigOptionArgs> _observeConfigOption;
            private IObservable<ConfigResultArgs> _observeConfigResult;
            private IObservable<ConfigSettingArgs> _observeConfigSetting;
            private IObservable<CountArgs> _observeIncomingCount;
            private IObservable<object> _observeMessages;
            private IObservable<UnknownCommandArgs> _observeUnknownCommand;
            private IObservable<IpRestrictionArgs> _observeIpRestriction;
            private IObservable<PermissionArgs> _observePermission;
            private IObservable<UserArgs> _observeUser;
            private IObservable<UserResultArgs> _observeUserResult;
            public Queue<object> Messages { get; } = new Queue<object>();

            private IObservable<object> ObserveMessages =>
                _observeMessages ??= Messages.ToObservable();

            public IObservable<CountArgs> ObserveIncomingCount =>
                _observeIncomingCount ??= ObserveMessages.OfType<CountArgs>();

            public IObservable<UnknownCommandArgs> ObserveUnknownCommand =>
                _observeUnknownCommand ??= ObserveMessages.OfType<UnknownCommandArgs>();

            public IObservable<ConfigChoiceArgs> ObserveConfigChoice =>
                _observeConfigChoice ??= ObserveMessages.OfType<ConfigChoiceArgs>();

            public IObservable<ConfigOptionArgs> ObserveConfigOption =>
                _observeConfigOption ??= ObserveMessages.OfType<ConfigOptionArgs>();

            public IObservable<ConfigSettingArgs> ObserveConfigSetting =>
                _observeConfigSetting ??= ObserveMessages.OfType<ConfigSettingArgs>();

            public IObservable<ConfigResultArgs> ObserveConfigResult =>
                _observeConfigResult ??= ObserveMessages.OfType<ConfigResultArgs>();

            public IObservable<IpRestrictionArgs> ObserveIpRestriction =>
                _observeIpRestriction ??= ObserveMessages.OfType<IpRestrictionArgs>();

            public IObservable<PermissionArgs> ObservePermission =>
                _observePermission ??= ObserveMessages.OfType<PermissionArgs>();

            public IObservable<UserArgs> ObserveUser =>
                _observeUser ??= ObserveMessages.OfType<UserArgs>();

            public IObservable<UserResultArgs> ObserveUserResult =>
                _observeUserResult ??= ObserveMessages.OfType<UserResultArgs>();
        }

        #region Events interface

        [Fact]
        public void TestReceiverString()
        {
            var receiver = new MockConfigServerUpdater();
            receiver.Messages.Enqueue(new ConfigOptionArgs(64, ConfigType.Str, "Barbaz"));
            receiver.Messages.Enqueue(new ConfigSettingArgs(64, ConfigType.Str, "FrankerZ"));

            Assert.Equal("", _configCache.GetValue(64, "", ConfigCache.NoIndex));
            _configCache.SubscribeToReceiver(receiver);
            Assert.Equal("FrankerZ", _configCache.GetValue(64, "", ConfigCache.NoIndex));
        }

        [Fact]
        public void TestReceiverChoice()
        {
            var receiver = new MockConfigServerUpdater();
            receiver.Messages.Enqueue(new ConfigOptionArgs(99, ConfigType.Choice, "Keepo"));
            receiver.Messages.Enqueue(new ConfigChoiceArgs(99, 0, "Yes"));
            receiver.Messages.Enqueue(new ConfigChoiceArgs(99, 1, "No"));
            receiver.Messages.Enqueue(new ConfigSettingArgs(99, ConfigType.Choice, 1));

            _configCache.SubscribeToReceiver(receiver);
            Assert.Equal(0, _configCache.FindChoiceIndexFor(99, "Yes"));
            Assert.Equal(1, _configCache.FindChoiceIndexFor(99, "No"));
            Assert.Equal(1, _configCache.GetValue(99, -1, ConfigCache.NoIndex));
        }

        #endregion Events interface
    }
}