using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace FalconStrike
{
    public class Player : GameObject
    {
        private int life = 3;

        public Player(Game game) : base(game)
        {
            position = new Vector2(game.GraphicsDevice.Viewport.Width / 2, game.GraphicsDevice.Viewport.Height * 0.8f);
            velocity = new Vector2(2f, 2f);
            maxTime = 3;
        }

        protected override void LoadContent()
        {
            texture = Game.Content.Load<Texture2D>("Player");
            frameBounds = new Rectangle(0, 0, texture.Width / totalFrames,
                texture.Height);
            colorData = new Color[texture.Width * texture.Height];
            texture.GetData(colorData);
            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            if (life == 0)
            {
                // Game.Exit();
            }

            UpdateInvincibility(gameTime);
            HandleInput();
            UpdateFrame(gameTime);
            base.Update(gameTime);
        }

        public void HandleInput()
        {
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

        public void PlayerGetHit()
        {
            if (!isInvincible)
            {
                isInvincible = true;
                position = new Vector2(Game.GraphicsDevice.Viewport.Width / 2,
                    Game.GraphicsDevice.Viewport.Height * 0.8f);
                life--;
            }
        }
    }
}