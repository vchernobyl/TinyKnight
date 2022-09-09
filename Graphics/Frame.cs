namespace Gravity.Graphics
{
    public class Frame
    {
        public readonly Subtexture Image;
        public readonly float Duration;

        public Frame(Subtexture image, float duration)
        {
            Image = image;
            Duration = duration;
        }
    }
}
