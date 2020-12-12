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
    // To implement a policy create a class deriving from HttpPipelinePolicy and overide ProcessAsync and Process methods.
    // Request can be acessed via message.Request.
    // Response is accessible via message.Response but only after ProcessNextAsync/ProcessNext was called.
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
