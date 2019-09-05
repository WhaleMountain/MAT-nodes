using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessagePack;

namespace MATNet
{
    [MessagePackObject]
    public class MNRoomData
    {
        public enum Status
        {
            WaitingPlayer,
            Connecting,
            InSession
        }
        [Key(0)]
        public Status status { get; set; }
        [Key(1)]
        public MNPlayer hostPlayer { get; set; }
        [Key(2)]
        public int hostPortNum { get; set; }
        [Key(3)]
        public int maxPlayer { get; set; }
        [Key(4)]
        public MNPlayer adminPlayer { get; set; }
        [Key(5)]
        public int roomID { get; set; }
        [Key(6)]
        public string roomName { get; set; }
        [Key(7)]
        public List<MNPlayer> players { get; set; }
        [Key(8)]
        public string clientMethod { get; set; }
        [Key(9)]
        public string serverMethod { get; set; }
        [Key(10)]
        public Dictionary<string, object> additionalDatas { get; set; }
    }

    public delegate void OnRoomDataChanged(MNRoomData roomData);

    public interface MNILobbyManager
    {
        /// <summary>
        /// このデータベースが使用可能かどうか
        /// </summary>
        /// <returns></returns>
        bool IsValidMethod();
        /// <summary>
        /// 
        /// </summary>
        /// <param name="roomId"></param>
        /// <returns></returns>
        MNRoomData GetRoomData(int roomId);
        /// <summary>
        /// ルームデータを設定
        /// </summary>
        /// <param name="roomData"></param>
        /// <returns>成功したか（権限等による失敗の可能性）</returns>
        bool SetRoomData(MNRoomData roomData);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="roomName"></param>
        /// <param name="admin"></param>
        /// <returns></returns>
        MNRoomData CreateRoom(string roomName, MNPlayer admin);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="roomID"></param>
        /// <returns>成功したか</returns>
        bool DeleteRoom(int roomID);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="roomID"></param>
        /// <param name="player"></param>
        /// <returns>成功したか</returns>
        bool JoinRoom(int roomID, MNPlayer player);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="roomID"></param>
        /// <param name="player"></param>
        void LeaveRoom(int roomID, MNPlayer player);
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        List<MNRoomData> GetRoomList();
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        string GetNewUuid();
        void OnDestroy();
        /// <summary>
        /// ルームデータに更新があった時に呼び出す
        /// </summary>
        event OnRoomDataChanged OnRoomDataChangedEvent;
    }
}
