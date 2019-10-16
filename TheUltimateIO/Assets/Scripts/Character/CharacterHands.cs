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
                        sp.connectedBody.AddForce((sp.connectedBody.transform.position - myModel.pelvisRb.transform.position).normalized * myModel.pushForce, ForceMode.Impulse);
                    }

                    photonView.RPC("RPCRemoveSP", RpcTarget.AllBuffered);
                }
            }
        }
        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.layer == Layers.PLAYER || _taken || !activeTaken) return;
            photonView.RPC("RPCAddSP", RpcTarget.AllBuffered, collision);
        }
        [PunRPC]
        void RPCAddSP(Collision col)
        {
            sp = gameObject.AddComponent<SpringJoint>();
            sp.connectedBody = col.rigidbody;
            sp.spring = 12000;
            sp.breakForce = 4000;
            _taken = true;
        }
        [PunRPC]
        void RPCRemoveSP()
        {
            if(GetComponent<SpringJoint>())
                DestroyImmediate(GetComponent<SpringJoint>());

            _taken = false;
        }
        private void OnJointBreak(float breakForce)
        {
            RPCRemoveSP();
        }
    }
}
