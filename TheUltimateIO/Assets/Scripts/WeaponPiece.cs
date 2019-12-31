using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponPiece : MonoBehaviour
{
    public int blockPrice;
    public int actualBlocks;
    [HideInInspector] public ConstructWeapon weapon;

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == Layers.CHARACTER)
        {
            //Llamo a funcion del player para mostrar precio y pieza
        }
    }

    public void ConstructActive(int blocks)
    {
        if (blockPrice >= actualBlocks) return;
    }
}
