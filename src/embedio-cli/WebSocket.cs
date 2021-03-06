﻿namespace Unosquare.Labs.EmbedIO.Command
{
    using Swan.Formatters;
    using System.Net;
    using Modules;

    public static class WebSocketWatcher
    {
        public static WebServer Server { get; private set; }

        public static void Setup()
        {
            Server = new WebServer($"http://localhost:{Program.WsPort}/");

            Server.RegisterModule(new WebSocketsModule());
            Server.Module<WebSocketsModule>().RegisterWebSocketsServer<WebSocketsWatcherServer>();

            Server.RunAsync();
        }
    }

    [WebSocketHandler("/watcher")]
    public class WebSocketsWatcherServer : WebSocketsServer
    {
        public WebSocketsWatcherServer()
            : base(true)
        {
            Watcher.Instance.RefreshPage += (s, e) => Broadcast(Json.Serialize(new { Update = true }));
        }

        public override string ServerName => nameof(WebSocketWatcher);

        protected override void OnClientConnected(IWebSocketContext context, IPEndPoint localEndPoint, IPEndPoint remoteEndPoint)
        {
            // placeholder
        }

        protected override void OnClientDisconnected(IWebSocketContext context)
        {
            // placeholder
        }

        protected override void OnFrameReceived(IWebSocketContext context, byte[] rxBuffer, IWebSocketReceiveResult rxResult)
        {
            // placeholder
        }

        protected override void OnMessageReceived(IWebSocketContext context, byte[] rxBuffer, IWebSocketReceiveResult rxResult)
        {
            // placeholder
        }
    }

}
