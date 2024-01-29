using System.Collections;
using System.Collections.Generic;

namespace TinyKnight.Coroutines
{
    public class CoroutineRunner
    {
        private readonly List<IEnumerator?> running;
        private readonly List<float> delays;

        public CoroutineRunner()
        {
            running = new List<IEnumerator?>();
            delays = new List<float>();
        }

        public CoroutineHandle Run(IEnumerator routine, float delay = 0f)
        {
            running.Add(routine);
            delays.Add(delay);
            return new CoroutineHandle(this, routine);
        }

        public bool Stop(IEnumerator routine)
        {
            var i = running.IndexOf(routine);
            if (i < 0)
                return false;

            running[i] = null;
            delays[i] = 0f;

            return true;
        }

        public void StopAll()
        {
            running.Clear();
            delays.Clear();
        }

        public bool IsRunning(IEnumerator routine)
        {
            return running.Contains(routine);
        }

        public void Update(float deltaTime)
        {
            if (running.Count > 0)
            {
                for (var i = 0; i < running.Count; i++)
                {
                    if (delays[i] > 0f)
                        delays[i] -= deltaTime;
                    else
                    {
                        var current = running[i];
                        if (current == null || !MoveNext(current, i))
                        {
                            running.RemoveAt(i);
                            delays.RemoveAt(i--);
                        }
                    }
                }
            }
        }

        private bool MoveNext(IEnumerator routine, int index)
        {
            if (routine.Current is IEnumerator current)
            {
                if (MoveNext(current, index))
                    return true;

                delays[index] = 0f;
            }

            bool result = routine.MoveNext();

            if (routine.Current is float delay)
                delays[index] = delay;

            return result;
        }

        public int Count
        {
            get { return running.Count; }
        }
    }
}
