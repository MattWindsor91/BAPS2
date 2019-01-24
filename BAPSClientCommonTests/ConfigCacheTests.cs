using System;
using BAPSClientCommon;
using BAPSClientCommon.BapsNet;
using NUnit.Framework;

namespace BAPSClientCommonTests
{
    /// <summary>
    ///     Tests for the <see cref="ConfigCache" />.
    /// </summary>
    public class ConfigCacheTests
    {
        private ConfigCache _cache;
        private MockReceiver _receiver;

        [SetUp]
        public void Setup()
        {
            _cache = new ConfigCache();
            _receiver = new MockReceiver();
            _cache.InstallReceiverEventHandlers(_receiver);
        }

        [Test]
        [Combinatorial]
        public void TestAddIntegralOptionDescription([Values(ConfigType.Int, ConfigType.Choice)]
            ConfigType type, [Values(true, false)] bool isIndexed)
        {
            _cache.AddOptionDescription(0, type, "Foo", isIndexed);
            Assert.That(_cache.GetValue<int>("Foo", isIndexed ? 0 : ConfigCache.NoIndex), Is.EqualTo(0));
        }

        [Test]
        [Combinatorial]
        public void TestAddStringOptionDescription([Values(true, false)] bool isIndexed)
        {
            _cache.AddOptionDescription(0, ConfigType.Str, "Foo", isIndexed);
            Assert.That(_cache.GetValue<string>("Foo", isIndexed ? 0 : ConfigCache.NoIndex), Is.Null);
        }

        [Test]
        public void TestFindChoiceIndexFor([Values(true, false)] bool isIndexed)
        {
            _cache.AddOptionDescription(0, ConfigType.Choice, "Foobar", isIndexed);
            _cache.AddOptionChoice(0, 0, "Yes");
            _cache.AddOptionChoice(0, 1, "No");
            Assert.That(_cache.FindChoiceIndexFor("Foobar", "Yes"), Is.EqualTo(0));
            Assert.That(_cache.FindChoiceIndexFor("Foobar", "No"), Is.EqualTo(1));
        }

        [Test]
        public void TestGetSetNonIndexedChoiceById()
        {
            _cache.AddOptionDescription(0, ConfigType.Choice, "Foobar", false);
            _cache.AddOptionChoice(0, 0, "Yes");
            _cache.AddOptionChoice(0, 1, "No");
            Assert.That(_cache.GetValue<int>("Foobar"), Is.EqualTo(0));
            _cache.AddOptionValue(0, _cache.FindChoiceIndexFor("Foobar", "No"));
            Assert.That(_cache.GetValue<int>("Foobar"), Is.EqualTo(1));
        }

        [Test]
        public void TestGetSetNonIndexedChoiceByDescription()
        {
            _cache.AddOptionDescription(0, ConfigType.Choice, "Foobar", false);
            _cache.AddOptionChoice(0, 0, "Yes");
            _cache.AddOptionChoice(0, 1, "No");
            Assert.That(_cache.GetChoice("Foobar"), Is.EqualTo("Yes"));
            _cache.SetChoice("Foobar", "No");
            Assert.That(_cache.GetChoice("Foobar"), Is.EqualTo("No"));
        }

        [Test]
        public void TestSetIndexedChoiceIndependence()
        {
            _cache.AddOptionDescription(0, ConfigType.Choice, "Foobar", true);
            _cache.AddOptionChoice(0, 0, "Some");
            _cache.AddOptionChoice(0, 1, "Most");
            _cache.AddOptionChoice(0, 2, "All");
            _cache.AddOptionValue(0, _cache.FindChoiceIndexFor("Foobar", "All"), 0);
            _cache.AddOptionValue(0, _cache.FindChoiceIndexFor("Foobar", "Most"), 1);
            _cache.AddOptionValue(0, _cache.FindChoiceIndexFor("Foobar", "Some"), 2);
            Assert.That(_cache.GetValue<int>("Foobar", 0), Is.EqualTo(2));
            Assert.That(_cache.GetValue<int>("Foobar", 1), Is.EqualTo(1));
            Assert.That(_cache.GetValue<int>("Foobar", 2), Is.EqualTo(0));
        }

        public class MockReceiver : IConfigServerUpdater
        {
            public event ServerUpdates.ConfigChoiceHandler ConfigChoice;

            public event ServerUpdates.ConfigOptionHandler ConfigOption;

            public event EventHandler<(Command cmdReceived, uint optionID, ConfigResult result)> ConfigResult;

            public event ServerUpdates.ConfigSettingHandler ConfigSetting;
            public event ServerUpdates.ChannelStateEventHandler ChannelState;

            public void OnConfigChoice(ServerUpdates.ConfigChoiceArgs e)
            {
                ConfigChoice?.Invoke(this, e);
            }

            public void OnConfigOption(ServerUpdates.ConfigOptionArgs e)
            {
                ConfigOption?.Invoke(this, e);
            }

            public void OnConfigSetting(ServerUpdates.ConfigSettingArgs e)
            {
                ConfigSetting?.Invoke(this, e);
            }

            protected virtual void OnChannelState(ServerUpdates.ChannelStateEventArgs e)
            {
                ChannelState?.Invoke(this, e);
            }

            protected virtual void OnConfigResult((Command cmdReceived, uint optionID, ConfigResult result) e)
            {
                ConfigResult?.Invoke(this, e);
            }
        }

        #region Events interface

        [Test]
        public void TestReceiverString()
        {
            _receiver.OnConfigOption(new ServerUpdates.ConfigOptionArgs(64, ConfigType.Str, "Barbaz", false));
            Assert.That(_cache.GetValue<string>("Barbaz"), Is.Null);

            _receiver.OnConfigSetting(new ServerUpdates.ConfigSettingArgs(64, ConfigType.Str, "FrankerZ"));
            Assert.That(_cache.GetValue<string>("Barbaz"), Is.EqualTo("FrankerZ"));
        }

        [Test]
        public void TestReceiverChoice()
        {
            _receiver.OnConfigOption(new ServerUpdates.ConfigOptionArgs(99, ConfigType.Choice, "Keepo", false));

            _receiver.OnConfigChoice(new ServerUpdates.ConfigChoiceArgs(99, 0, "Yes"));
            _receiver.OnConfigChoice(new ServerUpdates.ConfigChoiceArgs(99, 1, "No"));
            Assert.That(_cache.FindChoiceIndexFor("Keepo", "Yes"), Is.EqualTo(0), "Choice index for 'yes' incorrect");
            Assert.That(_cache.FindChoiceIndexFor("Keepo", "No"), Is.EqualTo(1), "Choice index for 'no' incorrect");

            _receiver.OnConfigSetting(new ServerUpdates.ConfigSettingArgs(99, ConfigType.Choice, 1));
            Assert.That(_cache.GetValue<int>("Keepo"), Is.EqualTo(1), "Choice hasn't changed");
        }

        #endregion Events interface
    }
}