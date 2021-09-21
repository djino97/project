using System;
using System.Net.Http;
using System.Threading.Tasks;
using Blazored.SessionStorage;
using LT.DigitalOffice.GUI.Helpers;
using LT.DigitalOffice.GUI.Services.ApiClients.FileService;
using LT.DigitalOffice.GUI.Services.Interfaces;

namespace LT.DigitalOffice.GUI.Services
{
    public class FileService : IFileService
    {
        private readonly RefreshTokenHelper _refreshToken;
        private readonly FileServiceClient _client;
        private readonly ISessionStorageService _storage;

        public FileService(ISessionStorageService storage, IAuthService authService)
        {
            _storage = storage;
            _refreshToken = new(authService, storage);
            _client = new FileServiceClient(new HttpClient());
        }

        public async Task<OperationResultResponseImageInfo> GetUserAvatarAsync()
        {
            await _refreshToken.RefreshAsync();

            var token = await _storage.GetItemAsync<string>(Consts.AccessToken);

            Guid imageId = await _storage.GetItemAsync<Guid>(Consts.UserAvatarId);

            if (imageId == Guid.Empty)
            {
                return null;
            }

            return await _client.GetImageAsync(token, imageId);
        }
    }
}