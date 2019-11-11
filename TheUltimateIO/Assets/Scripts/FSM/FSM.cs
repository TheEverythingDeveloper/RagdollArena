using System;

namespace FSM
{
    public class FSM<T>
    {
        State<T> _current;

        public FSM(State<T> c)
        {
            _current = c;
            _current.Enter(default(T));
        }

        public void ChangeState(T input)
        {
            State<T> newState;

            if (_current.CheckInput(input, out newState))
            {
                _current.Exit(input);
                _current = newState;
                _current.Enter(input);
            }
        }


        public void Update()
        {
            _current.Update();
        }

        public void FixedUpdate()
        {
            _current.FixedUpdate();
        }
    }
}
