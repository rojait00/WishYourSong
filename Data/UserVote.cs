using Amazon.DynamoDBv2.DataModel;

namespace WishYourSong.Data
{
    [DynamoDBTable("UserVote")]
    public class UserVote
    {
        /// <summary>
        /// Do not use this Constructor! It is only for generating Objects form JSON!
        /// </summary>
        [Obsolete("Do not use this Constructor! It is only for generating Objects form JSON!")]
        public UserVote()
        {
            UserId = "";
            TrackId = "";
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
