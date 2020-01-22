using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Character
{
    public class CharacterStats : MonoBehaviour
    {
        public float life = 1;
        public float damageAttack = 60;
        public float speed = 60;
        public float jumpSpeed = 200;
        public float rotationSpeed = 2;

        public float initialDistAttack;
        public float verticalDistAttack;
        public float horizontalDistAttack;
        public float delayMeleeAttackInSeconds;
        public float delayBowAttackInSeconds;
    }
}
