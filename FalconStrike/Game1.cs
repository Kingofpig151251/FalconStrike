﻿using System;
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
        private Texture2D bgTexture;

        public Random random = new Random();
        public bool isPaused = true, isGameOver = false, isWin = false;
        private ScrollingBackground scrollingBackground;
        public int score = 0, timer = 30, level = 0;

        int lineHeight = 20;
        int baseLine;

        private Player player;
        public HashSet<Enemy0> enemies = new HashSet<Enemy0>();
        public HashSet<Enemy0> enemiesToRemove = new HashSet<Enemy0>();
        public HashSet<Bullet> bullets = new HashSet<Bullet>();
        public HashSet<Bullet> bulletsToRemove = new HashSet<Bullet>();
        private KeyboardState previousState;
        private double elapsedTime = 0;

        public event Action<Enemy0> OnPlayerGotHit;
        public event Action<Bullet, Enemy0> OnEnemyGotHit;
        public event Action<int> OnTimeDown;

        public Game1()
        {
            OnEnemyGotHit += (bullet, enemy) =>
            {
                score += 100;
                CheckScore();
            };
            OnTimeDown += (level) =>
            {
                if (this.level < 4)
                {
                    this.level++;
                    timer = 30;
                    score += 500;
                }
                else
                {
                    this.level = 0;
                    timer = 30;
                    score = 0;
                    isPaused = true;
                    isWin = true;
                }

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
            baseLine = GraphicsDevice.Viewport.Height / 20;
            Components.Add(scrollingBackground);
            font = Content.Load<SpriteFont>("MyFont");
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
            KeyboardState currentState = Keyboard.GetState();

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
                Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            if (level == 0)
            {
                isPaused = true;
            }


            if (currentState.IsKeyDown(Keys.P) && previousState.IsKeyUp(Keys.P))
            {
                if (isGameOver)
                {
                    level = 0;
                    OnTimeDown?.Invoke(this.level);
                    isGameOver = false;
                    Components.Add(player);
                }
                else if (level == 0)
                {
                    level++;
                    isPaused = !isPaused;
                }
                else
                {
                    isPaused = !isPaused;
                }
            }


            previousState = Keyboard.GetState();

            if (!isPaused)
            {
                if (random.NextDouble() < gameTime.ElapsedGameTime.TotalSeconds * 0.5 * level)
                {
                    AddEnemy();
                }

                foreach (var enemy in enemies)
                {
                    foreach (var bullet in bullets)
                    {
                        if (bullet.CollidesWith(enemy) && enemy.position.Y > enemy.texture.Height)
                        {
                            OnEnemyGotHit?.Invoke(bullet, enemy);
                            bulletsToRemove.Add(bullet);
                            break;
                        }
                    }

                    if (player.CollidesWith(enemy) && player.isInvincible == false)
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
            base.Draw(gameTime);
            spriteBatch.Begin();
            if (level != 0)
            {
                spriteBatch.DrawString(font, "Score: " + score, new Vector2(GraphicsDevice.Viewport.Width / 20, baseLine), Color.Red);
                spriteBatch.DrawString(font, "Time: " + timer, new Vector2(GraphicsDevice.Viewport.Width / 20, baseLine + lineHeight), Color.Red);
                spriteBatch.DrawString(font, "Life:" + player.life, new Vector2(GraphicsDevice.Viewport.Width / 20, baseLine + lineHeight * 2), Color.Red);
                spriteBatch.DrawString(font, "Fire Rate: " + (10 - Math.Round(player.bulletInterval, 1) * 10), new Vector2(GraphicsDevice.Viewport.Width / 20, baseLine + lineHeight * 3), Color.Red);
            }

            if (isPaused)
            {
                string text1;
                string text2 = "Press P to Play";
                if (isGameOver)
                {
                    text1 = "Game Over!";
                }
                else if (isWin)
                {
                    text1 = "You Win!!!";
                }
                else if (level == 0 && !isWin)
                {   
                    text1 = "FalconStrike";
                }
                else
                {
                    text1 = "Next Level!";
                }


                Vector2 text1Size = font.MeasureString(text1);
                Vector2 text2Size = font.MeasureString(text2);

                Vector2 text1Position = new Vector2((GraphicsDevice.Viewport.Width - text1Size.X) / 2, (GraphicsDevice.Viewport.Height - text1Size.Y) / 2);
                Vector2 text2Position = new Vector2((GraphicsDevice.Viewport.Width - text2Size.X) / 2, (GraphicsDevice.Viewport.Height + text1Size.Y) / 2);

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
                foreach (var enemy in enemies)
                {
                    enemiesToRemove.Add(enemy);
                }

                OnTimeDown?.Invoke(this.level);
                
                isPaused = true;
            }
        }

        private void CheckScore()
        {
            if (score % 1000 == 0 && player.bulletInterval > 0.1f)
            {
                player.bulletInterval -= 0.1f;
            }
        }

        private void AddEnemy()
        {
            var enemy = new Enemy0(this, this);
            Components.Add(enemy);
            enemy.SetInitialPosition();
            enemies.Add(enemy);
        }

        public void GameOver()
        {
            isGameOver = true;
            Components.Remove(player);
        }
    }
}