using System;
using System.Threading.Tasks;
using LT.DigitalOffice.GUI.Services.ApiClients.FileService;

namespace LT.DigitalOffice.GUI.Services.Interfaces
{
    public interface IFileService
    {
        Task<OperationResultResponseImageInfo> GetUserAvatarAsync();
    }
}