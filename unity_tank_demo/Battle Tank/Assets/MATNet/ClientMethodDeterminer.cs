using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MATNet
{
    public class ClientMethodDeterminer
    {
        static Dictionary<string, string> methodNamePairs = new Dictionary<string, string>()
        {
            {"WebSocketServer2", "WebSocketClient"}
        };

        MNPlayer hostPlayer;
        string serverMethod;
        string clientMethod;

        public void DetermineHost(MNRoomData roomData)
        {
            //本来はここで評価、順序付けを行って最適なホスト、メソッドを決定する。今は仮置き。
            MNPlayer host = null;
            foreach (MNPlayer player in roomData.players)
            {
                if (player.hostableMethods.Length > 0)
                {
                    host = player;
                }
            }
            if (host == null) { return; }
            hostPlayer = host;
            serverMethod = host.hostableMethods[0];
            clientMethod = methodNamePairs[host.hostableMethods[0]];
        }

        public MNPlayer GetHost()
        {
            return hostPlayer;
        }

        public string GetServerMethod()
        {
            return serverMethod;
        }

        public string GetClientMethod()
        {
            return clientMethod;
        }

        public static string[] GetHostableMethods()
        {
            List<string> validNames = new List<string>();
            foreach (string name in MNManager.serverMethodNames)
            {
                MNIServer instance = (MNIServer)MNTools.GetInstance(name);
                if (instance.IsValidMethod())
                {
                    validNames.Add(name);
                }
            }
            return validNames.ToArray();
        }
    }
}
