using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using NUnit.Framework;
using URY.BAPS.Client.Common.BapsNet;
using URY.BAPS.Client.Common.Events;
using URY.BAPS.Client.Common.ServerConfig;
using URY.BAPS.Client.Common.Updaters;

namespace URY.BAPS.Client.Common.Tests.ServerConfig
{
    /// <summary>
    ///     Tests for the <see cref="ConfigCache" />.
    /// </summary>
    public class ConfigCacheTests
    {
        private ConfigCache _configCache;

        [SetUp]
        public void Setup()
        {
            _configCache = new ConfigCache();
        }

        [Test]
        [Combinatorial]
        public void TestAddIntegralOptionDescription([Values(ConfigType.Int, ConfigType.Choice)]
            ConfigType type, [Values(true, false)] bool isIndexed)
        {
            _configCache.AddOptionDescription(0, type, "Foo", isIndexed);
            Assert.That(_configCache.GetValue<int>(0, isIndexed ? 0 : ConfigCache.NoIndex), Is.EqualTo(0));
        }

        [Test]
        [Combinatorial]
        public void TestAddStringOptionDescription([Values(true, false)] bool isIndexed)
        {
            _configCache.AddOptionDescription(0, ConfigType.Str, "Foo", isIndexed);
            Assert.That(_configCache.GetValue<string>(0, isIndexed ? 0 : ConfigCache.NoIndex), Is.Null);
        }

        [Test]
        public void TestFindChoiceIndexFor([Values(true, false)] bool isIndexed)
        {
            _configCache.AddOptionDescription(0, ConfigType.Choice, "Foobar", isIndexed);
            _configCache.AddOptionChoice(0, 0, "Yes");
            _configCache.AddOptionChoice(0, 1, "No");
            Assert.That(_configCache.FindChoiceIndexFor(0, "Yes"), Is.EqualTo(0));
            Assert.That(_configCache.FindChoiceIndexFor(0, "No"), Is.EqualTo(1));
        }

        [Test]
        public void TestGetSetNonIndexedChoiceById()
        {
            _configCache.AddOptionDescription(0, ConfigType.Choice, "Foobar", false);
            _configCache.AddOptionChoice(0, 0, "Yes");
            _configCache.AddOptionChoice(0, 1, "No");
            Assert.That(_configCache.GetValue<int>(0), Is.EqualTo(0));
            _configCache.AddOptionValue(0, _configCache.FindChoiceIndexFor(0, "No"));
            Assert.That(_configCache.GetValue<int>(0), Is.EqualTo(1));
        }

        [Test]
        public void TestGetSetNonIndexedChoiceByDescription()
        {
            _configCache.AddOptionDescription(0, ConfigType.Choice, "Foobar", false);
            _configCache.AddOptionChoice(0, 0, "Yes");
            _configCache.AddOptionChoice(0, 1, "No");
            Assert.That(_configCache.GetChoice((uint) 0), Is.EqualTo("Yes"));
            _configCache.SetChoice(0, "No");
            Assert.That(_configCache.GetChoice((uint) 0), Is.EqualTo("No"));
        }

        [Test]
        public void TestSetIndexedChoiceIndependence()
        {
            _configCache.AddOptionDescription(0, ConfigType.Choice, "Foobar", true);
            _configCache.AddOptionChoice(0, 0, "Some");
            _configCache.AddOptionChoice(0, 1, "Most");
            _configCache.AddOptionChoice(0, 2, "All");
            _configCache.AddOptionValue(0, _configCache.FindChoiceIndexFor(0, "All"), 0);
            _configCache.AddOptionValue(0, _configCache.FindChoiceIndexFor(0, "Most"), 1);
            _configCache.AddOptionValue(0, _configCache.FindChoiceIndexFor(0, "Some"), 2);
            Assert.That(_configCache.GetValue<int>(0, 0), Is.EqualTo(2));
            Assert.That(_configCache.GetValue<int>(0, 1), Is.EqualTo(1));
            Assert.That(_configCache.GetValue<int>(0, 2), Is.EqualTo(0));
        }

        public class MockConfigServerUpdater : IConfigServerUpdater
        {
            private IObservable<Updates.ConfigChoiceEventArgs> _observeConfigChoice;
            private IObservable<Updates.ConfigOptionEventArgs> _observeConfigOption;
            private IObservable<(Command cmdReceived, uint optionID, ConfigResult result)> _observeConfigResult;
            private IObservable<Updates.ConfigSettingEventArgs> _observeConfigSetting;
            private IObservable<Updates.CountEventArgs> _observeIncomingCount;
            private IObservable<object> _observeMessages;
            public Queue<object> Messages { get; } = new Queue<object>();

            private IObservable<object> ObserveMessages =>
                _observeMessages ??
                (_observeMessages = Messages.ToObservable());

            public IObservable<Updates.CountEventArgs> ObserveIncomingCount =>
                _observeIncomingCount ??
                (_observeIncomingCount = ObserveMessages.OfType<Updates.CountEventArgs>());

            public IObservable<Updates.ConfigChoiceEventArgs> ObserveConfigChoice =>
                _observeConfigChoice ??
                (_observeConfigChoice = ObserveMessages.OfType<Updates.ConfigChoiceEventArgs>());

            public IObservable<Updates.ConfigOptionEventArgs> ObserveConfigOption =>
                _observeConfigOption ??
                (_observeConfigOption = ObserveMessages.OfType<Updates.ConfigOptionEventArgs>());

            public IObservable<Updates.ConfigSettingEventArgs> ObserveConfigSetting =>
                _observeConfigSetting ??
                (_observeConfigSetting = ObserveMessages.OfType<Updates.ConfigSettingEventArgs>());

            public IObservable<(Command cmdReceived, uint optionID, ConfigResult result)> ObserveConfigResult =>
                _observeConfigResult ??
                (_observeConfigResult =
                    ObserveMessages.OfType<(Command cmdReceived, uint optionID, ConfigResult result)>());
        }

        #region Events interface

        [Test]
        public void TestReceiverString()
        {
            var receiver = new MockConfigServerUpdater();
            receiver.Messages.Enqueue(new Updates.ConfigOptionEventArgs(64, ConfigType.Str, "Barbaz", false));
            receiver.Messages.Enqueue(new Updates.ConfigSettingEventArgs(64, ConfigType.Str, "FrankerZ"));

            Assert.That(_configCache.GetValue<string>(64), Is.Null);
            _configCache.SubscribeToReceiver(receiver);
            Assert.That(_configCache.GetValue<string>(64), Is.EqualTo("FrankerZ"));
        }

        [Test]
        public void TestReceiverChoice()
        {
            var receiver = new MockConfigServerUpdater();
            receiver.Messages.Enqueue(new Updates.ConfigOptionEventArgs(99, ConfigType.Choice, "Keepo", false));
            receiver.Messages.Enqueue(new Updates.ConfigChoiceEventArgs(99, 0, "Yes"));
            receiver.Messages.Enqueue(new Updates.ConfigChoiceEventArgs(99, 1, "No"));
            receiver.Messages.Enqueue(new Updates.ConfigSettingEventArgs(99, ConfigType.Choice, 1));

            _configCache.SubscribeToReceiver(receiver);
            Assert.That(_configCache.FindChoiceIndexFor(99, "Yes"), Is.EqualTo(0), "Choice index for 'yes' incorrect");
            Assert.That(_configCache.FindChoiceIndexFor(99, "No"), Is.EqualTo(1), "Choice index for 'no' incorrect");
            Assert.That(_configCache.GetValue<int>(99), Is.EqualTo(1), "Choice hasn't changed");
        }

        #endregion Events interface
    }
}