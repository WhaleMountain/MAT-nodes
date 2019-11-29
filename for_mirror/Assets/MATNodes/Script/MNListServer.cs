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
        string databaseName = "BarusFirebaseDatabase";
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

        //Mirrorを呼び出すコードのラッパー
        void StartClient(string ip)
        {
            NetworkManager.singleton.networkAddress = ip;
            NetworkManager.singleton.StartClient();
        }
        void StartHost()
        {
            NetworkManager.singleton.StartHost();
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
            database = (MNIDatabase)MNTools.GetInstance(databaseName);
            database.OnRoomDataChangedEvent += Database_OnRoomDataChangedEvent;
            Player = new MNPlayer("Player");
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
            if (roomData.status == MNRoomData.Status.Connecting)
            {
                if (roomData.hostPlayer.Equals(Player))
                {
                    OpenPort();
                    StartHost();
                    roomData.status = MNRoomData.Status.InSession;
                    database.SetRoomData(joiningRoomId, MNRoomData.ToJson(roomData));
                }
            }
            if (JoiningRoomData.status == MNRoomData.Status.Connecting && roomData.status == MNRoomData.Status.InSession)
            {
                if (!roomData.hostPlayer.Equals(Player))
                {
                    StartClient(GetProperAddress(roomData.hostPlayer));
                }
            }
            if (roomData.status == MNRoomData.Status.InSession && !OnRunningClient())
            {
                StartClient(GetProperAddress(roomData.hostPlayer));
            }
            JoiningRoomData = roomData;
        }

        // Update is called once per frame
        void Update()
        {

        }

        string GetProperAddress(MNPlayer hostPlayer)
        {
            if (hostPlayer.Equals(Player))
            {
                return "127.0.0.1";
            }else if (hostPlayer.wanAddress == Player.wanAddress)
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
            if (roomData.capacity != -1 && roomData.capacity <= roomData.players.Count)
            {
                Debug.Log("部屋が満員です！");
                return;
            }
            roomData.players.Add(Player);
            if (database.SetRoomData(roomId, MNRoomData.ToJson(roomData)))
            {
                joiningRoomId = roomId;
                JoiningRoomData = roomData;
                OnRoom = true;
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
            if (joiningRoomId == -1) return;
            if (JoiningRoomData.players.Count <= 1)
            {
                if (database.DeleteRoom(joiningRoomId))
                {
                    joiningRoomId = -1;
                    JoiningRoomData = null;
                }
                return;
            }
            JoiningRoomData.players.Remove(Player);
            if (JoiningRoomData.adminPlayer.Equals(Player))
            {
                JoiningRoomData.adminPlayer = JoiningRoomData.players[0];
            }
            if (JoiningRoomData.hostPlayer.Equals(Player))
            {
                //いろいろやる
            }
            database.SetRoomData(joiningRoomId, MNRoomData.ToJson(JoiningRoomData));
            OnRoom = false;
        }

        public void StartSession()
        {
            if (!JoiningRoomData.adminPlayer.Equals(Player))
            {
                //確定された仕様ではなく、簡単に作るための仮置き
                //send request?
                Debug.Log("部屋主でないためセッションをスタートできません");
                return;
            }
            HostDeterminer determiner = new HostDeterminer(JoiningRoomData.players);
            determiner.Run();
            if (determiner.GetHost() == null)
            {
                Debug.Log("ホスト可能なプレイヤーが存在しません！");
                return;
            }
            JoiningRoomData.hostPlayer = determiner.GetHost();
            JoiningRoomData.status = MNRoomData.Status.Connecting;
            database.SetRoomData(joiningRoomId, MNRoomData.ToJson(JoiningRoomData));
            Debug.Log("Start Session!");
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