using FileUploadApi.Dtos;
using MediatR;

namespace FileUploadApi.Commands;

public class UploadFileRequest : IRequest<BlobResponseDto>
{
    public IFormFile File { get; set;}

    public UploadFileRequest(IFormFile file)
    {
        File = file;
    }
}