using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace Multiball
{   FEL
    public class Game1 : Game
    {
        fel fel
   
        GraphicsDeviceManager _graphics;
        SpriteBatch _spriteBatch;

        List<Ball> _fiender = new List<Ball>();
      
        // Spelarens position och inställningar
        float _spelarX, _spelarY;
        int _spelarRadie = 15;
        float _spelarFart = 5f;

        bool _gameOver = false;
        int _antalBollar = 1;
        int _liv = 3;
        float _oskårbarTid = 0f;   // ← var denna som saknades
        float _fiendeMaxFart = 8f;
        Texture2D _explosion;
        float _explosionTid = 0f;
        float _explosionX, _explosionY;  // sparar var träffen skedde

        MenuScreen _meny;
        bool _visaMeny = true;
        SpriteFont _font;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = false;
            _graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            _graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            _graphics.IsFullScreen = true;
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            Draw2D.Init(GraphicsDevice);

            _explosion = Content.Load<Texture2D>("explosion");

            int bredd = GraphicsDevice.Viewport.Width;
            int höjd = GraphicsDevice.Viewport.Height;

            _spelarX = bredd / 2f;
            _spelarY = höjd / 2f;

            //SkapaFiender(bredd, höjd);

            _font = Content.Load<SpriteFont>("meny");
            _meny = new MenuScreen(_font);
        }

        protected override void Update(GameTime gameTime)
        {
            int bredd = GraphicsDevice.Viewport.Width;
            int höjd = GraphicsDevice.Viewport.Height;

            var tangenter = Keyboard.GetState();

            if (_visaMeny)
            {
                if (_meny.Update(gameTime))
                {
                    // Spelaren tryckte Enter – hämta inställningarna
                    _fiendeMaxFart = _meny.FiendeMaxFart;
                    System.Diagnostics.Debug.WriteLine($"Tilldelat: _fiendeMaxFart = {_fiendeMaxFart}");

                    _antalBollar = _meny.AntalFiender;
                    _liv = _meny.AntalLiv;
                    _visaMeny = false;
                    SkapaFiender(GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);
                }
                return;
            }

            if (_visaMeny)
            {
                if (_meny.Update(gameTime))
                {
                    // Spelaren tryckte Enter – hämta inställningarna
                    _antalBollar = _meny.AntalFiender;
                    _fiendeMaxFart = _meny.FiendeMaxFart;

                    _liv = _meny.AntalLiv;
                    _visaMeny = false;
                    SkapaFiender(GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);
                }
                return;
            }


            if (tangenter.IsKeyDown(Keys.Escape))
                Exit();

            if (_gameOver)
            {
                if (tangenter.IsKeyDown(Keys.R))
                    StaraOm();
                return;
            }

            // Flytta spelaren
            if (tangenter.IsKeyDown(Keys.Left)) _spelarX -= _spelarFart;
            if (tangenter.IsKeyDown(Keys.Right)) _spelarX += _spelarFart;
            if (tangenter.IsKeyDown(Keys.Up)) _spelarY -= _spelarFart;
            if (tangenter.IsKeyDown(Keys.Down)) _spelarY += _spelarFart;

            _spelarX = Math.Clamp(_spelarX, _spelarRadie, bredd - _spelarRadie);
            _spelarY = Math.Clamp(_spelarY, _spelarRadie, höjd - _spelarRadie);

            // Uppdatera fiender
            foreach (var boll in _fiender)
                boll.Update(bredd, höjd);

            // Räkna ner oskårbarhet
            if (_oskårbarTid > 0)
                _oskårbarTid -= (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Räkna ner explosion  ← här, direkt efter oskårbarheten
            if (_explosionTid > 0)
                _explosionTid -= (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Kollision – bara om spelaren är sårbar
            if (_oskårbarTid <= 0)
            {
                foreach (var fiende in _fiender)
                {
                    float dx = _spelarX - fiende.X;
                    float dy = _spelarY - fiende.Y;
                    float avstånd = (float)Math.Sqrt(dx * dx + dy * dy);

                    if (avstånd < _spelarRadie + fiende.Radie)
                    {
                        _liv--;
                        _oskårbarTid = 2f;
                        _explosionTid = 0.5f;    // ← sätt igång explosion
                        _explosionX = _spelarX;
                        _explosionY = _spelarY;
                        if (_liv <= 0)
                            _gameOver = true;
                        break;
                    }
                }
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            int bredd = GraphicsDevice.Viewport.Width;   // ← lägg till
            int höjd = GraphicsDevice.Viewport.Height;  // ← lägg till

            GraphicsDevice.Clear(Color.Black);

            _spriteBatch.Begin();

            if (_visaMeny)
            {
                _meny.Draw(_spriteBatch, bredd, höjd);
                _spriteBatch.End();
                base.Draw(gameTime);
                return;
            }

            // Rita fiender
            foreach (var boll in _fiender)
                boll.Draw(_spriteBatch);

            // Rita spelaren – blinka när oskårbar
            bool visaSpelaren = _oskårbarTid <= 0 || (int)(_oskårbarTid * 4) % 2 == 0;
            if (visaSpelaren)
                Draw2D.Cirkel(_spriteBatch, (int)_spelarX, (int)_spelarY, _spelarRadie, Color.DodgerBlue);
            
            if (_explosionTid > 0)
            {
                int storlek = (int)(150 * _explosionTid / 0.5f); // krymper från 150 → 0
                _spriteBatch.Draw(
                    _explosion,
                    new Rectangle(
                        (int)_explosionX - storlek / 2,
                        (int)_explosionY - storlek / 2,
                        storlek,
                        storlek),
                    Color.White * (_explosionTid / 0.5f)  // tonas ut
                );
            }

            // Rita liv som röda cirklar uppe till vänster
            for (int i = 0; i < _liv; i++)
                Draw2D.Cirkel(_spriteBatch, 30 + i * 35, 30, 12, Color.Red);

            // Game over
            if (_gameOver)
            {
                int cx = GraphicsDevice.Viewport.Width / 2;
                int cy = GraphicsDevice.Viewport.Height / 2;

                Draw2D.Linje(_spriteBatch, cx - 60, cy - 60, cx + 60, cy + 60, Color.Red);
                Draw2D.Linje(_spriteBatch, cx + 60, cy - 60, cx - 60, cy + 60, Color.Red);
                Draw2D.Rektangel(_spriteBatch, cx - 120, cy - 80, 240, 100, Color.Red);
            }

            _spriteBatch.End();

            base.Draw(gameTime);
        }

        void SkapaFiender(int bredd, int höjd)
        {
            System.Diagnostics.Debug.WriteLine($"SkapaFiender: _fiendeMaxFart = {_fiendeMaxFart}");

            Random rng = new Random();
            // Lägg till som instansvariabel
          
            for (int i = 0; i < _antalBollar; i++)
            {
                float x = rng.Next(50, bredd - 50);
                float y = rng.Next(50, höjd - 50);
                float fartX = (float)(rng.NextDouble() * _fiendeMaxFart + 2) * (rng.Next(2) == 0 ? 1 : -1);
                float fartY = (float)(rng.NextDouble() * _fiendeMaxFart + 2) * (rng.Next(2) == 0 ? 1 : -1);
                _fiender.Add(new Ball(x, y, fartX, fartY, radie: 10, färg: Color.Yellow));
            }
        }

        void StaraOm()
        {
            int bredd = GraphicsDevice.Viewport.Width;
            int höjd = GraphicsDevice.Viewport.Height;

            _visaMeny = true;
            _fiendeMaxFart = 8f;  // återställ

            _spelarX = bredd / 2f;
            _spelarY = höjd / 2f;
            _gameOver = false;
            _liv = 3;
            _oskårbarTid = 0f;

            _fiender.Clear();
            SkapaFiender(bredd, höjd);
        }
    }
}