using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using LT.DigitalOffice.GUI.Models.Filters;
using LT.DigitalOffice.GUI.Services.ApiClients.ProjectService;
using LT.DigitalOffice.GUI.Services.ApiClients.UserService;
using LT.DigitalOffice.GUI.Services.Interfaces;
using System.Linq;

namespace LT.DigitalOffice.GUI.Pages.ProjectTask
{
    public partial class TasksFilters
    {
        [Parameter]
        public Func<TasksFilter, Task> FindTaskByFilterParam { get; set; }

        private string _useFilterId;
        private bool _isSearchProject;
        private bool _isShowDropdownMenu;
        private Dictionary<string, bool> _stateFiltersSet;
        private Dictionary<string, bool> _searchStateByFilterParameters;
        private Func<ElementReference, Dictionary<string, bool>, bool> stateCheck =
            (elementRef, states) => states != null && states[elementRef.Id];
            
        private ElementReference _projectRef;
        private ElementReference _statusRef;
        private ElementReference _assignedToRef;

        private TasksFilter _tasksFilters;
        private FindResponseProjectInfo _projectsResponse;
        private FindResultResponseUserInfo _usersResponse;
        private FindResponseTaskProperty _taskPropertyResponse;

        [Inject]
        private IUserService _UserService { get; set; }

        [Inject]
        private IProjectService _ProjectService { get; set; }

        protected override void OnInitialized()
        {
            _tasksFilters = new();
        }

        protected override void OnAfterRender(bool firstRender)
        {
            if (firstRender)
            {
                _stateFiltersSet = new Dictionary<string, bool>
                {
                    {_projectRef.Id, false},
                    {_statusRef.Id, false},
                    {_assignedToRef.Id, false} 
                };

                _searchStateByFilterParameters = new Dictionary<string, bool>
                {
                    {_projectRef.Id, false},
                    {_statusRef.Id, false},
                    {_assignedToRef.Id, false}
                };
            }
        }

        private async Task RemoveFilter(bool isRemoveFilter, string elementId)
        {
            elementId = elementId ?? _useFilterId;
            _searchStateByFilterParameters[elementId] = false;

            if (elementId is null)
            {
                return;
            }

            if (string.Equals(elementId, _projectRef.Id))
            {
                _tasksFilters.ProjectId = null;
            }
            else if (string.Equals(elementId, _assignedToRef.Id))
            {
                _tasksFilters.AssignedTo = null;
            }
            else if (string.Equals(elementId, _statusRef.Id))
            {
                _tasksFilters.StatusId = null;
            }

            _stateFiltersSet[elementId] = false;

            await FindTaskByFilterParam(_tasksFilters);
        }

        private string GetStyleFiltersOfDropdown(ElementReference elementRef)
        {
            if (_stateFiltersSet is not null && !_stateFiltersSet[elementRef.Id])
            {
                return "dropdown-menu-item-hide";
            }

            return "dropdown-item";
        }
        
        private async Task SetFilter(ElementReference elementRef)
        {
            if (_searchStateByFilterParameters == null)
            {
                return;
            }

            _stateFiltersSet[elementRef.Id] = false;
            
            if (string.Equals(elementRef.Id, _projectRef.Id))
            {
                _projectsResponse = await _ProjectService.FindProjects(0, int.MaxValue);
            }
            else if (string.Equals(elementRef.Id, _statusRef.Id))
            {
                _taskPropertyResponse = await _ProjectService.GetTaskPropertiesAsync(0, int.MaxValue);

                _taskPropertyResponse.Body = _taskPropertyResponse.Body.Where(x => x.PropertyType == PropertyType.Status).ToList();
            }
            else if (string.Equals(elementRef.Id, _assignedToRef.Id))
            {
                _usersResponse = await _UserService.FindUsersAsync(0, int.MaxValue, null);
            }

            _isShowDropdownMenu = false;
        }

        private async Task SearchTask(string elementId, Func<TasksFilter, Task> findTask)
        {
            _searchStateByFilterParameters[elementId] = false; 
            _stateFiltersSet[elementId] = true; 

            await findTask(_tasksFilters);
        }
    }
}