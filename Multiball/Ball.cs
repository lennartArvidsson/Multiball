using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Multiball
{
    public class Ball
    {
        float _x, _y;
        float _fartX, _fartY;
        int _radie;
        Color _färg;
        public float X => _x;
        public float Y => _y;
        public int Radie => _radie;

        public Ball(float x, float y, float fartX, float fartY, int radie, Color färg)
        {
            _x = x;
            _y = y;
            _fartX = fartX;
            _fartY = fartY;
            _radie = radie;
            _färg = färg;
        }

        public void AppliceraMagnetism(float målX, float målY, int magnetism)
        {
            if (magnetism == 0) return;

            // Riktningsvektor mot spelaren
            float dx = målX - _x;
            float dy = målY - _y;
            float avstånd = (float)Math.Sqrt(dx * dx + dy * dy);

            if (avstånd < 1f) return;

            // Normalisera riktningen mot spelaren
            float rikX = dx / avstånd;
            float rikY = dy / avstånd;

            // Nuvarande fart – behåll den!
            float fart = (float)Math.Sqrt(_fartX * _fartX + _fartY * _fartY);

            // Blanda nuvarande riktning med riktningen mot spelaren
            // magnetism 1 = liten påverkan, magnetism 10 = stark påverkan
            float påverkan = magnetism * 0.02f;
            float nyX = _fartX + rikX * påverkan;
            float nyY = _fartY + rikY * påverkan;

            // Normalisera och återställ ursprunglig fart
            float nyFart = (float)Math.Sqrt(nyX * nyX + nyY * nyY);
            _fartX = nyX / nyFart * fart;
            _fartY = nyY / nyFart * fart;
        }

        // Properties
        public float FartX => _fartX;
        public float FartY => _fartY;

        // Flytta positionen direkt (för separation)
        public void Flytta(float dx, float dy)
        {
            _x += dx;
            _y += dy;
        }

        // Sätt ny hastighet
        public void SättFart(float fartX, float fartY)
        {
            _fartX = fartX;
            _fartY = fartY;
        }

        public void Update(int bredd, int höjd)
        {
            _x += _fartX;
            _y += _fartY;

            if (_x - _radie < 0 || _x + _radie > bredd) _fartX *= -1;
            if (_y - _radie < 0 || _y + _radie > höjd) _fartY *= -1;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            Draw2D.Cirkel(spriteBatch, (int)_x, (int)_y, _radie, _färg);
        }
    }
}