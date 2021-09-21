using LT.DigitalOffice.GUI.Services.ApiClients.CompanyService;
using Microsoft.AspNetCore.Components;
using System;
using System.Threading.Tasks;

namespace LT.DigitalOffice.GUI.Pages.Admin.Company
{
    public partial class CreateDepartment
    {
        private CreateDepartmentRequest _departmentData = new();
        private string _message;
        private bool _isSuccessOperation;

        private ElementReference _descriptionInput;
        private ElementReference _directorSelect;

        private async Task HandleValidSubmit()
        {
            try
            {
                await companyService.CreateDepartmentAsync(_departmentData);

                _message = "Successfully created";
                _isSuccessOperation = true;
            }
            catch (ApiException<ErrorResponse> ex)
            {
                _message = ex.Result.Message;
            }
            catch (ApiException<OperationResultResponse> ex)
            {
                _message = String.Join(" ", ex.Result.Errors);
            }
            catch (ApiException ex)
            {
                _message = "Something went wrong";
            }

            StateHasChanged();
        }
    }
}
