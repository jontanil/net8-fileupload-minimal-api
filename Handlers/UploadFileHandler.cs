using FileUploadApi.Commands;
using FileUploadApi.Data;
using FileUploadApi.Dtos;
using FileUploadApi.Models;
using FileUploadApi.Service;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace FileUploadApi.Handlers;

public class UploadFileHandler : IRequestHandler<UploadFileRequest, BlobResponseDto>
{
    private readonly FileService _fileService;
    private readonly EmailService _emailService;
    private readonly IBlobRepo _blobRepo;

    public UploadFileHandler(IOptions<EmailSettings> emailSettingsAccessor, IBlobRepo blobRepo)
    {
        _fileService = new FileService();
        _emailService = new EmailService(emailSettingsAccessor);
        _blobRepo = blobRepo;
    }

    public async Task<BlobResponseDto> Handle(UploadFileRequest request, CancellationToken cancellationToken)
    {
        var result  = await _fileService.UploadAsync(request.File);
        if(!result.Error)
            {
                await using(MemoryStream memoryStream = new MemoryStream())
                {
                    await request.File.CopyToAsync(memoryStream, cancellationToken);
                    result.Blob.Content = memoryStream.ToArray();
                }

                 await _blobRepo.AddFile(result.Blob);
                 await _blobRepo.SaveChanges();

                EmailDto mail = new EmailDto{
                    EmailSubject = "File Uploaded",
                    EmailBody= $"File {request.File.FileName} Uploaded Successfully",
                    EmailToId= "johntanil2000@gmail.com",
                    EmailToName= "John T Anil"
                };

                _emailService.SendEmail(mail);
            }
        return result;
    }
}
