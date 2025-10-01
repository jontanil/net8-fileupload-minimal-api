using FileUploadApi.Dtos;
using Amazon.S3;
using Amazon;
using Amazon.Runtime;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using Microsoft.AspNetCore.Mvc;

namespace FileUploadApi.Service;

public class FileService
{
    private readonly string _accessKeyId = "your-access-key";
    private readonly string _secretAccessKey  = "your-secret-access-key";
    private readonly string _bucketName = "bucket-name";

    private readonly AmazonS3Client _s3Client;

    public FileService()
    {
       BasicAWSCredentials credentials = new BasicAWSCredentials(_accessKeyId, _secretAccessKey);
        _s3Client = new AmazonS3Client(credentials, RegionEndpoint.EUNorth1);
    }

    public async Task<List<BlobDto>> ListAsync()
    {
        List<BlobDto> files = new List<BlobDto>();
        var response = await _s3Client.ListObjectsAsync(_bucketName);

        foreach(var file in response.S3Objects)
        {
            string uri = _bucketName;
            var name = file.Key; 
            var fullUri = $"{uri}/{name}";

            files.Add( new BlobDto{
                Uri= fullUri,
                Name= name,
                ContentType= file.StorageClass
            });
        }
        

        return files;
    }

    public async Task<BlobResponseDto> UploadAsync( IFormFile file)
    {
        BlobResponseDto response = new BlobResponseDto();
        try
        {
            var request = new TransferUtilityUploadRequest
            {
                BucketName = _bucketName,
                Key = file.FileName,
                InputStream = file.OpenReadStream() 
            };

            using (var transferUtility = new TransferUtility(_s3Client))
            {
                await transferUtility.UploadAsync(request);
            }

            response.Status = $"File {file.FileName} Uploaded Successfully";
            response.Error = false;
            response.Blob.Uri = $"https://{_bucketName}.s3.amazonaws.com/{file.FileName}";;
            response.Blob.Name = file.FileName;
            response.Blob.ContentType = "application/octet-stream";
        }
        catch
        {
            response.Status = $"Could not upload {file.FileName}!";
            response.Error = true;
        }

        return response;
    }

    public async Task<BlobDto> DownloadAsync(string fileName)
    {
        try
        {
            var memoryStream = new MemoryStream();

            using (var response = await _s3Client.GetObjectAsync(_bucketName, fileName))
            {
                await response.ResponseStream.CopyToAsync(memoryStream);

            }
            memoryStream.Position = 0;
            var fileResult = new FileStreamResult(memoryStream, "application/octet-stream")
            {
                FileDownloadName = fileName
            };
            var fileBytes = memoryStream.ToArray();
             
            return new BlobDto{Content = fileBytes, ContentType = "application/octet-stream", Name = fileName};
        }
        catch(Exception Ex)
        {
            Console.WriteLine(Ex.Message);
            return new BlobDto{Content = null};
        }
    }

    public async Task<BlobResponseDto> DeleteAsync(string fileName)
    {
        try
        {
            var checkRequest = new GetObjectMetadataRequest{
                BucketName = _bucketName,
                Key = fileName
            };
            await _s3Client.GetObjectMetadataAsync(checkRequest);

            DeleteObjectRequest request = new DeleteObjectRequest
            {
                BucketName = _bucketName,
                Key = fileName
            };
        
            await _s3Client.DeleteObjectAsync(request);
            return new BlobResponseDto {Error = false, Status = $"File {fileName} Deleted Successfully"};
        }
        catch
        {
            return new BlobResponseDto {Error = true, Status = $"File {fileName} Not Found"};
        }
    }
}
