using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Components;
using LT.DigitalOffice.GUI.Services.Interfaces;
using LT.DigitalOffice.GUI.Services.ApiClients.ProjectService;
using LT.DigitalOffice.GUI.Pages.ProjectTask.TaskWindow;
using LT.DigitalOffice.GUI.Models.Filters;

namespace LT.DigitalOffice.GUI.Pages.ProjectTask
{
    public partial class Tasks
    {
        private int _takeCount = 8;
        public const string UserLinkStyle = "text-decoration: none; color: #0b5ed7;";

        private Guid _taskId;
        private int _totalCount;
        private int _skipCount;
        private bool _onlyAuthorizedUser = true;

        public TasksFilter _taskFilter; 
        private List<TaskInfo> _tasks;
        private TaskModalWindow _taskModalWindow;

        [Inject]
        private IProjectService _ProjectService { get; set; }

        [Inject]
        private IUserService _UserService { get; set; }
        
        protected override async Task OnInitializedAsync()
        {
            _tasks = new();
            _taskFilter = new();

            await GetTasksAsync();
        }

        private async Task GetTasksAsync()
        {
            var tasksResponse = await _ProjectService.FindTasksAsync(
                skipCount: _skipCount, 
                takeCount: _takeCount,
                projectId: _taskFilter.ProjectId,
                statusId:  _taskFilter.StatusId,
                assignedTo: _taskFilter.AssignedTo, 
                onlyAuthorizedUser: _onlyAuthorizedUser);

            _tasks.AddRange(tasksResponse.Body.ToList());
            
            _totalCount = tasksResponse.TotalCount;
            _skipCount += _takeCount;
        }

        private string GetTaskTypeStyle(string typeName)
        {
            if (string.Equals("Feature", typeName))
            {
                return "color: green;";
            }
            else if (string.Equals("Bug", typeName))
            {
                return "color: red;";
            }
            else
            {
                return "color: blue";
            }
        }

        private async Task GetTasksPageAsync(int skipCount)
        {
            _skipCount = skipCount;
            _tasks = new List<TaskInfo>();

            await GetTasksAsync();
        }

        private async Task GetUserTasksByAllProjects()
        {
            var user = await _UserService.GetAuthorizedUserAsync(includeProjects: true);
        }

        private async Task SetTakeCount(ChangeEventArgs e)
        {
            if (int.TryParse(e.Value.ToString(), out int result))
            {
                _skipCount = 0;
                _tasks = new List<TaskInfo>();

                _takeCount = result;
                await GetTasksAsync();
            }
        }

        private async Task RefreshTasks(bool onlyAuthorizedUser)
        {
            _onlyAuthorizedUser = onlyAuthorizedUser;
            _tasks = new List<TaskInfo>();
            _skipCount = 0;

            await GetTasksAsync();
        }

        private async Task SetFilterParam(TasksFilter taskFilter)
        {
            _taskFilter = taskFilter;

            _skipCount = 0;
            _tasks = new List<TaskInfo>();
            await GetTasksAsync();

            StateHasChanged();
        }
    }
}