using System.IO;

namespace Dexma_cpt_ClientSide.Storages
{
    public class StorageManager
    {
        public string Token { get; set; }
        public StorageManager() { }

        public string ReadFromFile(string username)
        {
            string filePath = $"../../../../Dexma_cpt_ClientSide/Storages/token_{username}.txt";

            if (File.Exists(filePath))
            {
                return File.ReadAllText(filePath);
            }
            else
            {
                return null;
            }
        }

        public void WriteToFile(string username, string data)
        {
            string filePath = $"../../../../Dexma_cpt_ClientSide/Storages/token_{username}.txt";

            File.WriteAllText(filePath, data);
        }
    }
}
