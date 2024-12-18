using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Threading.Tasks;

namespace CarStore53.Azure.Storage
{
    public  class BlobService
    {
        private  readonly string _connectionString;
        private  readonly string _containerName;

        public BlobService()
        {

        }

        public  BlobService(IConfiguration configuration)
        {
            _connectionString = configuration["AzureBlobStorage:ConnectionString"];
            _containerName = configuration["AzureBlobStorage:ContainerName"];
        }

        public string UploadFileAsync(IFormFile file)
        {
            var containerClient = new BlobContainerClient(_connectionString, _containerName);
             containerClient.CreateIfNotExists();

            var blobClient = containerClient.GetBlobClient(file.FileName);
            using (var stream = file.OpenReadStream())
            {
                 blobClient.Upload(stream, true);
            }

            return blobClient.Uri.ToString();
        }

        public IFormFile ConvertToIFormFile(byte[] fileBytes, string fileName, string contentType)
        {
            var stream = new MemoryStream(fileBytes);
            var formFile = new FormFile(stream, 0, fileBytes.Length, "file", fileName)
            {
                Headers = new HeaderDictionary(),
                ContentType = contentType
            };
            return formFile;
        }


    }
}
