using System;
using System.IO;

namespace Multiball
{
    public static class Inställningar
    {
        static string _filväg = "inställningar.txt";

        public static void Spara(int antalFiender, float maxFart, int antalLiv, int styrLäge, int magnetism)
        {
            File.WriteAllLines(_filväg, new[]
            {
                antalFiender.ToString(),
                maxFart.ToString(),
                antalLiv.ToString(),
                styrLäge.ToString(),
                magnetism.ToString()
            });
        }

        public static (int antalFiender, float maxFart, int antalLiv, int styrLäge, int magnetism) Ladda()
        {
            // Standardvärden om filen inte finns
            if (!File.Exists(_filväg))
                return (5, 8f, 3, 0, 0);

            try
            {
                var rader = File.ReadAllLines(_filväg);
                return (
                    int.Parse(rader[0]),
                    float.Parse(rader[1]),
                    int.Parse(rader[2]),
                    int.Parse(rader[3]),
                    int.Parse(rader[4])
                );
            }
            catch
            {
                // Om filen är skadad – använd standardvärden
                return (5, 8f, 3, 0, 0);
            }
        }
    }
}