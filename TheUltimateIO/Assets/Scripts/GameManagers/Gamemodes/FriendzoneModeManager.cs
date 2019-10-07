using TMPro;
using UnityEngine;
using Utilities;

namespace Gamemodes
{
    public class FriendzoneModeManager : GameMode
    {
        [SerializeField] private TextMeshProUGUI _actualFriendsText;
        [SerializeField] private TextMeshProUGUI _minFriendsText;
        [SerializeField] private TextMeshProUGUI _maxFriendsText;
        [SerializeField] private TimeBar _myTimebar;

        private bool _gameModeOn;
        private TimeManager _timeMng;
        [Tooltip("El tiempo total del modo de juego, si es que tiene un maximo")]
        public float gameModeTime;
        private float _gameModeTotalTime;
        [Tooltip("Cantidad de waves maximas que van a suceder hasta que termine el modo")]
        public int wavesAmount;
        private int _totalWavesAmount;
        [Tooltip("Basicamente, el tiempo total de cada oleada")]
        public float waveTime;
        private float _waveTotalTime;

        private void Awake()
        {
            _waveTotalTime = waveTime;
            waveTime = 0f;
            _totalWavesAmount = wavesAmount;
            wavesAmount = 0;
            _gameModeTotalTime = gameModeTime;
            gameModeTime = 0f;
        }

        private void SetTimeBarCanvasCamera()
        {
            Camera setCamera;
            if (Camera.main != null)
                setCamera = Camera.main;
            else
                setCamera = new Camera();

            _myTimebar.transform.parent.GetComponent<Canvas>().worldCamera = setCamera;
        }

        public override void GamemodeActivation(bool on)
        {
            _gameModeOn = on;

            if (_gameModeOn)
                StartGameMode();
            else
                StopGameMode();
        }

        public override void ArtificialUpdate()
        {
            if (!_gameModeOn) return;

            _timeMng.ArtificialUpdate();
        }

        protected override void StartGameMode()
        {
            SetTimeBarCanvasCamera();
            _myTimebar.transform.parent.gameObject.SetActive(true);
            Debug.Log("Empezo el modo FriendZone");
            _timeMng = new TimeManager(waveTime);
            _timeMng.OnFinishedTimer += OnFinishedTimer;
            _timeMng.OnTimeUpdate += OnTimeUpdate;
        }

        public void OnFinishedTimer()
        {
            wavesAmount++;
            if (wavesAmount >= _totalWavesAmount)
            {
                Debug.Log("Se termino el modo de juego");
                StopGameMode();
            }
            else
            {
                _timeMng.ResetTimer(_waveTotalTime);
            }
        }

        public void OnTimeUpdate(float actualTime)
        {
            waveTime = actualTime;
            _myTimebar.UpdateTimebar(1f - (waveTime / _waveTotalTime));
        }

        protected override void StopGameMode()
        {
            _myTimebar.transform.parent.gameObject.SetActive(false);
            Debug.Log("Termino el modo FriendZone");
            _timeMng.PauseTimer(true);
            OnGamemodeEnded();
        }
    }
}
