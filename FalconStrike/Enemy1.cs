using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FalconStrike
{
    public class Enemy1 : GameObject
    {
        private Game1 game1;
        private Random random;
        int speed = 1000;

        public Enemy1(Game game, Game1 game1) : base(game)
        {
            this.game1 = game1;
            this.random = game1.random;
            game1.OnPlayerGotHit += HandleOnPlayerGotHit;
            game1.OnEnemyGotHit += HandleOnEnemyGotHit;
            game1.OnTimeDown += ChangeTextureIndex;
        }

        protected override void LoadContent()
        {
            texture = Game.Content.Load<Texture2D>("image\\Enemy1");
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
            velocity = new Vector2(0, this.random.Next(speed, speed + 1000) / 10f);
        }

        private void HandleOnPlayerGotHit(Enemy0 enemy0)
        {
            var explode = new Explode(Game, enemy0);
            Game.Components.Add(explode);
            ((Game1)Game).enemiesToRemove.Add(enemy0);
        }

        private void HandleOnEnemyGotHit(Bullet bullet, Enemy0 enemy0)
        {
            var explode = new Explode(Game, enemy0);
            Game.Components.Add(explode);
            ((Game1)Game).enemiesToRemove.Add(enemy0);
        }

        private void ChangeTextureIndex(int level)
        {
            switch (level)
            {
                case 1:

                    speed = 1000;
                    break;
                case 2:

                    speed = 2000;
                    break;
                case 3:
                    speed = 3000;
                    break;
            }
        }
    }
}