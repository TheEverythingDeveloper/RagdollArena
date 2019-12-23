using System;

namespace FSM
{
	public class Transition<T>
    {
		public event Action<T> OnTransition = delegate { };
		T _input;
		public State<T> targetState;


        public void OnTransitionExecute(T input)
        {
			OnTransition(input);
		}

		public Transition(T input, State<T> targetState)
        {
			this._input = input;
			this.targetState = targetState;
		}
	}
}