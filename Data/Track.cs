using SpotifyAPI.Web;
using System;

namespace WishYourSong.Data
{
    public class Track
    {
        private readonly Votes votes;

        public FullTrack FullTrack { get; set; }
        public string Title { get; set; }
        public string Artist { get; set; }
        public int Likes { get; set; }

        public Track(FullTrack fullTrack, Votes votes)
        {
            FullTrack = fullTrack;
            this.votes = votes;
            Title = fullTrack.Name;
            Artist = string.Join(", ", fullTrack.Artists.Select(x => x.Name).OrderBy(x => x));
            Likes = votes.GetVotes(fullTrack.Id);
        }

        public async Task RemoveAsync()
        {
            for (int i = 0; i < 100; i++)
            {
                await votes.AddVoteAsync(FullTrack, false);
            }

            Likes = votes.GetVotes(FullTrack.Id);
        }

        public async Task AddVoteAsync(bool isLike)
        {
            await votes.AddVoteAsync(FullTrack, isLike);

            Likes = votes.GetVotes(FullTrack.Id);
        }
    }
}
