using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Storage.V1;

namespace LessonManager.WebAPIs
{
    class Image
    {
        private const string CREDENTIAL_FILE_NAME = "Credentials/google-storage-account-key.json";
        private const string BUCKET_NAME = "third-being-175805.appspot.com";

        private static StorageClient client_;
        private static StorageClient Client()

        {
            if (client_ != null) return client_;

            var credential = GoogleCredential.FromStream(new FileStream(CREDENTIAL_FILE_NAME, FileMode.Open));
            return client_ = StorageClient.Create(credential);
        }
        
        public static async Task<string> Upload(Stream stream, string contentType)
        {
            var destination = new Google.Apis.Storage.v1.Data.Object();
            destination.Bucket = BUCKET_NAME;
            destination.Name = Guid.NewGuid().ToString();
            destination.ContentType = contentType;

            var item = await Client().UploadObjectAsync(destination, stream, new UploadObjectOptions() { PredefinedAcl = PredefinedObjectAcl.PublicRead }).ConfigureAwait(false);
            return item.MediaLink;
        }
    }
}
