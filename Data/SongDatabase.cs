using SpotifyAPI.Web;

namespace WishYourSong.Data
{
    public class SongDatabase
    {
        private readonly ISpotifyClient _spotifyClient;
        public List<FullTrack> Tracks { get; set; } = new List<FullTrack>();

        public SongDatabase(ISpotifyClient spotifyClient)
        {
            _spotifyClient = spotifyClient;
        }

        public async Task Init(Votes votes)
        {
            if(Tracks.Count != votes.CountTracks())
            {
                Tracks.Clear();
                var trackIdBatches = votes.GetIds().Select((x,i) => (i: i%50,id:x)).GroupBy(x=> x.i);

                foreach (var batch in trackIdBatches)
                {
                    var ids = batch.Select(grp => grp.id).ToList();
                    var request = new TracksRequest(ids);
                    var trackResponse = await _spotifyClient.Tracks.GetSeveral(request);
                    Tracks.AddRange(trackResponse.Tracks);
                }
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

        private static string GetName(FullTrack track)
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
