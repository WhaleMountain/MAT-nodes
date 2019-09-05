using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MessagePack;

namespace MATNet
{
    [MessagePack.MessagePackObject]
    public class MNPlayer
    {
        [Key(0)]
        public string displayName { get; set; }
        [Key(1)]
        public string uuid { get; set; }
        [Key(2)]
        public string lanIP { get; set; }
        [Key(3)]
        public string wanIP { get; set; }
        [Key(4)]
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
