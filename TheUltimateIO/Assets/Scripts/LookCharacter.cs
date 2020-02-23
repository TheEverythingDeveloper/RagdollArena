using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookCharacter : MonoBehaviour
{
    Transform _target;
    Coroutine _coroutineLook;

    public void LookActive(Transform target)
    {
        _target = target;
        if(_coroutineLook == null) _coroutineLook = StartCoroutine(Look());
    }

    public void LookOff()
    {
        StopAllCoroutines();
        _coroutineLook = null;
    }
    IEnumerator Look()
    {
        while (true)
        {
            transform.LookAt(_target);
            yield return new WaitForEndOfFrame();
        }
    }
}
