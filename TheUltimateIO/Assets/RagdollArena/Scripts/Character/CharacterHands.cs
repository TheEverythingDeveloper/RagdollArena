using UnityEngine;
using Photon.Pun;
using System.Collections.Generic;
namespace Character
{
    public class CharacterHands : MonoBehaviourPun, IUpdatable, IConstructable
    {
        public CharacterModel myModel;
        public Rigidbody _hand;
        public int buttonMouse;
        public bool activeTaken;
        bool _taken;
        SpringJoint sp;

        public void ArtificialAwake()
        {
            myModel = GetComponentInParent<CharacterModel>();
            _hand = GetComponent<Rigidbody>();
            myModel.hands.Add(this);
        }

        public void ArtificialFixedUpdate()
        {
            if (Input.GetMouseButtonDown(buttonMouse))
            {
                _hand.velocity = Vector3.zero;
                _hand.AddForce(myModel.pelvisRb.transform.up * 50, ForceMode.Impulse);
                activeTaken = true;
            }

            if (Input.GetMouseButtonUp(buttonMouse))
            {
                activeTaken = false;
                if (_taken)
                {
                    if (sp && sp.connectedBody)
                    {
                        sp.connectedBody.AddForce((sp.connectedBody.transform.position - myModel.pelvisRb.transform.position).normalized * myModel.pushForce, ForceMode.Impulse);
                    }

                    DestroyImmediate(GetComponent<SpringJoint>());
                    _taken = false;
                }
            }
        }
        public void ArtificialLateUpdate() { }
        public void ArtificialStart() { }
        public void ArtificialUpdate() { }
        private void OnCollisionEnter(Collision collision)
        {
            if (!myModel.owned) return;

            if (collision.gameObject.layer == Layers.PLAYER || _taken || !activeTaken) return;
            sp = gameObject.AddComponent<SpringJoint>();
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
