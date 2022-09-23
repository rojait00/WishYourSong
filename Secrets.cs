namespace WishYourSong
{
    public class Secrets
    {
        public Secrets(ConfigurationManager configuration)
        {
            AwsUserId = configuration["AwsUserId"];
            AwsUserPassword = configuration["AwsUserPassword"];
            SpotifyUserId = configuration["SpotifyUserId"];
            SpotifyUserPassword = configuration["SpotifyUserPassword"];
        }

        /// <summary>
        /// To Do: Replace with your own values
        /// Follow this instructions to get the credentials and create the specified files
        /// https://johnnycrazy.github.io/SpotifyAPI-NET/docs/auth_introduction
        /// 
        /// Create App here: https://developer.spotify.com/dashboard/
        /// </summary>
        public string SpotifyUserId { get; set; }
        public string SpotifyUserPassword { get; set; }

        /// <summary>
        /// Create Account and DynamoDB Table Named "UserVote" with PArtition-Key "UserId" and Order-Key "TrackId".
        /// https://www.youtube.com/watch?v=BbUmLRaxZG8
        /// </summary>
        public string AwsUserId { get; set; }
        public string AwsUserPassword { get; set; }
    }
}
