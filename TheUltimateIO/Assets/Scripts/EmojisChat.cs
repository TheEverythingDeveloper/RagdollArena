using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

namespace Character
{
    public class EmojisChat : MonoBehaviourPun
    {
        private Chat _chat;

        public void InitializedEmojis(Chat c)
        {
            _chat = c;
            gameObject.SetActive(false);
        }

        public void ActivateEmoji(int number)
        {
            FindObjectOfType<Server>().photonView.RPC("RPCActivateEmoji", RpcTarget.MasterClient ,PhotonNetwork.LocalPlayer, number);
            ClosePanel();
        }

        public void ClosePanel()
        {
            if(_chat != null)
                _chat.ActivePanelEmojis(false);
        }
    }
}
