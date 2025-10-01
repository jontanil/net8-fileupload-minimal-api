using FileUploadApi.Dtos;
using FileUploadApi.Queries;
using FileUploadApi.Service;
using MediatR;

namespace FileUploadApi.Handlers;

public class ListAllFilesHandler : IRequestHandler<ListAllFilesQuery, List<BlobDto>>
{
    private readonly FileService _fileService;
    public ListAllFilesHandler()
    {
        _fileService = new FileService();
    }
    public async Task<List<BlobDto>> Handle(ListAllFilesQuery request, CancellationToken cancellationToken)
    {
        var result = await _fileService.ListAsync();

        return result;
    }
}