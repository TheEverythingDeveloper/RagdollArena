using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Random = UnityEngine.Random;

namespace Gamemodes
{
    public abstract class GameMode : MonoBehaviour, IUpdatable
    {
        public string gameModeName = "GenericGameMode";
        public Action OnGamemodeEnded = delegate { };
        [Tooltip("Cantidad de puntos que va a ganar cuando gane el modo")] public int winningPoints;
        [Tooltip("Cantidad de puntos que va a perder cuando pierda el modo")] public int loosingPoints;

        public abstract void ArtificialUpdate();
        public abstract void GamemodeActivation(bool on);
        protected abstract void StartGameMode();
        protected abstract void StopGameMode();

        public virtual void ArtificialFixedUpdate() { }
        public virtual void ArtificialLateUpdate() { }
    }
}

