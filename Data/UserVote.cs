using Amazon.DynamoDBv2.DataModel;

namespace WishYourSong.Data
{
    [DynamoDBTable("UserVote")]
    public class UserVote
    {
        public UserVote()
        {
        }

        public UserVote(Guid userId, string trackId, bool isLike)
        {
            UserId = userId.ToString();
            TrackId = trackId;
            IsLike = isLike;
        }

        [DynamoDBHashKey]
        public string UserId { get; set; }

        [DynamoDBRangeKey]
        public string TrackId { get; set; }

        public bool IsLike { get; set; } = true;
    }
}
