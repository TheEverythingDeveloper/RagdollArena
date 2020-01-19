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
    }

    public void PlayAgainButton()
    {
        SceneManager.LoadSceneAsync(0); //TODO: No tiene que volver, sino quedarse y reiniciar bien
    }

    public void BackToMenuButton()
    {
        SceneManager.LoadSceneAsync(0);
    }
}
