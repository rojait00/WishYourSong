using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

namespace WishYourSong.Data
{
    public class User
    {

        public Guid Id { get; set; } = Guid.NewGuid();

        public bool IsAdmin { get; set; } = false;
        
        public User()
        {
        }
        
        public User(Guid id)
        {
            Id = id;
        }
        public async Task<Guid> GetOrGenerateUserIdAsync(ProtectedLocalStorage? browserStorage)
        {
            if (browserStorage == null)
            {
                throw new InvalidOperationException("_browserStorage has not been initialized");
            }

            var result = await browserStorage.GetAsync<string>("userId");
            if (result.Success == true && !string.IsNullOrEmpty(result.Value))
            {
                Id = Guid.Parse(result.Value);
            IsAdmin = Id == Guid.Empty;
            }
            else
            {
                Id = Guid.NewGuid();
                await browserStorage.SetAsync("userId", Id.ToString());
            }

            return Id;
        }

        internal async Task<bool> SetUserIdAsync(ProtectedLocalStorage? browserStorage, string? id)
        {
            if (browserStorage != null )
            {
                if (!Guid.TryParse(id, out Guid guid))
                {
                    guid = Guid.NewGuid();
                }

                await browserStorage.SetAsync("userId", guid.ToString());
                var newId = Id != guid;
                Id = guid;
                return newId;
            }

            return false;
        }
    }
}
