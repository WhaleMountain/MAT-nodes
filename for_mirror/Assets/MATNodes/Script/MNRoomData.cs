using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Mirror;
using Newtonsoft.Json;

namespace MATNodes
{
    [Serializable]
    public class MNRoomData
    {
        public enum RoomStatus
        {
            WaitingPlayer,
            DeterminingHost,
            Connecting,
            InSession
        }
        public RoomStatus Status { get; set; }
        public MNPlayer HostPlayer { get; set; }
        public MNPlayer AdminPlayer { get; set; }
        public string RoomName { get; set; }
        public int Capacity { get; set; }
        public List<MNPlayer> Players { get; set; }
        public List<MNPlayer> JoinRequests { get; set; }

        public MNRoomData(string roomName, MNPlayer adminPlayer, int capacity)
        {
            Status = RoomStatus.WaitingPlayer;
            HostPlayer = null;
            this.AdminPlayer = adminPlayer;
            this.RoomName = roomName;
            this.Capacity = capacity;
            Players = new List<MNPlayer>();
            Players.Add(adminPlayer);
            JoinRequests = new List<MNPlayer>();
        }

        public static MNRoomData FromJson(string json)
        {
            //return JsonUtility.FromJson<MNRoomData>(json);
            return JsonConvert.DeserializeObject<MNRoomData>(json);
        }
        public static string ToJson(MNRoomData data)
        {
            //return JsonUtility.ToJson(data);
            return JsonConvert.SerializeObject(data);
        }
    }
}
