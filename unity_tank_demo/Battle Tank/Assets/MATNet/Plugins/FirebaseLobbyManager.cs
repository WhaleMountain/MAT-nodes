using System;
using System.Collections.Generic;
using System.Linq;

namespace MATNet.Plugins
{
    public class FirebaseLobbyManager : MNILobbyManager
    {
        private Dictionary<int, MNRoomData> roomList;
        public event OnRoomDataChanged OnRoomDataChangedEvent;

        public FirebaseLobbyManager()
        {
            FirebaseManager.CheckDatabase();
            roomList = new Dictionary<int, MNRoomData>();
        }

        public MNRoomData CreateRoom(string roomName, MNPlayer admin)
        {
            MNRoomData roomData = new MNRoomData();

            try
            {
                roomList = FirebaseManager.GetRoomList();
            } catch (NullReferenceException e)
            {
                MNTools.DebugLog(e);
            }

            roomData.status = MNRoomData.Status.WaitingPlayer;
            roomData.roomName = roomName;
            roomData.roomID = GetNewRoomID();
            roomData.adminPlayer = admin;
            roomData.adminPlayer.uuid = GetNewUuid();

            FirebaseManager.pushRoomData(roomData);

            return roomData;
        }

        private int GetNewRoomID()
        {
            if (roomList.Count == 0)
            {
                return 0;
            }
            else
            {
                return roomList.Last().Value.roomID + 1;
            }
        }

        public bool DeleteRoom(int roomID)
        {
            FirebaseManager.deleteRoomData(roomID.ToString());
            return true;
        }

        public string GetNewUuid()
        {
            return RequestNewUuid();
        }

        private string RequestNewUuid()
        {
            return Guid.NewGuid().ToString();
        }

        public MNRoomData GetRoomData(int roomId)
        {
            return roomList[roomId];
        }

        public List<MNRoomData> GetRoomList()
        {
            roomList = FirebaseManager.GetRoomList();
            return roomList.Values.ToList();//FirebaseManager.GetRoomList().Values.ToList();
        }

        public bool IsValidMethod()
        {
            return FirebaseManager.IsValidDatabase();
        }

        public bool JoinRoom(int roomID, MNPlayer player)
        {
            roomList = FirebaseManager.GetRoomList();
            MNRoomData roomData = roomList[roomID];
            if (roomData.players == null)
            {
                roomData.players = new List<MNPlayer>();
            }
            roomData.players.Add(player);

            FirebaseManager.pushRoomData(roomData);
            return true;
        }

        public void LeaveRoom(int roomID, MNPlayer player)
        {
            MNRoomData roomData = roomList[roomID];
            roomData.players.Remove(player);
            
            FirebaseManager.pushRoomData(roomData);
        }

        public bool SetRoomData(MNRoomData roomData)
        {
            FirebaseManager.pushRoomData(roomData);
            return true;
        }

        public void OnDestroy()
        {
            return;
        }
    }
}