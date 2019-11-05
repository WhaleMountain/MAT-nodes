using System.IO;
using System.Collections;
using System.Threading.Tasks;
using System.Collections.Generic;
using Firebase;
using Firebase.Database;
using Firebase.Unity.Editor;
using Newtonsoft.Json;

namespace MATNet.Plugins
{
    class FirebaseManager
    {

        private static string AllroomData = null;

        public static void CreateJson(object roomData)
        {
            string json = JsonConvert.SerializeObject(roomData);
            File.WriteAllText("roomData.json", json);
        }

        public static void pushRoomData(MNRoomData roomData)
        {
            FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://mat-nodes.firebaseio.com");
            DatabaseReference reference = FirebaseDatabase.DefaultInstance.RootReference;

            CreateJson(roomData);

            string json = File.ReadAllText("roomData.json");

            Task.Run(async () =>
            {
                await reference.Child("Rooms").Child(roomData.roomID.ToString()).SetRawJsonValueAsync(json);
            });
        }

        public static void deleteRoomData(string roomID)
        {
            FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://mat-nodes.firebaseio.com");
            DatabaseReference reference = FirebaseDatabase.DefaultInstance.RootReference;

            Task.Run(async () =>
            {
                await reference.Child("Rooms").Child(roomID).SetRawJsonValueAsync(null);
            });
        }

        public static Dictionary<int, MNRoomData> GetRoomList()
        {
            Dictionary<int, MNRoomData> roomList = new Dictionary<int, MNRoomData>();
            MNRoomData roomData = new MNRoomData();
            ArrayList splitJsonData = roomDataSplit();
            int cnt = 0;

            foreach ( string json in splitJsonData)
            {
                if (json != null)
                {
                    string jsontmp = json.TrimStart('[').TrimEnd(']');

                    roomData = JsonConvert.DeserializeObject<MNRoomData>(jsontmp);
                    roomList.Add(cnt, roomData);
                    cnt += 1;
                }
            }

            return roomList;
        }

        public static bool IsValidDatabase()
        {
            return true;
        }

        public static void CheckDatabase()
        {
            FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://mat-nodes.firebaseio.com");
            DatabaseReference reference = FirebaseDatabase.DefaultInstance.RootReference;

            reference.Child("Rooms").ValueChanged += HandleValueChanged;
        }

        public  static void HandleValueChanged(object sender, ValueChangedEventArgs args)
        {
            if (args.DatabaseError != null)
            {
                MNTools.DebugLog(args.DatabaseError.Message);
                return;
            }
            string roomDataJson = args.Snapshot.GetRawJsonValue().Replace(@"k__BackingField", "");
            AllroomData = roomDataJson;
        }

        public static ArrayList roomDataSplit()
        {
            string[] roomData = AllroomData.Split(',');
            ArrayList roomList = new ArrayList();
            int roomCount = 0;

            roomList.Add(roomData[0]);
            roomData[0] = "";

            foreach (string str in roomData)
            {
                if( str.IndexOf("adminPlayer") > 0)
                {
                    roomList.Add(str);
                    roomCount += 1;
                } else if( str != "")
                {
                    roomList[roomCount] += ',' + str;
                }
            }

            return roomList;
        }
    }
}