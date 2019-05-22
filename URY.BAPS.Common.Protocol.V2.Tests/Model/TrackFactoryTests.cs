using System;
using System.Text;
using JetBrains.Annotations;
using URY.BAPS.Common.Model.Track;
using URY.BAPS.Common.Protocol.V2.Model;
using URY.BAPS.Common.Protocol.V2.Tests.Utils;
using Xunit;

namespace URY.BAPS.Common.Protocol.V2.Tests.Model
{
    public class TrackFactoryTests
    {

        [Fact]
        public void TestTrackFactory_AudioFile()
        {
            var track = TrackFactory.Create(TrackType.File, "Genesis", 2753, "ABACAB");
            Assert.Equal("e|l|A|i|t|Genesis|2753|", track.Summarise());
        }

        [Fact]
        public void TestTrackFactory_AudioLibrary()
        {
            var track = TrackFactory.Create(TrackType.Library, "Genesis", 2753, "ABACAB");
            Assert.Equal("e|l|A|I|t|Genesis|2753|", track.Summarise());
        }

        [Fact]
        public void TestTrackFactory_LoadFailed()
        {
            var track = TrackFactory.Create(TrackType.Void, SpecialTrackDescriptions.LoadFailed);
            Assert.Equal("E|l|a|i|t|Load failed|0|", track.Summarise());
        }

        [Fact]
        public void TestTrackFactory_Loading()
        {
            var track = TrackFactory.Create(TrackType.Void, SpecialTrackDescriptions.Loading);
            Assert.Equal("e|L|a|i|t|Loading|0|", track.Summarise());
        }

        [Fact]
        public void TestTrackFactory_Null()
        {
            var track = TrackFactory.Create(TrackType.Void, SpecialTrackDescriptions.None);
            Assert.Equal("e|l|a|i|t|--NONE--|0|", track.Summarise());
        }

        [Fact]
        public void TestTrackFactory_Text()
        {
            var track = TrackFactory.Create(TrackType.Text, "Genesis", 32767, "ABACAB");
            Assert.Equal("e|l|a|i|T|Genesis|0|ABACAB", track.Summarise());
        }
    }
}