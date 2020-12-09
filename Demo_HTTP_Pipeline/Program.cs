using System;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;
using System.Threading.Tasks;
using Azure.Identity;
using Azure.Storage.Blobs;
using System.IO;
using System.Text;
using Azure.Storage.Blobs.Models;
using Azure.Core;

namespace Demo_HTTP_Pipeline
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // Prepare HTTP Pipeline options
            BlobClientOptions options = new BlobClientOptions();
            options.Retry.MaxRetries = 10;
            options.Retry.Delay = TimeSpan.FromSeconds(1);
            options.Diagnostics.IsLoggingEnabled = false;
            options.Diagnostics.IsTelemetryEnabled = false;

            // Add our custom policy to the HTTP Pipeline
            options.AddPolicy(new MyCustomPolicy(), HttpPipelinePosition.PerRetry);

            // Get a reference to our storage account using the connection string (stored as environment variable)
            //string connectionString = Environment.GetEnvironmentVariable("ITPRODEV_STORAGE_ACCOUNT_CONNECTION_STRING");
            //BlobServiceClient blobServiceClient = new BlobServiceClient(connectionString, options);

            // Get a reference to our storage account using DefaultAzureCredential
            Uri blobServiceUri = new Uri("https://itprodev2020storage.blob.core.windows.net/");
            BlobServiceClient blobServiceClient  = new BlobServiceClient(blobServiceUri, new DefaultAzureCredential(), options);

            //Console.WriteLine($"Storage account name: {blobServiceClient.AccountName}");

            // Get a reference to a container named "itprodev-container" and create it if not exists
            BlobContainerClient blobContainerClient = blobServiceClient.GetBlobContainerClient("itprodev-container");
            await blobContainerClient.CreateIfNotExistsAsync();

            //Console.WriteLine($"Blob container Uri: {blobContainerClient.Uri}");

            for (int i = 0; i < 5; i++)
            {
                // Create a dummy file
                string path = Utilities.CreateTempFile();

                // Get a reference to a blob
                BlobClient blob = blobContainerClient.GetBlobClient(Utilities.RandomName());

                // Upload file to blob
                await blob.UploadAsync(path);

                // Delete local file
                Utilities.DeleteFile(path);
            }

            await foreach (BlobItem item in blobContainerClient.GetBlobsAsync()) {
                Console.WriteLine($"Blob name: {item.Name}");
                
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
