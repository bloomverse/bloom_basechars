using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterInputHandler : MonoBehaviour
{
    Vector2 moveInputVector = Vector2.zero;
    Vector2 viewInputVector = Vector2.zero;
    bool isJumpButtonPressed = false;
    bool isFireButtonPressed = false;

    private StarterAssetsInputs _input;

    //Other components
    LocalCameraHandler localCameraHandler;
    CharacterMovementHandler characterMovementHandler;

    private void Awake()
    {
        localCameraHandler = GetComponentInChildren<LocalCameraHandler>();
        characterMovementHandler = GetComponent<CharacterMovementHandler>();
    }

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        _input = GetComponent<StarterAssetsInputs>();
    }

    // Update is called once per frame
    void Update()
    {
        if(!characterMovementHandler.Object.HasInputAuthority)
        return;

         //View input
        viewInputVector.x = _input.look.x; //Input.GetAxis("Mouse X");
        viewInputVector.y = _input.look.y * -1; //Input.GetAxis("Mouse Y") * -1; //Invert the mouse look

        //Move input
        moveInputVector.x = _input.move.x; // Input.GetAxis("Horizontal");
        moveInputVector.y = _input.move.y; //Input.GetAxis("Vertical");

//        Debug.Log(moveInputVector.x + " reaccion x " + moveInputVector.y);

        //Debug.Log(_input.jump + "jump");
        isJumpButtonPressed = _input.jump; //Input.GetButtonDown("Jump");
        isFireButtonPressed = _input.leftStatus;

        localCameraHandler.SetViewInputVector(viewInputVector);


    }

    public NetworkInputData GetNetworkInput()
    {
        NetworkInputData networkInputData = new NetworkInputData();

        //Aim data
        networkInputData.aimForwardVector = localCameraHandler.transform.forward;

        networkInputData.cameraTransform = localCameraHandler.transform.position;

        //View data
        networkInputData.rotationInput = viewInputVector.x;

        //Move data
        networkInputData.movementInput = moveInputVector;

        //Jump data
        networkInputData.isJumpPressed = isJumpButtonPressed;

        //Fire data
        networkInputData.isFireButtonPressed = isFireButtonPressed;

         isJumpButtonPressed = false;
        isFireButtonPressed = false;

        return networkInputData;
    }
}
