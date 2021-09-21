using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Components;
using LT.DigitalOffice.GUI.Services.Interfaces;
using LT.DigitalOffice.GUI.Services.ApiClients.UserService;
using LT.DigitalOffice.GUI.Services.ApiClients.ProjectService;

namespace LT.DigitalOffice.GUI.Shared.UsersWindow
{
    public partial class AddUsers 
    {
        [Parameter]
        public bool IsShowEmployeesForm { get; set; }

        [Parameter]
        public ProjectRequest ProjectRequest { get; set; }

        [Parameter]
        public List<UserInfo> FoundEmployees { get; set; }

        [Parameter]
        public EventCallback<bool> IsShowEmployeesFormChanged { get; set; }

        [Parameter]
        public EventCallback<List<UserInfo>> FoundEmployeesChanged { get; set; }

        [Parameter]
        public EventCallback<ProjectRequest> ProjectRequestChanged { get; set; }

        private List<UserInfo> _selectedEmployees;
        private Dictionary<int, bool> _hideFoundEmployees;
        private Dictionary<int, bool> _hideSelectedEmployees;

        private Users _foundEmployeesComponent;
        private Users _selectedEmployeesComponent;

        private Dictionary<int, bool> _statesSelectedFoundEmployees;
        private Dictionary<int, bool> _statesSelectedAddedEmployees;

        [Inject]
        private IUserService _userService { get; set; }

        [Inject]
        private IProjectService _projectService { get; set; }

        protected override async Task OnInitializedAsync()
        {
            _selectedEmployees = new();
            _hideFoundEmployees = new();
            _hideSelectedEmployees = new();
            _statesSelectedFoundEmployees = new();
            _statesSelectedAddedEmployees = new();

            if (ProjectRequest.Users is null)
            {
                var response = await _userService.FindUsersAsync( 
                    skipCount:0, 
                    takeCount:int.MaxValue,
                    departmentId: ProjectRequest.DepartmentId);

                FoundEmployees = response.Body.ToList();
            }

            for (int i = 0; i < FoundEmployees.Count; i++)
            {
                _statesSelectedAddedEmployees.Add(i, false);
                _hideSelectedEmployees.Add(i, false);
                _hideFoundEmployees.Add(i, false);
                _statesSelectedFoundEmployees.Add(i, false);
            }

            await FoundEmployeesChanged.InvokeAsync(FoundEmployees);
        }

        protected override void OnAfterRender(bool firstRender)
        {
            if (ProjectRequest.Users is not null && firstRender)
            {
                foreach (var user in ProjectRequest.Users)
                {
                    int index = FoundEmployees.FindIndex(fe => fe.Id == user.UserId);
                    _statesSelectedFoundEmployees[index] = true;
                }

                _selectedEmployees.AddRange(_foundEmployeesComponent.AddFoundEmployees());
                ResetStatesSelectedAddedEmployees();
                StateHasChanged();
            }
        }

        private void ClickAddEmployees()
        {
            _selectedEmployees.AddRange(_foundEmployeesComponent.AddFoundEmployees());

            SaveEmployees(_selectedEmployees);
            ResetStatesSelectedAddedEmployees();
        }

        private void ClickRemoveSelectedEmployees()
        {
            var userIds = _selectedEmployeesComponent.RemoveSelectedEmployees(out List<UserInfo> employees);

            foreach (var id in userIds)
            {
                var index = FoundEmployees.FindIndex(x => x.Id == id);
                _hideFoundEmployees[index] = false;
            }

            SaveEmployees(employees);
            ResetStatesSelectedAddedEmployees();
        }

        private void SaveEmployees(List<UserInfo> employees)
        {
            ProjectRequest.Users = employees
                .Select(x =>
                    new ProjectUserRequest()
                    {
                        UserId = x.Id
                    })
                .ToList();
            
            ProjectRequestChanged.InvokeAsync(ProjectRequest);
        }

        private void ResetStatesSelectedAddedEmployees()
        {
            _statesSelectedAddedEmployees.Clear();
            for (int i = 0; i < _selectedEmployees.Count; i++)
            {
                _statesSelectedAddedEmployees.Add(i, false);
            }
        }
    }
}
