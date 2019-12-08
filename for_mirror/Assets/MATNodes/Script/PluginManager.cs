using System;

namespace MATNodes
{
    public class PluginInfo
    {
        private string _location;
        private string _className;

        /// <summary>
        /// PluginInfoクラスのコンストラクタ
        /// </summary>
        /// <param name="path">アセンブリファイルのパス</param>
        /// <param name="cls">クラスの名前</param>
        private PluginInfo(string path, string cls)
        {
            _location = path;
            _className = cls;
        }

        /// <summary>
        /// アセンブリファイルのパス
        /// </summary>
        public string Location
        {
            get { return _location; }
        }

        /// <summary>
        /// クラスの名前
        /// </summary>
        public string ClassName
        {
            get { return _className; }
        }

        /// <summary>
        /// 有効なプラグインを探す
        /// </summary>
        /// <returns>有効なプラグインのPluginInfo配列</returns>
        public static PluginInfo[] FindPlugins(string findFolder)
        {
            System.Collections.ArrayList plugins = new System.Collections.ArrayList();
            string pluginName = typeof(MNIDatabase).FullName;
            string[] dlls = System.IO.Directory.GetFiles(findFolder, "*.dll");
            foreach (string dll in dlls)
            {
                try
                {
                    System.Reflection.Assembly asm = System.Reflection.Assembly.LoadFrom(dll);
                    foreach (Type t in asm.GetTypes())
                    {
                        if (t.IsClass && t.IsPublic && !t.IsAbstract && t.GetInterface(pluginName) != null)
                        {
                            plugins.Add(new PluginInfo(dll, t.FullName));
                        }
                    }
                }
                catch
                {
                }
            }
            return (PluginInfo[])plugins.ToArray(typeof(PluginInfo));
        }

        /// <summary>
        /// プラグインクラスのインスタンスを作成する
        /// </summary>
        /// <returns>プラグインクラスのインスタンス</returns>
        public MNIDatabase CreateInstance()
        {
            try
            {
                System.Reflection.Assembly asm = System.Reflection.Assembly.LoadFrom(this.Location);
                return (MNIDatabase)asm.CreateInstance(this.ClassName);
            }
            catch
            {
                return null;
            }
        }
    }

    public class PluginManager
    {
        public static MNIDatabase GetAvailableDatabase()
        {
            //System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + 
            string path = "Plugins";
            PluginInfo[] pluginInfos = PluginInfo.FindPlugins(path);
            foreach (var info in pluginInfos)
            {
                MNIDatabase instance = info.CreateInstance();
                if (instance.IsValid())
                {
                    return instance;
                }
            }
            return null;
        }
    }
}
