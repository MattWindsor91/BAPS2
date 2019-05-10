using System.Text;
using JetBrains.Annotations;
using URY.BAPS.Model.Track;
using URY.BAPS.Protocol.V2.Model;
using Xunit;

namespace URY.BAPS.Protocol.V2.Tests.Model
{
    public class TrackFactoryTests
    {
        [Pure]
        private string SummariseTrack(ITrack track)
        {
            return new StringBuilder()
                .Append(track.IsError ? 'E' : 'e')
                .Append(track.IsLoading ? 'L' : 'l')
                .Append(track.IsAudioItem ? 'A' : 'a')
                .Append(track.IsFromLibrary ? 'I' : 'i')
                .Append(track.IsTextItem ? 'T' : 't')
                .Append('|')
                .Append(track.Description)
                .Append('|')
                .Append(track.Duration)
                .Append('|')
                .Append(track.Text)
                .ToString();
        }
        
        [Fact]
        public void TestTrackFactory_LoadFailed()
        {
            var track = TrackFactory.Create(TrackType.Void, SpecialTrackDescriptions.LoadFailed);
            Assert.Equal("Elait|Load failed|0|", SummariseTrack(track));
        }
        
        [Fact]
        public void TestTrackFactory_Loading()
        {
            var track = TrackFactory.Create(TrackType.Void, SpecialTrackDescriptions.Loading);
            Assert.Equal("eLait|Loading|0|", SummariseTrack(track));
        } 
        
        [Fact]
        public void TestTrackFactory_Null()
        {
            var track = TrackFactory.Create(TrackType.Void, SpecialTrackDescriptions.None);
            Assert.Equal("elait|--NONE--|0|", SummariseTrack(track));
        }        
        
        [Fact]
        public void TestTrackFactory_AudioLibrary()
        {
            var track = TrackFactory.Create(TrackType.Library, "Genesis", 2753, "ABACAB");
            Assert.Equal("elAIt|Genesis|2753|", SummariseTrack(track));
        }       
        
        [Fact]
        public void TestTrackFactory_AudioFile()
        {
            var track = TrackFactory.Create(TrackType.File, "Genesis", 2753, "ABACAB");
            Assert.Equal("elAit|Genesis|2753|", SummariseTrack(track));
        }       
         
        [Fact]
        public void TestTrackFactory_Text()
        {
            var track = TrackFactory.Create(TrackType.Text, "Genesis", 32767, "ABACAB");
            Assert.Equal("elaiT|Genesis|0|ABACAB", SummariseTrack(track));
        }               
    }
}