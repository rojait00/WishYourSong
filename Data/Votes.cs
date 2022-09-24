using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Radzen.Blazor;
using SpotifyAPI.Web;

namespace WishYourSong.Data
{
    public class Votes
    {
        Dictionary<string, UserVotes> songVotes = new();
        private ProtectedLocalStorage? _browserStorage;
        private IDynamoDBContext _dbContext;


        public Votes(IDynamoDBContext dbContext)
        {
            _dbContext = dbContext;
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

                votes?.ForEach(x => AddVoteByTrackId(x.TrackId, new User(Guid.Parse(x.UserId)), x.IsLike));
            }
        }

        public async Task AddVoteAsync(FullTrack track, bool isLike)
        {
            var user = new User();
            await user.GetOrGenerateUserIdAsync(_browserStorage);

            AddVoteByTrackId(track.Id, user, isLike);

            // Save to DB
            await _dbContext.SaveAsync(new UserVote(user.Id, track.Id, isLike));
        }

        private void AddVoteByTrackId(string trackId, User user, bool isLike)
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
    }
}