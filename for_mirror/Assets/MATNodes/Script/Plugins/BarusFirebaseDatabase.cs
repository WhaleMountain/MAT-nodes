using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FireSharp.Core.Interfaces;
using FireSharp.Core.Response;

namespace MATNodes.Plugins
{
    class BarusFirebaseDatabase : MNIDatabase
    {
        public event OnRoomDataChanged OnRoomDataChangedEvent;

        private IFirebaseClient client;
        private Dictionary<int, string> roomDatas;

        class RoomData
        {
            public string data { get; set; }
        }

        public BarusFirebaseDatabase()
        {
            var config = new FireSharp.Core.Config.FirebaseConfig
            {
                AuthSecret = "9dZg3lsWe9BYGfbAcOXt5Y2Ll7fkVHVZhmj5TGPc",
                BasePath = "https://matnodes-demo.firebaseio.com"
            };
            client = new FireSharp.Core.FirebaseClient(config);
            ListenStream();
            roomDatas = new Dictionary<int, string>();
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
            await client.SetAsync("rooms/"+roomId, new RoomData { data = data });
        }
        async void UpdateData(int roomId, string data)
        {
            await client.UpdateAsync("rooms/" + roomId, new RoomData { data = data });
        }
        async void DeleteData(int roomId)
        {
            await client.DeleteAsync("rooms/" + roomId);
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
            if (!roomDatas.ContainsKey(roomId)) return "";
            return roomDatas[roomId];
        }

        public Dictionary<int, string> GetRoomList()
        {
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
