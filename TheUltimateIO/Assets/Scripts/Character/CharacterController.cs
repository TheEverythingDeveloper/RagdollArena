using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Character
{
    public class CharacterController : IUpdatable
    {
        CharacterModel _myModel;
        public CharacterController(CharacterModel model)
        {
            _myModel = model;
        }

        public void ArtificialUpdate()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                _myModel.TryJump();
            }
        }

        public void ArtificialFixedUpdate() { }

        public void ArtificialLateUpdate() { }
    }
}
