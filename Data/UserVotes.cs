namespace WishYourSong.Data
{
    internal class UserVotes
    {
        Dictionary<Guid, bool> _votes = new Dictionary<Guid, bool>();

        public Dictionary<Guid, bool> Votes { get => _votes; }

        public void AddVote(Guid userId, bool isLike)
        {
            _votes[userId] = isLike;
        }

        public int GetResult()
        {
            return _votes.Values.Where(x => x).Count() - _votes.Values.Where(x => !x).Count();
        }
    }
}