using System;
using System.Collections.Generic;
using System.Net.Mime;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace FalconStrike
{
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        public Random random = new Random();
        private ScrollingBackground scrollingBackground;
        public Level CurrentLevel { get; private set; }

        private Player player;
        private HashSet<Enemy> enemies = new HashSet<Enemy>();
        public HashSet<Enemy> enemiesToRemove = new HashSet<Enemy>();
        public HashSet<Bullet> bullets = new HashSet<Bullet>();
        public HashSet<Bullet> bulletsToRemove = new HashSet<Bullet>();
        public event Action<Enemy> OnPlayerGotHit;
        public event Action<Bullet, Enemy> OnEnemyGotHit;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            graphics.PreferredBackBufferWidth = 400;
            graphics.PreferredBackBufferHeight = 600;
        }

        protected override void Initialize()
        {
            scrollingBackground = new ScrollingBackground(this, spriteBatch);
            CurrentLevel = Level.WaterLevel;
            Components.Add(scrollingBackground);
            player = new Player(this, this);
            Components.Add(player);
            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            Services.AddService(typeof(SpriteBatch), spriteBatch);
        }

        protected override void UnloadContent()
        {
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
                Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            if (random.NextDouble() < gameTime.ElapsedGameTime.TotalSeconds * 0.5)
            {
                var enemy = new Enemy(this, this);
                Components.Add(enemy);
                enemy.SetInitialPosition();
                enemies.Add(enemy);
            }

            foreach (var enemy in enemies)
            {
                if (player.CheckCollision(enemy) && player.isInvincible == false)
                {
                    OnPlayerGotHit?.Invoke(enemy);
                }

                foreach (var bullet in bullets)
                {
                    if (bullet.CheckCollision(enemy) && enemy.position.Y > 0)
                    {
                        OnEnemyGotHit?.Invoke(bullet, enemy);
                        bulletsToRemove.Add(bullet);
                    }
                }
            }

            foreach (var enemy in enemiesToRemove)
            {
                Components.Remove(enemy);
                enemies.Remove(enemy);
            }

            foreach (var bullet in bulletsToRemove)
            {
                Components.Remove(bullet);
                bullets.Remove(bullet);
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            base.Draw(gameTime);
        }

        public void ChangeLevel(Level level)
        {
            CurrentLevel = level;
            scrollingBackground.ChangeTexture(CurrentLevel);
        }

        public void GameOver()
        {
        }
    }
}