using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

namespace Character
{
    public class EmojisChat : MonoBehaviourPun
    {
        private CharacterModel _characterModel;
        Chat _chat;
        public void InitializedEmojis(CharacterModel model, Chat c)
        {
            _chat = c;
            _characterModel = model;
            gameObject.SetActive(false);
        }

        public void ActivateEmoji(int number)
        {

            if (_characterModel.particlesPlayer.particlesEmoji[number])
            {
                photonView.RPC("RPCActivateEmoji", RpcTarget.All, number);
                ClosePanel();
            }
        }
        [PunRPC] public void RPCActivateEmoji(int number)
        {
            _characterModel.particlesPlayer.particlesEmoji[number].Play();
        }

        public void ClosePanel()
        {
            _chat.ActivePanelEmojis(false);
        }
    }
}
