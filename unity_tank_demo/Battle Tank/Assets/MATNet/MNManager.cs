using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Reflection;

namespace MATNet
{
    public class InstanceMethodPair
    {
        public object instance { get; set; }
        public MethodInfo method { get; set; }
    }

    public class MNManager
    {
        public static MNManager Instance;
        public enum Status
        {
            RoomSelecting,
            WaitPlayer,
            Playing
        }
        public Status CurrentStatus
        {
            get
            {
                if (joiningRoomData != null && joiningRoomData.status == MNRoomData.Status.InSession)
                {
                    return Status.Playing;
                }else if (joiningRoomData != null)
                {
                    return Status.WaitPlayer;
                }
                else
                {
                    return Status.RoomSelecting;
                }
            }
        }
        public bool IsHosting
        {
            get
            {
                return hostingServer != null;
            }
        }
        public bool IsPaused { get; set; }

        public MNPlayer player;

        public static string lobbyManagerName = "FirebaseLobbyManager";
        public static string[] clientMethodNames = { "WebSocketClient" };
        public static string[] serverMethodNames = { "WebSocketServer2" };

        public MNILobbyManager lobbyManager { get; set; }
        MNIServer hostingServer;
        MNIClient client;
        MNRoomData joiningRoomData;
        public event OnRoomDataChanged OnJoiningRoomDataChangedEvent;

        private Dictionary<string, InstanceMethodPair> methodDict;

        static MNManager()
        {
            Instance = new MNManager();
        }

        public MNManager()
        {
            methodDict = new Dictionary<string, InstanceMethodPair>();

            lobbyManager = (MNILobbyManager)MNTools.GetInstance(lobbyManagerName);
            lobbyManager.OnRoomDataChangedEvent += LobbyManager_OnRoomDataChangedEvent;
            //MNTools.DebugLog(lobbyManager.IsValidMethod());
        }

        private void LobbyManager_OnRoomDataChangedEvent(MNRoomData roomData)
        {
            MNTools.DebugLog($"receive roomdata changed : id={roomData.roomID} name={roomData.roomName}");
            if (joiningRoomData == null || roomData.roomID != joiningRoomData.roomID)
            {
                MNTools.DebugLog("pass because this is not my room.");
                return;
            }
            if (roomData.status == MNRoomData.Status.Connecting)
            {
                MNTools.DebugLog("prepare for connection");
                //通信の準備を行う
                if (roomData.hostPlayer.uuid == player.uuid)
                {
                    MNTools.DebugLog("start hosting server");
                    hostingServer = (MNIServer)MNTools.GetInstance(roomData.serverMethod);
                    hostingServer.Start();
                    roomData.hostPortNum = hostingServer.listeningPort;
                    roomData.status = MNRoomData.Status.InSession;
                    lobbyManager.SetRoomData(roomData);
                }
                client = (MNIClient)MNTools.GetInstance(roomData.clientMethod);
                client.OnDataReceivedEvent += Client_OnDataReceivedEvent;
                client.OnMessageReceivedEvent += Client_OnMessageReceivedEvent;
            }
            if (joiningRoomData != null && joiningRoomData.status == MNRoomData.Status.Connecting && roomData.status == MNRoomData.Status.InSession)
            {
                MNTools.DebugLog("start connecting");
                //hostの準備ができたから接続を行う(localhostの場合にサーバの起動が完了していない可能性があるので少し待つ)
                Task.Run(async () =>
                {
                    await Task.Delay(500);
                    client.Connect(roomData.hostPlayer, roomData.hostPortNum);
                });
            }
            if (joiningRoomData != null && joiningRoomData.status == MNRoomData.Status.WaitingPlayer && roomData.status == MNRoomData.Status.InSession)
            {
                //途中参加
                client = (MNIClient)MNTools.GetInstance(roomData.clientMethod);
                client.OnDataReceivedEvent += Client_OnDataReceivedEvent;
                client.OnMessageReceivedEvent += Client_OnMessageReceivedEvent;

                client.Connect(roomData.hostPlayer, roomData.hostPortNum);
            }
            joiningRoomData = roomData;
            OnJoiningRoomDataChangedEvent(roomData);
            MNTools.DebugLog("end on room data changed event");
        }

        private void Client_OnMessageReceivedEvent(string message)
        {
            throw new NotImplementedException();
        }

        private void Client_OnDataReceivedEvent(string id, object[] data)
        {
            if (methodDict.ContainsKey(id))
            {
                InstanceMethodPair pair = methodDict[id];
                pair.method.Invoke(pair.instance, data);
            }
        }

        public void OnStart()
        {
            MNTools.DebugLog("Hello unity");
            player = new MNPlayer();
        }

        public void Connect()
        {
            if (hostingServer != null)
            {
                Connect("127.0.0.1", hostingServer.listeningPort);
            }
            else
            {
                MNTools.DebugLogError("ローカルサーバがスタートされていません。");
            }
        }

        public void Connect(string address, int port)
        {
            client.Connect(address, port);
        }

        public void Disconnect()
        {
            client.Disconnect();
        }

        public void JoinRoom(int roomId)
        {
            if (player == null)
            {
                MNTools.DebugLog("player instance is null");
                return;
            }
            lobbyManager.JoinRoom(roomId, player);
            joiningRoomData = new MNRoomData();
            joiningRoomData.roomID = roomId;
        }

        public void CreateRoom(string roomName)
        {
            MNRoomData roomData = lobbyManager.CreateRoom(roomName, player);
            //lobbyManager.JoinRoom(roomData.roomID, player);
        }

        public void JoinRandomRoom()
        {
            
        }

        public void LeaveRoom()
        {
            lobbyManager.LeaveRoom(joiningRoomData.roomID, player);
        }

        public void DeleteRoom()
        {
            int id = joiningRoomData.roomID;
            lobbyManager.LeaveRoom(id, player);
            lobbyManager.DeleteRoom(id);
        }

        public void StartSession()
        {
            if (joiningRoomData.adminPlayer.uuid != player.uuid)
            {
                //send request?
                MNTools.DebugLog("You dont have admin permission");
                return;
            }
            ClientMethodDeterminer determiner = new ClientMethodDeterminer();
            determiner.DetermineHost(joiningRoomData);
            MNRoomData data = joiningRoomData;
            data.hostPlayer = determiner.GetHost();
            data.clientMethod = determiner.GetClientMethod();
            data.serverMethod = determiner.GetServerMethod();
            data.status = MNRoomData.Status.Connecting;
            lobbyManager.SetRoomData(data);
            MNTools.DebugLog("Start connecting!");
        }

        public List<MNRoomData> GetRoomList()
        {
            return lobbyManager.GetRoomList();
        }

        public string GetNewUuid()
        {
            return lobbyManager.GetNewUuid();
        }

        public void SendMessage(string message)
        {
            client.SendMessage(message);
        }

        public void SendRPC(string id, params object[] args)
        {
            client.SendData(id, args);
        }

        public void RegisterRpcMethod(string id, object instance, string methodName)
        {
            if (methodDict.ContainsKey(id)) { return; }
            methodDict.Add(id, new InstanceMethodPair { instance = instance, method = instance.GetType().GetMethod(methodName) });
        }

        public bool ContainsPlayer(List<MNPlayer> list, MNPlayer player)
        {
            foreach (MNPlayer item in list)
            {
                if (item.Equals(player))
                {
                    return true;
                }
            }
            return false;
        }

        public void OnDestroy()
        {
            lobbyManager.OnDestroy();
            if (client != null)
            {
                client.OnDestroy();
            }
            if (hostingServer != null)
            {
                hostingServer.Stop();
            }
        }
    }
}

