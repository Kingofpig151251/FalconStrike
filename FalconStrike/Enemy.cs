using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FalconStrike
{
    public class Enemy : GameObject
    {
        private Game1 game1;
        private Random random;
        public int textureIndex = 0;
        int speed = 1000;

        public Enemy(Game game, Game1 game1, int textureIndex) : base(game)
        {
            this.game1 = game1;
            this.random = game1.random;
            this.textureIndex = textureIndex;
            game1.OnPlayerGotHit += HandleOnPlayerGotHit;
            game1.OnEnemyGotHit += HandleOnEnemyGotHit;
            game1.OnTimeDown += ChangeTextureIndex;
        }

        public override void Initialize()
        {
            switch (textureIndex)
            {
                case 0:
                    texture = Game.Content.Load<Texture2D>("image\\Enemy0");
                    break;
                case 1:
                    texture = Game.Content.Load<Texture2D>("image\\Enemy1");
                    break;
                case 2:
                    texture = Game.Content.Load<Texture2D>("image\\Enemy2");
                    break;
                case 3:
                    texture = Game.Content.Load<Texture2D>("image\\Enemy3");
                    break;
            }

            frameBounds = new Rectangle(0, 0, texture.Width / totalFrames, texture.Height);
            colorData = new Color[texture.Width * texture.Height];
            texture.GetData(colorData);
            base.Initialize();
        }

        protected override void LoadContent()
        {
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
            position = new Vector2(random.Next(GraphicsDevice.Viewport.Width - texture.Width / 2) + texture.Width / 4, -texture.Height / 2);
            velocity = new Vector2(0, this.random.Next(speed, speed + 1000) / 10f);
        }

        private void HandleOnPlayerGotHit(Enemy enemy)
        {
            var explode = new Explode(Game, enemy);
            Game.Components.Add(explode);
            ((Game1)Game).enemiesToRemove.Add(enemy);
        }

        private void HandleOnEnemyGotHit(Bullet bullet, Enemy enemy)
        {
            var explode = new Explode(Game, enemy);
            Game.Components.Add(explode);
            ((Game1)Game).enemiesToRemove.Add(enemy);
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