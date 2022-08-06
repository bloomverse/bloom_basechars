
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController), typeof(PlayerInput))]
public class PlayerController : MonoBehaviour
{
    private CharacterController controller;
    private Vector3 playerVelocity;
    private bool groundedPlayer;

    private Transform cameraTransform;

    private PlayerInput playerInput;

    [SerializeField]
    private float playerSpeed = 2.0f;
    [SerializeField]
    private float jumpHeight = 1.0f;
    [SerializeField]
    private float gravityValue = -9.81f;
    
    [SerializeField]
    private float rotationSpeed = .8f;


     [SerializeField]
    private GameObject bulletPrefab;

     [SerializeField]
    private Transform barrelTransform;

     [SerializeField]
    private Transform bulletParent;

    //[SerializeField]
    //private int bulletHitMissDistance = 25;

    [SerializeField]
    private float animationSmoothTime = 0.1f;

    [SerializeField]
    private float animationPlayTransition = 0.15f;

   
    [SerializeField]  private Gun Gun;

    private InputAction moveAction;
    private InputAction jumpAction;
    private InputAction shootAction;
    private InputAction aimAction;
    private InputAction aimRifleAction;
    private InputAction interactAction;

    private InputAction spellAction1;
    private InputAction spellAction2;
    private InputAction spellAction3;
    private InputAction spellAction4;
    private InputAction spellAction5;

    private Animator animator;
    int jumpAnimation;
    int interactAnimation;
    int moveXAnimationParameterId;
    int moveZAnimationParameterId;

    Vector2 currentAnimationBlendVector;
    Vector2 animationVelocity;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        playerInput = GetComponent<PlayerInput>();
        cameraTransform = Camera.main.transform;
        moveAction = playerInput.actions["Move"];
        jumpAction = playerInput.actions["Jump"];
        shootAction = playerInput.actions["Shoot"];
        aimAction = playerInput.actions["Aim"];
       // aimRifleAction = playerInput.actions["AimRifle"];
        // Spellls
        spellAction1 = playerInput.actions["CastSpell1"];
        spellAction2 = playerInput.actions["CastSpell2"];
        spellAction3 = playerInput.actions["CastSpell3"];
        spellAction4 = playerInput.actions["CastSpell4"];
        spellAction5 = playerInput.actions["CastSpell5"];

        interactAction = playerInput.actions["Interact"];

        Cursor.lockState = CursorLockMode.Locked;
        animator = GetComponent<Animator>();
        jumpAnimation = Animator.StringToHash("Jump");
        interactAnimation = Animator.StringToHash("Interact");
        moveXAnimationParameterId = Animator.StringToHash("MoveX");
        moveZAnimationParameterId = Animator.StringToHash("MoveZ");

     // calculate the correct vertical position:
     float correctHeight = controller.center.y + controller.skinWidth;
     // set the controller center vector:
     controller.center = new Vector3(0, correctHeight, 0);

    
       
    }

     void shoot(){

         RaycastHit hit;

         if(Physics.Raycast(cameraTransform.position, cameraTransform.forward, out hit, Mathf.Infinity)){
            Gun.target = hit;
            Gun.hit = true;
        }else{
            Gun.target = hit;
          //  Gun.target = cameraTransform.position + cameraTransform.forward * bulletHitMissDistance ;
            Gun.hit = false;
        }

            Gun.Shoot();
    }

    
    private void ShootGun(){
        //RaycastHit hit;
        GameObject bullet = GameObject.Instantiate(bulletPrefab, barrelTransform.position, Quaternion.identity, bulletParent);
        //BulletControler bulletControler = bullet.GetComponent<BulletControler>();

     

       /* if(Physics.Raycast(cameraTransform.position, cameraTransform.forward, out hit, Mathf.Infinity)){
            bulletControler.target = hit.point;
            bulletControler.hit = true;
        }else{
            bulletControler.target = cameraTransform.position + cameraTransform.forward * bulletHitMissDistance ;
            bulletControler.hit = false;
        }*/
    }


    private void OnEnable(){
        shootAction.performed += _ => shoot();

        aimAction.performed += _ => startAim("AimingRifle");
        aimAction.canceled += _ => stopAim("AimingRifle");

      
        spellAction1.performed += _ => castSpell1();
        spellAction1.canceled += _ => stopCast1();
        spellAction2.performed += _ => castSpell2();
        spellAction2.canceled += _ => stopCast2();
        spellAction3.performed += _ => castSpell3();
        spellAction3.canceled += _ => stopCast3();
        spellAction4.performed += _ => castSpell4();
        spellAction4.canceled += _ => stopCast4();
        spellAction5.performed += _ => castSpell5();
        spellAction5.canceled += _ => stopCast5();
      
    }
     private void OnDisable(){
        //shootAction.performed -= _ => ShootGun();
    }

    private void startAim(string type){

        
        animator.SetBool(type, true);
//        animator.CrossFade(Animator.StringToHash("Pistol Idle"),.1f);
    }
    private void stopAim(string type){
        animator.SetBool(type, false);
    }


   

    private void castSpell1(){

        Spell spellClon;
         animator.SetBool("Spell1", true);
         spellClon = Instantiate(spellToCast_1,castPoint.position, cameraTransform.rotation);
         //spellClon.GetComponent<Rigidbody>().AddRelativeForce(Vector3.forward * 500);
         //spellClon.GetComponent<Rigidbody>().velocity = 30 * cameraTransform.forward;
    }
    private void stopCast1(){
        currentCastTime = 0;
        animator.SetBool("Spell1", false);
    }
    private void castSpell2(){

        Spell spellClon;
         animator.SetBool("Spell1", true);
         spellClon = Instantiate(spellToCast_2,castPoint.position, cameraTransform.rotation);

        
    }
    private void stopCast2(){
        currentCastTime = 0;
        animator.SetBool("Spell1", false);
    }
    private void castSpell3(){
        Spell spellClon;
         animator.SetBool("Spell1", true);
        spellClon = Instantiate(spellToCast_2,castPoint.position, cameraTransform.rotation);

    }
    private void stopCast3(){
        currentCastTime = 0;
        animator.SetBool("Spell1", false);
    }
    private void castSpell4(){
         animator.SetBool("Spell1", true);
         Instantiate(spellToCast_4,castPoint.position,castPoint.rotation);
    }
    private void stopCast4(){
        currentCastTime = 0;
        animator.SetBool("Spell1", false);
    }
    private void castSpell5(){
         animator.SetBool("Spell1", true);
         Instantiate(spellToCast_5,castPoint.position,castPoint.rotation);
    }
    private void stopCast5(){
        currentCastTime = 0;
        animator.SetBool("Spell1", false);
    }
   




    private void Interaction(){

    }

      // DATA PLAYER
    [SerializeField] private Spell spellToCast_1; 
    [SerializeField] private Spell spellToCast_2; 
    [SerializeField] private Spell spellToCast_3; 
    [SerializeField] private Spell spellToCast_4; 
    [SerializeField] private Spell spellToCast_5; 

    [SerializeField] private float maxEnergy = 100f;
    [SerializeField] private float currentEnergy = 100f;
    [SerializeField] private float currentCastTime = .25f;
   // [SerializeField] private float rechargeRateEnergy = 0.2f;
   // [SerializeField] private int currentBloomies = 100;
    [SerializeField] private float timeBetweenCasts = 25f;
    [SerializeField] private Transform castPoint;
    private bool castingMagic;
    

    void CastSpell(){
       
    }

    void Update()
    {

        // CASTING
       /* bool isSpellCastHeldDown = false; //playerInput.actions["CastSpell"];
        if(!castingMagic && isSpellCastHeldDown){
            castingMagic = true;
            currentCastTime = 0;
            CastSpell();
        }
               
         if(castingMagic){
            currentCastTime += Time.deltaTime;
            if(currentCastTime > timeBetweenCasts) castingMagic = false;
         }   
         
*/

          // RAY EVERYTHING
        RaycastHit hit;
        if(Physics.Raycast(cameraTransform.position, cameraTransform.forward, out hit, Mathf.Infinity)){
             //
         /*     if(hit.collider.tag == "Interactive"){
                  
                  InteractiveController interactiveController = hit.transform.gameObject.GetComponent<InteractiveController>();
                 // Debug.Log(hit.collider.tag);
                  interactiveController.target = transform.position;
                  interactiveController.hit = true;

              }  */
        }else{
           
        }



        groundedPlayer = controller.isGrounded;
        if (groundedPlayer && playerVelocity.y < 0)
        {
            playerVelocity.y = 0f;
        }

        Vector2 Input = moveAction.ReadValue<Vector2>();
        currentAnimationBlendVector = Vector2.SmoothDamp(currentAnimationBlendVector, Input, ref animationVelocity, animationSmoothTime);

        Vector3 move = new Vector3(currentAnimationBlendVector.x, 0, currentAnimationBlendVector.y);
        move = move.x * cameraTransform.right.normalized + move.z * cameraTransform.forward.normalized ;
        move.y = 0f;
        controller.Move(move * Time.deltaTime * playerSpeed);

        //Debug.Log(currentAnimationBlendVector.x);

       animator.SetFloat(moveXAnimationParameterId, currentAnimationBlendVector.x);
       animator.SetFloat(moveZAnimationParameterId, currentAnimationBlendVector.y);

          if (jumpAction.triggered && groundedPlayer)
        {
            playerVelocity.y += Mathf.Sqrt(jumpHeight * -3.0f * gravityValue);
            animator.CrossFade(jumpAnimation,animationPlayTransition);
        }


        
      


        if (interactAction.triggered )
        {
            animator.CrossFade(interactAnimation,animationPlayTransition);
        }

        playerVelocity.y += gravityValue * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);

        //Rotate Player towards camera
       // float targetAngle = cameraTransform.eulerAngles.y;
        
        //Debug.Log(moveAction.ReadValue<Vector2>()[0]);
       // if(moveAction.ReadValue<Vector2>()[0] !=0 || moveAction.ReadValue<Vector2>()[1] !=0){

            Quaternion targetRotation = Quaternion.Euler(0,cameraTransform.eulerAngles.y,0);
           transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed * rotationSpeed * Time.deltaTime);

        //   }
      
      //  

    }
}