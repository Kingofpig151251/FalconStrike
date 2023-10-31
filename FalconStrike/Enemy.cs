using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FalconStrike
{
    public class Enemy : GameObject
    {
        private Game1 game1;
        private Random random;

        public Enemy(Game game, Game1 game1) : base(game)
        {
            this.game1= game1;
            this.random = game1.random;
            game1.OnPlayerGotHit += HandleOnPlayerGotHit;
            game1.OnEnemyGotHit += HandleOnEnemyGotHit;
            velocity = new Vector2(0, this.random.Next(1000, 2000) / 10f);
        }

        protected override void LoadContent()
        {
            texture = Game.Content.Load<Texture2D>("image\\Enemy");
            frameBounds = new Rectangle(0, 0, texture.Width / totalFrames, texture.Height);
            colorData = new Color[texture.Width * texture.Height];
            texture.GetData(colorData);
            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            if (!game1.isPaused)
            {
                position.Y += velocity.Y * (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (position.Y > GraphicsDevice.Viewport.Height)
                {
                    SetInitialPosition();
                }

                UpdateFrame(gameTime);
            }

            base.Update(gameTime);
        }

        public void SetInitialPosition()
        {
            position = new Vector2(random.Next(GraphicsDevice.Viewport.Width - texture.Width / 2) + texture.Width / 4,
                -texture.Height / 2);
        }

        private void HandleOnPlayerGotHit(Enemy enemy)
        {
            var explode = new Explode(Game, enemy);
            Game.Components.Add(explode);
        }

        private void HandleOnEnemyGotHit(Bullet bullet, Enemy enemy)
        {
            var explode = new Explode(Game, enemy);
            Game.Components.Add(explode);
        }
    }
}