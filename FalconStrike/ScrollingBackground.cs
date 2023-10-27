using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FalconStrike
{
    public enum Level
    {
        WaterLevel,
        DesertLevel,
        BaseLevel
    }

    public class ScrollingBackground : DrawableGameComponent
    {
        private SpriteBatch spriteBatch;
        private Texture2D CurrentTexture, textureWater, textureDesert, textureBase;
        private Vector2[,] positions;
        private const int speed = 1;

        public ScrollingBackground(Game game, SpriteBatch spriteBatch) : base(game)
        {
            this.spriteBatch = spriteBatch;
            LoadContent();
        }

        protected override void LoadContent()
        {
            CurrentTexture = textureWater = Game.Content.Load<Texture2D>("Water");
            textureDesert = Game.Content.Load<Texture2D>("Desert");
            textureBase = Game.Content.Load<Texture2D>("Base");

            // Calculate the number of tiles needed to fill the screen
            int tilesX = GraphicsDevice.Viewport.Width / CurrentTexture.Width + 1;
            int tilesY = GraphicsDevice.Viewport.Height / CurrentTexture.Height + 2;

            // Initialize the positions array
            positions = new Vector2[tilesX, tilesY];

            // Fill the positions array
            for (int x = 0; x < tilesX; x++)
            {
                for (int y = 0; y < tilesY; y++)
                {
                    positions[x, y] = new Vector2(x * CurrentTexture.Width, y * CurrentTexture.Height - CurrentTexture.Height);
                }
            }
        }

        public override void Update(GameTime gameTime)
        {
            // Update the positions of the backgrounds
            for (int x = 0; x < positions.GetLength(0); x++)
            {
                for (int y = 0; y < positions.GetLength(1); y++)
                {
                    positions[x, y].Y += speed;

                    // If one background has moved off the screen, reset its position
                    if (positions[x, y].Y >= GraphicsDevice.Viewport.Height)
                    {
                        positions[x, y].Y = -CurrentTexture.Height;
                    }
                }
            }
        }

        public override void Draw(GameTime gameTime)
        {
            SpriteBatch spriteBatch = Game.Services.GetService<SpriteBatch>();
            spriteBatch.Begin();
            // Draw the backgrounds
            for (int x = 0; x < positions.GetLength(0); x++)
            {
                for (int y = 0; y < positions.GetLength(1); y++)
                {
                    spriteBatch.Draw(CurrentTexture, positions[x, y], Color.White);
                }
            }

            spriteBatch.End();
        }

        public void ChangeTexture(Level level)
        {
            switch (level)
            {
                case Level.WaterLevel:
                    CurrentTexture = textureWater;
                    break;
                case Level.DesertLevel:
                    CurrentTexture = textureDesert;
                    break;
                case Level.BaseLevel:
                    CurrentTexture = textureBase;
                    break;
            }
        }
    }
}