using System.IO;

namespace Application.DTOs
{
    public class ProfilePhotoDataDto
    {
        public string FileNameWithExtension { get; set; }
        
        public byte[] FileStream { get; set; }
    }
}