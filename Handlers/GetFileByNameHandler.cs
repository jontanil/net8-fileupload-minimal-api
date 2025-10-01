using FileUploadApi.Dtos;
using FileUploadApi.Service;
using MediatR;

namespace FileUploadApi.Queries
{
    
    public class GetFileByNameHandler : IRequestHandler<GetFileByNameQuery, BlobDto>
    {
        private readonly FileService _fileService;
        public GetFileByNameHandler()
        {
            _fileService = new FileService();
        }

        public async Task<BlobDto> Handle(GetFileByNameQuery request, CancellationToken cancellationToken)
        {
            var result = await _fileService.DownloadAsync(request.FileName);

            if(result == null)
                return new BlobDto{ Content = null};

            return result;
        }
    }
}