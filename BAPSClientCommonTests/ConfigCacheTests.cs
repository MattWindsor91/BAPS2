using NUnit.Framework;
using BAPSCommon;
using System;

namespace BAPSClientCommonTests
{
    /// <summary>
    /// Tests for the <see cref="ConfigCache"/>.
    /// </summary>
    public class ConfigCacheTests
    {
        public class MockReceiver : IConfigServerUpdater
        {
            public event ServerUpdates.ChannelStateEventHandler ChannelState;

            public event ServerUpdates.ConfigChoiceHandler ConfigChoice;
            public void OnConfigChoice(ServerUpdates.ConfigChoiceArgs e) => ConfigChoice.Invoke(this, e);

            public event ServerUpdates.ConfigOptionHandler ConfigOption;
            public void OnConfigOption(ServerUpdates.ConfigOptionArgs e) => ConfigOption?.Invoke(this, e);

            public event EventHandler<(Command cmdReceived, uint optionID, ConfigResult result)> ConfigResult;

            public event ServerUpdates.ConfigSettingHandler ConfigSetting;
            public void OnConfigSetting(ServerUpdates.ConfigSettingArgs e) => ConfigSetting?.Invoke(this, e);
        }

        private ConfigCache cache;
        private MockReceiver receiver;

        [SetUp]
        public void Setup()
        {
            cache = new ConfigCache();
            receiver = new MockReceiver();
            cache.InstallReceiverEventHandlers(receiver);
        }

        [Test, Combinatorial]
        public void TestAddIntegralOptionDescription([Values(ConfigType.INT, ConfigType.CHOICE)] ConfigType type, [Values(true, false)] bool isIndexed)
        {
            cache.AddOptionDescription(0, type, "Foo", isIndexed);
            Assert.That(cache.GetValue<int>("Foo", isIndexed ? 0 : ConfigCache.NO_INDEX), Is.EqualTo(0));
        }

        [Test, Combinatorial]
        public void TestAddStringOptionDescription([Values(true, false)] bool isIndexed)
        {
            cache.AddOptionDescription(0, ConfigType.STR, "Foo", isIndexed);
            Assert.That(cache.GetValue<string>("Foo", isIndexed ? 0 : ConfigCache.NO_INDEX), Is.Null);
        }

        [Test]
        public void TestFindChoiceIndexFor([Values(true, false)] bool isIndexed)
        {
            cache.AddOptionDescription(0, ConfigType.CHOICE, "Foobar", isIndexed);
            cache.AddOptionChoice(0, 0, "Yes");
            cache.AddOptionChoice(0, 1, "No");
            Assert.That(cache.FindChoiceIndexFor("Foobar", "Yes"), Is.EqualTo(0));
            Assert.That(cache.FindChoiceIndexFor("Foobar", "No"), Is.EqualTo(1));
        }

        [Test]
        public void TestGetSetNonIndexedChoiceById()
        {
            cache.AddOptionDescription(0, ConfigType.CHOICE, "Foobar", false);
            cache.AddOptionChoice(0, 0, "Yes");
            cache.AddOptionChoice(0, 1, "No");
            Assert.That(cache.GetValue<int>("Foobar"), Is.EqualTo(0));
            cache.AddOptionValue(0, cache.FindChoiceIndexFor("Foobar", "No"));
            Assert.That(cache.GetValue<int>("Foobar"), Is.EqualTo(1));
        }

        [Test]
        public void TestGetSetNonIndexedChoiceByDescription()
        {
            cache.AddOptionDescription(0, ConfigType.CHOICE, "Foobar", false);
            cache.AddOptionChoice(0, 0, "Yes");
            cache.AddOptionChoice(0, 1, "No");
            Assert.That(cache.GetChoice("Foobar"), Is.EqualTo("Yes"));
            cache.SetChoice("Foobar", "No");
            Assert.That(cache.GetChoice("Foobar"), Is.EqualTo("No"));
        }

        [Test]
        public void TestSetIndexedChoiceIndependence()
        {
            cache.AddOptionDescription(0, ConfigType.CHOICE, "Foobar", true);
            cache.AddOptionChoice(0, 0, "Some");
            cache.AddOptionChoice(0, 1, "Most");
            cache.AddOptionChoice(0, 2, "All");
            cache.AddOptionValue(0, cache.FindChoiceIndexFor("Foobar", "All"), 0);
            cache.AddOptionValue(0, cache.FindChoiceIndexFor("Foobar", "Most"), 1);
            cache.AddOptionValue(0, cache.FindChoiceIndexFor("Foobar", "Some"), 2);
            Assert.That(cache.GetValue<int>("Foobar", 0), Is.EqualTo(2));
            Assert.That(cache.GetValue<int>("Foobar", 1), Is.EqualTo(1));
            Assert.That(cache.GetValue<int>("Foobar", 2), Is.EqualTo(0));
        }

        #region Events interface

        [Test]
        public void TestReceiverString()
        {
            receiver.OnConfigOption(new ServerUpdates.ConfigOptionArgs(64, ConfigType.STR, "Barbaz", hasIndex: false));
            Assert.That(cache.GetValue<string>("Barbaz"), Is.Null);

            receiver.OnConfigSetting(new ServerUpdates.ConfigSettingArgs(64, ConfigType.STR, "FrankerZ"));
            Assert.That(cache.GetValue<string>("Barbaz"), Is.EqualTo("FrankerZ"));
        }

        [Test]
        public void TestReceiverChoice()
        {
            receiver.OnConfigOption(new ServerUpdates.ConfigOptionArgs(99, ConfigType.CHOICE, "Keepo", hasIndex: false));

            receiver.OnConfigChoice(new ServerUpdates.ConfigChoiceArgs(99, 0, "Yes"));
            receiver.OnConfigChoice(new ServerUpdates.ConfigChoiceArgs(99, 1, "No"));
            Assert.That(cache.FindChoiceIndexFor("Keepo", "Yes"), Is.EqualTo(0), "Choice index for 'yes' incorrect");
            Assert.That(cache.FindChoiceIndexFor("Keepo", "No"), Is.EqualTo(1), "Choice index for 'no' incorrect");

            receiver.OnConfigSetting(new ServerUpdates.ConfigSettingArgs(99, ConfigType.CHOICE, 1));
            Assert.That(cache.GetValue<int>("Keepo"), Is.EqualTo(1), "Choice hasn't changed");
        }

        #endregion Events interface
    }
}