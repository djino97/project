using System;
using System.IO;
using System.Threading.Tasks;
using LT.DigitalOffice.GUI.Services.ApiClients.ProjectService;
using LT.DigitalOffice.GUI.Services.Interfaces;
using Microsoft.AspNetCore.Components;

namespace LT.DigitalOffice.GUI.Pages.Project.ProjectWindow
{
    public partial class ProjectModalWindow
    {
        private ProjectResponse _project;

        [Inject]
        private IProjectService _ProjectService { get; set; }

        public async Task GetProjectAsync(Guid projectId)
        {
            var response = await _ProjectService.GetProjectAsync(projectId, includeUsers: true, includeFiles: true);
            _project = response.Body;

            StateHasChanged();
        }
    }
}