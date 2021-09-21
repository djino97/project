using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LT.DigitalOffice.GUI.Services.ApiClients.MessageService;

namespace LT.DigitalOffice.GUI.Services.Interfaces
{
    public interface IMessageService
    {
        Task<OperationResultResponse> CreateWorkspaceAsync(CreateWorkspaceRequest request);

        Task<FindResultResponseShortWorkspaceInfo> FindWorkspaceAsync(int skipCount, int takeCount, bool? includeDeactivated = false);
        
        Task<OperationResultResponseWorkspaceInfo> GetWorkspaceAsync(
            Guid workspaceId, 
            bool? includeUsers = false, 
            bool? includeChannels = false);

        Task<OperationResultResponse> EditWorkspaceAsync(
            Guid workspaceId,
            List<PatchWorkspaceDocument> body);
        
        Task<OperationResultResponse> CreateChannelAsync(CreateChannelRequest request);
    }
}