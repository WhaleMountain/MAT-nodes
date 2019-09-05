using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimpleWebSocketServerLibrary;

namespace MATNet.Plugins
{
    public class WebSocketServer2 : MNIServer
    {
        public int listeningPort { get; set; }

        private SimpleWebSocketServer server;

        public WebSocketServer2()
        {
            listeningPort = 51024;
        }

        public bool IsValidMethod()
        {
            return true;
        }

        public void Start()
        {
            SimpleWebSocketServerSettings settings = new SimpleWebSocketServerSettings();
            settings.port = listeningPort;
            server = new SimpleWebSocketServer(settings);
            server.WebsocketServerEvent += Server_WebsocketServerEvent;
            server.StartServer();
            MNTools.DebugLog("Start websocket server");
        }

        private void Server_WebsocketServerEvent(object sender, WebSocketEventArg e)
        {
            if (e.data == null) { return; }
            if (e.isText)
            {
                string message = Encoding.UTF8.GetString(e.data);
                server.SendTextMessageAsync(message);
            }else if (e.isBinary)
            {
                server.SendBinaryMessageAsync(e.data);
            }
        }

        public void Stop()
        {
            server.StopAll();
        }


    }
}
