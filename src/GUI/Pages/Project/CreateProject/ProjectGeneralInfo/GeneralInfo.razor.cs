using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Components;
using LT.DigitalOffice.GUI.Services.Interfaces;
using LT.DigitalOffice.GUI.Services.ApiClients.ProjectService;
using Company = LT.DigitalOffice.GUI.Services.ApiClients.CompanyService;

namespace LT.DigitalOffice.GUI.Pages.Project.CreateProject.ProjectGeneralInfo
{
    public partial class GeneralInfo
    {
        [Parameter]
        public ProjectRequest ProjectRequest { get; set; }

        [Parameter]
        public bool IsShowEmployeesForm { get; set; }

        [Parameter]
        public EventCallback<bool> IsShowEmployeesFormChanged { get; set; }

        [Parameter]
        public EventCallback<ProjectRequest> ProjectRequestChanged { get; set; }

        private string _departmentValue;
        private string _validationMessage;
        private string _statusValidationMessage;
        private string _fullNameValidationMessage;
        private string _shortNameValidationMessage;
        private string _departmentValidationMessage;

        private List<Company.DepartmentInfo> _departments;

        [Inject]
        private ICompanyService companyService { get; set; }

        protected override async Task OnInitializedAsync()
        {
            if (ProjectRequest is null)
            {
                ProjectRequest = new();
            }

            _departments = (await companyService.FindDepartmentsAsync()).Body.ToList()
                ?? new List<Company.DepartmentInfo>();
        }

        public void HandleSubmit()
        {
            IsShowEmployeesFormChanged.InvokeAsync(true);
            ProjectRequestChanged.InvokeAsync(ProjectRequest);
        }
    }
}
