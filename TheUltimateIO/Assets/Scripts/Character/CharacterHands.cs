using UnityEngine;
using Photon.Pun;
using System.Collections.Generic;
namespace Character
{
    public class CharacterHands : MonoBehaviourPun
    {
        public CharacterModel myModel;
        public Rigidbody _hand;
        public int buttonMouse;
        public bool activeTaken;
        bool _taken;
        SpringJoint sp;

        private void Awake()
        {
            if (!photonView.IsMine) return;
            myModel = GetComponentInParent<CharacterModel>();
            _hand = GetComponent<Rigidbody>();
            myModel.hands.Add(this);
        }

        private void FixedUpdate()
        {
            if (!photonView.IsMine) return;

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
                    if(sp && sp.connectedBody)
                    {
                        photonView.ObservedComponents.Remove(sp);
                        sp.connectedBody.AddForce((sp.connectedBody.transform.position - myModel.pelvisRb.transform.position).normalized * myModel.pushForce, ForceMode.Impulse);
                    }

                    DestroyImmediate(GetComponent<SpringJoint>());
                    _taken = false;
                }
            }
        }
        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.layer == Layers.PLAYER || _taken || !activeTaken) return;
            sp = gameObject.AddComponent<SpringJoint>();
            sp.connectedBody = collision.rigidbody;
            sp.spring = 12000;
            sp.breakForce = 4000;
            List<Component> x = photonView.ObservedComponents;
            x.Add(sp);
            photonView.ObservedComponents = x;
            _taken = true;
        }
        private void OnJointBreak(float breakForce)
        {
            if (sp)
                photonView.ObservedComponents.Remove(sp);

            _taken = false;
        }
    }
}
