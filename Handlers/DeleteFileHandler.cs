using System.Reflection.Metadata;
using FileUploadApi.Commands;
using FileUploadApi.Data;
using FileUploadApi.Dtos;
using FileUploadApi.Models;
using FileUploadApi.Service;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace FileUploadApi.Handlers;

public class DeleteFileHandler : IRequestHandler<DeleteFileRequest, BlobResponseDto>
{
    private readonly FileService _fileService;
     private readonly EmailService _emailService;
     private readonly IBlobRepo _blobRepo;

    public DeleteFileHandler(IOptions<EmailSettings> emailSettingsAccessor, IBlobRepo blobRepo)
    {
        _fileService = new FileService();
        _emailService = new EmailService(emailSettingsAccessor);
        _blobRepo = blobRepo;
    }
    
    public async Task<BlobResponseDto> Handle(DeleteFileRequest request, CancellationToken cancellationToken)
    {
        var result = await _fileService.DeleteAsync(request.FileName);
        if(!result.Error)
            {
                _blobRepo.DeleteFile(new BlobDto { Name= request.FileName});
                await _blobRepo.SaveChanges();

                EmailDto mail = new EmailDto{
                    EmailSubject = "File Deleted",
                    EmailBody= $"File {request.FileName} deleted Successfully",
                    EmailToId= "johntanil2000@gmail.com",
                    EmailToName= "John T Anil"
                };

                _emailService.SendEmail(mail);
            }
        return result;
    }
}