using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FalconStrike
{
    public class ScrollingBackground : DrawableGameComponent
    {
        private SpriteBatch spriteBatch;
        private Texture2D CurrentTexture, textureWater, textureDesert, textureBase;
        private Vector2[,] positions;
        private const int speed = 1;
        private int level = 1;
        private float transitionAlpha;
        private const float transitionSpeed = 0.02f;
        private Game1 game1;

        public ScrollingBackground(Game game, Game1 game1, SpriteBatch spriteBatch) : base(game)
        {
            this.spriteBatch = spriteBatch;
            this.game1 = game1;
            game1.OnTimeDown += ChangeTexture;
            LoadContent();
        }

        protected override void LoadContent()
        {
            CurrentTexture = textureWater = Game.Content.Load<Texture2D>("image\\Water");
            textureDesert = Game.Content.Load<Texture2D>("image\\Desert");
            textureBase = Game.Content.Load<Texture2D>("image\\Base");
            int tilesX = GraphicsDevice.Viewport.Width / CurrentTexture.Width + 1;
            int tilesY = GraphicsDevice.Viewport.Height / CurrentTexture.Height + 2;
            positions = new Vector2[tilesX, tilesY];
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
            if (!game1.isPaused)
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

            if (transitionAlpha < 1)
            {
                transitionAlpha += transitionSpeed;
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
                    spriteBatch.Draw(CurrentTexture, positions[x, y], Color.White * transitionAlpha);
                }
            }

            spriteBatch.End();
        }

        private void ChangeTexture()
        {
            level++;
            if (level > 3)
            {
                level = 1;
            }

            switch (level)
            {
                case 1:
                    CurrentTexture = textureWater;
                    break;
                case 2:
                    CurrentTexture = textureDesert;
                    break;
                case 3:
                    CurrentTexture = textureBase;
                    break;
            }

            transitionAlpha = 0;
        }
    }
}