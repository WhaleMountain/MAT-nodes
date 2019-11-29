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
        public enum Status
        {
            WaitingPlayer,
            Connecting,
            InSession
        }
        public Status status { get; set; }
        public MNPlayer hostPlayer { get; set; }
        public MNPlayer adminPlayer { get; set; }
        public string roomName { get; set; }
        public int capacity { get; set; }
        public List<MNPlayer> players { get; set; }

        public MNRoomData(string roomName, MNPlayer adminPlayer, int capacity)
        {
            status = Status.WaitingPlayer;
            hostPlayer = null;
            this.adminPlayer = adminPlayer;
            this.roomName = roomName;
            this.capacity = capacity;
            players = new List<MNPlayer>();
            players.Add(adminPlayer);
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
