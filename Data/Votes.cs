using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Radzen.Blazor;
using SpotifyAPI.Web;

namespace WishYourSong.Data
{
    public class Votes
    {
        Dictionary<string, UserVotes> songVotes = new();

        public async Task AddVoteAsync(FullTrack song, bool isLike, ProtectedLocalStorage browserStorage)
        {
            Guid userId;

            var result = await browserStorage.GetAsync<string>("userId");
            if (result.Success == true && result.Value != null)
            {
                userId = Guid.Parse(result.Value);
            }
            else
            {
                userId= Guid.NewGuid();
                await browserStorage.SetAsync("userId", userId.ToString());
            }

            songVotes.TryAdd(song.Id, new UserVotes());
            songVotes[song.Id].AddVote(userId, isLike);
        }

        public int GetVotes(string songId)
        {
            songVotes.TryGetValue(songId, out UserVotes? votes);
            return votes?.GetResult() ?? 0;
        }
    }
}