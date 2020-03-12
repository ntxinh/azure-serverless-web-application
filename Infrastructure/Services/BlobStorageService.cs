using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace Infrastructure.Services
{
    public class BlobStorageService : IBlobStorageService
    {
        #region Variables

        private readonly ILogger logger;
        private readonly BlobStorageOptions options;
        private readonly CloudBlobClient client;

        #endregion

        #region Constructors

        public BlobStorageService(ILogger<BlobStorageService> logger, IOptions<BlobStorageOptions> options)
        {
            // Set values for instance variables
            this.logger = logger;
            this.options = options.Value;

            // Get a storage account
            CloudStorageAccount account = CloudStorageAccount.Parse(this.options.ConnectionString);

            // Get a client
            this.client = account.CreateCloudBlobClient();

        } // End of the constructor

        #endregion

        #region Add methods

        public async Task<CloudBlockBlob> UploadFromStream(string container_name, string blob_name, Stream stream)
        {
            // Get a container reference
            CloudBlobContainer container = GetContainerReference(container_name);

            // Get a blob reference
            CloudBlockBlob blob = container.GetBlockBlobReference(blob_name);

            // Catch exceptions to make sure that the stream is disposed
            try
            {
                // Write to the blob
                await blob.UploadFromStreamAsync(stream);
            }
            catch(Exception ex)
            {
                // Log the exception
                logger.LogError(ex, $"UploadFromStream: {blob_name}", null);
            }
            finally
            {
                // Dispose of the stream
                stream.Dispose();
            }

            // Fetch blob properties
            await blob.FetchAttributesAsync();

            // Return the blob
            return blob;

        } // End of the UploadFromStream method

        public async Task<Stream> GetWriteStream(string container_name, string blob_name)
        {
            // Get a container reference
            CloudBlobContainer container = GetContainerReference(container_name);

            // Get a blob object
            CloudBlockBlob blob = container.GetBlockBlobReference(blob_name);
            blob.StreamWriteSizeInBytes = 500 * 1024;

            //blockBlob.Properties.ContentType = contentType;

            // Create a stream
            CloudBlobStream stream = null;

            try
            {
                stream = await blob.OpenWriteAsync();
            }
            catch(Exception ex)
            {
                // Log the exception
                logger.LogError(ex, $"GetWriteStream: {blob_name}", null);
            }

            // Return a stream
            return stream;

        } // End of the GetWriteStream method

        public async Task UploadChunk(string container_name, string blob_name, string block_id, Stream stream)
        {
            // Get a container reference
            CloudBlobContainer container = GetContainerReference(container_name);

            // Get a blob reference
            CloudBlockBlob blob = container.GetBlockBlobReference(blob_name);

            // Catch exceptions to make sure that the stream is disposed
            try
            {
                // Write to the blob
                await blob.PutBlockAsync(block_id, stream, null);
            }
            catch (Exception ex)
            {
                // Log the exception
                logger.LogError(ex, $"UploadChunk: {blob_name}", null);
            }
            finally
            {
                // Dispose of the stream
                stream.Dispose();
            }

        } // End of the UploadChunk method

        public async Task<CloudBlockBlob> UploadChunkList(string container_name, string blob_name, string md5, IList<string> chunks)
        {
            // Get a container reference
            CloudBlobContainer container = GetContainerReference(container_name);

            // Get a blob reference
            CloudBlockBlob blob = container.GetBlockBlobReference(blob_name);

            // Catch exceptions to make sure that the stream is disposed
            try
            {
                // Upload a list with block id
                blob.Properties.ContentMD5 = md5;
                await blob.PutBlockListAsync(chunks);
            }
            catch (Exception ex)
            {
                // Log the exception
                logger.LogError(ex, $"UploadChunkList: {blob_name}", null);
            }

            // Fetch blob properties
            await blob.FetchAttributesAsync();

            // Return the blob
            return blob;

        } // End of the UploadChunkList method

        #endregion

        #region Get methods

        public async Task DownloadToStream(string container_name, string blob_name, Stream stream)
        {
            // Get a container reference
            CloudBlobContainer container = GetContainerReference(container_name);

            // Get a blob object
            CloudBlockBlob blob = container.GetBlockBlobReference(blob_name);

            // Download the blob to a stream
            try
            {
                await blob.DownloadToStreamAsync(stream);
            }
            catch(Exception ex)
            {
                // Log the exception
                logger.LogError(ex, $"DownloadToStream: {blob_name}", null);
            }

        } // End of the DownloadToStream method

        public async Task DownloadRangeToStream(CloudBlockBlob blob, Stream stream, long? offset, long? length)
        {
            // Download the blob to a stream
            try
            {
                await blob.DownloadRangeToStreamAsync(stream, offset, length);
            }
            catch (Exception ex)
            {
                // Log the exception
                logger.LogError(ex, $"DownloadRangeToStream", null);
            }

        } // End of the DownloadRangeToStream method

        public async Task<Stream> GetReadStream(string container_name, string blob_name)
        {
            // Get a container reference
            CloudBlobContainer container = GetContainerReference(container_name);

            // Get a blob object
            CloudBlockBlob blob = container.GetBlockBlobReference(blob_name);
            blob.StreamMinimumReadSizeInBytes = 500 * 1024;

            // Create a stream
            Stream stream = null;

            try
            {
                stream = await blob.OpenReadAsync();
            }
            catch (Exception ex)
            {
                // Log the exception
                logger.LogError(ex, $"GetReadStream: {blob_name}", null);
            }

            // Return the stream
            return stream;

        } // End of the GetReadStream method

        public string GetReadableSasUri(string container_name, string blob_name, string filename, string encoding)
        {
            // Get a container reference
            CloudBlobContainer container = GetContainerReference(container_name);

            // Get a blob object
            CloudBlockBlob blob = container.GetBlockBlobReference(blob_name);

            // Set content type
            // string content_type = MimeTypes.GetMimeType(filename);
            string content_type = GetMimeType(filename);
            if (encoding == "ascii" || encoding == "utf-8" || encoding == "utf-16" || encoding == "utf-32")
            {
                content_type += "; charset=" + encoding;
            }

            // Create a SAS token
            string sasToken = blob.GetSharedAccessSignature(new SharedAccessBlobPolicy()
            {
                // Assuming the blob can be downloaded in 5 minutes
                Permissions = SharedAccessBlobPermissions.Read,
                SharedAccessExpiryTime = DateTime.UtcNow.AddMinutes(5)

            }, new SharedAccessBlobHeaders()
            {
                ContentDisposition = "attachment; filename=" + filename,
                ContentType = content_type

            }, null, SharedAccessProtocol.HttpsOnly, null);

            // Return the url
            return blob.Uri + sasToken;

        } // End of the GetReadableSasUri method

        public string GetWriteableSasUri(string container_name, string blob_name, string ip_address = null)
        {
            // Get a container reference
            CloudBlobContainer container = GetContainerReference(container_name);

            // Create an ip range
            IPAddressOrRange ip_range = string.IsNullOrEmpty(ip_address) == false ? new IPAddressOrRange(ip_address) : null;

            // Create a SAS token
            string sasToken = container.GetSharedAccessSignature(new SharedAccessBlobPolicy()
            {
                // Assuming the blob can be created in 24 hours
                Permissions = SharedAccessBlobPermissions.Read | SharedAccessBlobPermissions.Write | SharedAccessBlobPermissions.Create | SharedAccessBlobPermissions.Add | SharedAccessBlobPermissions.Delete,
                SharedAccessExpiryTime = DateTime.UtcNow.AddHours(24)

            }, null, SharedAccessProtocol.HttpsOnly, ip_range);

            // Return the url
            return container.Uri + "/" + blob_name + sasToken;

        } // End of the GetWriteableSasUri method

        public string GetBlobUrl(string container_name, string blob_name)
        {
            // Get the container name from the dictionary
            container_name = this.options.Containers[container_name];

            // Return the url
            return "https://mysite.blob.core.windows.net/" + container_name + "/" + blob_name;

        } // End of the GetBlobUrl method

        public async Task<CloudBlockBlob> GetBlobAttributes(string container_name, string blob_name)
        {
            // Get a container reference
            CloudBlobContainer container = GetContainerReference(container_name);

            // Get a blob reference
            CloudBlockBlob blob = container.GetBlockBlobReference(blob_name);

            // Fetch blob properties
            await blob.FetchAttributesAsync();

            // Return the blob
            return blob;

        } // End of the GetBlobAttributes method

        #endregion

        #region Delete methods

        public async Task Delete(string container_name, string blob_name)
        {
            // Get a container reference
            CloudBlobContainer container = GetContainerReference(container_name);

            // Get a blob object
            CloudBlockBlob blob = container.GetBlockBlobReference(blob_name);

            // Delete the blob
            try
            {
                await blob.DeleteIfExistsAsync();
            }
            catch(Exception ex)
            {
                // Log the exception
                logger.LogError(ex, $"Delete blob: {blob_name}", null);
            }

        } // End of the Delete method

        #endregion

        #region Helper methods

        private CloudBlobContainer GetContainerReference(string name)
        {
            // Get the name from the dictionary
            name = this.options.Containers[name];

            // Get a reference to a cloud blob contaner
            CloudBlobContainer container = this.client.GetContainerReference(name);

            // Create the container if it doesn't exist
            //await container.CreateIfNotExistsAsync();

            // Return the container reference
            return container;

        } // End of the GetContainerReference method

        #endregion

        private string GetMimeType (string fileName)
        {
            string mimeType = "application/unknown";
            string ext = System.IO.Path.GetExtension(fileName).ToLower();
            Microsoft.Win32.RegistryKey regKey = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey(ext);
            if (regKey != null && regKey.GetValue("Content Type") != null)
            mimeType = regKey.GetValue("Content Type").ToString();
            return mimeType;
        }

    } // End of the class
}
