using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;

namespace HelpLib.Config
{
    [DataContract]
    public struct ConfigFile
    {
        [DataMember]
        public IPEndPoint LocalHost { get; private set; }
        [DataMember]
        public IPEndPoint RemoteHost { get; private set; }
        [DataMember]
        public string UserName { get; private set; }

        public ConfigFile(string local, string host, string nickName)
        {
            var args = local.Split(':');
            LocalHost = new IPEndPoint(IPAddress.Parse(args[0]), int.Parse(args[1]));
            args = host.Split(':');
            RemoteHost = new IPEndPoint(IPAddress.Parse(args[0]), int.Parse(args[1]));
            UserName = nickName;
        }
    }

    public class Config
    {
        public static ConfigFile GlobalConfig { get; set; }

        public static void Load(string path, out ConfigFile configFile)
        {
            if (!File.Exists(path))
                throw new FileNotFoundException("File not found: " + path);

            FileStream file = new FileStream(path, FileMode.Open);
            DataContractJsonSerializer serialize = new DataContractJsonSerializer(typeof(ConfigFile));
            configFile = (ConfigFile)serialize.ReadObject(file);
            file.Close();
        }

        public static void Save(string path, ConfigFile configFile)
        {
            FileStream file = new FileStream(path, FileMode.Create);
            DataContractJsonSerializer serialize = new DataContractJsonSerializer(typeof(ConfigFile));
            serialize.WriteObject(file, configFile);
            file.Close();
        }
    }
}
