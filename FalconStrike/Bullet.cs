using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FalconStrike
{
    public class Bullet : GameObject
    {
        public Bullet(Game game, Game1 game1, Vector2 position) : base(game)
        {
            this.position = position;
            this.velocity = new Vector2(0, 200);
            game1.OnEnemyGotHit += HandleOnEnemyGotHit;
        }


        protected override void LoadContent()
        {
            texture = Game.Content.Load<Texture2D>("Bullet");
            frameBounds = new Rectangle(0, 0, texture.Width / totalFrames, texture.Height);
            colorData = new Color[texture.Width * texture.Height];
            texture.GetData(colorData);
            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            if (!isPaused)
            {
                position.Y -= velocity.Y * (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (this.position.Y - texture.Height / 2 < 0)
                {
                    Game.Components.Remove(this);
                    return;
                }

                UpdateFrame(gameTime);
                base.Update(gameTime);
            }
        }

        private void HandleOnEnemyGotHit(Bullet bullet, Enemy enemy)
        {
            ((Game1)Game).bulletsToRemove.Add(bullet);
        }
    }
}