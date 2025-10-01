using FileUploadApi.Dtos;

namespace FileUploadApi.Data
{
    public class BlobRepo : IBlobRepo
    {
        private readonly AppDbContext _context;

        public BlobRepo(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddFile(BlobDto blob)
        {
            await _context.AddAsync(blob);
        }

        public void DeleteFile(BlobDto? blob)
        {
            if(blob != null)
                blob = _context.Blob.Where(b => b.Name == blob.Name).FirstOrDefault();
                
            if (blob != null)
                _context.Remove(blob);
        }

        public async Task SaveChanges()
        {
            await _context.SaveChangesAsync();
        }
    }
}