using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FileUploadApi.Dtos
{

    public class BlobDto
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string? Uri { get; set; }
        public string? Name {get; set;}
        public string? ContentType { get; set; }
        public byte[]? Content { get; set; }
    }
}