using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.OutputCaching;

namespace AITextSummarizer.Api.Caching;

public class SummarizeCachePolicy : IOutputCachePolicy
{
    public async ValueTask CacheRequestAsync(OutputCacheContext context, CancellationToken ct)
    {
        var request = context.HttpContext.Request;

        var canCache = HttpMethods.IsPost(request.Method)
                       && string.IsNullOrEmpty(request.Headers.Authorization);

        context.EnableOutputCaching = true;
        context.AllowCacheLookup = canCache;
        context.AllowCacheStorage = canCache;
        context.AllowLocking = true;
        context.ResponseExpirationTimeSpan = TimeSpan.FromMinutes(10);

        if (canCache)
        {
            request.EnableBuffering();
            using var reader = new StreamReader(request.Body, leaveOpen: true);
            var body = await reader.ReadToEndAsync(ct);
            request.Body.Position = 0;

            var hash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(body)));
            context.CacheVaryByRules.VaryByValues.Add("body", hash);
        }
    }

    public ValueTask ServeFromCacheAsync(OutputCacheContext context, CancellationToken ct)
        => ValueTask.CompletedTask;

    public ValueTask ServeResponseAsync(OutputCacheContext context, CancellationToken ct)
    {
        if (context.HttpContext.Response.StatusCode != StatusCodes.Status200OK)
            context.AllowCacheStorage = false;
        return ValueTask.CompletedTask;
    }
}