using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Photon.Pun;
using UnityEngine.SceneManagement;

public class OfflineManager : MonoBehaviour
{
    [MenuItem("Modes/Offline ON")]
    public static void OfflineModeON()
    {
        Debug.Log("Offline mode : ON");
        if(SceneManager.GetActiveScene().name != "MainScene"
            && SceneManager.GetActiveScene().name != "TowerLevel" 
            && SceneManager.GetActiveScene().name != "TwoTeams")
        {
            Debug.LogError("Se necesita estar en la escena MainScene para activar modo offline");
            return;
        }
        GameObject testLvlMng = Instantiate(Resources.Load("LevelManager")) as GameObject;
        testLvlMng.GetComponent<LevelManager>().offlineMode = true;
    }

    [MenuItem("Modes/Offline OFF")]
    public static void OfflineModeOFF()
    {
        Debug.Log("Offline mode : OFF");
        if (SceneManager.GetActiveScene().name != "MainScene")
        {
            Debug.LogError("Se necesita estar en la escena MainScene para desactivar modo offline");
            return;
        }
        var lvlMng = FindObjectOfType<LevelManager>();
        if(lvlMng != null)
        {
            DestroyImmediate(lvlMng.gameObject);
        }
    }
}
