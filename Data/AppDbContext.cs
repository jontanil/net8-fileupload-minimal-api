using FileUploadApi.Dtos;
using Microsoft.EntityFrameworkCore;

namespace FileUploadApi.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
        
    }

    public DbSet<BlobDto> Blob => Set<BlobDto>();
}