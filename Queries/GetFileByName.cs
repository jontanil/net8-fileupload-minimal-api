using FileUploadApi.Dtos;
using MediatR;

namespace FileUploadApi.Queries
{
    public class GetFileByNameQuery : IRequest<BlobDto>
    {
        public string FileName { get; set; }
        public GetFileByNameQuery(string fileName)
        {
            FileName = fileName;
        }
    }
}