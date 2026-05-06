using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Multiball
{
    // Statisk klass = inga instanser, anropas direkt: Draw2D.Cirkel(...)
    // Tänk på den som ett ritbibliotek – precis som Processing
    public static class Draw2D
    {
        // _pixel skapas en gång och delas av alla metoder
        static Texture2D _pixel;

        // Måste anropas en gång i LoadContent innan ritfunktionerna används
        public static void Init(GraphicsDevice graphicsDevice)
        {
            _pixel = new Texture2D(graphicsDevice, 1, 1);
            _pixel.SetData(new[] { Color.White });
        }

        // --- Cirkel ---
        public static void Cirkel(SpriteBatch sb, int cx, int cy, int radie, Color färg)
        {
            for (int y = -radie; y <= radie; y++)
            {
                int bredd = (int)Math.Sqrt(radie * radie - y * y) * 2;
                int x = cx - bredd / 2;
                sb.Draw(_pixel, new Rectangle(x, cy + y, bredd, 1), färg);
            }
        }

        // --- Linje (Bresenhams algoritm) ---
        public static void Linje(SpriteBatch sb, int x0, int y0, int x1, int y1, Color färg)
        {
            int dx = Math.Abs(x1 - x0);
            int dy = Math.Abs(y1 - y0);
            int sx = x0 < x1 ? 1 : -1;
            int sy = y0 < y1 ? 1 : -1;
            int err = dx - dy;

            while (true)
            {
                sb.Draw(_pixel, new Rectangle(x0, y0, 1, 1), färg);
                if (x0 == x1 && y0 == y1) break;
                int e2 = 2 * err;
                if (e2 > -dy) { err -= dy; x0 += sx; }
                if (e2 < dx) { err += dx; y0 += sy; }
            }
        }

        // --- Rektangel (bara kanten) ---
        public static void Rektangel(SpriteBatch sb, int x, int y, int bredd, int höjd, Color färg)
        {
            Linje(sb, x, y, x + bredd, y, färg); // topp
            Linje(sb, x, y + höjd, x + bredd, y + höjd, färg); // botten
            Linje(sb, x, y, x, y + höjd, färg); // vänster
            Linje(sb, x + bredd, y, x + bredd, y + höjd, färg); // höger
        }

        // --- Fylld rektangel ---
        public static void FylldRektangel(SpriteBatch sb, int x, int y, int bredd, int höjd, Color färg)
        {
            sb.Draw(_pixel, new Rectangle(x, y, bredd, höjd), färg);
        }

        // Båge, polygon m.m. kan du lägga till här senare...
    }
}