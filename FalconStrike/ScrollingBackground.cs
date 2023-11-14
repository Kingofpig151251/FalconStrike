using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FalconStrike
{
    public class ScrollingBackground : DrawableGameComponent
    {
        private SpriteBatch spriteBatch;
        private Texture2D[] texture = new Texture2D[3];
        public int textureindex = 0;
        private Vector2[,] positions;
        private const int speed = 1;
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
            texture[0] = Game.Content.Load<Texture2D>("image\\Water");
            texture[1] = Game.Content.Load<Texture2D>("image\\Desert");
            texture[2] = Game.Content.Load<Texture2D>("image\\Base");
            int tilesX = GraphicsDevice.Viewport.Width / texture[0].Width + 1;
            int tilesY = GraphicsDevice.Viewport.Height / texture[0].Height + 2;
            positions = new Vector2[tilesX, tilesY];
            for (int x = 0; x < tilesX; x++)
            {
                for (int y = 0; y < tilesY; y++)
                {
                    positions[x, y] = new Vector2(x * texture[0].Width, y * texture[0].Height - texture[0].Height);
                }
            }
        }

        public override void Update(GameTime gameTime)
        {
            if (!game1.isPaused && game1.level != 0)
            {
                for (int x = 0; x < positions.GetLength(0); x++)
                {
                    for (int y = 0; y < positions.GetLength(1); y++)
                    {
                        positions[x, y].Y += speed;
                        if (positions[x, y].Y >= GraphicsDevice.Viewport.Height)
                        {
                            positions[x, y].Y = -texture[0].Height;
                        }
                    }
                }
            }

            if (transitionAlpha < 1)
            {
                transitionAlpha += transitionSpeed;
            }

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            SpriteBatch spriteBatch = Game.Services.GetService<SpriteBatch>();
            spriteBatch.Begin();

            for (int x = 0; x < positions.GetLength(0); x++)
            {
                for (int y = 0; y < positions.GetLength(1); y++)
                {
                    spriteBatch.Draw(texture[textureindex], positions[x, y], Color.White * transitionAlpha);
                }
            }

            spriteBatch.End();
        }

        private void ChangeTexture(int level)
        {
            if (textureindex<2)
            {
                textureindex++;
            }
            else
            {
                textureindex = 0;
            }
            transitionAlpha = 0;
        }
    }
}