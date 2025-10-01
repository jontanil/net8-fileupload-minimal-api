using FileUploadApi.Dtos;

namespace FileUploadApi.Data;

public interface IBlobRepo
{
    public Task SaveChanges();
    public Task AddFile(BlobDto blob);
    public void DeleteFile(BlobDto blob);

}
