namespace Gravity.GFX
{
    // This is the animation player. State.
    public class AnimationTrack
    {
        private readonly Animation animation;
        private int currentFrameIndex;
        private float time;
        private bool finished;

        public AnimationTrack(Animation animation)
        {
            this.animation = animation;
        }

        public void Start(int frameIndex)
        {
            if (frameIndex >= animation.FrameCount)
                return;

            currentFrameIndex = frameIndex;
            var currentFrame = animation.GetFrame(currentFrameIndex);
            time = currentFrame.Duration;
        }

        public void Update(float deltaTime)
        {
            if (animation.FrameCount == 0 || finished)
                return;

            time -= deltaTime;
            if (time <= 0f)
            {
                currentFrameIndex++;
                if (currentFrameIndex >= animation.FrameCount)
                {
                    if (animation.Looping)
                        currentFrameIndex = 0;
                    else
                    {
                        finished = true;
                        currentFrameIndex = animation.FrameCount - 1;
                        return;
                    }
                }

                var currentFrame = animation.GetFrame(currentFrameIndex);
                time = currentFrame.Duration;
            }
        }

        public Animation.Frame CurrentFrame
        {
            get { return animation.GetFrame(currentFrameIndex); }
        }
    }
}
