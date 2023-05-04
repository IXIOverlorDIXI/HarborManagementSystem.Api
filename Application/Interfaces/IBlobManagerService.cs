using System;
using System.IO;
using System.Threading.Tasks;
using BlobType = Domain.Enums.BlobType;

namespace Application.Interfaces
{
    public interface IBlobManagerService
    {
        Task<string> UploadToBlobStorageAsync(Guid guid, string fileNameWithExtension, Stream file, BlobType type);
        
        Task<bool> RemoveFromBlobStorageAsync(string url);
    }
}
