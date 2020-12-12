using Azure.Core;
using Azure.Core.Pipeline;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo_HTTP_Pipeline
{
    public class MyCustomPolicy : HttpPipelinePolicy
    {
        public override void Process(HttpMessage message, ReadOnlyMemory<HttpPipelinePolicy> pipeline)
        {
            throw new NotImplementedException();
        }

        public override async ValueTask ProcessAsync(HttpMessage message, ReadOnlyMemory<HttpPipelinePolicy> pipeline)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            await ProcessNextAsync(message, pipeline);

            stopwatch.Stop();
            Console.WriteLine($">> Request: {message.Request.Method} {message.Request.Uri} took {stopwatch.Elapsed.TotalMilliseconds}ms");

        }
    }
}
