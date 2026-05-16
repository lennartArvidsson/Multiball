using System;
using System.IO;

namespace Multiball
{
    public static class Logg
    {
        static string _filväg = "resultat.csv";

        public static void SkrivaRad(int antalFiender, float maxFart, int antalLiv,
                                      int styrLäge, int magnetism, float överlevnadsTid)
        {
            // Skapa rubrikrad om filen inte finns
            if (!File.Exists(_filväg))
                File.WriteAllText(_filväg,
                    "Datum;Klockslag;Fiender;MaxFart;Liv;Styrning;Magnetism;Tid\n");

            string datum = DateTime.Now.ToString("yyyy-MM-dd");
            string klocka = DateTime.Now.ToString("HH:mm");
            string styrning = styrLäge == 0 ? "Direkt" : "Troghet";
            int minuter = (int)överlevnadsTid / 60;
            int sekunder = (int)överlevnadsTid % 60;
            string tid = minuter > 0
                ? $"{minuter} min {sekunder} sek"
                : $"{sekunder} sek";

            string rad = $"{datum};{klocka};{antalFiender};{maxFart};" +
                         $"{antalLiv};{styrning};{magnetism};{tid}";

            File.AppendAllText(_filväg, rad + "\n");
        }
    }
}