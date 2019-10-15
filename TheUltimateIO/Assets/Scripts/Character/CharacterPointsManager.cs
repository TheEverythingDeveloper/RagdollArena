using UnityEngine;
using Unity;

namespace Character
{
    public class CharacterPointsManager : IUpdatable
    {
        CharacterModel _owner;
        LevelManager _lvlMng;
        string _nickName;

        public CharacterPointsManager(CharacterModel model, LevelManager lvlMng, string nickName)
        {
            _owner = model;
            _lvlMng = lvlMng;
            _nickName = nickName;
            _owner.OnPointsUpdate += OnUpdateUserPoints;
        }

        public void OnUpdateUserPoints(int newPoints)
        {
            _lvlMng.UpdateUserPoints(_nickName, newPoints);
        }

        public void ArtificialUpdate()
        {
            if (Input.GetKeyDown(KeyCode.P))
            {
                _owner.UpdatePoints(Random.Range(0,100));
            }
        }
        public void ArtificialFixedUpdate() { }
        public void ArtificialLateUpdate() { }
    }
}

