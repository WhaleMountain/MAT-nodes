using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net;
using System.Net.WebSockets;
using System.IO;

namespace MATNet.Plugins
{
    public class WebSocketServer: MNIServer
    {
        public event OnDataReceived OnDataReceivedEvent;
        public event OnMessagaeReceived OnMessageReceivedEvent;

        HttpListener httpListener;
        List<WebSocket> wsConnections;

        public bool IsValidMethod()
        {
            return true;
        }

        public int listeningPort { get; set; }

        public WebSocketServer()
        {
            listeningPort = 51024;
            wsConnections = new List<WebSocket>();
        }

        public void Start()
        {
            httpListener = new HttpListener();
            wsConnections = new List<WebSocket>();
            httpListener.Prefixes.Add($"http://localhost:{listeningPort}/");
            httpListener.Start();
            httpListener.BeginGetContext(OnRequested, null);
            MNTools.DebugLog("WebSocket Server Start!");
        }

        void OnRequested(IAsyncResult ar)
        {
            if (!httpListener.IsListening)
            {
                return;
            }
            HttpListenerContext context = httpListener.EndGetContext(ar);
            httpListener.BeginGetContext(OnRequested, httpListener);
            try
            {
                if (ProcessWebSocketRequest(context))
                {
                    return;
                }
            }
            catch (Exception ex)
            {
                ReturnInternalError(context.Response, ex);
            }
        }

        bool ProcessWebSocketRequest(HttpListenerContext context)
        {
            if (!context.Request.IsWebSocketRequest) { return false; }
            WebSocket webSocket = context.AcceptWebSocketAsync(null).Result.WebSocket;
            wsConnections.Add(webSocket);
            ProcessReceivedMessage(webSocket);
            return true;
        }

        async void ProcessReceivedMessage(WebSocket webSocket)
        {
            try
            {
                ArraySegment<byte> buffer = new ArraySegment<byte>(new byte[1024]);
                while (webSocket.State == WebSocketState.Open)
                {
                    WebSocketReceiveResult receiveResult = await webSocket.ReceiveAsync(buffer, System.Threading.CancellationToken.None);
                    if (receiveResult.MessageType == WebSocketMessageType.Text)
                    {
                        string message = Encoding.UTF8.GetString(buffer.Take(receiveResult.Count).ToArray());
                        //OnMessageReceivedEvent(message);
                        SendMessage(message);
                    }
                    else if (receiveResult.MessageType == WebSocketMessageType.Binary)
                    {
                        //OnDataReceivedEvent(buffer.Take(receiveResult.Count).ToArray());
                        SendData(buffer.Take(receiveResult.Count).ToArray());
                    }
                }
            }catch (Exception)
            {

            }
            finally
            {
                wsConnections.Remove(webSocket);
                webSocket.Dispose();
            }
        }

        void ReturnInternalError(HttpListenerResponse response, Exception cause)
        {
            MNTools.DebugLog(cause);
            response.StatusCode = (int)HttpStatusCode.InternalServerError;
            response.ContentType = "text/plain";
            try
            {
                using (var writer = new StreamWriter(response.OutputStream, Encoding.UTF8))
                {
                    writer.Write(cause.ToString());
                }
                response.Close();
            }
            catch (Exception ex)
            {
                MNTools.DebugLogError(ex);
                response.Abort();
            }
        }

        public void Stop()
        {
            httpListener.Stop();
            httpListener.Close();
        }

        async void SendMessage(string message)
        {
            foreach (WebSocket ws in wsConnections)
            {
                ArraySegment<byte> buffer = new ArraySegment<byte>(Encoding.UTF8.GetBytes(message));
                await ws.SendAsync(buffer, WebSocketMessageType.Text, true, System.Threading.CancellationToken.None);
            }
        }

        async void SendData(byte[] data)
        {
            foreach (WebSocket ws in wsConnections)
            {
                ArraySegment<byte> buffer = new ArraySegment<byte>(data);
                await ws.SendAsync(buffer, WebSocketMessageType.Binary, true, System.Threading.CancellationToken.None);
            }
        }
    }
}
