namespace Api.Middleware
{
    public class HeaderSanitizerMiddleware
    {
        private readonly RequestDelegate _next;

        public HeaderSanitizerMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            context.Response.OnStarting(() =>
            {
                context.Response.Headers.Remove("Server");
                context.Response.Headers.Remove("X-Powered-By");
                return Task.CompletedTask;
            });

            await _next(context);
        }
    }
}
