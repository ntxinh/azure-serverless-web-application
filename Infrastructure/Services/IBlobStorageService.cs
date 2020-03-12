using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Blob;

namespace Infrastructure.Services
{
    public interface IBlobStorageService
    {
        Task<CloudBlockBlob> UploadFromStream(string container_name, string blob_name, Stream stream);
        Task<Stream> GetWriteStream(string container_name, string blob_name);
        Task UploadChunk(string container_name, string blob_name, string block_id, Stream stream);
        Task<CloudBlockBlob> UploadChunkList(string container_name, string blob_name, string md5, IList<string> chunks);
        Task DownloadToStream(string container_name, string blob_name, Stream stream);
        Task DownloadRangeToStream(CloudBlockBlob blob, Stream stream, long? offset, long? length);
        Task<Stream> GetReadStream(string container_name, string blob_name);
        string GetReadableSasUri(string container_name, string blob_name, string filename, string encoding);
        string GetWriteableSasUri(string container_name, string blob_name, string ip_address = null);
        string GetBlobUrl(string container_name, string blob_name);
        Task<CloudBlockBlob> GetBlobAttributes(string container_name, string blob_name);
        Task Delete(string container_name, string blob_name);

    } // End of the interface
}
