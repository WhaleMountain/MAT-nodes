using System.Collections;
using System.Threading.Tasks;
using System.Collections.Generic;
using Firebase;
using Firebase.Database;
using Firebase.Unity.Editor;

namespace MATNodes.Plugins
{
    public class FirebaseDatabase : MNIDatabase
    {
        public event OnRoomDataChanged OnRoomDataChangedEvent;

        private DatabaseReference reference;
        private Dictionary<int, string> AllroomData = new Dictionary<int, string>(); // roomData を管理する。
        private int NextRoomID = 0; // 次に roomID を保持する。

        FirebaseDatabase()
        {
            const string url = "https://DB-URL";
            FirebaseApp.DefaultInstance.SetEditorDatabaseUrl(url);
            //reference = FirebaseApp.DefaultInstance.RootReference;
            reference = null;
            CheckDatabase();
        }

        public int CreateRoom(string roomData)
        {
            while(AllroomData.ContainsKey(NextRoomID)) // roomIDが重複していたら NextRoomID をプラスする。
            {
                NextRoomID += 1;
            }

            Task.Run(async () =>
            {
                await reference.Child("Rooms").Child(NextRoomID.ToString()).SetValueAsync(roomData); // Firebase にデータを投げる。
                //await reference.Child("Rooms").Child(NextRoomID.ToString()).SetRawJsonValueAsync(roomData); // Jsonの時
            });

            AllroomData.Add(NextRoomID, roomData);
            return NextRoomID;
        }

        public bool DeleteRoom(int roomId)
        {
            Task.Run(async () =>
            {
                await reference.Child("Rooms").Child(roomId.ToString()).SetRawJsonValueAsync(null);
            });

            AllroomData.Remove(roomId);
            return true; // とりあえず true
        }

        public string GetRoomData(int roomId)
        {
            return AllroomData[roomId];
        }

        public Dictionary<int, string> GetRoomList()
        {
            return AllroomData;
        }

        public bool IsValid()
        {
            return true; //とりあえず true
        }

        public void OnDestroy()
        {
            
        }

        public bool SetRoomData(int roomId, string roomData)
        {
            Task.Run(async () =>
            {
                await reference.Child("Rooms").Child(roomId.ToString()).SetValueAsync(roomData); // Firebase にデータを投げる。
                //await reference.Child("Rooms").Child(roomId.ToString()).SetRawJsonValueAsync(roomData); // Jsonの時
            });

            AllroomData[roomId] = roomData;
            return true; // とりあえず true
        }

        public void CheckDatabase() // データベース の変更を検知する
        {
            reference.Child("Rooms").ValueChanged += HandleValueChanged;
        }

        public  static void HandleValueChanged(object sender, ValueChangedEventArgs args) // 変更があったら実行される。 今は動かない
        {
            if (args.DatabaseError != null)
            {
                return;
            }
            //string roomDataJson = args.Snapshot.GetValue(); // ここのデータがどうなるか
            //string roomDataJson = args.Snapshot.GetRawJsonValue(); // Jsonの時
        }
    }
}
