using SpotifyAPI.Web;

namespace WishYourSong.Data
{
    public class SongDatabase
    {
        public List<FullTrack> Tracks { get; set; } = new List<FullTrack>();

        public void AddTrack(FullTrack track)
        {
            if (!Tracks.Any(x => EqualSong(x, track)))
            {
                Tracks.Add(track);
            }
        }

        public bool EqualSong(FullTrack track1, FullTrack track2)
        {
            return GetName(track1) == GetName(track2);
        }

        private string GetName(FullTrack track)
        {
            return track.Name + " - " + String.Join(",", track.Artists.Select(x => x.Name).OrderBy(x => x));
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
