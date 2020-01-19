using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameUI;
using TMPro;
using Photon;
using Photon.Realtime;
using Photon.Pun;
using System.Linq;
using UnityEngine.SceneManagement;

public class EndPanel : MonoBehaviourPun
{
    [SerializeField] private TextMeshProUGUI[] _winnersTexts = new TextMeshProUGUI[4];
    [SerializeField] private TextMeshProUGUI _resultText;
    [SerializeField] private TextMeshProUGUI _rematchButtonText;
    [SerializeField] private Image _generalBackground;
    [SerializeField] private Image _winnersBackground;
    [Tooltip("0 = BGwin, 1 = BGLose, 2 = BGBlue, 3 = BGRed, 4 = BGYellow, 5 = BGGreen")] public Color[] winLoseColors = new Color[3];

    public void UpdateWinnersData(int winID, Player[] winners)
    {
        if(winners.ToList().Contains(PhotonNetwork.LocalPlayer))
        {
            _generalBackground.color = winLoseColors[1];
            _resultText.text = "You Won!";
        }
        else
        {
            _generalBackground.color = winLoseColors[0];
            _resultText.text = "You lose..";
        }

        _winnersBackground.color = winID == 0 ? winLoseColors[2] : winID == 1 ? winLoseColors[3] : 
            winID == 2 ? winLoseColors[4] : winLoseColors[5];

        for (int i = 0; i < winners.Length; i++)
        {
            _winnersTexts[i].gameObject.SetActive(true);
            _winnersTexts[i].text = winners[i].NickName;
        }

        if (PhotonNetwork.IsMasterClient)
            _rematchButtonText.text = "Rematch";
    }

    private List<Player> _allRematchPlayers = new List<Player>();
    [SerializeField] private TextMeshProUGUI _rematchText;

    public void PlayAgainButton()
    {
        if (PhotonNetwork.IsMasterClient)
            StartRematchButton();
        else
            photonView.RPC("RPCRematch", RpcTarget.MasterClient, PhotonNetwork.LocalPlayer);

        //Que se bloquee tanto este boton como el de back to menu cuando pusimos playagain.
    }

    [PunRPC] private void RPCRematch(Player addedPlayer)
    {
        if (_allRematchPlayers.Contains(addedPlayer)) return;

        _allRematchPlayers.Add(addedPlayer);
        photonView.RPC("RPCUpdateRematchPlayersNumber", RpcTarget.All, _allRematchPlayers.Count);
    }
    [PunRPC] private void RPCUpdateRematchPlayersNumber(int actualNum) => _rematchText.text = actualNum.ToString();
    public void BackToMenuButton() { SceneManager.LoadSceneAsync(0); }
    public void StartRematchButton() { FindObjectOfType<Server>().StartRematch(_allRematchPlayers); }
}
