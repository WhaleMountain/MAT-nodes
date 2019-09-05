using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace MATNet
{
    public delegate void OnDataReceived(string id, object[] data);
    public delegate void OnMessagaeReceived(string message);

    public interface MNIClient
    {
        bool IsValidMethod();
        void Connect(string ip, int port);
        void Connect(MNPlayer hostPlayer, int port);
        void Disconnect();
        void SendMessage(string message);
        void SendData(string id, object[] data);
        void OnDestroy();
        event OnDataReceived OnDataReceivedEvent;
        event OnMessagaeReceived OnMessageReceivedEvent;
    }
}
