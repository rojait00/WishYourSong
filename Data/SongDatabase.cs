using SpotifyAPI.Web;

namespace WishYourSong.Data
{
    public class SongDatabase
    {
        public List<FullTrack> Tracks { get; set; } = new List<FullTrack>();

        public Votes Votes { get; set; }

        public void AddTrack(FullTrack track)
        {
            track.Popularity = 1;
            Tracks.Add(track);
        }

        public async Task<List<FullTrack>> SearchTrackAsync(string term)
        {
            var spotify = new SpotifyClient(SpotifyTokens.OAuth);

            var request = new SearchRequest(SearchRequest.Types.Track, term)
            {
                Limit = 50,
                Market = "DE"
            };

            var items = await spotify.Search.Item(request);
            return items?.Tracks?.Items ?? new List<FullTrack>();
        }

        public List<FullTrack> GetContainedTracks(List<FullTrack> result)
        {
            return result.Where(x => Tracks.Any(dbT => dbT.Id == x.Id)).ToList();
        }
    }
}
