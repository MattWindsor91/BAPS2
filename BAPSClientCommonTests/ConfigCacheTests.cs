using System;
using BAPSClientCommon;
using BAPSClientCommon.BapsNet;
using BAPSClientCommon.Events;
using BAPSClientCommon.ServerConfig;
using NUnit.Framework;

namespace BAPSClientCommonTests
{
    /// <summary>
    ///     Tests for the <see cref="ConfigCache" />.
    /// </summary>
    public class ConfigCacheTests
    {
        private ConfigCache _configCache;
        private MockReceiver _receiver;

        [SetUp]
        public void Setup()
        {
            _configCache = new ConfigCache();
            _receiver = new MockReceiver();
            _configCache.InstallReceiverEventHandlers(_receiver);
        }

        [Test]
        [Combinatorial]
        public void TestAddIntegralOptionDescription([Values(ConfigType.Int, ConfigType.Choice)]
            ConfigType type, [Values(true, false)] bool isIndexed)
        {
            _configCache.AddOptionDescription(0, type, "Foo", isIndexed);
            Assert.That(_configCache.GetValue<int>((uint)0, isIndexed ? 0 : ConfigCache.NoIndex), Is.EqualTo(0));
        }

        [Test]
        [Combinatorial]
        public void TestAddStringOptionDescription([Values(true, false)] bool isIndexed)
        {
            _configCache.AddOptionDescription(0, ConfigType.Str, "Foo", isIndexed);
            Assert.That(_configCache.GetValue<string>((uint)0, isIndexed ? 0 : ConfigCache.NoIndex), Is.Null);
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
            Assert.That(_configCache.GetValue<int>((uint)0), Is.EqualTo(0));
            _configCache.AddOptionValue(0, _configCache.FindChoiceIndexFor(0, "No"));
            Assert.That(_configCache.GetValue<int>((uint)0), Is.EqualTo(1));
        }

        [Test]
        public void TestGetSetNonIndexedChoiceByDescription()
        {
            _configCache.AddOptionDescription(0, ConfigType.Choice, "Foobar", false);
            _configCache.AddOptionChoice(0, 0, "Yes");
            _configCache.AddOptionChoice(0, 1, "No");
            Assert.That(_configCache.GetChoice((uint)0), Is.EqualTo("Yes"));
            _configCache.SetChoice(0, "No");
            Assert.That(_configCache.GetChoice((uint)0), Is.EqualTo("No"));
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
            Assert.That(_configCache.GetValue<int>((uint)0, 0), Is.EqualTo(2));
            Assert.That(_configCache.GetValue<int>((uint)0, 1), Is.EqualTo(1));
            Assert.That(_configCache.GetValue<int>((uint)0, 2), Is.EqualTo(0));
        }

        public class MockReceiver : IConfigServerUpdater
        {
            public event EventHandler<Updates.ConfigChoiceArgs> ConfigChoice;

            public event EventHandler<Updates.ConfigOptionArgs> ConfigOption;

            public event EventHandler<(Command cmdReceived, uint optionID, ConfigResult result)> ConfigResult;

            public event EventHandler<Updates.ConfigSettingArgs> ConfigSetting;
            public event Updates.ChannelStateEventHandler ChannelState;

            public void OnConfigChoice(Updates.ConfigChoiceArgs e)
            {
                ConfigChoice?.Invoke(this, e);
            }

            public void OnConfigOption(Updates.ConfigOptionArgs e)
            {
                ConfigOption?.Invoke(this, e);
            }

            public void OnConfigSetting(Updates.ConfigSettingArgs e)
            {
                ConfigSetting?.Invoke(this, e);
            }

            protected virtual void OnChannelState(Updates.PlayerStateEventArgs e)
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
            _receiver.OnConfigOption(new Updates.ConfigOptionArgs(64, ConfigType.Str, "Barbaz", false));
            Assert.That(_configCache.GetValue<string>(64), Is.Null);

            _receiver.OnConfigSetting(new Updates.ConfigSettingArgs(64, ConfigType.Str, "FrankerZ"));
            Assert.That(_configCache.GetValue<string>(64), Is.EqualTo("FrankerZ"));
        }

        [Test]
        public void TestReceiverChoice()
        {
            _receiver.OnConfigOption(new Updates.ConfigOptionArgs(99, ConfigType.Choice, "Keepo", false));

            _receiver.OnConfigChoice(new Updates.ConfigChoiceArgs(99, 0, "Yes"));
            _receiver.OnConfigChoice(new Updates.ConfigChoiceArgs(99, 1, "No"));
            Assert.That(_configCache.FindChoiceIndexFor(99, "Yes"), Is.EqualTo(0), "Choice index for 'yes' incorrect");
            Assert.That(_configCache.FindChoiceIndexFor(99, "No"), Is.EqualTo(1), "Choice index for 'no' incorrect");

            _receiver.OnConfigSetting(new Updates.ConfigSettingArgs(99, ConfigType.Choice, 1));
            Assert.That(_configCache.GetValue<int>(99), Is.EqualTo(1), "Choice hasn't changed");
        }

        #endregion Events interface
    }
}