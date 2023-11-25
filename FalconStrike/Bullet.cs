using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FalconStrike
{
    public class Bullet : GameObject
    {
        Game1 game1;

        public Bullet(Game game, Game1 game1, GameObject player) : base(game)
        {
            this.position = new Vector2(player.position.X, player.position.Y - player.texture.Height / 2);
            this.velocity = new Vector2(0, 150);
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
                if (position.Y - texture.Height / 2 < 0)
                {
                    Game.Components.Remove(this);
                }

                foreach (var enemy in game1.enemies)
                {
                    if (this.CollidesWith(enemy))
                    {
                        game1.enemiesToRemove.Add(enemy);
                    }
                    
                }
                
                
                UpdateFrame(gameTime);
            }

            base.Update(gameTime);
        }

        private void HandleOnEnemyGotHit(Bullet bullet, Enemy enemy)
        {
                ((Game1)Game).bulletsToRemove.Add(bullet);
        }
    }
}