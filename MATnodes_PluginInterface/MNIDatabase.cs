using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MATNodes
{
    public delegate void OnRoomDataChanged(int roomId);

    public interface MNIDatabase
    {
        event OnRoomDataChanged OnRoomDataChangedEvent;
        bool IsValid();
        string GetRoomData(int roomId);
        bool SetRoomData(int roomId, string roomData);
        int CreateRoom(string roomData);
        bool DeleteRoom(int roomId);
        Dictionary<int, string> GetRoomList();
        void OnDestroy();
    }
}
