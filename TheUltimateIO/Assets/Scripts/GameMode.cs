using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Es como los gamemodes de unreal. Va a tener todas las cosas base del juego, como si es online, si se empieza desde algun lugar especifico, etc.
/// Es singleton porque no va a haber ninguno otro asi.
/// </summary>
public class GameMode : MonoBehaviour
{
    public static GameMode _Instance;
    public bool online;

    private void Awake()
    {
        _Instance = this;
    }
}