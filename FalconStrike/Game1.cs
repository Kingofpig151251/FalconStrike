using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace FalconStrike
{
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        private SpriteFont font;

        public Random random = new Random();
        public bool isPaused = false, isGameOver = false;
        private ScrollingBackground scrollingBackground;
        public int score = 0, timer = 30;

        private Player player;
        public Enemy[] enemies = new Enemy[15];
        public Bullet[] bullets = new Bullet[20];
        private KeyboardState previousState;
        private double elapsedTime = 0;

        public event Action<Enemy> OnPlayerGotHit;
        public event Action<Bullet, Enemy> OnEnemyGotHit;
        public event Action OnTimeDown;

        public Game1()
        {
            OnEnemyGotHit += (bullet, enemy) =>
            {
                score += 100;
                CheckScore();
            };
            OnTimeDown += () =>
            {
                timer = 30;
                score += 500;
                CheckScore();
            };
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            graphics.PreferredBackBufferWidth = 400;
            graphics.PreferredBackBufferHeight = 600;
        }

        protected override void Initialize()
        {
            scrollingBackground = new ScrollingBackground(this, this, spriteBatch);
            Components.Add(scrollingBackground);
            font = Content.Load<SpriteFont>("MyFont");
            player = new Player(this, this);
            Components.Add(player);

            for (int i = 0; i < enemies.Length; i++)
            {
                enemies[i]= new Enemy(this, this);
                Components.Add(enemies[i]);
            }
            for (int i = 0; i < bullets.Length; i++)
            {
                bullets[i]= new Bullet(this, this,player);
                Components.Add(bullets[i]);
            }

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


            if (Keyboard.GetState().IsKeyDown(Keys.P) && previousState.IsKeyUp(Keys.P))
            {
                isPaused = !isPaused;
            }

            previousState = Keyboard.GetState();

            if (random.Next() * 100 < 30)
            {
            }
            if (!isPaused)
            {
                if (random.NextDouble() < gameTime.ElapsedGameTime.TotalSeconds * 0.5)
                {
                    foreach (var enemy in enemies)
                    {
                    }
                }

                foreach (var enemy in enemies)
                {
                    foreach (var bullet in bullets)
                    {
                        if (bullet.CollidesWith(enemy) && enemy.position.Y > enemy.texture.Height)
                        {
                            OnEnemyGotHit?.Invoke(bullet, enemy);
                        }
                    }

                    if (player.CollidesWith(enemy) && player.isInvincible == false&& enemy.isActivate==true)
                    {
                        OnPlayerGotHit?.Invoke(enemy);
                    }
                }


                if (!isGameOver || !isPaused)
                {
                    elapsedTime += gameTime.ElapsedGameTime.TotalSeconds;
                    if (elapsedTime >= 1.0)
                    {
                        elapsedTime -= 1.0;
                        CheckTime(gameTime);
                    }
                }

                base.Update(gameTime);
            }
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            base.Draw(gameTime);
            spriteBatch.Begin();
            int lineHeight = 20;
            int baseLine = GraphicsDevice.Viewport.Height / 20;
            spriteBatch.DrawString(font, "Score: " + score, new Vector2(GraphicsDevice.Viewport.Width / 20, baseLine),
                Color.Red);
            spriteBatch.DrawString(font, "Time: " + timer,
                new Vector2(GraphicsDevice.Viewport.Width / 20, baseLine + lineHeight), Color.Red);
            spriteBatch.DrawString(font, "Life:" + player.life,
                new Vector2(GraphicsDevice.Viewport.Width / 20, baseLine + lineHeight * 2), Color.Red);
            spriteBatch.DrawString(font, "Fire Rate: " + (10 - Math.Round(player.bulletInterval, 1) * 10),
                new Vector2(GraphicsDevice.Viewport.Width / 20, baseLine + lineHeight * 3), Color.Red);

            if (isGameOver)
                spriteBatch.DrawString(font, "Game Over",
                    new Vector2(GraphicsDevice.Viewport.Width / 2 - 50, GraphicsDevice.Viewport.Height / 2), Color.Red);
            if (isPaused)
            {
                string text1 = "Next Level!";
                string text2 = "Press P to Play";

                Vector2 text1Size = font.MeasureString(text1);
                Vector2 text2Size = font.MeasureString(text2);

                Vector2 text1Position = new Vector2((GraphicsDevice.Viewport.Width - text1Size.X) / 2,
                    (GraphicsDevice.Viewport.Height - text1Size.Y) / 2);
                Vector2 text2Position = new Vector2((GraphicsDevice.Viewport.Width - text2Size.X) / 2,
                    (GraphicsDevice.Viewport.Height + text1Size.Y) / 2);

                spriteBatch.DrawString(font, text1, text1Position, Color.Red);
                spriteBatch.DrawString(font, text2, text2Position, Color.Red);
            }

            spriteBatch.End();
        }

        private void CheckTime(GameTime gameTime)
        {
            timer--;
            if (timer <= 0)
            {
                isPaused = true;
                OnTimeDown?.Invoke();
            }
        }

        private void CheckScore()
        {
            if (score % 1000 == 0 && player.bulletInterval > 0.1f)
            {
                player.bulletInterval -= 0.1f;
            }
        }


        public void GameOver()
        {
            isGameOver = true;
            Components.Remove(player);
        }
    }
}