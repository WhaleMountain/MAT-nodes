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

        class PlayerPingPair
        {
            public MNPlayer player;
            public long ping;

            public PlayerPingPair(MNPlayer player)
            {
                this.player = player;
                ping = 4000;//pingのデフォルトタイムアウト時間
            }
        }

        public HostDeterminer(List<MNPlayer> playerList)
        {
            players = playerList;
        }
        public MNPlayer GetHost()
        {
            return hostPlayer;
        }
        public void Run()
        {
            hostPlayer = null;
            List<PlayerPingPair> hostables = new List<PlayerPingPair>();
            System.Net.NetworkInformation.Ping ping = new System.Net.NetworkInformation.Ping();
            foreach (MNPlayer player in players)
            {
                if (player.canHost)
                {
                    var pair = new PlayerPingPair(player);
                    var reply = ping.Send(MNListServer.Instance.GetProperAddress(player));
                    if (reply.Status == System.Net.NetworkInformation.IPStatus.Success)
                    {
                        pair.ping = reply.RoundtripTime;
                    }
                    hostables.Add(pair);
                }
            }
            ping.Dispose();
            if (hostables.Count <= 0) return;
            PlayerPingPair best = hostables[0];
            foreach (PlayerPingPair pair in hostables)
            {
                if (best.ping > pair.ping)
                {
                    best = pair;
                }
            }
            hostPlayer = best.player;
        }
    }
}
