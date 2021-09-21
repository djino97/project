using Blazored.SessionStorage;
using LT.DigitalOffice.GUI.Services.ApiClients.CompanyService;
using LT.DigitalOffice.GUI.Services.Interfaces;
using LT.DigitalOffice.GUI.Helpers;
using System.Net.Http;
using System.Threading.Tasks;

namespace LT.DigitalOffice.GUI.Services
{
    public class CompanyService : ICompanyService
    {
        private readonly RefreshTokenHelper _refreshToken;
        private readonly ISessionStorageService _storage;
        private readonly CompanyServiceClient _client;
        private string _token;

        public CompanyService(ISessionStorageService storage, IAuthService authService)
        {
            _storage = storage;
            _refreshToken = new(authService, storage);
            _client = new CompanyServiceClient(new HttpClient());
        }

        public async Task CreateDepartmentAsync(CreateDepartmentRequest request)
        {
            await _refreshToken.RefreshAsync();

            _token = await _storage.GetItemAsync<string>(Consts.AccessToken);

            await _client.AddDepartmentAsync(request, _token);
        }

        public async Task CreatePositionAsync(CreatePositionRequest request)
        {
            await _refreshToken.RefreshAsync();

            _token = await _storage.GetItemAsync<string>(Consts.AccessToken);

            await _client.AddPositionAsync(request, _token);
        }

        public async Task<FindResultResponseDepartmentInfo> FindDepartmentsAsync()
        {
            await _refreshToken.RefreshAsync();

            _token = await _storage.GetItemAsync<string>(Consts.AccessToken);

            return await _client.FindDepartmentsAsync(_token, 0, 100, false);
        }

        public async Task<FindResultResponsePositionInfo> FindPositionsAsync()
        {
            await _refreshToken.RefreshAsync();

            var token = await _storage.GetItemAsync<string>(Consts.AccessToken);

            return await _client.FindPositionsAsync(token, 0, 100, false);
        }

        public async Task<FindResultResponseOfficeInfo> FindOfficesAsync()
        {
            await _refreshToken.RefreshAsync();

            var token = await _storage.GetItemAsync<string>(Consts.AccessToken);

            return await _client.FindOfficesAsync(token, 0, int.MaxValue, null);
        }
    }
}
