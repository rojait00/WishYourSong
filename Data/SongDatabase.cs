using SpotifyAPI.Web;

namespace WishYourSong.Data
{
    public class SongDatabase
    {
        private ISpotifyClient _spotifyClient;
        public List<FullTrack> Tracks { get; set; } = new List<FullTrack>();

        public SongDatabase(ISpotifyClient spotifyClient)
        {
            _spotifyClient = spotifyClient;
        }

        public async Task Init(Votes votes)
        {
            if(Tracks.Count == 0 && votes.Any())
            {
                var trackIds = votes.GetIds();
                var trackResponse = await _spotifyClient.Tracks.GetSeveral(new TracksRequest(trackIds));
                
                Tracks.AddRange(trackResponse.Tracks);
            }
        }


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
            var request = new SearchRequest(SearchRequest.Types.Track, term)
            {
                Limit = 50,
                Market = "DE"
            };

            var items = await _spotifyClient.Search.Item(request);
            return items?.Tracks?.Items ?? new List<FullTrack>();
        }

        public List<FullTrack> GetContainedTracks(List<FullTrack> result)
        {
            return result.Where(x => Tracks.Any(dbT => dbT.Id == x.Id)).ToList(); // ToDo: use Votes
        }
    }
}
