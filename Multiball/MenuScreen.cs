using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace Multiball
{
    public class MenuScreen
    {
        SpriteFont _font;
      

        // Inställningar med start-, min- och maxvärden
        public int AntalFiender { get; private set; } = 5;
        public float FiendeMaxFart { get; private set; } = 8f;
        public int AntalLiv { get; private set; } = 3;
        public int StyrLäge { get; private set; } = 0;  // 0=Direkt, 1=Tröghet
        public int Magnetism { get; private set; } = 0;
        public bool Logga { get; private set; } = false;


        // Vilket alternativ är markerat just nu
        int _valt = 0;
        int _antalVal = 6;

        // Tangenthantering – vänta lite mellan knapptryckningar
        float _knapptid = 0f;

        public MenuScreen(SpriteFont font)
        {
            _font = font;
        }

        // Returnerar true när spelaren tryckt Enter = starta spelet
        public bool Update(GameTime gameTime)
        {
            _knapptid -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (_knapptid > 0) return false;

            var k = Keyboard.GetState();

            // Navigera upp/ner
            if (k.IsKeyDown(Keys.Up)) { _valt = (_valt - 1 + _antalVal) % _antalVal; _knapptid = 0.2f; }
            if (k.IsKeyDown(Keys.Down)) { _valt = (_valt + 1) % _antalVal; _knapptid = 0.2f; }

            // Justera värden med vänster/höger
            if (k.IsKeyDown(Keys.Left))
            {
                if (_valt == 0) AntalFiender = System.Math.Max(1, AntalFiender - 1);
                if (_valt == 1) FiendeMaxFart = System.Math.Max(2f, FiendeMaxFart - 1f);
                if (_valt == 2) AntalLiv = System.Math.Max(1, AntalLiv - 1);
                if (_valt == 3) StyrLäge = (StyrLäge == 0) ? 1 : 0;
                if (_valt == 4) Magnetism = System.Math.Max(0, Magnetism - 1);
                if (_valt == 5) Logga = false;
                _knapptid = 0.15f;
            }
            if (k.IsKeyDown(Keys.Right))
            {
                if (_valt == 0) AntalFiender = System.Math.Min(20, AntalFiender + 1);
                if (_valt == 1) FiendeMaxFart = System.Math.Min(15f, FiendeMaxFart + 1f);
                if (_valt == 2) AntalLiv = System.Math.Min(10, AntalLiv + 1);
                if (_valt == 3) StyrLäge = (StyrLäge == 0) ? 1 : 0;
                if (_valt == 4) Magnetism = System.Math.Min(10, Magnetism + 1);
                if (_valt == 5) Logga = true;
                _knapptid = 0.15f;
            }

            // Enter = starta
            if (k.IsKeyDown(Keys.Enter)) return true;

            return false;
        }

        public void Draw(SpriteBatch sb, int bredd, int höjd)
        {
            int cx = bredd / 2;
            int cy = höjd / 2;

            // Titel
            RitaCentreradText(sb, "M U L T I B A L L", cx, cy - 160, Color.Yellow);

            // Menyval
            RitaMenyRad(sb, "Antal fiender", AntalFiender.ToString(), cx, cy - 60, _valt == 0);
            RitaMenyRad(sb, "Fiendefart max", FiendeMaxFart.ToString(), cx, cy, _valt == 1);
            RitaMenyRad(sb, "Antal liv", AntalLiv.ToString(), cx, cy + 60, _valt == 2);

            string styrText = StyrLäge == 0 ? "Direkt" : "Tröghet";
            RitaMenyRad(sb, "Styrning", styrText, cx, cy + 120, _valt == 3);
            RitaMenyRad(sb, "Magnetism", Magnetism.ToString(), cx, cy + 180, _valt == 4);

            // Instruktion längst ner
            RitaCentreradText(sb, "Piltangenter for att justera   Enter for att starta", cx, cy + 340, Color.Gray);

            string loggaText = Logga ? "Ja" : "Nej";
            RitaMenyRad(sb, "Logga resultat", loggaText, cx, cy + 240, _valt == 5);
        }

        void RitaMenyRad(SpriteBatch sb, string etikett, string värde, int cx, int cy, bool markerad)
        {
            Color färg = markerad ? Color.White : Color.DarkGray;

            // Pil vid markerat val
            if (markerad)
            {
                RitaCentreradText(sb, "< " + värde + " >", cx + 120, cy, Color.Yellow);
                RitaCentreradText(sb, etikett, cx - 80, cy, Color.White);
            }
            else
            {
                RitaCentreradText(sb, värde, cx + 120, cy, Color.DarkGray);
                RitaCentreradText(sb, etikett, cx - 80, cy, Color.DarkGray);
            }
        }

        void RitaCentreradText(SpriteBatch sb, string text, int x, int y, Color färg)
        {
            var storlek = _font.MeasureString(text);
            sb.DrawString(_font, text, new Vector2(x - storlek.X / 2, y - storlek.Y / 2), färg);
        }

        public void LaddaVärden(int antalFiender, float maxFart, int antalLiv, int styrLäge, int magnetism, bool logga)
        {
            AntalFiender = antalFiender;
            FiendeMaxFart = maxFart;
            AntalLiv = antalLiv;
            StyrLäge = styrLäge;
            Magnetism = magnetism;
            Logga = logga;
        }
    }
}