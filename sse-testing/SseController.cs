using EmbedIO;
using EmbedIO.Routing;
using EmbedIO.WebApi;

namespace SseExample;

public class SSEController : WebApiController
{
    [Route(HttpVerbs.Get, "/stream")]
    public async Task Get()
    {
        HttpContext.Response.ContentType = "text/event-stream";
        HttpContext.Response.KeepAlive = true;
        HttpContext.Response.DisableCaching();

        using (var writer = new StreamWriter(HttpContext.OpenResponseStream()))
        {
            while (true)
            {
                for (var i = 0; i < 5; i++) // Sending 5 events
                {
                    await writer.WriteAsync($"event: usermessage\ndata: {DateTime.UtcNow}\n\n");
                    await writer.FlushAsync();

                    await Task.Delay(500);
                }

                writer.Close();
                return;
            }
        }
    }
}
