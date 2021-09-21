using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Blazored.SessionStorage;
using LT.DigitalOffice.GUI.Helpers;
using LT.DigitalOffice.GUI.Services.ApiClients.MessageService;
using LT.DigitalOffice.GUI.Services.Interfaces;

namespace LT.DigitalOffice.GUI.Services
{
    public class MessageService : IMessageService
    {
        private readonly RefreshTokenHelper _refreshToken;
        private readonly MessageServiceClient _client;
        private readonly ISessionStorageService _storage;

        public MessageService(ISessionStorageService storage, IAuthService authService)
        {
            _storage = storage;
            _refreshToken = new(authService, storage);
            _client = new MessageServiceClient(new HttpClient());
        }

        public async Task<OperationResultResponse> CreateWorkspaceAsync(CreateWorkspaceRequest request)
        {
            await _refreshToken.RefreshAsync();
            var token = await _storage.GetItemAsync<string>(Consts.AccessToken);

            return await _client.CreateWorkspaceAsync(request, token);
        }

        public async Task<FindResultResponseShortWorkspaceInfo> FindWorkspaceAsync(
            int skipCount, 
            int takeCount, 
            bool? includeDeactivated = false)
        {
            await _refreshToken.RefreshAsync();
            var token = await _storage.GetItemAsync<string>(Consts.AccessToken);

            return await _client.FindWorkspaceAsync(token, skipCount, takeCount, includeDeactivated);
        }

        public async Task<OperationResultResponseWorkspaceInfo> GetWorkspaceAsync(Guid workspaceId, 
            bool? includeUsers = false ,
            bool? includeChannels = false)
        {
            await _refreshToken.RefreshAsync();
            var token = await _storage.GetItemAsync<string>(Consts.AccessToken);

            return await _client.GetWorkspaceAsync(token, workspaceId, includeUsers, includeChannels);
        }

        public async Task<OperationResultResponse> EditWorkspaceAsync(
            Guid workspaceId,
            List<PatchWorkspaceDocument> body)
        {
            await _refreshToken.RefreshAsync();
            var token = await _storage.GetItemAsync<string>(Consts.AccessToken);

            return await _client.EditWorkspaceAsync(body, token, workspaceId);
        }

        public async Task<OperationResultResponse> CreateChannelAsync(CreateChannelRequest request)
        {
            await _refreshToken.RefreshAsync();
            var token = await _storage.GetItemAsync<string>(Consts.AccessToken);

            return await _client.CreateChannelAsync(request, token);
        }
    }
}