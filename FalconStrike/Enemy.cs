using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FalconStrike
{
    public class Enemy : GameObject
    {
        private Random random;

        public Enemy(Game game, Random random) : base(game)
        {
            this.random = random;
        }

        protected override void LoadContent()
        {
            texture = Game.Content.Load<Texture2D>("Enemy");
            frameBounds = new Rectangle(0, 0, texture.Width / totalFrames, texture.Height);
            colorData = new Color[texture.Width * texture.Height];
            texture.GetData(colorData);
            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            if (!isPaused)
            {
                position.Y += velocity.Y * (float)gameTime.ElapsedGameTime.TotalSeconds;

                if (position.Y > GraphicsDevice.Viewport.Height)
                {
                    position.X = random.Next(GraphicsDevice.Viewport.Width - frameBounds.Width);
                    position.Y = -texture.Height;
                }
            }

            UpdateFrame(gameTime);
            base.Update(gameTime);
        }

        public void SetInitialPosition()
        {
            position = new Vector2(random.Next(GraphicsDevice.Viewport.Width - texture.Width / 2) + texture.Width / 4,
                -texture.Height / 2);
        }
    }
}