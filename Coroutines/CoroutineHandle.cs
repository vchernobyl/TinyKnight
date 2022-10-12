using System.Collections;

namespace Gravity.Coroutines
{
    public class CoroutineHandle
    {
        public readonly CoroutineRunner Runner;
        public readonly IEnumerator Enumerator;

        public CoroutineHandle(CoroutineRunner runner, IEnumerator enumerator)
        {
            Runner = runner;
            Enumerator = enumerator;
        }

        public bool Stop()
        {
            return IsRunning && Runner.Stop(Enumerator);
        }

        public IEnumerator Wait()
        {
            while (Runner.IsRunning(Enumerator))
                yield return null;
        }

        public bool IsRunning
        {
            get { return Runner.IsRunning(Enumerator); }
        }
    }
}
