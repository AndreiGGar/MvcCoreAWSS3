using Amazon.S3;
using Amazon.S3.Model;

namespace MvcCoreAWSS3.Services
{
    public class ServiceStorageS3
    {
        private string BucketName;
        private IAmazonS3 ClientS3;

        public ServiceStorageS3(IConfiguration configuration, IAmazonS3 clientS3)
        {
            BucketName = configuration.GetValue<string>("AWS:BucketName");
            ClientS3 = clientS3;
        }

        public async Task<bool> UploadFileAsync(string fileName, Stream stream)
        {
            PutObjectRequest request = new PutObjectRequest
            {
                InputStream = stream,
                Key = fileName,
                BucketName = this.BucketName
            };
            PutObjectResponse response = await ClientS3.PutObjectAsync(request);
            if (response.HttpStatusCode == System.Net.HttpStatusCode.OK)
            {
                return true;
            } else
            {
                return false;
            }
        }

        public async Task<bool> DeleteFileAsync(string fileName)
        {
            DeleteObjectResponse response = await this.ClientS3.DeleteObjectAsync(this.BucketName, fileName);
            if (response.HttpStatusCode == System.Net.HttpStatusCode.OK)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<List<string>> GetVersionsFilesAsync()
        {
            ListVersionsResponse response = await this.ClientS3.ListVersionsAsync(this.BucketName);
            List<string> versiones = response.Versions.Select(x => x.Key).ToList();
            return versiones;
        }

        public async Task<Stream> GetFileAsync(string fileName)
        {
            GetObjectResponse response = await this.ClientS3.GetObjectAsync(this.BucketName, fileName);
            return response.ResponseStream;
        }

        public async Task<List<Tuple<string, List<string>>>> GetFilesAsync()
        {
            ListObjectsResponse response = await this.ClientS3.ListObjectsAsync(this.BucketName);
            List<Tuple<string, List<string>>> filesVersions = new();
            foreach (var file in response.S3Objects)
            {
                var fileVersions = await this.GetFileVersionsAsync(file.Key);
                filesVersions.Add(Tuple.Create(file.Key, fileVersions));
            }

            return filesVersions;
        }

        public async Task<List<string>> GetFileVersionsAsync(string fileName)
        {
            ListVersionsResponse response = await this.ClientS3.ListVersionsAsync(this.BucketName);
            List<S3ObjectVersion> versions = response.Versions.Where(x => x.Key == fileName).ToList();
            return versions.Select(x => x.VersionId).ToList();
        }
    }
}
