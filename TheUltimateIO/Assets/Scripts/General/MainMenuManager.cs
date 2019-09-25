using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    [Tooltip("0 = config, 1 = shop, 2 = stats, 3 = checklist")]
    public GameObject[] allCanvas = new GameObject[5];

    public Color selectedColor;
    public Image[] allImages = new Image[5];
    public void OpenPanel(int ID)
    {
        for (int i = 0; i < allCanvas.Length; i++)
        {
            ClosePanel(i);
        }
        allCanvas[ID].SetActive(true);
        allImages[ID].color = selectedColor;
    }

    public void ClosePanel(int ID)
    {
        allImages[ID].color = Color.white;
        allCanvas[ID].SetActive(false);
    }

    public void GameButton()
    {
        allImages[4].color = selectedColor;
        SceneManager.LoadSceneAsync(1);
    }
}
