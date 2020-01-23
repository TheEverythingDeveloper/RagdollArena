using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

namespace Character
{
    public class EmojisChat : MonoBehaviourPun
    {
        private CharacterModel _characterModel;
        public ParticlesEmojis _particlesEmojis;
        Chat _chat;
        public void InitializedEmojis(CharacterModel model, Chat c)
        {
            _chat = c;
            _characterModel = model;
            _particlesEmojis = model.GetComponentInChildren<ParticlesEmojis>();
            gameObject.SetActive(false);
        }

        public void ActivateEmoji(int number)
        {
            if (!photonView.IsMine) return;

            if (_particlesEmojis.particlesEmoji[number])
            {
                photonView.RPC("RPCActivateEmoji", RpcTarget.All, number);
                ClosePanel();
            }
        }
        [PunRPC] public void RPCActivateEmoji(int number)
        {
            _particlesEmojis.particlesEmoji[number].Play();
        }

        public void ClosePanel()
        {
            _chat.ActivePanelEmojis(false);
        }
    }
}
