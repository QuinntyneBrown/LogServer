using System;
using System.IO;
using static Newtonsoft.Json.JsonConvert;

namespace LogServer.API
{
    public class AppInitializer
    {
        public static void Seed() {
            File.WriteAllLines($@"{Environment.CurrentDirectory}\storedEvents.json", new string[1] {
                SerializeObject(new string[0]{ })                
            });
        }
    }
}
