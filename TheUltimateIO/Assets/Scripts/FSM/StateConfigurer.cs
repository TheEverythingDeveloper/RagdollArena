using System.Collections.Generic;

namespace FSM
{
	public class StateConfigurer<T>
    {
		State<T> _instance;
		Dictionary<T, Transition<T>> _transitions = new Dictionary<T, Transition<T>>();

		public StateConfigurer(State<T> state) {
			_instance = state;
		}

		public StateConfigurer<T> SetTransition(T input, State<T> target) {
			_transitions.Add(input, new Transition<T>(input, target));
			return this;
		}

		public void Done() {
			_instance.Configure(_transitions);
		}
	}

	public static class StateConfigurer
    {
		public static StateConfigurer<T> Create<T>(State<T> state)
        {
			return new StateConfigurer<T>(state);
		}
	}
}