using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Character
{
    public class CharacterView : IConstructable, IUpdatable
    {
        CharacterModel _myModel;
        public CharacterView(CharacterModel model)
        {
            _myModel = model;
        }

        public void ArtificialAwake() { }

        public void ArtificialStart() { }

        public void ArtificialUpdate() { }

        public void ArtificialFixedUpdate() { }

        public void ArtificialLateUpdate() { }
    }
}
