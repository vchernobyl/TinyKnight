namespace Gravity
{
    public interface IState<T> where T : Entity
    {
        void Enter(T entity);
        void Execute(T entity);
        void Exit(T entity);
    }

    public class StateMachine<T> where T : Entity
    {
        private readonly T owner;

        private IState<T> currentState;
        private IState<T> previousState;
        private IState<T> globalState;

        public StateMachine(T owner)
        {
            this.owner = owner;
        }

        public void Update()
        {
            globalState?.Execute(owner);
            currentState?.Execute(owner);
        }

        public void ChangeState(IState<T> newState)
        {
            previousState = currentState;
            currentState.Exit(owner);
            currentState = newState;
            currentState.Enter(owner);
        }

        public void RevertToPreviousState()
        {
            ChangeState(previousState);
        }

        public bool IsInState(IState<T> state)
        {
            return currentState == state;
        }
    }
}
