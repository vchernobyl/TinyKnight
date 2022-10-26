using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Gravity.Weapons
{
    // TODO: This could be an entity.
    public class WeaponPickupText : DrawableGameComponent
    {
        private readonly SpriteBatch spriteBatch;
        private readonly SpriteFont font;
        private readonly string weaponText;
        private readonly Curve alphaOverTime;

        private Vector2 position;
        private float time;
        private float alpha;

        public WeaponPickupText(Game game, string weaponText, Vector2 position)
            : base(game)
        {
            this.weaponText = weaponText;
            spriteBatch = (SpriteBatch)game.Services.GetService(typeof(SpriteBatch));
            font = game.Content.Load<SpriteFont>("Fonts/font");
            this.position = position - font.MeasureString(weaponText) / 2f;
            this.position.Y -= Level.CellSize / 2f;

            alphaOverTime = new Curve();
            alphaOverTime.Keys.Add(new CurveKey(0f, 1f));
            alphaOverTime.Keys.Add(new CurveKey(.5f, .8f));
            alphaOverTime.Keys.Add(new CurveKey(.75f, .5f));
            alphaOverTime.Keys.Add(new CurveKey(1f, 0f));
        }

        public override void Update(GameTime gameTime)
        {
            time += gameTime.DeltaTime();
            alpha = alphaOverTime.Evaluate(time);
            position.Y -= gameTime.DeltaTime() * 100f;
        }

        public override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin(transformMatrix: GravityGame.WorldCamera.Transform);
            spriteBatch.DrawString(font, weaponText, position, Color.White * alpha);
            spriteBatch.End();
        }
    }
}
