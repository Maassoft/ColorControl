using ColorControl;
using System.IO;

namespace LgTv
{
    public class ClientKeyStore
    {
        private string ip;

        public ClientKeyStore(string ip)
        {
            this.ip = ip;
        }

        public string GetClientKey()
        {
            var filename = Path.Combine(Utils.GetDataPath(), ip + "_ClientKey.txt");
            if (File.Exists(filename))
            {
                return File.ReadAllText(filename);
            }
            return null;
        }

        public void SaveClientKey(string key)
        {
            var filename = Path.Combine(Utils.GetDataPath(), ip + "_ClientKey.txt");
            File.WriteAllText(filename, key);
        }

        public bool HasValidHandShake(string expectedHandShake)
        {
            var handShake = GetHandShake();

            return handShake?.Equals(expectedHandShake) ?? false;
        }

        public string GetHandShake()
        {
            var filename = Path.Combine(Utils.GetDataPath(), ip + "_HandShake.txt");
            if (File.Exists(filename))
            {
                return File.ReadAllText(filename);
            }
            return null;
        }

        public void SaveHandShake(string data)
        {
            var filename = Path.Combine(Utils.GetDataPath(), ip + "_HandShake.txt");
            File.WriteAllText(filename, data);
        }
    }
}
