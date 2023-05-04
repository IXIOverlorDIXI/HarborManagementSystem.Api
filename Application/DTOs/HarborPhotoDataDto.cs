using System;

namespace Application.DTOs;

public class HarborPhotoDataDto
{
    public string FileNameWithExtension { get; set; }
        
    public byte[] FileStream { get; set; }
        
    public Guid HarborId { get; set; }
}