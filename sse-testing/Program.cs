using EmbedIO;
using SseExample;

var url = "http://localhost:9696/";
var server = new WebServer(o => o
         .WithUrlPrefix(url)
         .WithMode(HttpListenerMode.EmbedIO))
         .WithWebApi("/api", m => m.RegisterController<SSEController>());

await server.RunAsync();