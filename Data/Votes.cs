using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Radzen.Blazor;
using SpotifyAPI.Web;

namespace WishYourSong.Data
{
    public class Votes
    {
        readonly Dictionary<string, UserVotes> songVotes = new();
        private ProtectedLocalStorage? _browserStorage;
        private readonly IDynamoDBContext _dbContext;
        private readonly ISpotifyClient _spotifyClient;

        public Votes(IDynamoDBContext dbContext, ISpotifyClient spotifyClient)
        {
            _dbContext = dbContext;
            _spotifyClient = spotifyClient;
        }

        public async Task Init(ProtectedLocalStorage browserStorage)
        {
            _browserStorage = browserStorage;
            await new User().GetOrGenerateUserIdAsync(_browserStorage);

            if (songVotes.Count == 0)
            {
                var conditions = new List<ScanCondition>()
                {
                    new ScanCondition("TrackId", ScanOperator.IsNotNull),
                    new ScanCondition("UserId", ScanOperator.IsNotNull)
                };

                var scan = _dbContext.ScanAsync<UserVote>(conditions, new DynamoDBOperationConfig());

                List<UserVote> votes = await scan.GetRemainingAsync();

                votes?.ForEach(async x => await AddVoteByTrackIdAsync(x.TrackId, new User(Guid.Parse(x.UserId)), x.IsLike));
            }
        }

        public async Task AddPlaylistAsync(string playlistId)
        {
            var user = new User();
            await user.GetOrGenerateUserIdAsync(_browserStorage);
            if (!user.IsAdmin)
            {
                return;
            }

            var newVotes = 0;
            Paging<PlaylistTrack<IPlayableItem>> response;
            do
            {
                var options = new PlaylistGetItemsRequest()
                {
                    Offset = newVotes,
                    Limit = 100
                };

                response = await _spotifyClient.Playlists.GetItems(playlistId, options);
                response.Items?
                        .Select(x => x.Track)
                        .Cast<FullTrack>()
                        .Select(x => x.Id)
                        .Where(x => songVotes.All(y => x != y.Key))
                        .ToList()
                        .ForEach(async x => await AddVoteByTrackIdAsync(x, user, true));

                newVotes += response.Items?.Count() ?? 999999; // if anything is null stop
            }
            while ((response?.Total ?? 0) > newVotes);
        }

        public async Task AddVoteAsync(FullTrack track, bool isLike)
        {
            var user = new User();
            await user.GetOrGenerateUserIdAsync(_browserStorage);

            await AddVoteByTrackIdAsync(track.Id, user, isLike);
        }

        private async Task AddVoteByTrackIdAsync(string trackId, User user, bool isLike)
        {
            songVotes.TryAdd(trackId, new UserVotes());
            if (user.IsAdmin)
            {
                songVotes[trackId].AddVote(Guid.NewGuid(), isLike);
            }
            else
            {
                songVotes[trackId].AddVote(user.Id, isLike);
            }
            // Save to DB
            await _dbContext.SaveAsync(new UserVote(user.Id, trackId, isLike));
        }

        public int GetVotes(string songId)
        {
            songVotes.TryGetValue(songId, out UserVotes? votes);
            return votes?.GetResult() ?? 0;
        }

        internal bool Any()
        {
            return songVotes.Any();
        }

        internal List<string> GetIds()
        {
            return songVotes.Keys.ToList();
        }

        internal int CountTracks()
        {
            return songVotes.Count;
        }
    }
}