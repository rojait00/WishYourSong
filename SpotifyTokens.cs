using SpotifyAPI.Web;

namespace WishYourSong
{
    public class SpotifyTokens
    {
        /// <summary>
        /// To Do: Replace with your own values
        /// Follow this instructions to get the credentials and create the specified files
        /// https://johnnycrazy.github.io/SpotifyAPI-NET/docs/auth_introduction
        /// 
        /// Create App here: https://developer.spotify.com/dashboard/
        /// </summary>
        public static SpotifyClientConfig OAuth { get { return GetOAuth(); } }

        private static SpotifyClientConfig GetOAuth()
        {
            string id = File.ReadAllText(@"C:\WishYourSong\clientId.txt").Trim();
            string pass = File.ReadAllText(@"C:\WishYourSong\clientSecret.txt").Trim();

            return SpotifyClientConfig
                .CreateDefault()
                .WithAuthenticator(new ClientCredentialsAuthenticator(id, pass));
        }
    }
}
