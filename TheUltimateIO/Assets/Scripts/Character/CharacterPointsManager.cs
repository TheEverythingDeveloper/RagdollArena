using UnityEngine;
using Unity;

namespace Character
{
    public class CharacterPointsManager : IUpdatable
    {
        CharacterModel _myModel;
        LevelManager _lvlMng;
        string _nickName;

        public CharacterPointsManager(CharacterModel model, LevelManager lvlMng, string nickName)
        {
            _myModel = model;
            _lvlMng = lvlMng;
            _nickName = nickName;
        }
        public void ArtificialUpdate()
        {
            if (Input.GetKeyDown(KeyCode.P))
            {
                _lvlMng.UpdateUserPoints(_nickName,Random.Range(0,100));
            }
        }
        public void ArtificialFixedUpdate() { }
        public void ArtificialLateUpdate() { }
    }
}

