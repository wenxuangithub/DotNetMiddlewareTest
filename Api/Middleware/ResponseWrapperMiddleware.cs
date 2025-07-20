using System.Text.Json;
using System.Text;

namespace Api.Middleware
{
    public class ResponseWrapperMiddleware
    {
        private readonly RequestDelegate _next;

        public ResponseWrapperMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            var originalBody = context.Response.Body;
            using var memStream = new MemoryStream();
            context.Response.Body = memStream;

            await _next(context);

            if (context.Response.ContentType?.Contains("application/json") == true)
            {
                memStream.Seek(0, SeekOrigin.Begin);
                var bodyText = await new StreamReader(memStream).ReadToEndAsync();

                var payload = new
                {
                    success = context.Response.StatusCode is >= 200 and < 300,
                    statusCode = context.Response.StatusCode,
                    data = JsonSerializer.Deserialize<object>(bodyText)
                };

                var wrappedJson = JsonSerializer.Serialize(payload);
                var wrappedBytes = Encoding.UTF8.GetBytes(wrappedJson);

                context.Response.Body = originalBody;
                context.Response.ContentLength = wrappedBytes.Length;
                context.Response.ContentType = "application/json";
                await context.Response.Body.WriteAsync(wrappedBytes, 0, wrappedBytes.Length);
            }
            else
            {
                memStream.Seek(0, SeekOrigin.Begin);
                await memStream.CopyToAsync(originalBody);
            }
        }
    }
}
