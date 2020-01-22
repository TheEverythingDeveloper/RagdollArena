using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Character;
using System.Linq;

namespace Character
{
    public class CharacterFriendsManager : IUpdatable
    {
        private CharacterModel _owner;
        private LayerMask _playersLayermask;

        public CharacterFriendsManager(CharacterModel owner, LayerMask playersLayermask)
        {
            _playersLayermask = playersLayermask;
            _owner = owner;
            _owner.GetActiveModeValue = () => FriendsAmount;
        }

        private int _friendsAmount; //cantidad de amigos que tenemos cerca
        public int FriendsAmount
        {
            get { return _friendsAmount; }
            set { _friendsAmount = Mathf.Clamp(value, 0, 1000000); }
        }

        public void ArtificialUpdate()
        {
            //tirar overlapsphere para actualizarnos la cantidad que tenemos cerca
            var allCharacters = Physics.OverlapSphere(
                _owner.rb.transform.position, _owner.contactRadius, _playersLayermask, QueryTriggerInteraction.Collide)
                .Where(x => x.GetComponentInParent<CharacterModel>() != _owner);
            FriendsAmount = allCharacters.Count();
            //Debug.LogWarning("Amigos actuales ---- " + FriendsAmount);
        }

        public void ArtificialFixedUpdate() { }
        public void ArtificialLateUpdate() { }
    }
}
