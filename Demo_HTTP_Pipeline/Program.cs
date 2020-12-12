using System;
using System.IO;
using System.Threading.Tasks;

using Azure;
using Azure.Identity;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Core;
using Azure.Core.Diagnostics;


namespace Demo_HTTP_Pipeline
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // Listen to events produced from all Azure SDK clients
            using AzureEventSourceListener listener = AzureEventSourceListener.CreateConsoleLogger();

            // We don't want to use connection string, since it is highly insecure
            //string connectionString = Environment.GetEnvironmentVariable("ITPRODEV_STORAGE_ACCOUNT_CONNECTION_STRING");
            //BlobServiceClient blobServiceClient = new BlobServiceClient(connectionString);

            // We can configure and extend the HTTP pipeline through XxxClientOptions
            BlobClientOptions options = new BlobClientOptions();
            options.Retry.MaxRetries = 10;
            options.Diagnostics.IsTelemetryEnabled = false;
            options.AddPolicy(new MyCustomPolicy(), HttpPipelinePosition.PerCall);

            // Get a reference to our storage account. We use the public Uri and DefaultAzureCredential for authentication
            Uri blobServiceUri = new Uri("https://itprodev2020storage.blob.core.windows.net/");
            BlobServiceClient blobServiceClient = new BlobServiceClient(blobServiceUri, new DefaultAzureCredential(), options);

            // Get a reference to "itprodev-container" container. This clients inherits the HTTP Pipeline options from BlobServiceClient
            BlobContainerClient blobContainerClient = blobServiceClient.GetBlobContainerClient("itprodev-container");

            // Create container if not exists
            await blobContainerClient.CreateIfNotExistsAsync();

            // Create a dummy file
            string path = Utilities.CreateTempFile();

            // Get a reference to a blob
            BlobClient blob = blobContainerClient.GetBlobClient(Utilities.RandomName());

            // Upload file to blob
            await blob.UploadAsync(path);

            // Delete local file
            Utilities.DeleteFile(path);

            // asynchronously iterate through all blobs
            await foreach (BlobItem item in blobContainerClient.GetBlobsAsync())
            {
                // Get a path on disk where we can download the file
                string downloadPath = $"{item.Name}.txt";

                // Get a reference to blob and download its contents
                BlobDownloadInfo download = await blobContainerClient.GetBlobClient(item.Name).DownloadAsync();

                // Copy its contents to disk
                using FileStream file = File.OpenWrite(downloadPath);
                await download.Content.CopyToAsync(file);
            }

            // When a service call fails Azure.RequestFailedException would get thrown. 
            // The exception type provides a Status property with an HTTP status code an an ErrorCode property with a service-specific error code.
            try
            {
                //...
            }
            // handle exception with status code 404
            catch (RequestFailedException e) when (e.Status == 404)
            {
                Console.WriteLine("ErrorCode " + e.ErrorCode);
            }
        }
    }
}
