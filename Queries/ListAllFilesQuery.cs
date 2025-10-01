using FileUploadApi.Dtos;
using MediatR;

namespace FileUploadApi.Queries;

public class ListAllFilesQuery : IRequest<List<BlobDto>>
{
   
}