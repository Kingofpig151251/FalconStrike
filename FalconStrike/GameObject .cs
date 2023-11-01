using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FalconStrike
{
    public abstract class GameObject : DrawableGameComponent
    {
        protected Game Game1;
        public Vector2 position;
        protected Vector2 velocity;
        protected float rotation = 0;

        public bool isInvincible;
        protected double elapsedTime;
        protected double maxTime;

        public Texture2D texture;
        protected Color[] colorData;

        private int currentFrame;
        private double frameElapsedTime;
        protected int totalFrames;
        private double frameTimeStep;
        protected Rectangle frameBounds;

        // Add a property to store the bounding rectangle after transformation
        public Rectangle TransformedBounds
        {
            get
            {
                // Use the CalculateBoundingRectangle function to calculate the transformed bounds
                return CalculateBoundingRectangle(new Rectangle(0, 0, texture.Width, texture.Height),
                    Matrix.CreateTranslation(new Vector3(-frameBounds.Width / 2, -frameBounds.Height / 2, 0.0f)) *
                    Matrix.CreateRotationZ(rotation) *
                    Matrix.CreateTranslation(new Vector3(position, 0.0f)));
            }
        }

        protected GameObject(Game game) : base(game)
        {
            this.Game1 = game;
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
            Color color = isInvincible && (int)(gameTime.TotalGameTime.TotalSeconds * 10) % 2 == 0
                ? Color.Transparent
                : Color.White;
            spriteBatch.Begin();
            spriteBatch.Draw(texture, position, frameBounds, color, rotation,
                new Vector2(frameBounds.Width / 2, frameBounds.Height / 2), 1.0f, SpriteEffects.None, 0);
            spriteBatch.End();
            base.Draw(gameTime);
        }

        // Combine the CheckCollision and PixelCollision functions into one method
        public bool CollidesWith(GameObject other)
        {
            // Check if the transformed bounds of the two objects intersect
            if (this.TransformedBounds.Intersects(other.TransformedBounds))
            {
                // Find the intersection rectangle
                Rectangle intersect = Rectangle.Intersect(this.TransformedBounds, other.TransformedBounds);

                // Calculate the transformation matrices of the two objects
                Matrix transformA = Matrix.CreateTranslation(new Vector3(-frameBounds.Width / 2, -frameBounds.Height / 2, 0.0f)) *
                                    Matrix.CreateRotationZ(rotation) *
                                    Matrix.CreateTranslation(new Vector3(position, 0.0f));
                Matrix transformB = Matrix.CreateTranslation(new Vector3(-other.frameBounds.Width / 2, -other.frameBounds.Height / 2, 0.0f)) *
                                    Matrix.CreateRotationZ(other.rotation) *
                                    Matrix.CreateTranslation(new Vector3(other.position, 0.0f));

                // Loop through the pixels in the intersection rectangle
                for (int y = intersect.Top; y < intersect.Bottom; y++)
                {
                    for (int x = intersect.Left; x < intersect.Right; x++)
                    {
                        // Transform the pixel coordinates from the intersection rectangle to the original rectangles
                        Vector2 pixelA = Vector2.Transform(new Vector2(x, y), Matrix.Invert(transformA));
                        Vector2 pixelB = Vector2.Transform(new Vector2(x, y), Matrix.Invert(transformB));

                        // Round the pixel coordinates to integers
                        int xA = (int)Math.Floor(pixelA.X);
                        int yA = (int)Math.Floor(pixelA.Y);
                        int xB = (int)Math.Floor(pixelB.X);
                        int yB = (int)Math.Floor(pixelB.Y);

                        // Check if the pixel coordinates are within the bounds of the original rectangles
                        if (0 <= xA && xA < frameBounds.Width && 0 <= yA && yA < frameBounds.Height &&
                            0 <= xB && xB < other.frameBounds.Width && 0 <= yB && yB < other.frameBounds.Height)
                        {
                            // Get the colors of the pixels
                            Color colorA = this.colorData[xA + yA * frameBounds.Width];
                            Color colorB = other.colorData[xB + yB * other.frameBounds.Width];

                            // Check if the pixels are not transparent
                            if (colorA.A != 0 && colorB.A != 0)
                            {
                                // A pixel collision has occurred
                                return true;
                            }
                        }
                    }
                }
            }

            // No collision has occurred
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

        // Add a helper function to calculate the bounding rectangle after transformation
        private Rectangle CalculateBoundingRectangle(Rectangle rectangle, Matrix transform)
        {
            // Get the four corners of the original rectangle
            Vector2 leftTop = new Vector2(rectangle.Left, rectangle.Top);
            Vector2 rightTop = new Vector2(rectangle.Right, rectangle.Top);
            Vector2 leftBottom = new Vector2(rectangle.Left, rectangle.Bottom);
            Vector2 rightBottom = new Vector2(rectangle.Right, rectangle.Bottom);

            // Transform the four corners using the transformation matrix
            Vector2.Transform(ref leftTop, ref transform, out leftTop);
            Vector2.Transform(ref rightTop, ref transform, out rightTop);
            Vector2.Transform(ref leftBottom, ref transform, out leftBottom);
            Vector2.Transform(ref rightBottom, ref transform, out rightBottom);

            // Find the minimum and maximum x and y values of the transformed corners
            Vector2 min = Vector2.Min(Vector2.Min(leftTop, rightTop), Vector2.Min(leftBottom, rightBottom));
            Vector2 max = Vector2.Max(Vector2.Max(leftTop, rightTop), Vector2.Max(leftBottom, rightBottom));

            // Create a new rectangle using the minimum and maximum values
            return new Rectangle((int)min.X, (int)min.Y, (int)(max.X - min.X), (int)(max.Y - min.Y));
        }
    }
}