using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MATNodes
{
    [Serializable]
    public class MNPlayer
    {
        public string displayName { get; set; }
        public string uuid { get; set; }
        public string lanAddress { get; set; }
        public string wanAddress { get; set; }
        public bool canHost { get; set; }

        public MNPlayer(string displayName)
        {
            this.displayName = displayName;
            uuid = Guid.NewGuid().ToString();
            lanAddress = MNTools.localAddress.ToString();
            wanAddress = MNTools.globalAddress.ToString();

        }

        public override bool Equals(object obj)
        {
            if (obj == null || this.GetType() != obj.GetType())
            {
                return false;
            }
            MNPlayer player = (MNPlayer)obj;
            return this.uuid == player.uuid;
        }
        public override int GetHashCode()
        {
            return this.uuid.GetHashCode();
        }
    }
}
