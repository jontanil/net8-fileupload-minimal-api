using Amazon.Runtime.Internal;
using FileUploadApi.Dtos;
using MediatR;

namespace FileUploadApi.Commands;

public class DeleteFileRequest : IRequest<BlobResponseDto>
{
    public string FileName { get; set; }

    public DeleteFileRequest(string fileName)
    {
        FileName = fileName;
    }
}