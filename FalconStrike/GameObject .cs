using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FalconStrike
{
    public abstract class GameObject : DrawableGameComponent
    {
        protected bool isPaused;
        protected Vector2 position, velocity;
        protected float rotation = 0;

        protected bool isInvincible;
        protected double elapsedTime;
        protected double maxTime;

        protected Texture2D texture;
        protected Color[] colorData;

        private int currentFrame;
        private double frameElapsedTime;
        protected int totalFrames;
        private double frameTimeStep;
        protected Rectangle frameBounds;

        public Rectangle Bounds
        {
            get { return new Rectangle((int)position.X, (int)position.Y, frameBounds.Width, frameBounds.Height); }
        }

        protected GameObject(Game game) : base(game)
        {
            totalFrames = 3;
            frameTimeStep = 0.3;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            SpriteBatch spriteBatch = Game.Services.GetService<SpriteBatch>();
            Color color = isInvincible && (int)(gameTime.TotalGameTime.TotalSeconds * 10) % 2 == 0 ? Color.Transparent : Color.White;
            spriteBatch.Begin();
            spriteBatch.Draw(texture, position, frameBounds, color, rotation, new Vector2(frameBounds.Width / 2, frameBounds.Height / 2), 1.0f, SpriteEffects.None, 0);
            spriteBatch.End();
            base.Draw(gameTime);
        }

        public bool CheckCollision(GameObject other)
        {
            if (this.Bounds.Intersects(other.Bounds))
            {
                Rectangle intersect = Rectangle.Intersect(this.Bounds, other.Bounds);

                for (int y = intersect.Top; y < intersect.Bottom; y++)
                {
                    for (int x = intersect.Left; x < intersect.Right; x++)
                    {
                        Color colorA = this.colorData[(x - this.Bounds.Left) + (y - this.Bounds.Top) * this.texture.Width];
                        Color colorB = other.colorData[(x - other.Bounds.Left) + (y - other.Bounds.Top) * other.texture.Width];

                        if (colorA.A != 0 && colorB.A != 0)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        public void UpdateFrame(GameTime gameTime)
        {
            frameElapsedTime += gameTime.ElapsedGameTime.TotalSeconds;

            if (frameElapsedTime > frameTimeStep)
            {
                currentFrame = (currentFrame + 1) % totalFrames;
                frameBounds.X = currentFrame * texture.Width / totalFrames;
                frameElapsedTime = 0;
            }
        }
    }
}