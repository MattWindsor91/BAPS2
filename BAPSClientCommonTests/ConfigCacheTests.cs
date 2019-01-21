using NUnit.Framework;
using BAPSCommon;
using System;

namespace Tests
{
    /// <summary>
    /// Tests for the <see cref="ConfigCache"/>.
    /// </summary>
    public class ConfigCacheTests
    {
        public class MockReceiver : IReceiver
        {
            public event ServerUpdates.ChannelStateEventHandler ChannelState;

            public event EventHandler<(uint optionID, uint choiceIndex, string choiceDescription)> ConfigChoice;
            public void OnConfigChoice(uint optionID, uint choiceIndex, string choiceDescription) => ConfigChoice.Invoke(this, (optionID, choiceIndex, choiceDescription));

            public event ServerUpdates.ConfigOptionHandler ConfigOption;
            public void OnConfigOption(ServerUpdates.ConfigOptionArgs e) => ConfigOption?.Invoke(this, e);

            public event EventHandler<(Command cmdReceived, uint optionID, ConfigResult result)> ConfigResult;

            public event ServerUpdates.ConfigSettingHandler ConfigSetting;
            public void OnConfigSetting(ServerUpdates.ConfigSettingArgs e) => ConfigSetting?.Invoke(this, e);

            public event EventHandler<(ushort directoryID, uint index, string description)> DirectoryFileAdd;
            public event EventHandler<(ushort directoryID, string directoryName)> DirectoryPrepare;
            public event EventHandler<(ushort channelID, uint duration)> Duration;
            public event ServerUpdates.ErrorEventHandler Error;
            public event ServerUpdates.CountEventHandler IncomingCount;
            public event EventHandler<(Command cmdReceived, string ipAddress, uint mask)> IPRestriction;
            public event ServerUpdates.ItemAddEventHandler ItemAdd;
            public event ServerUpdates.ItemDeleteEventHandler ItemDelete;
            public event ServerUpdates.ItemMoveEventHandler ItemMove;
            public event EventHandler<(uint resultID, byte dirtyStatus, string description)> LibraryResult;
            public event EventHandler<(uint listingID, uint channelID, string description)> ListingResult;
            public event EventHandler<(ushort channelID, uint index, TracklistItem entry)> LoadedItem;
            public event EventHandler<(uint permissionCode, string description)> Permission;
            public event EventHandler<(ushort channelID, PositionType type, uint position)> Position;
            public event ServerUpdates.ChannelResetEventHandler ResetPlaylist;
            public event EventHandler<bool> ServerQuit;
            public event EventHandler<(uint showID, string description)> ShowResult;
            public event EventHandler<(ushort ChannelID, uint index, TextTracklistItem entry)> TextItem;
            public event EventHandler<ServerUpdates.UpDown> TextScroll;
            public event EventHandler<ServerUpdates.UpDown> TextSizeChange;
            public event EventHandler<(Command command, string description)> UnknownCommand;
            public event EventHandler<(string username, uint permissions)> User;
            public event EventHandler<(byte resultCode, string description)> UserResult;
            public event EventHandler<Receiver.VersionInfo> Version;
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
            receiver.OnConfigOption(new ServerUpdates.ConfigOptionArgs { OptionID = 64, Description = "Barbaz", HasIndex = false, Index = -1, Type = ConfigType.STR });
            Assert.That(cache.GetValue<string>("Barbaz"), Is.Null);

            receiver.OnConfigSetting(new ServerUpdates.ConfigSettingArgs { OptionID = 64, Index = -1, Type = ConfigType.STR, Value = "FrankerZ" });
            Assert.That(cache.GetValue<string>("Barbaz"), Is.EqualTo("FrankerZ"));
        }

        [Test]
        public void TestReceiverChoice()
        {
            receiver.OnConfigOption(new ServerUpdates.ConfigOptionArgs { OptionID = 99, Description = "Keepo", HasIndex = false, Index = -1, Type = ConfigType.CHOICE });

            receiver.OnConfigChoice(99, 0, "Yes");
            receiver.OnConfigChoice(99, 1, "No");
            Assert.That(cache.FindChoiceIndexFor("Keepo", "Yes"), Is.EqualTo(0), "Choice index for 'yes' incorrect");
            Assert.That(cache.FindChoiceIndexFor("Keepo", "No"), Is.EqualTo(1), "Choice index for 'no' incorrect");

            receiver.OnConfigSetting(new ServerUpdates.ConfigSettingArgs { OptionID = 99, Index = -1, Type = ConfigType.CHOICE, Value = 1 });
            Assert.That(cache.GetValue<int>("Keepo"), Is.EqualTo(1), "Choice hasn't changed");
        }

        #endregion Events interface
    }
}