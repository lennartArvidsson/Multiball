using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

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