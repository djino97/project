using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using LT.DigitalOffice.GUI.Services.Interfaces;
using LT.DigitalOffice.GUI.Services.ApiClients.ProjectService;
using System.Linq;
using Blazored.SessionStorage;
using LT.DigitalOffice.GUI.Helpers;

namespace LT.DigitalOffice.GUI.Pages.ProjectTask.TaskWindow
{
    public partial class TaskModalWindow
    {
        private bool IsUserAdmin;
        private bool _isEditTask;
        private bool _isClickContentModal;

        private ProjectUserRoleType _userRole;
        private TaskResponse _task;

        [Inject]
        private IProjectService _ProjectService { get; set; }

        [Inject]
        private ISessionStorageService _Storage { get; set; }

        private void CheckEditingTasks()
        {
            _isEditTask = _isClickContentModal is true && _isEditTask;
            _isClickContentModal = false;
        }

        protected override void OnInitialized()
        {
            _task = new();
        }

        public async Task GetTaskAsync(Guid taskId)
        {
            var taskResponse = await _ProjectService.GetTaskAsync(taskId);
            _task = taskResponse.Body;
            
            var project = await _ProjectService.GetProjectAsync(taskResponse.Body.Project.Id, includeUsers: true);

            Guid userId = await _Storage.GetItemAsync<Guid>(Consts.UserId);
            _userRole = project.Body.Users.FirstOrDefault(x => x.Id == userId).Role;

            IsUserAdmin = await _Storage.GetItemAsync<bool>(Consts.IsUserAdmin);

            StateHasChanged();
        }
    }
}