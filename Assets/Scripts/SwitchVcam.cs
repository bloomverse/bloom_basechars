
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;

public class SwitchVcam : MonoBehaviour
{
    // Start is called before the first frame update
     [SerializeField]
    private PlayerInput playerInput;

    private InputAction aimAction;
    private CinemachineVirtualCamera virtualCamera;

     [SerializeField]
    private int priorityBoostAmout = 10;

    [SerializeField]
    private Canvas thirdPersonCanvas;

    [SerializeField]
    private Canvas aimCanvas;


    void Awake()
    {
        virtualCamera = GetComponent<CinemachineVirtualCamera>();
        aimAction = playerInput.actions["RightClick"];


        
        
    }

    private void OnEnable(){
        aimAction.performed += _ => StartAim();
        aimAction.canceled += _ => CancelAim();
    }

     private void OnDisable(){
        aimAction.performed -= _ => StartAim();
        aimAction.canceled -= _ => CancelAim();
    }

    private void StartAim(){
        virtualCamera.Priority += priorityBoostAmout;
        //animator.CrossFade(jumpAnimation,animationPlayTransition);
        aimCanvas.enabled = true;
      
      //  thirdPersonCanvas.enabled =false;
    }

    private void CancelAim(){
        virtualCamera.Priority -= priorityBoostAmout;
         aimCanvas.enabled = false;
       
      //  thirdPersonCanvas.enabled =true;
    }

}
