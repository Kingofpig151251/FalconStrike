using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FalconStrike
{
    public class Enemy : GameObject
    {
        private Game1 game1;
        private Random random;
        Texture2D[] textures = new Texture2D[4];
        int maxTexture = 2;
        int speed = 1000;

        public Enemy(Game game, Game1 game1) : base(game)
        {
            this.game1 = game1;
            this.random = game1.random;
            game1.OnPlayerGotHit += HandleOnPlayerGotHit;
            game1.OnEnemyGotHit += HandleOnEnemyGotHit;
            game1.OnTimeDown += ChangeTextureIndex;
        }

        protected override void LoadContent()
        {
            textures[0] = Game.Content.Load<Texture2D>("image\\Enemy0");
            textures[1] = Game.Content.Load<Texture2D>("image\\Enemy1");
            textures[2] = Game.Content.Load<Texture2D>("image\\Enemy2");
            textures[3] = Game.Content.Load<Texture2D>("image\\Enemy3");
            texture = textures[random.Next(4)];
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
                    maxTexture = 2;
                    speed = 1000;
                    LoadContent();
                    break;
                case 2:
                    maxTexture = 3;
                    speed = 1500;
                    LoadContent();
                    break;
                case 3:
                    maxTexture = 4;
                    speed = 2000;
                    LoadContent();
                    break;
            }
        }
    }
}