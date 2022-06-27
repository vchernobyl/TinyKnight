using Microsoft.Xna.Framework;

namespace Gravity
{
    public interface IProjectile
    {
        Vector2 Velocity { get; set; }
        int Damage { get; set; }
    }
}
