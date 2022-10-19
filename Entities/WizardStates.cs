using Microsoft.Xna.Framework;

namespace Gravity.Entities
{
    // TODO: Let's make it a a ghost/wraith instead of a wizard.
    // It is going to chase player through walls and other solids,
    // as a self respecting spectral enemy should ;)

    // TODO: Check if it makes sense to make states singletons so that
    // we don't recreate a new instance every time.
    public class WizardChaseState : IState<Wizard>
    {
        private readonly Hero hero;

        // TODO: We need a mechanism to communicate with other entities somehow.
        // In the case of a wizard, we want to have a reference to the hero so 
        // that the wizard can chase him and throw fire-balls at him.
        public WizardChaseState(Hero hero)
        {
            this.hero = hero;
        }

        public void Enter(Wizard entity)
        {
        }

        public void Execute(Wizard entity)
        {
            const float speed = .005f;
            var dir = Vector2.Normalize(hero.Position - entity.Position) * speed;
            entity.DX += dir.X;
            entity.DY += dir.Y;
        }

        public void Exit(Wizard entity)
        {
        }
    }

    public class WizardDeathState : IState<Wizard>
    {
        public void Enter(Wizard entity)
        {
            entity.Destroy();
        }

        public void Execute(Wizard entity)
        {
        }

        public void Exit(Wizard entity)
        {
        }
    }
}
