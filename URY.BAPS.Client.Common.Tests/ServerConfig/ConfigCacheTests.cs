using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using JetBrains.Annotations;
using URY.BAPS.Client.Common.BapsNet;
using URY.BAPS.Client.Common.Events;
using URY.BAPS.Client.Common.ServerConfig;
using URY.BAPS.Client.Common.Updaters;
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
            Assert.Equal(0, _configCache.GetValue<int>(0, isIndexed ? 0 : ConfigCache.NoIndex));
        }

        [Theory, MemberData(nameof(IsIndexedData))]
        public void TestAddStringOptionDescription(bool isIndexed)
        {
            _configCache.AddOptionDescription(0, ConfigType.Str, "Foo", isIndexed);
            Assert.Null(_configCache.GetValue<string>(0, isIndexed ? 0 : ConfigCache.NoIndex));
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
            Assert.Equal(0, _configCache.GetValue<int>(0));
            _configCache.AddOptionValue(0, _configCache.FindChoiceIndexFor(0, "No"));
            Assert.Equal(1, _configCache.GetValue<int>(0));
        }

        [Fact]
        public void TestGetSetNonIndexedChoiceByDescription()
        {
            _configCache.AddOptionDescription(0, ConfigType.Choice, "Foobar", false);
            _configCache.AddOptionChoice(0, 0, "Yes");
            _configCache.AddOptionChoice(0, 1, "No");
            Assert.Equal("Yes", _configCache.GetChoice((uint) 0));
            _configCache.SetChoice(0, "No");
            Assert.Equal("No", _configCache.GetChoice((uint) 0));
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
            Assert.Equal(2, _configCache.GetValue<int>(0, 0));
            Assert.Equal(1, _configCache.GetValue<int>(0, 1));
            Assert.Equal(0, _configCache.GetValue<int>(0, 2));
        }

        public class MockConfigServerUpdater : IConfigServerUpdater
        {
            private IObservable<ConfigChoiceEventArgs> _observeConfigChoice;
            private IObservable<ConfigOptionEventArgs> _observeConfigOption;
            private IObservable<(Command cmdReceived, uint optionID, ConfigResult result)> _observeConfigResult;
            private IObservable<ConfigSettingEventArgs> _observeConfigSetting;
            private IObservable<CountEventArgs> _observeIncomingCount;
            private IObservable<object> _observeMessages;
            public Queue<object> Messages { get; } = new Queue<object>();

            private IObservable<object> ObserveMessages =>
                _observeMessages ??
                (_observeMessages = Messages.ToObservable());

            public IObservable<CountEventArgs> ObserveIncomingCount =>
                _observeIncomingCount ??
                (_observeIncomingCount = ObserveMessages.OfType<CountEventArgs>());

            public IObservable<ConfigChoiceEventArgs> ObserveConfigChoice =>
                _observeConfigChoice ??
                (_observeConfigChoice = ObserveMessages.OfType<ConfigChoiceEventArgs>());

            public IObservable<ConfigOptionEventArgs> ObserveConfigOption =>
                _observeConfigOption ??
                (_observeConfigOption = ObserveMessages.OfType<ConfigOptionEventArgs>());

            public IObservable<ConfigSettingEventArgs> ObserveConfigSetting =>
                _observeConfigSetting ??
                (_observeConfigSetting = ObserveMessages.OfType<ConfigSettingEventArgs>());

            public IObservable<(Command cmdReceived, uint optionID, ConfigResult result)> ObserveConfigResult =>
                _observeConfigResult ??
                (_observeConfigResult =
                    ObserveMessages.OfType<(Command cmdReceived, uint optionID, ConfigResult result)>());
        }

        #region Events interface

        [Fact]
        public void TestReceiverString()
        {
            var receiver = new MockConfigServerUpdater();
            receiver.Messages.Enqueue(new ConfigOptionEventArgs(64, ConfigType.Str, "Barbaz", false));
            receiver.Messages.Enqueue(new ConfigSettingEventArgs(64, ConfigType.Str, "FrankerZ"));

            Assert.Null(_configCache.GetValue<string>(64));
            _configCache.SubscribeToReceiver(receiver);
            Assert.Equal("FrankerZ", _configCache.GetValue<string>(64));
        }

        [Fact]
        public void TestReceiverChoice()
        {
            var receiver = new MockConfigServerUpdater();
            receiver.Messages.Enqueue(new ConfigOptionEventArgs(99, ConfigType.Choice, "Keepo", false));
            receiver.Messages.Enqueue(new ConfigChoiceEventArgs(99, 0, "Yes"));
            receiver.Messages.Enqueue(new ConfigChoiceEventArgs(99, 1, "No"));
            receiver.Messages.Enqueue(new ConfigSettingEventArgs(99, ConfigType.Choice, 1));

            _configCache.SubscribeToReceiver(receiver);
            Assert.Equal(0, _configCache.FindChoiceIndexFor(99, "Yes"));
            Assert.Equal(1, _configCache.FindChoiceIndexFor(99, "No"));
            Assert.Equal(1, _configCache.GetValue<int>(99));
        }

        #endregion Events interface
    }
}