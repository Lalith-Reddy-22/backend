using Medtrix_PmTool_V2_Api.DTOs.ResourceAllocation;

namespace Medtrix_PmTool_V2_Api.Interfaces.ResourceAllocation
{
    public interface IResourceAllocation
    {
        Task<ResourceAllocationResponseDto> GetResourceAllocationAsync(ResourceAllocationRequestDto request);
        Task<EmpTaskDetailResponseDto> GetEmpTaskDetailAsync(EmpTaskDetailRequestDto request);
    }
}
