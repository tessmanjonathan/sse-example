using EmbedIO;

namespace SseExample.Test;

[TestClass]
public class ExampleTest
{
    private WebServer server;
    private const string url = "http://localhost:9696/";

    [TestInitialize]
    public void TestInitialize()
    {

        server = new WebServer(o => o
                 .WithUrlPrefix(url)
                 .WithMode(HttpListenerMode.EmbedIO))
                 .WithWebApi("/api", m => m.RegisterController<SSEController>());


        server.RunAsync();
    }

    [TestCleanup]
    public void TestCleanup()
    {
        server.Dispose();
    }

    [TestMethod]
    public async Task SseClientReceivesFiveEvents()
    {
        var events = new List<EventMessage>();

        using var client = new HttpClient();
        
        // could add custom headers for auth if required
        client.DefaultRequestHeaders.Add("CustomHeader", "HeaderDetailHere");

        var request = new HttpRequestMessage(HttpMethod.Get, "http://localhost:9696/api/stream");
        using var response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
            
        EventMessage currentEvent = null;
        using var streamReader = new StreamReader(await response.Content.ReadAsStreamAsync());
        while (!streamReader.EndOfStream)
        {
            var line = await streamReader.ReadLineAsync();
            if (line == null)
                break;

            if (line.StartsWith("event:"))
            {
                currentEvent = new EventMessage { EventType = line.Substring(6).Trim() };
            }
            else if (line.StartsWith("data:") && currentEvent != null)
            {
                currentEvent.Data = line.Substring(5).Trim();
                events.Add(currentEvent);
                currentEvent = null;
            }
        }

        Assert.AreEqual(events.Count, 5);
    }
}
