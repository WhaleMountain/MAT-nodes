using System;
using System.Collections.Generic;
using WebSocket4Net;
using MessagePack;

namespace MATNet.Plugins
{
    public class WebSocketClient : MNIClient
    {
        [MessagePackObject]
        public class DataContainer
        {
            [Key(0)]
            public string id { get; set; }
            [Key(1)]
            public object[] data { get; set; }
        }

        private WebSocket webSocket;

        public event OnDataReceived OnDataReceivedEvent;
        public event OnMessagaeReceived OnMessageReceivedEvent;

        public WebSocketClient()
        {
            /*
            MessagePack.Resolvers.CompositeResolver.RegisterAndSetAsDefault(
                MessagePack.Unity.UnityResolver.Instance,
                MessagePack.Resolvers.StandardResolver.Instance
            );*/
        }

        public void Connect(string ip, int port)
        {
            MNTools.DebugLog($"Connect to ws://{ip}:{port}");

            webSocket = new WebSocket($"ws://{ip}:{port}");
            webSocket.Opened += new EventHandler(WS_Opened);
            webSocket.MessageReceived += new EventHandler<MessageReceivedEventArgs>(WS_MessageReceived);
            webSocket.DataReceived += new EventHandler<DataReceivedEventArgs>(WS_DataReceived);
            webSocket.Error += new EventHandler<SuperSocket.ClientEngine.ErrorEventArgs>(WS_Error);
            webSocket.Closed += new EventHandler(WS_Closed);

            webSocket.EnableAutoSendPing = true;
            webSocket.Open();
        }

        private void WS_Closed(object sender, EventArgs e)
        {
            MNTools.DebugLog("websocket client closed.");
        }

        private void WS_Error(object sender, SuperSocket.ClientEngine.ErrorEventArgs e)
        {
            MNTools.DebugLogError(e.Exception.Message);
        }

        private void WS_DataReceived(object sender, DataReceivedEventArgs e)
        {
            DataContainer container = MessagePackSerializer.Deserialize<DataContainer>(e.Data);
            OnDataReceivedEvent(container.id, container.data);
        }

        private void WS_MessageReceived(object sender, MessageReceivedEventArgs e)
        {
            OnMessageReceivedEvent(e.Message);
        }

        private void WS_Opened(object sender, EventArgs e)
        {
            MNTools.DebugLog("websocket client opend");
        }

        public void Connect(MNPlayer hostPlayer, int port)
        {
            if (hostPlayer.wanIP == MNManager.Instance.player.wanIP && hostPlayer.lanIP == MNManager.Instance.player.lanIP)
            {
                Connect("127.0.0.1", port);
            }
            else if (hostPlayer.wanIP == MNManager.Instance.player.wanIP)
            {
                Connect(hostPlayer.lanIP, port);
            }
            else
            {
                Connect(hostPlayer.wanIP, port);
            }
        }

        public void Disconnect()
        {
            webSocket.Close();
        }

        public bool IsValidMethod()
        {
            return true;
        }

        public void SendMessage(string message)
        {
            webSocket.Send(message);
        }

        public void SendData(string id, object[] data)
        {
            DataContainer container = new DataContainer { id = id, data = data };
            List<ArraySegment<byte>> sendData = new List<ArraySegment<byte>>();
            sendData.Add(new ArraySegment<byte>(MessagePackSerializer.Serialize(container)));
            
            webSocket.Send(sendData);
        }

        public void OnDestroy()
        {
            if (webSocket.State == WebSocketState.Open)
            {
                webSocket.Close();
            }
            webSocket.Dispose();
        }
    }
}
