using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Components;
using LT.DigitalOffice.GUI.Services.Interfaces;
using LT.DigitalOffice.GUI.Services.ApiClients.ProjectService;

namespace LT.DigitalOffice.GUI.Pages.ProjectTask.CreateTask
{
    public partial class CreateTaskModalWindow
    {
        private string _messageUser;
        
        private bool _isSuccessOperation;
        private Guid? _selectProjectId;
        private List<TaskInfo> _projectTasks;
        private List<ProjectInfo> _projects;
        private CreateTaskRequest _taskRequest;
        private List<ProjectUserInfo> _projectUsers;
        private List<TaskPropertyInfo> _taskStatuses;
        private List<TaskPropertyInfo> _taskTypes;
        private List<TaskPropertyInfo> _taskPriorities;

        [Inject]
        private IProjectService _ProjectService { get; set; }

        [Inject]
        private IUserService _UserService { get; set; }

        protected override void OnInitialized()
        {
            _taskTypes = new();
            _projectUsers = new();
            _taskStatuses = new();
            _taskPriorities = new();
            _taskRequest = new();
            _projectTasks = new();
            
            _projects = _projects ?? new();
        }
        
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                var projectsResponse = await _ProjectService.FindProjects(0, int.MaxValue);
                _projects = projectsResponse.Body.ToList();

                StateHasChanged();
            }
        }

        private async Task GetTaskPropertiesAsync(ChangeEventArgs arg)
        {
            _taskTypes = new();
            _projectUsers = new();
            _taskStatuses = new();
            _taskPriorities = new();
            _projectTasks = new();

            _selectProjectId = Guid.Parse(arg.Value.ToString());
            _taskRequest.ProjectId = _selectProjectId.Value;

            var taskResponse = await _ProjectService.FindTasksAsync(0, int.MaxValue, projectId: _selectProjectId);
            _projectTasks = taskResponse.Body.Where(x => 
                string.Equals("Feature", x.TypeName) || string.Equals("Task", x.TypeName)).ToList();

            var projectResponse = await _ProjectService.GetProjectAsync(_selectProjectId.Value, includeUsers: true);
            _projectUsers = projectResponse.Body.Users.ToList();

            var taskPropertiesResponse = (await _ProjectService.GetTaskPropertiesAsync(
                skipCount:0, 
                takeCount: int.MaxValue, 
                projectId: _selectProjectId.Value)).Body.ToList();
            
            var defaultTaskProperties = (await _ProjectService.GetTaskPropertiesAsync(
                skipCount:0, 
                takeCount: int.MaxValue, 
                projectId: Guid.Empty)).Body.ToList();

            taskPropertiesResponse.AddRange(defaultTaskProperties.Select(x => x));

            ParseProperties(taskPropertiesResponse);
        }

        private void ParseProperties(List<TaskPropertyInfo> properties)
        {
            foreach (var property in properties)
            {
                if (PropertyType.Type == property.PropertyType)
                {
                    _taskTypes.Add(property);
                }
                else if (PropertyType.Status == property.PropertyType)
                {
                    _taskStatuses.Add(property);
                }
                else if (PropertyType.Priority == property.PropertyType)
                {
                    _taskPriorities.Add(property);
                }
            }
        }

        public async Task HandleSubmit()
        {
            _messageUser = null;
            OperationResultResponse response = null;
            _isSuccessOperation = false;

            try
            {
                response = await _ProjectService.CreateTaskAsync(_taskRequest);

                if (response.Status == OperationResultStatusType.FullSuccess)
                {
                    _messageUser = $"The task was created successfully!";
                    _isSuccessOperation = true;
                    
                    return;
                }

                _messageUser = $"Something went wrong, please try again later.\nMessage: { response.Errors }";
            }
            catch (ApiException<ErrorResponse> ex)
            {   
                _isSuccessOperation = false;
                _messageUser = $"Something went wrong, please try again later.\nMessage: { ex.Result.Message }";
            }
        }
    }
}