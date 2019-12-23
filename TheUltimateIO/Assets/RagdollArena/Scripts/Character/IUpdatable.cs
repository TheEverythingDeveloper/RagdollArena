using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IUpdatable
{
    void ArtificialUpdate();
    void ArtificialFixedUpdate();
    void ArtificialLateUpdate();
}
