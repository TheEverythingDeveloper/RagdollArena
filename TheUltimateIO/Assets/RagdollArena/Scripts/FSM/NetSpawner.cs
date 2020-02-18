using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class NetSpawner : MonoBehaviour
{
    public GameObject canvasWaitingStart;
    public TextMeshProUGUI textNamesPlayers, textWaiting;
    public GameObject cameraGo;
    public Transform cameraPos;

    private void Awake()
    {
        cameraGo.SetActive(false);
        if (FindObjectOfType<Camera>() == null)
        {
            Debug.Log("new camera");
            NetManager netCam = ((GameObject)Instantiate(Resources.Load("NetManagerCamera")) as GameObject).GetComponent<NetManager>();
            netCam.canvasWaitingStart = canvasWaitingStart;
            netCam.textNamesPlayers = textNamesPlayers;
            netCam.textWaiting = textWaiting;
        }
        else
        {
            FindObjectOfType<Camera>().transform.position = cameraPos.position;
            FindObjectOfType<Camera>().transform.rotation = cameraPos.rotation;
        }
    }
}
