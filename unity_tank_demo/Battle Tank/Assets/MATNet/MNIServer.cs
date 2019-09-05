using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MATNet
{
    /// <summary>
    /// 通信をリレーするサーバ。単純なエコーサーバを実装してください。
    /// </summary>
    public interface MNIServer
    {
        bool IsValidMethod();
        int listeningPort { get; set; }
        void Start();
        void Stop();
    }
}
