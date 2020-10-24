using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IControlScheme
{
    void Initialize(OVRPlayerController body, Transform camera, float sensitivity);
    void SetMovementEnabled(bool enabled);
    void Deactivate();
}
