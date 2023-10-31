using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FalconStrike
{
    public class Bullet : GameObject
    {
        Game1 game1;

        public Bullet(Game game, Game1 game1, GameObject player) : base(game)
        {
            this.game1 = game1;
            game1.OnEnemyGotHit += HandleOnEnemyGotHit;
        }


        protected override void LoadContent()
        {
            texture = Game.Content.Load<Texture2D>("image\\Bullet");
            frameBounds = new Rectangle(0, 0, texture.Width / totalFrames, texture.Height);
            colorData = new Color[texture.Width * texture.Height];
            texture.GetData(colorData);
            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            if (!game1.isPaused)
            {
                position.Y -= velocity.Y * (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (this.position.Y - texture.Height / 2 < 0)
                {
                    ResetThis();
                    return;
                }

                UpdateFrame(gameTime);
            }

            base.Update(gameTime);
        }

        private void ResetThis()
        {
            this.isActivate = false;
            this.position = new Vector2(0, 0);
            this.velocity = new Vector2(0, 0);
        }

        private void HandleOnEnemyGotHit(Bullet bullet, Enemy enemy)
        {
            ResetThis();
        }
    }
}