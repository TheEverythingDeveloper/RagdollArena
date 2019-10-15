using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Random = UnityEngine.Random;
using System.Linq;
using Utilities;
using Character;

namespace Gamemodes
{
    /// <summary>
    /// GGM = GENERAL GAMEMODE MANAGER
    /// </summary>
    public class GGM : MonoBehaviour
    {
        public static GGM Instance; //singleton porque solo va a haber un GGM si o si
        public List<GameMode> allGameModes = new List<GameMode>();
        public GameMode actualGM;
        private GameModePanel _gameModePanel;
        private TimeManager _timeMng;
        private float _randomTime;
        public CharacterModel user;

        LevelManager _lvlManager;
        private void Awake()
        {
            Instance = this;
            _gameModePanel = transform.parent.GetComponentInChildren<GameModePanel>();
            allGameModes = GetComponentsInChildren<GameMode>().ToList();
            allGameModes.Add(null);
            allGameModes.Select(x =>
            {
                if(x != null)
                {
                    Debug.Log("Gamemode: " + x.gameModeName);
                    x.gameObject.SetActive(false);
                }
                return x;
            }).ToArray();
            ChangeGeneralGamemode();
            _lvlManager = FindObjectOfType<LevelManager>();
        }

        private void Update()
        {
            if (_timeMng != null)
                _timeMng.ArtificialUpdate();

            if (actualGM == null) return;
            actualGM.ArtificialUpdate();
        }

        public void OnFreeTimeUpdate(float actualTime)
        {
            Debug.Log("Free Time: " + (_randomTime - actualTime));
        }

        public void ChangeGeneralGamemode()
        {
            ChangeGamemode(GamemodeType.FriendzoneGM);
        }

        public void ChangeGamemode(GamemodeType gamemodeID)
        {
            Debug.LogWarning("Changing gamemode...");
            GameMode newGameMode = allGameModes[Mathf.Clamp((int)gamemodeID, 0,allGameModes.Count - 1)];

            if(actualGM != null)
            {
                if (actualGM.gameObject != null) //seria el previo en realidad porque no se actualizo todavia
                    actualGM.gameObject.SetActive(false);
            }
            

            if (newGameMode == null)
            {
                _randomTime = Random.Range(4f, 10f);
                _timeMng = new TimeManager(_randomTime);
                _timeMng.OnTimeUpdate += OnFreeTimeUpdate;
                _timeMng.OnFinishedTimer += ChangeGeneralGamemode;
                Debug.Log("Changed gamemode to nothing for : " + _randomTime);
                _gameModePanel.SetGameModeText("FREE TIME!");
                actualGM = null;
            }
            else
            {
                newGameMode.gameObject.SetActive(true);
                Debug.Log("Gamemode changed to " + newGameMode.gameModeName);

                if (_lvlManager) _lvlManager.NewGM(newGameMode);
                else Debug.LogError("LevelManager null");
                //StartGameMode(newGameMode);
            }
        }

        public void StartGameMode(GameMode newGM)
        {
            Debug.LogWarning("new mode: "+ newGM.gameModeName);
            actualGM = newGM;
            actualGM.OnGamemodeEnded += ChangeGeneralGamemode;
            actualGM.GamemodeActivation(true);
        }
    }
}
