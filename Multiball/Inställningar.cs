using System;
using System.IO;

namespace Multiball
{
    public static class Inställningar
    {
        static string _filväg = "inställningar.txt";

        public static void Spara(int antalFiender, float maxFart, int antalLiv, int styrLäge, int magnetism, bool logga)
        {
            File.WriteAllLines(_filväg, new[]
            {
                antalFiender.ToString(),
                maxFart.ToString(),
                antalLiv.ToString(),
                styrLäge.ToString(),
                magnetism.ToString(),
                logga ? "1" : "0"
            });
        }

        public static (int antalFiender, float maxFart, int antalLiv, int styrLäge, int magnetism, bool logga) Ladda()
        {
            // Standardvärden om filen inte finns
            if (!File.Exists(_filväg))
                return (5, 8f, 3, 0, 0, false);

            try
            {
                var rader = File.ReadAllLines(_filväg);
                return (
                    int.Parse(rader[0]),
                    float.Parse(rader[1]),
                    int.Parse(rader[2]),
                    int.Parse(rader[3]),
                    int.Parse(rader[4]),
                    rader[5] == "1"
                );
            }
            catch
            {
                // Om filen är skadad – använd standardvärden
                return (5, 8f, 3, 0, 0, false);
            }
        }
    }
}