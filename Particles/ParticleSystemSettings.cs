using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace TinyKnight.Particles
{
    public enum AccelerationMode
    {
        /// <summary>
        /// The particle system does not use acceleration.
        /// </summary>
        None,

        /// <summary>
        /// The particle system computes the acceleration by
        /// using the MinAccelerationScale and MaxAccelerationScale
        /// to compute a random scalar value which is then multiplied
        /// by the direction of the particles.
        /// </summary>
        Scalar,

        /// <summary>
        /// The particle system computes the acceleration by using the
        /// EndVelocity value and solving the equation vt = (a0 * t) for a0.
        /// See ParticleSystem.cs for more details.
        /// </summary>
        EndVelocity,

        /// <summary>
        /// The particle system computes the acceleration by using the
        /// MinAccelearionVector and MaxAccelerationVector values to
        /// compute a random vector value which is used as the acceleration
        /// of the particles.
        /// </summary>
        Vector,
    }

    /// <summary>
    /// Settings class describes all the tweakable options used
    /// to control the appearance of a particle system. Many of
    /// the settings are marked with an attribute that makes them
    /// optional so that XML files can be simpler if the wish to
    /// use the default values.
    /// </summary>
    public class ParticleSystemSettings
    {
        // Sets the range of particles used for each "effect" when
        // the particle system is used.
        public int MinNumParticles;
        public int MaxNumParticles;

        public string? TextureFilename = null;

        // MinDirectionAngle and MaxDirectionAngle are used to control
        // the possible directions of motion of the particles. We use
        // degrees instead of radians for the settings to make it easier
        // to construct the XML. The ParticleSystem will convert these
        // to radians as it needs.
        [ContentSerializer(Optional = true)]
        public float MinDirectionAngle = 0f;
        [ContentSerializer(Optional = true)]
        public float MaxDirectionAngle = 360f;

        public float MinInitialSpeed;
        public float MaxInitialSpeed;

        public AccelerationMode AccelerationMode = AccelerationMode.None;

        // Controls how the particle velocity will change over their lifetime.
        // If set to 1, the particles will keep going at the same speed
        // as when they were created. If set to 0, particles will come to
        // a complete stop right before they die.
        // Values greater than 1 make the particles speed up over time. This
        // field is used when using the AccelerationMode.EndVelocity mode.
        [ContentSerializer(Optional = true)]
        public float EndVelocity = 1f;

        // Controls the minimum and maximum acceleration for the particle
        // when using the AccelerationMode.Scalar mode.
        [ContentSerializer(Optional = true)]
        public float MinAccelerationScale = 0f;
        [ContentSerializer(Optional = true)]
        public float MaxAccelerationScale = 0;

        // Controls the minimum and maximum acceleration for the particle
        // when using the AccelerationMode.Vector mode.
        [ContentSerializer(Optional = true)]
        public Vector2 MinAccelerationVector = Vector2.Zero;
        [ContentSerializer(Optional = true)]
        public Vector2 MaxAccelerationVector = Vector2.Zero;

        // Controls how much particles are influenced by the velocity of
        // the object which created them. AddParticles takes in a Vector2
        // which is the base velocity for the particles being created.
        // That velocity is first multiplied by this EmitterVelocitySensitivity
        // to determine how much the particles are actually affected by that velocity.
        [ContentSerializer(Optional = true)]
        public float EmitterVelocitySensitivity = 0f;

        // Range of values controlling how fast the particles rotate.
        // Again, these values should be in degrees for easier XML authoring.
        [ContentSerializer(Optional = true)]
        public float MinRotationSpeed = 0f;
        [ContentSerializer(Optional = true)]
        public float MaxRotationSpeed = 0f;

        public float MinLifetime;
        public float MaxLifetime;

        [ContentSerializer(Optional = true)]
        public float MinSize = 1f;
        [ContentSerializer(Optional = true)]
        public float MaxSize = 1f;

        // Controls the gravity applied to the particles. This can pull
        // particles down to simulate gravity, up for effects like smoke,
        // or any other direction.
        [ContentSerializer(Optional = true)]
        public Vector2 Gravity = Vector2.Zero;

        // Controls the color transition from beginning of the particle's life
        // to the end. We are using Vector3 as Color is not serializable by default.
        [ContentSerializer(Optional = true)]
        public Vector3 StartColor = new Vector3(255f);
        [ContentSerializer(Optional = true)]
        public Vector3 EndColor = new Vector3(255f);

        // Alpha blending settings. Our default gives us a BlendState
        // equivalent to BlendState.AlphaBlend which is suitable for many
        // particle effects.
        [ContentSerializer(Optional = true)]
        public Blend SourceBlend = Blend.One;
        [ContentSerializer(Optional = true)]
        public Blend DestinationBlend = Blend.InverseSourceAlpha;
    }
}
