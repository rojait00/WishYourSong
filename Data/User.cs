using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

namespace WishYourSong.Data
{
    public class User
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public async Task<Guid> GetOrGenerateUserIdAsync(ProtectedLocalStorage? browserStorage)
        {
            if (browserStorage == null)
            {
                throw new InvalidOperationException("_browserStorage has not been initialized");
            }

            var result = await browserStorage.GetAsync<string>("userId");
            if (result.Success == true && result.Value != null)
            {
                Id = Guid.Parse(result.Value);
            }
            else
            {
                Id = Guid.NewGuid();
                await browserStorage.SetAsync("userId", Id.ToString());
            }

            return Id;
        }

        internal async Task SetUserIdAsync(ProtectedLocalStorage? browserStorage, string? id)
        {
            if (browserStorage != null && Guid.TryParse(id,out Guid guid))
            {
                await browserStorage.SetAsync("userId", id);
                Id = guid;
            }
        }
    }
}
