using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using LT.DigitalOffice.GUI.Services.Interfaces;
using LT.DigitalOffice.GUI.Services.ApiClients.ProjectService;
using System.Collections.Generic;
using System.Linq;

namespace LT.DigitalOffice.GUI.Pages.ProjectTask.TaskWindow
{
    public partial class EditTaskWindow
    {
        private string _messageUser;

        private List<PatchDocument> _requestBody;
        private List<TaskInfo> _projectTasks;
        private List<ProjectInfo> _projects;
        private CreateTaskRequest _taskRequest;
        private List<ProjectUserInfo> _projectUsers;
        private List<TaskPropertyInfo> _taskStatuses;
        private List<TaskPropertyInfo> _taskTypes;
        private List<TaskPropertyInfo> _taskPriorities;
        public OperationResultResponse _editTaskResponse;
        
        [Parameter]
        public TaskResponse Task { get; set; }

        [Parameter]
        public Guid TaskId { get; set; }

        [Inject]
        private IProjectService _ProjectService {get; set;}

        protected override void OnInitialized()
        {
            _requestBody = new();
            _taskTypes = new();
            _projectUsers = new();
            _taskStatuses = new();
            _taskPriorities = new();
            _projectTasks = new();
            _projects = new();
        }

        protected override async Task OnInitializedAsync()
        {
            _projects = (await _ProjectService.FindProjects(0, int.MaxValue)).Body.ToList();
            await GetTaskPropertiesAsync(); 
        }

        private void SetValueToPatchDocument(PatchDocumentPath path, object value)
        {
            PatchDocument patch = _requestBody.FirstOrDefault(x => x.Path == path);

            if (patch is null)
            {
                _requestBody.Add(
                    new PatchDocument
                    {
                        Op = PatchDocumentOp.Replace,
                        Path = path,
                        Value = value
                    }
                );
            }
            else
            {
                patch.Value = value;
            }
        }

        private async Task GetTaskPropertiesAsync()
        {
            var taskResponse = await _ProjectService.FindTasksAsync(0, int.MaxValue, projectId: Task.Project.Id);
            _projectTasks = taskResponse.Body.ToList();

            var projectResponse = await _ProjectService.GetProjectAsync(Task.Project.Id, includeUsers: true);
            _projectUsers = projectResponse.Body.Users.ToList();

            var taskPropertiesResponse = (await _ProjectService.GetTaskPropertiesAsync(
                skipCount:0, 
                takeCount: int.MaxValue, 
                projectId: Task.Project.Id)).Body.ToList();
            
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

        private async Task EditTask()
        {
            _messageUser = null;
            _editTaskResponse = null;

            if (!_requestBody.Any())
            {
                _messageUser = "The task has not changes";
                return;
            }

            try
            {
                _editTaskResponse = await _ProjectService.EditTaskAsync(_requestBody, Task.Id);

                _messageUser = _editTaskResponse.Status == OperationResultStatusType.FullSuccess ? 
                    "Changes were saved successfully!" : 
                    $"Something went wrong, please try again later.\nMessage: { _editTaskResponse.Errors }";
            }
            catch (ApiException ex)
            {
                _messageUser = $"Something went wrong, please try again later.\nMessage: { ex.Response }";
            }

            _requestBody = new();
        }
    }
}