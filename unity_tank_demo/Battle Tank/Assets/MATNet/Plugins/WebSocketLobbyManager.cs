using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessagePack;
using WebSocket4Net;

namespace MATNet.Plugins
{
    public class WebSocketLobbyManager : MNILobbyManager
    {
        [MessagePackObject]
        public class DataContainer
        {
            [Key(0)]
            public int roomID { get; set; }
            [Key(1)]
            public MNRoomData data { get; set; }
        }

        public event OnRoomDataChanged OnRoomDataChangedEvent;

        private WebSocket webSocket;
        private Dictionary<int, MNRoomData> roomList;

        public WebSocketLobbyManager()
        {
            webSocket = new WebSocket("ws://localhost:51026");
            webSocket.Opened += new EventHandler(WebSocket_Opened);
            webSocket.DataReceived += new EventHandler<DataReceivedEventArgs>(WebSocket_DataReceived);
            webSocket.MessageReceived += new EventHandler<MessageReceivedEventArgs>(WebSocket_MessageReceived);
            webSocket.Error += new EventHandler<SuperSocket.ClientEngine.ErrorEventArgs>(WebSocket_Error);
            webSocket.Closed += new EventHandler(WebSocket_Closed);


            webSocket.EnableAutoSendPing = true;
            webSocket.Open();

            roomList = new Dictionary<int, MNRoomData>();
        }

        private void WebSocket_Closed(object sender, EventArgs e)
        {
            MNTools.DebugLog("WSLobbyMg Closed");
        }

        private void WebSocket_Error(object sender, SuperSocket.ClientEngine.ErrorEventArgs e)
        {
            MNTools.DebugLogError(e.Exception);
        }

        private void WebSocket_MessageReceived(object sender, MessageReceivedEventArgs e)
        {
            MNTools.DebugLog("message received");
            string[] temp = e.Message.Split(':');
            if (temp[0] == "delete")
            {
                roomList.Remove(int.Parse(temp[1]));
            }
        }

        private void WebSocket_DataReceived(object sender, DataReceivedEventArgs e)
        {
            DataContainer container = MessagePackSerializer.Deserialize<DataContainer>(e.Data, MessagePack.Resolvers.ContractlessStandardResolver.Instance);
            MNTools.DebugLog($"data received : room_id={container.roomID}");
            string json = MessagePackSerializer.ToJson(e.Data);
            MNTools.DebugLog($"data: {json}");
            if (roomList.ContainsKey(container.roomID))
            {
                roomList[container.roomID] = (MNRoomData)container.data;
            }
            else
            {
                roomList.Add(container.roomID, (MNRoomData)container.data);
            }
            OnRoomDataChangedEvent((MNRoomData)container.data);
        }

        private void WebSocket_Opened(object sender, EventArgs e)
        {
            MNTools.DebugLog("WSLobbyMg ws opend");
        }

        private void SendRoomData(MNRoomData roomData)
        {
            DataContainer container = new DataContainer { roomID = roomData.roomID, data = roomData };
            List<ArraySegment<byte>> sendData = new List<ArraySegment<byte>>();
            sendData.Add(new ArraySegment<byte>(MessagePackSerializer.Serialize(container, MessagePack.Resolvers.ContractlessStandardResolver.Instance)));
            webSocket.Send(sendData);
        }

        public MNRoomData CreateRoom(string roomName, MNPlayer admin)
        {
            MNRoomData roomData = new MNRoomData();
            roomData.status = MNRoomData.Status.WaitingPlayer;
            roomData.roomName = roomName;
            roomData.roomID = GetNewRoomID();
            roomData.adminPlayer = admin;

            SendRoomData(roomData);

            return roomData;
        }

        private int GetNewRoomID()
        {
            if (roomList.Count == 0)
            {
                return 0;
            }
            else
            {
                return roomList.Last().Value.roomID + 1;
            }
        }

        public bool DeleteRoom(int roomID)
        {
            webSocket.Send($"delete:{roomID}");
            return true;
        }

        public string GetNewUuid()
        {
            return RequestNewUuid();
        }

        private string RequestNewUuid()
        {
            return Guid.NewGuid().ToString();
        }

        public MNRoomData GetRoomData(int roomId)
        {
            if (!roomList.ContainsKey(roomId))
            {
                return null;
            }
            return roomList[roomId];
        }

        public List<MNRoomData> GetRoomList()
        {
            return roomList.Values.ToList();
        }

        public bool IsValidMethod()
        {
            if (webSocket.State == WebSocketState.Open)
            {
                return true;
            }
            return false;
        }

        public bool JoinRoom(int roomID, MNPlayer player)
        {
            MNRoomData roomData = roomList[roomID];
            if (roomData.players == null)
            {
                roomData.players = new List<MNPlayer>();
            }
            roomData.players.Add(player);

            SendRoomData(roomData);

            return true;
        }

        public void LeaveRoom(int roomID, MNPlayer player)
        {
            MNRoomData roomData = roomList[roomID];
            roomData.players.Remove(player);

            SendRoomData(roomData);
        }

        public bool SetRoomData(MNRoomData roomData)
        {
            SendRoomData(roomData);

            return true;
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
