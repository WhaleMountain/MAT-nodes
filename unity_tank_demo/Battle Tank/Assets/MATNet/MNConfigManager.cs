using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace MATNet
{
    /// <summary>
    /// とりあえず動くように書いたけど、後で書き直す予定。
    /// </summary>
    public class MNConfigManager
    {
        public static MNConfigManager instance;
        private Dictionary<string, IDictionary<string, object>> data;

        public string jsonPath = "config.json";

        static MNConfigManager()
        {
            instance = new MNConfigManager();
        }

        public MNConfigManager()
        {
            Load();
        }

        public void Load()
        {
            Load(jsonPath);
        }
        public void Load(string path)
        {
            if (!File.Exists(path))
            {
                data = new Dictionary<string, IDictionary<string, object>>();
                return;
            }
            using (StreamReader sr = new StreamReader(path))
            {
                data = (Dictionary<string, IDictionary<string, object>>)MiniJSON.Json.Deserialize(sr.ReadToEnd());
            }
        }

        public void Save()
        {
            Save(jsonPath);
        }
        public void Save(string path)
        {
            using (StreamWriter sw = new StreamWriter(path))
            {
                sw.Write(MiniJSON.Json.Serialize(data));
            }
        }

        public static IDictionary<string, object> GetNode(string moduleName)
        {
            return instance.data[moduleName];
        }

        public static void SetNode(string moduleName, IDictionary<string, object> jsonNode)
        {
            instance.data[moduleName] = jsonNode;
        }
    }
}
