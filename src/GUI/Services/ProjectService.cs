using System;
using Blazored.SessionStorage;
using LT.DigitalOffice.GUI.Helpers;
using LT.DigitalOffice.GUI.Services.ApiClients.ProjectService;
using LT.DigitalOffice.GUI.Services.Interfaces;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace LT.DigitalOffice.GUI.Services
{
    public class ProjectService : IProjectService
    {
        private readonly ISessionStorageService _storage;
        private readonly RefreshTokenHelper _refreshToken;
        private readonly ProjectServiceClient _projectServiceClient;

        public ProjectService(ISessionStorageService storage, IAuthService authService)
        {
            _storage = storage;
            _refreshToken = new(authService, storage);
            _projectServiceClient = new ProjectServiceClient(new HttpClient());
        }

        public async Task<FindResponseProjectInfo> FindProjects(
            int skipCount,
            int takeCount,
            Guid? departmentId = null)
        {
            FindResponseProjectInfo projectsResponse = null;
            try
            {
                await _refreshToken.RefreshAsync();
                var token = await _storage.GetItemAsync<string>(Consts.AccessToken);

                projectsResponse =  await _projectServiceClient.FindProjectsAsync(token, departmentId, skipCount, takeCount);
            }
            catch (ApiException<ErrorResponse> exc)
            {
                // TODO add exception handler
                throw;
            }

            return projectsResponse;
        }

        public async Task<OperationResultResponse> CreateProject(ProjectRequest request)
        {
            OperationResultResponse response = null;
            try
            {
                await _refreshToken.RefreshAsync();
                var token = await _storage.GetItemAsync<string>(Consts.AccessToken);

                response =  await _projectServiceClient.CreateProjectAsync(request, token);
            }
            catch (ApiException<ErrorResponse> exc)
            {
                // TODO add exception handler
                throw;
            }

            return response;
        }

        public async Task<FindResponseTaskProperty> GetTaskPropertiesAsync(
            int skipCount,
            int takeCount, 
            string name = null,
            Guid? authorId = null,
            Guid? projectId = null)
        {
            await _refreshToken.RefreshAsync();
            var token = await _storage.GetItemAsync<string>(Consts.AccessToken);

            return await _projectServiceClient.FindTaskPropertiesAsync(token, name, authorId, projectId, skipCount, takeCount);
        }

        public async Task<OperationResultResponseProjectResponse> GetProjectAsync(
            Guid projectId, 
            bool includeUsers = false, 
            bool includeFiles = false, 
            bool showNotActiveUsers = false)
        {
            await _refreshToken.RefreshAsync();
            var token = await _storage.GetItemAsync<string>(Consts.AccessToken);

            return await _projectServiceClient.GetProjectAsync(token, projectId, includeUsers, showNotActiveUsers, includeFiles);
        }

        public async Task<OperationResultResponse> CreateTaskAsync(CreateTaskRequest request)
        {
            await _refreshToken.RefreshAsync();
            var token = await _storage.GetItemAsync<string>(Consts.AccessToken);

            return await _projectServiceClient.CreateTaskAsync(request, token);
        }

        public async Task<OperationResultResponseTaskResponse> GetTaskAsync(Guid taskId)
        {
            await _refreshToken.RefreshAsync();
            var token = await _storage.GetItemAsync<string>(Consts.AccessToken);

            return await _projectServiceClient.GetTaskAsync(token, taskId);
        }

        public async Task<FindResponseTaskInfo> FindTasksAsync(
            int skipCount,
            int takeCount,
            int? number = null,
            Guid? projectId = null,
            Guid? statusId = null,
            Guid? assignedTo = null,
            bool onlyAuthorizedUser = false)
        {
            if (onlyAuthorizedUser)
            {
                assignedTo = await _storage.GetItemAsync<Guid>(Consts.UserId);
            }

            await _refreshToken.RefreshAsync();
            var token = await _storage.GetItemAsync<string>(Consts.AccessToken);

            return await _projectServiceClient.FindTasksAsync(token, number, projectId, assignedTo, statusId, skipCount, takeCount);
        }

        public async Task<OperationResultResponse> EditTaskAsync(IEnumerable<PatchDocument> body, Guid taskId)
        {
            var token = await _storage.GetItemAsync<string>(Consts.AccessToken);

            return await _projectServiceClient.EditTaskAsync(body, token, taskId);
        }
    }
}