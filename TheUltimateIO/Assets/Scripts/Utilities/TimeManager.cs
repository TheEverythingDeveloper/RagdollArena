using UnityEngine;
using System;
using Random = UnityEngine.Random;
using System.Collections.Generic;

namespace Utilities
{
    public class TimeManager : IUpdatable
    {
        private float _actualTime;
        private bool _timerOn;
        public float ActualTime
        {
            get { return _actualTime; }
        }
        private float _totalTime;
        public event Action OnFinishedTimer = delegate { };
        public event Action<float> OnTimeUpdate = delegate { };
        /// <summary>
        /// No olvidarse de que hay 2 eventos: OnFinishedTimer para cuando termina y OnTimeUpdate cada frame
        /// </summary>
        /// <param name="totalTimerTime"></param>
        public TimeManager(float totalTimerTime) //constructor
        {
            ResetTimer(totalTimerTime);
        }

        public void ResetTimer(float newTime)
        {
            _actualTime = 0f;
            _totalTime = newTime;
            _timerOn = true;
        }

        public void PauseTimer(bool pause)
        {
            _timerOn = pause;
        }

        public void ArtificialUpdate()
        {
            if (!_timerOn) return;

            _actualTime += Time.deltaTime;
            if (_actualTime >= _totalTime)
            {
                _timerOn = false;
                Debug.Log("Timer terminado");
                OnFinishedTimer();
            }
            else
                OnTimeUpdate(_actualTime);
        }

        public void ArtificialFixedUpdate() { }
        public void ArtificialLateUpdate() { }
    }
}
