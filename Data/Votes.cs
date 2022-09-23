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
            await GetOrSetUserId();
            
            if (songVotes.Count == 0)
            {
                var conditions = new List<ScanCondition>()
                {
                    new ScanCondition("TrackId", ScanOperator.IsNotNull),
                    new ScanCondition("UserId", ScanOperator.IsNotNull)
                };

                var scan =  _dbContext.ScanAsync<UserVote>(conditions, new DynamoDBOperationConfig());

                List<UserVote> votes = await scan.GetRemainingAsync();

                votes?.ForEach(x => AddVoteByTrackId(x.TrackId, Guid.Parse(x.UserId), x.IsLike));
            }
        }

        public async Task AddVoteAsync(FullTrack track, bool isLike)
        {
            Guid userId = await GetOrSetUserId();

            AddVoteByTrackId(track.Id, userId, isLike);

            // Save to DB
            await _dbContext.SaveAsync(new UserVote(userId, track.Id, isLike));
        }

        private void AddVoteByTrackId(string trackId, Guid userId, bool isLike)
        {
            songVotes.TryAdd(trackId, new UserVotes());
            songVotes[trackId].AddVote(userId, isLike);
        }

        private async Task<Guid> GetOrSetUserId()
        {
            if (_browserStorage == null)
            {
                throw new InvalidOperationException("_browserStorage has not been initialized");
            }

            Guid userId;
            var result = await _browserStorage.GetAsync<string>("userId");
            if (result.Success == true && result.Value != null)
            {
                userId = Guid.Parse(result.Value);
            }
            else
            {
                userId = Guid.NewGuid();
                await _browserStorage.SetAsync("userId", userId.ToString());
            }

            return userId;
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