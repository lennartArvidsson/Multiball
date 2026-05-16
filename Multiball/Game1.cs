using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace Multiball
{    
    //Ny ren Start
    public class Game1 : Game
    {
       
   
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
        int _startLiv = 3;
        float _oskårbarTid = 0f;   // ← var denna som saknades
        float _fiendeMaxFart = 8f;
        Texture2D _explosion;
        float _explosionTid = 0f;
        float _explosionX, _explosionY;  // sparar var träffen skedde

        MenuScreen _meny;
        bool _visaMeny = true;
        SpriteFont _font;
        SpriteFont _fontGameOver;

        int _styrLäge = 0;      // 0=Direkt, 1=Tröghet
        float _hastighetX = 2f;     // tröghetshastighet vid start
        float _hastighetY = 2f;
        float _acceleration = 0.4f; // hur mycket en puff ger
        float _bromsning = 0.85f; // Space-broms (multipliceras varje bildruta)
        float _maxFart = 12f;   // tak för tröghetsläget

        int _magnetism = 0;
        float _överlevnadsTid = 0f;

        bool _logga = false;
        //bool _harLoggat = false;  // förhindrar dubbelloggning

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
            _fontGameOver = Content.Load<SpriteFont>("gameover");

            _meny = new MenuScreen(_font);

         

            // Ladda sparade inställningar
            var (antalFiender, maxFart, antalLiv, styrLäge, _magnetism, logga) = Inställningar.Ladda();
            _meny.LaddaVärden(antalFiender, maxFart, antalLiv, styrLäge, _magnetism, logga);
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

                    _styrLäge = _meny.StyrLäge;
                    _antalBollar = _meny.AntalFiender;
                    _liv = _meny.AntalLiv;
                    _startLiv = _meny.AntalLiv;
                    _magnetism = _meny.Magnetism;
                    _logga = _meny.Logga;

                  

                    _visaMeny = false;

                    Inställningar.Spara(_antalBollar, _fiendeMaxFart, _liv, _styrLäge, _magnetism, _logga);

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
                    StartaOm();
                return;
            }

            // Flytta spelaren
            if (_styrLäge == 0)
            {
                // --- Direkt styrning ---
                if (tangenter.IsKeyDown(Keys.Left)) _spelarX -= _spelarFart;
                if (tangenter.IsKeyDown(Keys.Right)) _spelarX += _spelarFart;
                if (tangenter.IsKeyDown(Keys.Up)) _spelarY -= _spelarFart;
                if (tangenter.IsKeyDown(Keys.Down)) _spelarY += _spelarFart;

                _spelarX = Math.Clamp(_spelarX, _spelarRadie, bredd - _spelarRadie);
                _spelarY = Math.Clamp(_spelarY, _spelarRadie, höjd - _spelarRadie);
            }
            else
            {
                // --- Tröghetsstyrning ---

                // Piltangenter ger acceleration
                if (tangenter.IsKeyDown(Keys.Left)) _hastighetX -= _acceleration;
                if (tangenter.IsKeyDown(Keys.Right)) _hastighetX += _acceleration;
                if (tangenter.IsKeyDown(Keys.Up)) _hastighetY -= _acceleration;
                if (tangenter.IsKeyDown(Keys.Down)) _hastighetY += _acceleration;

                // Space bromsar
                if (tangenter.IsKeyDown(Keys.Space))
                {
                    _hastighetX *= _bromsning;
                    _hastighetY *= _bromsning;
                }

                // Begränsa maxfart
                float fart = (float)Math.Sqrt(_hastighetX * _hastighetX + _hastighetY * _hastighetY);
                if (fart > _maxFart)
                {
                    _hastighetX = _hastighetX / fart * _maxFart;
                    _hastighetY = _hastighetY / fart * _maxFart;
                }

                // Flytta spelaren
                _spelarX += _hastighetX;
                _spelarY += _hastighetY;

                // Studsa mot väggar
                if (_spelarX - _spelarRadie < 0)
                {
                    _spelarX = _spelarRadie;
                    _hastighetX *= -1;
                }
                if (_spelarX + _spelarRadie > bredd)
                {
                    _spelarX = bredd - _spelarRadie;
                    _hastighetX *= -1;
                }
                if (_spelarY - _spelarRadie < 0)
                {
                    _spelarY = _spelarRadie;
                    _hastighetY *= -1;
                }
                if (_spelarY + _spelarRadie > höjd)
                {
                    _spelarY = höjd - _spelarRadie;
                    _hastighetY *= -1;
                }
            }

            // Uppdatera fiender
            foreach (var boll in _fiender)
                boll.Update(bredd, höjd);

            // Magnetism – dra fienderna mot spelaren
            foreach (var fiende in _fiender)
                fiende.AppliceraMagnetism(_spelarX, _spelarY, _magnetism, gameTime);
           
            foreach (var fiende in _fiender)
                fiende.AppliceraRepulsionOchBrus(_fiender, _magnetism);

            KollisionMellanFiender();

            void KollisionMellanFiender()
            {
                for (int i = 0; i < _fiender.Count; i++)
                {
                    for (int j = i + 1; j < _fiender.Count; j++)
                    {
                        var a = _fiender[i];
                        var b = _fiender[j];

                        float dx = b.X - a.X;
                        float dy = b.Y - a.Y;
                        float avstånd = (float)Math.Sqrt(dx * dx + dy * dy);
                        float minAvstånd = a.Radie + b.Radie;

                        if (avstånd < minAvstånd && avstånd > 0.01f)
                        {
                            // Normaliserad kollisionsaxel
                            float nx = dx / avstånd;
                            float ny = dy / avstånd;

                            // Separera bollarna så de inte fastnar i varandra
                            float överlapp = (minAvstånd - avstånd) / 2f;
                            a.Flytta(-nx * överlapp, -ny * överlapp);
                            b.Flytta(nx * överlapp, ny * överlapp);

                            // Byt hastighetskomponent längs kollisionsaxeln
                            float aDotN = a.FartX * nx + a.FartY * ny;
                            float bDotN = b.FartX * nx + b.FartY * ny;

                            a.SättFart(
                                a.FartX - aDotN * nx + bDotN * nx,
                                a.FartY - aDotN * ny + bDotN * ny
                            );
                            b.SättFart(
                                b.FartX - bDotN * nx + bDotN * nx,
                                b.FartY - bDotN * ny + bDotN * ny
                            );
                        }
                    }
                }
            }
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
                        if (_liv <= 0) { 
                            _gameOver = true;
                        if (_logga)  // ← logga direkt när spelet är över
                            Logg.SkrivaRad(_antalBollar, _fiendeMaxFart, _startLiv,
                                           _styrLäge, _magnetism, _överlevnadsTid);
                        }

                        break;
                    }
                }
            }
            if (!_gameOver && !_visaMeny)
                _överlevnadsTid += (float)gameTime.ElapsedGameTime.TotalSeconds;

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

                string text1 = "GAME OVER";
                string text2 = "Tryck R for att spela igen";
                string text3 = "ESC for att Avsluta";

                var storlek1 = _fontGameOver.MeasureString(text1);
                var storlek2 = _font.MeasureString(text2);

                // Skugga – rita texten lite förskjuten i mörkt först
                _spriteBatch.DrawString(_fontGameOver, text1,
                    new Vector2(cx - storlek1.X / 2 + 4, cy - storlek1.Y / 2 + 4),
                    Color.DarkRed);

                // Huvudtext i rött
                _spriteBatch.DrawString(_fontGameOver, text1,
                    new Vector2(cx - storlek1.X / 2, cy - storlek1.Y / 2),
                    Color.Red);

                // Mindre text under
                _spriteBatch.DrawString(_font, text2,
                    new Vector2(cx - storlek2.X / 2, cy + storlek1.Y / 2 + 60),
                    Color.White);
                int minuter = (int)_överlevnadsTid / 60;
                int sekunder = (int)_överlevnadsTid % 60;

                string tidText = $"Du klarade dig {minuter} min och {sekunder} sek";
                var tidStorlek = _font.MeasureString(tidText);

                _spriteBatch.DrawString(_font, tidText,
                    new Vector2(cx - tidStorlek.X / 2, cy + storlek1.Y / 2 + 10),
                    Color.Yellow);

                _spriteBatch.DrawString(_font, text3,
                    new Vector2(cx - storlek2.X / 2, cy + storlek1.Y / 2 + 110),
                    Color.White);
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

        void StartaOm()
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
            _hastighetX = 2f;
            _hastighetY = 2f;

            _fiender.Clear();

            _överlevnadsTid = 0f;
            //_harLoggat = false;

        }
    }
}