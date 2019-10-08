using TMPro;
using UnityEngine;
using System.Collections.Generic;
using Utilities;
using System;
using Sirenix.OdinInspector;
using Photon.Pun;
using Photon;
using System.Linq;
using Random = UnityEngine.Random;

namespace Gamemodes
{
    public class FriendzoneModeManager : GameMode
    {
        [SerializeField] private TextMeshProUGUI _actualFriendsText;
        public Tuple<int, int> actualCombination;
        [SerializeField] private TextMeshProUGUI _minFriendsText;
        [SerializeField] private TextMeshProUGUI _maxFriendsText;
        [SerializeField] private TimeBar _myTimebar;

        //General Variables
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

        //Waves Variables
        [TableMatrix(HorizontalTitle = "All Combinations Matrix", SquareCells = true)]
        public List<Tuple<int, int>> allCombinations = new List<Tuple<int, int>>();

        private void Awake()
        {
            allCombinations = new List<Tuple<int, int>>
            {
                new Tuple<int, int>(0,0),
                new Tuple<int, int>(1,1),
                new Tuple<int, int>(1,2),
                new Tuple<int, int>(2,2),
                new Tuple<int, int>(2,4),
                new Tuple<int, int>(2,5),
                new Tuple<int, int>(3,3),
                new Tuple<int, int>(3,4),
                new Tuple<int, int>(3,5),
                new Tuple<int, int>(4,4),
                new Tuple<int, int>(4,5),
                new Tuple<int, int>(4,6),
                new Tuple<int, int>(5,7),
                new Tuple<int, int>(5,8),
                new Tuple<int, int>(5,10)
            };
            actualCombination = allCombinations[4];
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
            SetCombinations();
            SetTimeBarCanvasCamera();
            _myTimebar.transform.parent.gameObject.SetActive(true);
            Debug.Log("Empezo el modo FriendZone");
            _timeMng = new TimeManager(waveTime);
            _timeMng.OnFinishedTimer += OnFinishedTimer;
            _timeMng.OnTimeUpdate += OnTimeUpdate;
        }

        private void SetCombinations()
        {
            //setear los valores de rango del modo de juego
            var newCombinations = allCombinations.Where(x => x.Item2 <= PhotonNetwork.CurrentRoom.PlayerCount).ToList();
            if (newCombinations.Count >= 1)
                actualCombination = newCombinations[Random.Range(0, newCombinations.Count)];
            _minFriendsText.text = actualCombination.Item1.ToString();
            _maxFriendsText.text = actualCombination.Item2.ToString();
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
                SetCombinations();
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
