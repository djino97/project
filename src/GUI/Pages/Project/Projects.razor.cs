using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Components;
using LT.DigitalOffice.GUI.Models.Filters;
using Microsoft.AspNetCore.Components.Web;
using LT.DigitalOffice.GUI.Services.Interfaces;
using LT.DigitalOffice.GUI.Pages.Project.ProjectWindow;
using LT.DigitalOffice.GUI.Services.ApiClients.ProjectService;

namespace LT.DigitalOffice.GUI.Pages.Project
{
    public partial class Projects
    {
        private int _takeCount;
        private const int _takeCountDefault = 5;

        private Guid _projectId;
        private string _searchValue;
        private string _useFilterId;
        private bool _isShowDropdownMenu;
        private ProjectModalWindow _projectWindow;
        private ProjectsFilter _projectsFilter;
        private ElementReference _projectNameRef;
        private ElementReference _shortNameRef;
        private ElementReference _departmentRef;
        private Dictionary<string, bool> _showStateOfFiltersDropdown;
        private Func<ElementReference, Dictionary<string, bool>, bool> stateCheck =
            (elementRef, states) => states is not null && !states[elementRef.Id];

        private int _totalCount;
        private int _skipCount;
        private ICollection<ProjectInfo> _projectsInfo { get; set; }

        [Inject]
        private IProjectService _projectService { get; set; }

        protected override void OnInitialized()
        {
            _takeCount = _takeCountDefault;
            _isShowDropdownMenu = true;
            _projectsInfo = new List<ProjectInfo>();
            _projectsFilter = new ProjectsFilter();
        }

        protected override void OnAfterRender(bool firstRender)
        {
            // if (firstRender)
            // {
            //     _showStateOfFiltersDropdown = new Dictionary<string, bool>
            //     {
            //         {_projectNameRef.Id, true},
            //         {_shortNameRef.Id, true},
            //         {_departmentRef.Id, true}
            //     };
            // }
        }

        protected override async Task OnParametersSetAsync()
        {
            _isShowDropdownMenu = false;

            await GetProjectsAsync();
        }

        private string GetStyleFiltersOfDropdown(ElementReference elementRef)
        {
            if (_showStateOfFiltersDropdown is not null && !_showStateOfFiltersDropdown[elementRef.Id])
            {
                return "dropdown-menu-item-hide";
            }

            return "dropdown-item";
        }

        private string GetStyleFilter(string elementId)
        {
            return _useFilterId == elementId ? "set-filter" : "filter";
        }

        private void SetFilter(ElementReference elementRef)
        {
            _showStateOfFiltersDropdown[elementRef.Id] = false;

            _isShowDropdownMenu = false;
        }

        private void ShowDropdownMenu()
        {
            if (_showStateOfFiltersDropdown.Any())
            {
                _isShowDropdownMenu = true;
            }
        }

        private async Task ChangeProjectsCountAsync()
        {
            _skipCount += _takeCount;

            await GetProjectsAsync();
        }

        private async Task SearchProjectsAsync(KeyboardEventArgs args)
        {
            if (!string.Equals(args.Code, "Enter"))
            {
                return;
            }

            _skipCount = 0;
            _projectsInfo.Clear();

            SetValueToFilter(isRemoveFilter: false);

            await GetProjectsAsync();
        }

        private void SetValueToFilter(bool isRemoveFilter, string elementId = null)
        {
            elementId = elementId ?? _useFilterId;

            if (elementId is null)
            {
                return;
            }

            if (string.Equals(elementId, _projectNameRef.Id))
            {
                _projectsFilter.Name = isRemoveFilter ? null : _searchValue;
            }
            else if (string.Equals(elementId, _departmentRef.Id))
            {
                //_projectsFilter.Department = isRemoveFilter ? null : _searchValue;
            }
            else
            {
                _projectsFilter.ShortName = isRemoveFilter ? null : _searchValue;
            }

            _showStateOfFiltersDropdown[elementId] = isRemoveFilter;
        }

        private async Task GetProjectsAsync()
        {
            var responseProjects = await _projectService.FindProjects(
                _skipCount,
                _takeCount);

            if (_projectsInfo.Count > 0)
            {
                foreach (var project in responseProjects.Body)
                {
                    _projectsInfo.Add(project);
                }
            }
            else
            {
                _projectsInfo = responseProjects.Body;
                _totalCount = responseProjects.TotalCount;
            }
        }

        private async Task SetTakeCount(ChangeEventArgs e)
        {
            if (int.TryParse(e.Value.ToString(), out int result))
            {
                _skipCount = 0;
                _projectsInfo = new List<ProjectInfo>();

                _takeCount = result;
                await GetProjectsAsync();
            }
        }

        private async Task GetProjectsPageAsync(int skipCount)
        {
            _skipCount = skipCount;
            _projectsInfo = new List<ProjectInfo>();

            await GetProjectsAsync();
        }
    }
}