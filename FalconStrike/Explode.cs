using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FalconStrike
{
    public class Explode : GameObject
    {
        public Explode(Game game, GameObject enemy) : base(game)
        {
            this.position = enemy.position;
            maxTime = 2;
        }


        protected override void LoadContent()
        {
            texture = Game.Content.Load<Texture2D>("Explode");
            frameBounds = new Rectangle(0, 0, texture.Width / totalFrames, texture.Height);
            colorData = new Color[texture.Width * texture.Height];
            texture.GetData(colorData);
            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            elapsedTime += gameTime.ElapsedGameTime.TotalSeconds;
            if (elapsedTime >= maxTime)
            {
                Game.Components.Remove(this);
                return;
            }

            UpdateFrame(gameTime);
            base.Update(gameTime);
        }
        
    }
}