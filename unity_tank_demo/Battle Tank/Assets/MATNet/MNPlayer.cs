using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using MessagePack;

namespace MATNet
{
    [MessagePack.MessagePackObject, Serializable, DataContract]
    public class MNPlayer
    {
        [Key(0)]
        [DataMember(Name = "displayName")]
        public string displayName { get; set; }
        [Key(1)]
        [DataMember(Name = "uuid")]
        public string uuid { get; set; }
        [Key(2)]
        [DataMember(Name = "lanIP")]
        public string lanIP { get; set; }
        [Key(3)]
        [DataMember(Name = "wanIP")]
        public string wanIP { get; set; }
        [Key(4)]
        [DataMember(Name = "hostableMethods")]
        public string[] hostableMethods { get; set; }

        public MNPlayer() : this("Player")
        {
            
        }
        public MNPlayer(string displayName)
        {
            this.displayName = displayName;
            uuid = MNManager.Instance.GetNewUuid();
            hostableMethods = ClientMethodDeterminer.GetHostableMethods();
            lanIP = MNTools.localAddress.ToString();
            wanIP = MNTools.globalAddress.ToString();
        }
        public void ChangeDisplayName(string name)
        {
            displayName = name;
        }
    }
}
