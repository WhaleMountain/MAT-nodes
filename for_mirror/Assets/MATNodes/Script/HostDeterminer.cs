using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MATNodes
{
    public class HostDeterminer
    {
        private List<MNPlayer> players;
        private MNPlayer hostPlayer;

        public HostDeterminer(List<MNPlayer> playerList)
        {
            players = playerList;
        }
        public MNPlayer GetHost()
        {
            if (hostPlayer == null)
            {
                Run();
            }
            return hostPlayer;
        }
        public void Run()
        {
            foreach (MNPlayer player in players)
            {
                if (player.canHost)
                {
                    hostPlayer = player;
                }
            }
        }
    }
}
