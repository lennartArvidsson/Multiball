using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

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

        static Random _rng = new Random();

        float _fokusTid = 0f;  // hur länge bollen fokuserar på spelaren
        float _viloTid = 0f;  // hur länge bollen kör rakt fram
        bool _harFokus = true;

        public Ball(float x, float y, float fartX, float fartY, int radie, Color färg)
        {
            _x = x;
            _y = y;
            _fartX = fartX;
            _fartY = fartY;
            _radie = radie;
            _färg = färg;

            // Slumpmässig starttid så inte alla tappar siktet samtidigt
            _fokusTid = (float)(_rng.NextDouble() * 4 + 1);
        }

        public void AppliceraMagnetism(float målX, float målY, int magnetism, GameTime gameTime)
{
    if (magnetism == 0) return;

    float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;

    // Uppdatera fokuscykeln
    if (_harFokus)
    {
        _fokusTid -= delta;
        if (_fokusTid <= 0)
        {
            _harFokus = false;
            NyFokusCykel(magnetism);  // beräkna ny viloTid
        }
    }
    else
    {
        _viloTid -= delta;
        if (_viloTid <= 0)
        {
            _harFokus = true;
            NyFokusCykel(magnetism);  // beräkna ny fokusTid
        }
    }

    // Kör bara magnetismen om bollen har fokus
    if (!_harFokus) return;

    // Spara ursprungsfarten
    float ursprungsFart = (float)Math.Sqrt(_fartX * _fartX + _fartY * _fartY);

    // Riktningsvektor mot spelaren
    float dx      = målX - _x;
    float dy      = målY - _y;
    float avstånd = (float)Math.Sqrt(dx * dx + dy * dy);

    if (avstånd < 1f) return;

    float rikX    = dx / avstånd;
    float rikY    = dy / avstånd;
    float påverkan = magnetism * 0.05f;

    float nyX = _fartX + rikX * påverkan;
    float nyY = _fartY + rikY * påverkan;

    float nyFart = (float)Math.Sqrt(nyX * nyX + nyY * nyY);
    _fartX = nyX / nyFart * ursprungsFart;
    _fartY = nyY / nyFart * ursprungsFart;
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

        void NyFokusCykel(int magnetism)
        {
            // Hög magnetism = kort fokus, lång vila
            // Låg magnetism = lång fokus, kort vila
            float maxFokus = magnetism <= 3 ? 6f : magnetism <= 6 ? 4f : 2f;
            float minFokus = magnetism <= 3 ? 4f : magnetism <= 6 ? 2f : 0.5f;
            float maxVila = magnetism <= 3 ? 1f : magnetism <= 6 ? 2f : 4f;
            float minVila = magnetism <= 3 ? 0.5f : magnetism <= 6 ? 1f : 2f;

            _fokusTid = (float)(_rng.NextDouble() * (maxFokus - minFokus) + minFokus);
            _viloTid = (float)(_rng.NextDouble() * (maxVila - minVila) + minVila);
        }

        public void AppliceraRepulsionOchBrus(List<Ball> allaBollar, int magnetism)
        {
            if (magnetism == 0) return;

            // Spara ursprungsfarten innan vi rör något
            float ursprungsFart = (float)Math.Sqrt(_fartX * _fartX + _fartY * _fartY);

            float repulsionsAvstånd = _radie * 3f;
            float repulsionsKraft = magnetism * 0.004f;
            float brusKraft = magnetism * 0.0008f;

            foreach (var annan in allaBollar)
            {
                if (annan == this) continue;

                float dx = _x - annan._x;
                float dy = _y - annan._y;
                float avstånd = (float)Math.Sqrt(dx * dx + dy * dy);

                if (avstånd < repulsionsAvstånd && avstånd > 0.01f)
                {
                    float nx = dx / avstånd;
                    float ny = dy / avstånd;
                    float styrka = repulsionsKraft * (1f - avstånd / repulsionsAvstånd);

                    _fartX += nx * styrka;
                    _fartY += ny * styrka;
                }
            }

            // Brus
            float vinkel = (float)(_rng.NextDouble() * Math.PI * 2);
            _fartX += (float)Math.Cos(vinkel) * brusKraft;
            _fartY += (float)Math.Sin(vinkel) * brusKraft;

            // Återställ till ursprungsfarten – bara riktningen ska ändras
            float nyFart = (float)Math.Sqrt(_fartX * _fartX + _fartY * _fartY);
            if (nyFart > 0.01f)
            {
                _fartX = _fartX / nyFart * ursprungsFart;
                _fartY = _fartY / nyFart * ursprungsFart;
            }
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