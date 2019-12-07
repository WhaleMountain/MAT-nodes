using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FireSharp;
using System.Timers;
using MATNodes;
using FireSharp.Interfaces;
using FireSharp.Response;

namespace FirebaseDatabase
{
    public class BarusFirebaseDatabase : MNIDatabase
    {
        public event OnRoomDataChanged OnRoomDataChangedEvent;

        private IFirebaseClient client;
        private Dictionary<int, string> roomDatas;
        private Timer timer;

        class RoomData
        {
            public string data { get; set; }
        }

        public BarusFirebaseDatabase()
        {
            roomDatas = new Dictionary<int, string>();
            timer = new Timer(2000);
            timer.Elapsed += Timer_Elapsed;

            var config = new FireSharp.Config.FirebaseConfig
            {
                AuthSecret = "9dZg3lsWe9BYGfbAcOXt5Y2Ll7fkVHVZhmj5TGPc",
                BasePath = "https://matnodes-demo.firebaseio.com"
            };
            client = new FirebaseClient(config);
            ListenStream();
            RefleshDatas();

            timer.Start();
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            RefleshDatas();
        }

        async void ListenStream()
        {
            await client.OnAsync("rooms", (sender, args, context) =>
            {
                string[] temp = args.Path.Split('/');// rooms/0 => 0
                int id = int.Parse(temp[1]);
                if (roomDatas.ContainsKey(id))
                {
                    roomDatas[id] = args.Data;
                }
                else
                {
                    roomDatas.Add(id, args.Data);
                }
                OnRoomDataChangedEvent(id);
            });
        }
        async void SetData(int roomId, string data)
        {
            await client.SetAsync("rooms/" + roomId, new RoomData { data = data });
        }
        async void UpdateData(int roomId, string data)
        {
            await client.UpdateAsync("rooms/" + roomId, new RoomData { data = data });
        }
        async void DeleteData(int roomId)
        {
            await client.DeleteAsync("rooms/" + roomId);
        }
        async void RefleshDatas()
        {
            FirebaseResponse response = await client.GetAsync("rooms/");
            //MNListServer.Instance.Log("body: " + response.Body);
            if (response.Body == null || response.Body == "") return;
            List<RoomData> datas = response.ResultAs<List<RoomData>>();
            Dictionary<int, string> temp = new Dictionary<int, string>(roomDatas);
            List<int> changedId = new List<int>();
            roomDatas.Clear();
            if (datas == null || datas.Count <= 0) return;
            for (int i = 0; i < datas.Count; i++)
            {
                if (temp.ContainsKey(i) && temp[i] != datas[i].data)
                {
                    changedId.Add(i);
                }
                roomDatas.Add(i, datas[i].data);
            }
            foreach (int i in changedId)
            {
                OnRoomDataChangedEvent(i);
            }
        }

        public int CreateRoom(string roomData)
        {
            int id = roomDatas.Count;
            SetData(id, roomData);
            return id;
        }

        public bool DeleteRoom(int roomId)
        {
            DeleteData(roomId);
            return true;
        }

        public string GetRoomData(int roomId)
        {
            RefleshDatas();
            if (!roomDatas.ContainsKey(roomId)) return "";
            return roomDatas[roomId];
        }

        public Dictionary<int, string> GetRoomList()
        {
            RefleshDatas();
            return roomDatas;
        }

        public bool IsValid()
        {
            return true;
        }

        public void OnDestroy()
        {

        }

        public bool SetRoomData(int roomId, string roomData)
        {
            if (roomDatas.ContainsKey(roomId))
            {
                UpdateData(roomId, roomData);
            }
            else
            {
                SetData(roomId, roomData);
            }
            return true;
        }
    }
}
