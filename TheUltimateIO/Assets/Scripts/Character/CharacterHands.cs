using UnityEngine;
using Photon.Pun;

namespace Character
{
    public class CharacterHands : MonoBehaviourPun
    {
        public CharacterModel myModel;
        public Rigidbody _hand;
        public int buttonMouse;
        bool _taken, _activeTaken;
        private void Awake()
        {
            if (!photonView.IsMine) return;
            myModel = GetComponentInParent<CharacterModel>();
            _hand = GetComponent<Rigidbody>();
        }

        private void FixedUpdate()
        {
            if (!photonView.IsMine) return;

            if (Input.GetMouseButtonDown(buttonMouse))
            {
                //_hand.velocity = Vector3.zero;
                //_hand.AddForce(myModel.pelvisRb.transform.up * 50, ForceMode.Impulse);
                _activeTaken = true;
            }
            if (Input.GetMouseButtonUp(buttonMouse))
            {
                _activeTaken = false;
                if (_taken)
                {
                    DestroyImmediate(GetComponent<SpringJoint>());
                    _taken = false;
                }
            }
        }
        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.layer == Layers.PLAYER && (_taken || !_activeTaken)) return;

            SpringJoint sp = gameObject.AddComponent<SpringJoint>();
            sp.connectedBody = collision.rigidbody;
            sp.spring = 12000;
            sp.breakForce = 4000;
            _taken = true;
        }
        private void OnJointBreak(float breakForce)
        {
            _taken = false;
        }
    }
}
