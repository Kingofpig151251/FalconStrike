using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace FalconStrike
{
    public class Player : GameObject
    {
        private Game1 game1;
        public int life = 3;
        private KeyboardState previousState;

        // Add a variable to store the time since last bullet fired
        private double timeSinceLastBullet;

        // Add a variable to store the bullet firing interval (in seconds)
        public double bulletInterval = 1;

        public Player(Game game, Game1 game1) : base(game)
        {
            this.game1 = game1;
            game1.OnPlayerGotHit += HandleOnPlayerGotHit;
            this.isActivate = true;
            maxTime = 3;
            position = new Vector2(game.GraphicsDevice.Viewport.Width / 2, game.GraphicsDevice.Viewport.Height * 0.8f);
            velocity = new Vector2(2f, 2f);
        }

        protected override void LoadContent()
        {
            texture = Game.Content.Load<Texture2D>("image\\Player");
            frameBounds = new Rectangle(0, 0, texture.Width / totalFrames,
                texture.Height);
            colorData = new Color[texture.Width * texture.Height];
            texture.GetData(colorData);
            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            if (!game1.isPaused)
            {
                if (life == 0)
                {
                    game1.GameOver();
                }

                UpdateInvincibility(gameTime);
                HandleInput(gameTime);
                UpdateFrame(gameTime);
            }

            base.Update(gameTime);
        }

        public void HandleInput(GameTime gameTime)
        {
            timeSinceLastBullet += gameTime.ElapsedGameTime.TotalSeconds;

            KeyboardState state = Keyboard.GetState();

            if (state.IsKeyDown(Keys.Up))
            {
                position.Y -= velocity.Y;
                position.Y = MathHelper.Clamp(position.Y, frameBounds.Height / 2,
                    Game.GraphicsDevice.Viewport.Height - frameBounds.Height / 2);
            }

            if (state.IsKeyDown(Keys.Down))
            {
                position.Y += velocity.Y;
                position.Y = MathHelper.Clamp(position.Y, frameBounds.Height / 2,
                    Game.GraphicsDevice.Viewport.Height - frameBounds.Height / 2);
            }

            if (state.IsKeyDown(Keys.Left))
            {
                position.X -= velocity.X;
                position.X = MathHelper.Clamp(position.X, frameBounds.Width / 2,
                    Game.GraphicsDevice.Viewport.Width - frameBounds.Width / 2);
                rotation = -(float)(Math.PI / 8);
            }

            if (state.IsKeyDown(Keys.Right))
            {
                position.X += velocity.X;
                position.X = MathHelper.Clamp(position.X, frameBounds.Width / 2,
                    Game.GraphicsDevice.Viewport.Width - frameBounds.Width / 2);
                rotation = (float)(Math.PI / 8);
            }

            if (!state.IsKeyDown(Keys.Left) && !state.IsKeyDown(Keys.Right))
            {
                rotation = 0;
            }

            if (state.IsKeyDown(Keys.Space) && previousState.IsKeyUp(Keys.Space) &&
                timeSinceLastBullet >= bulletInterval)
            {
                timeSinceLastBullet = 0;
            }

            previousState = state;
        }

        public void UpdateInvincibility(GameTime gameTime)
        {
            if (isInvincible)
            {
                elapsedTime += gameTime.ElapsedGameTime.TotalSeconds;
                if (elapsedTime >= maxTime)
                {
                    isInvincible = false;
                    elapsedTime = 0;
                }
            }
        }

        private void HandleOnPlayerGotHit(GameObject enemy)
        {
            if (!isInvincible)
            {
                isInvincible = true;
                var explode = new Explode(Game, this);
                Game.Components.Add(explode);
                position = new Vector2(Game.GraphicsDevice.Viewport.Width / 2,
                    Game.GraphicsDevice.Viewport.Height * 0.8f);
                life--;
            }
        }
    }
}