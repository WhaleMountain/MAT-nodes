using Mirror;
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

        public MNPlayer player;
        public static string databaseName = "FirebaseDatabase";
        MNIDatabase database;
        int joiningRoomId;
        MNRoomData joiningRoomData;

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
        bool OnRunningClient()
        {
            return NetworkManager.singleton.isNetworkActive;
        }

        // Start is called before the first frame update
        void Start()
        {
            database = (MNIDatabase)MNTools.GetInstance(databaseName);
            database.OnRoomDataChangedEvent += Database_OnRoomDataChangedEvent;
            player = new MNPlayer("Player");
        }

        private void Database_OnRoomDataChangedEvent(int roomId)
        {
            if (roomId != joiningRoomId)
            {
                return;
            }
            MNRoomData roomData = MNRoomData.FromJson(database.GetRoomData(roomId));
            if (roomData.status == MNRoomData.Status.Connecting)
            {
                if (roomData.hostPlayer.Equals(player))
                {
                    StartHost();
                    roomData.status = MNRoomData.Status.InSession;
                    database.SetRoomData(joiningRoomId, MNRoomData.ToJson(roomData));
                }
            }
            if (joiningRoomData.status == MNRoomData.Status.Connecting && roomData.status == MNRoomData.Status.InSession)
            {
                if (!roomData.hostPlayer.Equals(player))
                {
                    StartClient(GetProperAddress(roomData.hostPlayer));
                }
            }
            if (roomData.status == MNRoomData.Status.InSession && !OnRunningClient())
            {
                StartClient(GetProperAddress(roomData.hostPlayer));
            }
            joiningRoomData = roomData;
        }

        // Update is called once per frame
        void Update()
        {

        }

        string GetProperAddress(MNPlayer hostPlayer)
        {
            if (hostPlayer.Equals(player))
            {
                return "127.0.0.1";
            }else if (hostPlayer.wanAddress == player.wanAddress)
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
            if (player == null)
            {
                Debug.Log("player instance is null");
                return;
            }
            MNRoomData roomData = MNRoomData.FromJson(database.GetRoomData(roomId));
            if (roomData.capacity <= roomData.players.Count)
            {
                Debug.Log("部屋が満員です！");
                return;
            }
            roomData.players.Add(player);
            if (database.SetRoomData(roomId, MNRoomData.ToJson(roomData)))
            {
                joiningRoomId = roomId;
                joiningRoomData = roomData;
            }
            else
            {
                Debug.Log("部屋への参加に失敗しました");
            }
        }

        public void CreateRoom(string roomName)
        {
            if (player == null)
            {
                Debug.Log("player instance is null");
                return;
            }
            MNRoomData roomData = new MNRoomData(roomName, player);
            joiningRoomId = database.CreateRoom(MNRoomData.ToJson(roomData));
            joiningRoomData = roomData;
        }

        public void LeaveRoom()
        {
            if (joiningRoomData.players.Count <= 1)
            {
                if (database.DeleteRoom(joiningRoomId))
                {
                    joiningRoomId = -1;
                    joiningRoomData = null;
                }
                return;
            }
            joiningRoomData.players.Remove(player);
            if (joiningRoomData.adminPlayer.Equals(player))
            {
                joiningRoomData.adminPlayer = joiningRoomData.players[0];
            }
            if (joiningRoomData.hostPlayer.Equals(player))
            {
                //いろいろやる
            }
            database.SetRoomData(joiningRoomId, MNRoomData.ToJson(joiningRoomData));
        }

        public void StartSession()
        {
            if (!joiningRoomData.adminPlayer.Equals(player))
            {
                //確定された仕様ではなく、簡単に作るための仮置き
                //send request?
                Debug.Log("部屋主でないためセッションをスタートできません");
                return;
            }
            HostDeterminer determiner = new HostDeterminer(joiningRoomData.players);
            determiner.Run();
            joiningRoomData.hostPlayer = determiner.GetHost();
            joiningRoomData.status = MNRoomData.Status.Connecting;
            database.SetRoomData(joiningRoomId, MNRoomData.ToJson(joiningRoomData));
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
            if (database != null)
            {
                database.OnDestroy();
            }
        }
    }
}