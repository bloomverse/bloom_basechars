using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public struct NetworkInputData : INetworkInput
{
    public Vector2 movementInput;
    public float rotationInput;
    public Vector3 aimForwardVector;
    public Vector3 cameraTransform;
    public NetworkBool isJumpPressed;
    public NetworkBool isFireButtonPressed;
   
}
