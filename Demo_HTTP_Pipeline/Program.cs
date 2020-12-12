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
            //string connectionString = Environment.GetEnvironmentVariable("ITPRODEV_STORAGE_ACCOUNT_CONNECTION_STRING");
            BlobClientOptions options = new BlobClientOptions();
            options.Retry.MaxRetries = 10;

            options.AddPolicy(new MyCustomPolicy(), HttpPipelinePosition.PerCall);

            Uri blobServiceUri = new Uri("https://itprodev2020storage.blob.core.windows.net/");
            BlobServiceClient blobServiceclient = new BlobServiceClient(blobServiceUri, new DefaultAzureCredential(), options);

            //BlobServiceClient blobServiceclient = new BlobServiceClient(connectionString, options);

            BlobContainerClient blobContainerClient = blobServiceclient.GetBlobContainerClient("itprodev-container");

            await blobContainerClient.CreateIfNotExistsAsync();

            // Create a dummy file
            string path = Utilities.CreateTempFile();

            // Get a reference to a blob
            BlobClient blob = blobContainerClient.GetBlobClient(Utilities.RandomName());

            // Upload file to blob
            await blob.UploadAsync(path);

            // Delete local file
            Utilities.DeleteFile(path);

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
        }
    }
}
