using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Character
{
    public class CharacterBody : MonoBehaviour
    {
        public GameObject[] _allBodyParts;
        public GameObject[] _allHeadParts;
        [Tooltip("Aactivo solo cuando estamos en la ventana de customizacion de personaje")] public bool customizationCharacter;

        private int actualHead;
        public void ChangeHead(bool right)
        {
            foreach (var x in _allHeadParts)
                x.SetActive(false);
            actualHead += right ? 1 : -1;
            _allHeadParts[actualHead % _allHeadParts.Length].SetActive(true);
        }

        public void UpdateAllSkinColors(Color newColor)
        {
            foreach (var x in _allHeadParts)
                x.GetComponentInChildren<MeshRenderer>().material.SetColor("_SkinColor", newColor);
        }

        private void Awake()
        {
            if (!customizationCharacter) return;
            actualHead = _allHeadParts.Length * 10000;
            StartCoroutine(BodyChangeInTimeCoroutine());
        }

        private void Update()
        {
            if (!customizationCharacter) return;

            if (!Input.GetMouseButton(0))
            {
                transform.Rotate(Vector3.up, Time.deltaTime * -100f);
            }
        }

        private int counter;
        IEnumerator BodyChangeInTimeCoroutine()
        {
            while(true)
            {
                foreach (var x in _allBodyParts)
                    x.SetActive(false);
                _allBodyParts[counter % _allBodyParts.Length].SetActive(true);
                counter++;
                yield return new WaitForSeconds(3f);
            }
        }
    }
}
