using System;

namespace Application.DTOs;

public class BerthPhotoDataDto
{
    public string FileNameWithExtension { get; set; }
        
    public byte[] FileStream { get; set; }
        
    public Guid BerthId { get; set; }
}