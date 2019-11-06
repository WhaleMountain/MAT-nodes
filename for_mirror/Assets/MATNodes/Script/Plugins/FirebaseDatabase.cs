using System.Collections;
using System.Collections.Generic;

namespace MATNodes.Plugins
{
    public class FirebaseDatabase : MNIDatabase
    {
        public event OnRoomDataChanged OnRoomDataChangedEvent;

        public int CreateRoom(string roomData)
        {
            throw new System.NotImplementedException();
        }

        public bool DeleteRoom(int roomId)
        {
            throw new System.NotImplementedException();
        }

        public string GetRoomData(int roomId)
        {
            throw new System.NotImplementedException();
        }

        public Dictionary<int, string> GetRoomList()
        {
            throw new System.NotImplementedException();
        }

        public bool IsValid()
        {
            throw new System.NotImplementedException();
        }

        public void OnDestroy()
        {
            throw new System.NotImplementedException();
        }

        public bool SetRoomData(int roomId, string roomData)
        {
            throw new System.NotImplementedException();
        }
    }
}
