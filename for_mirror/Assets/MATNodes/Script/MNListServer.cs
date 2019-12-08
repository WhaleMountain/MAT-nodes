using Mirror;
using Mono.Nat;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MATNodes
{
    [RequireComponent(typeof(NetworkManager))]
    public class MNListServer : MonoBehaviour
    {
        public static MNListServer Instance;
        private void Awake()
        {
            if (Instance != null)
            {
                Debug.LogWarning("Overwritten instance!");
            }
            Instance = this;
        }

        //Unityから設定する項目
        [SerializeField]
        Protocol serverProtocol;
        [SerializeField]
        int serverPort;

        public MNPlayer Player { get; set; }
        MNIDatabase database;
        int joiningRoomId;
        public MNRoomData JoiningRoomData { get; set; }
        public bool OnRoom { get; set; }

        //Nat
        private INatDevice natDevice;
        public bool EnabledUpnp { get; set; }

        //メインスレッドからしか呼び出せないため
        private bool startHostOnNextFrame;
        private bool startClientOnNextFrame;

        //Mirrorを呼び出すコードのラッパー
        void StartClient(string ip)
        {
            NetworkManager.singleton.networkAddress = ip;
            startClientOnNextFrame = true;
            //NetworkManager.singleton.StartClient();
        }
        void StartHost()
        {
            startHostOnNextFrame = true;
        }
        public bool OnRunningClient()
        {
            return NetworkManager.singleton.isNetworkActive;
        }
        Protocol GetServerProtocol()
        {
            return serverProtocol;
        }
        int GetServerPort()
        {
            return serverPort;
        }
        int GetServerCapacity()
        {
            return NetworkManager.singleton.maxConnections;
        }
        public void OpenPort()
        {
            CreatePortMap(GetServerProtocol(), GetServerPort());
        }
        public void ClosePort()
        {
            DeletePortMap(GetServerProtocol(), GetServerPort());
        }
        
        // Start is called before the first frame update
        void Start()
        {
            joiningRoomId = -1;
            NatUtility.DeviceFound += NatUtility_DeviceFound;
            NatUtility.DeviceLost += NatUtility_DeviceLost;
            natDevice = null;
            NatUtility.StartDiscovery();
            database = PluginManager.GetAvailableDatabase();
            database.OnRoomDataChangedEvent += Database_OnRoomDataChangedEvent;
            Player = new MNPlayer("Player");
        }

        // Update is called once per frame
        void Update()
        {
            if (startHostOnNextFrame)
            {
                startHostOnNextFrame = false;
                NetworkManager.singleton.StartHost();
            }
            if (startClientOnNextFrame)
            {
                startClientOnNextFrame = false;
                NetworkManager.singleton.StartClient();
            }
        }

        //for debug
        public void Log(string text)
        {
            Debug.Log(text);
        }

        private void NatUtility_DeviceLost(object sender, DeviceEventArgs e)
        {
            natDevice = null;
            EnabledUpnp = false;
            Debug.Log("Lost NAT Device.");
        }

        private void NatUtility_DeviceFound(object sender, DeviceEventArgs e)
        {
            natDevice = e.Device;
            EnabledUpnp = true;
            Debug.Log("Enabled UPnP.\nexisting maps:");
            foreach (Mapping portMap in natDevice.GetAllMappings())
            {
                Debug.Log(portMap.ToString());
            }
        }
        public void CreatePortMap(Protocol protocol, int number)
        {
            if (!EnabledUpnp || natDevice == null) return;
            natDevice.CreatePortMap(new Mapping(protocol, number, number));
        }
        public void DeletePortMap(Protocol protocol, int number)
        {
            if (!EnabledUpnp || natDevice == null) return;
            natDevice.DeletePortMap(new Mapping(protocol, number, number));
        }
        public bool ExistingPortMap(Protocol protocol, int number)
        {
            if (!EnabledUpnp || natDevice == null) return false;
            return natDevice.GetSpecificMapping(protocol, number).PublicPort != -1;
        }

        private void Database_OnRoomDataChangedEvent(int roomId)
        {
            if (roomId != joiningRoomId)
            {
                return;
            }
            string temp = database.GetRoomData(roomId);
            Debug.Log("datachanged:\n" + temp);
            MNRoomData roomData = MNRoomData.FromJson(temp);
            if (roomData.Status == MNRoomData.RoomStatus.Connecting)
            {
                if (roomData.HostPlayer.Equals(Player))
                {
                    Debug.Log("Starting host..");
                    OpenPort();
                    StartHost();
                    roomData.Status = MNRoomData.RoomStatus.InSession;
                    database.SetRoomData(joiningRoomId, MNRoomData.ToJson(roomData));
                }
            }
            if (OnRoom && roomData.Status == MNRoomData.RoomStatus.InSession && !OnRunningClient())
            {
                if (!roomData.HostPlayer.Equals(Player))
                {
                    StartClient(GetProperAddress(roomData.HostPlayer));
                }
            }
            //入室が承認された場合
            if (!OnRoom && roomData.Players.Contains(Player))
            {
                OnRoom = true;
                Debug.Log("入室が承認されました。");
            }
            //自分がadminPlayerの場合にやること
            if (roomData.AdminPlayer.Equals(Player))
            {
                //入室の承認
                if (roomData.JoinRequests.Count > 0)
                {
                    foreach (MNPlayer player in roomData.JoinRequests)
                    {
                        if (roomData.Capacity >= roomData.Players.Count)
                        {
                            roomData.Players.Add(player);
                        }
                    }
                    roomData.JoinRequests.Clear();
                    database.SetRoomData(roomId, MNRoomData.ToJson(roomData));
                }
                //セッションを開始する
                if (roomData.Status == MNRoomData.RoomStatus.DeterminingHost)
                {
                    Debug.Log("ホスト決定を開始");
                    HostDeterminer determiner = new HostDeterminer(roomData.Players);
                    determiner.Run();
                    if (determiner.GetHost() == null)
                    {
                        Debug.Log("ホスト可能なプレイヤーが存在しません！");
                        roomData.Status = MNRoomData.RoomStatus.WaitingPlayer;
                        database.SetRoomData(roomId, MNRoomData.ToJson(roomData));
                    }
                    else
                    {
                        roomData.HostPlayer = determiner.GetHost();
                        roomData.Status = MNRoomData.RoomStatus.Connecting;
                        database.SetRoomData(roomId, MNRoomData.ToJson(roomData));
                        Debug.Log("Start Session!");
                    }
                }
            }
            JoiningRoomData = roomData;
        }

        public string GetProperAddress(MNPlayer hostPlayer)
        {
            if (hostPlayer.Equals(Player))
            {
                return "127.0.0.1";
            }
            else if (hostPlayer.wanAddress == Player.wanAddress)
            {
                return hostPlayer.lanAddress;
            }
            else
            {
                return hostPlayer.wanAddress;
            }
        }

        public void JoinRoom(int roomId)
        {
            if (Player == null)
            {
                Debug.Log("player instance is null");
                return;
            }
            MNRoomData roomData = MNRoomData.FromJson(database.GetRoomData(roomId));
            if (roomData.Capacity != -1 && roomData.Capacity <= roomData.Players.Count)
            {
                Debug.Log("部屋が満員です！");
                return;
            }
            roomData.JoinRequests.Add(Player);
            if (database.SetRoomData(roomId, MNRoomData.ToJson(roomData)))
            {
                joiningRoomId = roomId;
                JoiningRoomData = roomData;
            }
            else
            {
                Debug.Log("部屋への参加に失敗しました");
            }
        }

        public void CreateRoom(string roomName)
        {
            if (Player == null)
            {
                Debug.Log("player instance is null");
                return;
            }
            MNRoomData roomData = new MNRoomData(roomName, Player, GetServerCapacity());
            string temp = MNRoomData.ToJson(roomData);
            Debug.Log("create room:\n" + temp);
            joiningRoomId = database.CreateRoom(temp);
            JoiningRoomData = roomData;
            OnRoom = true;
        }

        public void LeaveRoom()
        {
            if (joiningRoomId == -1 || !OnRoom) return;
            if (JoiningRoomData.Players.Count <= 1)
            {
                if (database.DeleteRoom(joiningRoomId))
                {
                    joiningRoomId = -1;
                    JoiningRoomData = null;
                }
                return;
            }
            JoiningRoomData.Players.Remove(Player);
            if (JoiningRoomData.AdminPlayer.Equals(Player))
            {
                JoiningRoomData.AdminPlayer = JoiningRoomData.Players[0];
            }
            if (JoiningRoomData.HostPlayer != null && JoiningRoomData.HostPlayer.Equals(Player))
            {
                //ホストが落ちる場合
            }
            database.SetRoomData(joiningRoomId, MNRoomData.ToJson(JoiningRoomData));
            OnRoom = false;
        }

        public void StartSession()
        {
            JoiningRoomData.Status = MNRoomData.RoomStatus.DeterminingHost;
            database.SetRoomData(joiningRoomId, MNRoomData.ToJson(JoiningRoomData));
            Debug.Log("send request that start session.");
        }

        public Dictionary<int, MNRoomData> GetRoomList()
        {
            Dictionary<int, MNRoomData> temp = new Dictionary<int, MNRoomData>();
            foreach (KeyValuePair<int, string> pair in database.GetRoomList())
            {
                temp.Add(pair.Key, MNRoomData.FromJson(pair.Value));
            }
            return temp;
        }

        void OnApplicationQuit()
        {
            ClosePort();
            LeaveRoom();
            if (database != null)
            {
                database.OnDestroy();
            }
        }
    }
}