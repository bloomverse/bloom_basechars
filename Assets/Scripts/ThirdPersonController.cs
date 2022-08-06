using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Animations.Rigging;
using Cinemachine;
using DG.Tweening;
using RayFire;



using UnityEngine.UI;

	[RequireComponent(typeof(CharacterController))]
	[RequireComponent(typeof(PlayerInput))]

	public class ThirdPersonController : MonoBehaviour, IEffectable
	{

		[Header("Stats")]
		private int _maxHealth = 1000;
        [SerializeField] private int currentHealth;
		private StatusEffectData _data;
		private bool live = true;
		//damage popup
		[SerializeField] private Transform  pfDamagePopup;
		[SerializeField] private Transform  pfDamageSpawn;
		[SerializeField] private Slider  HealthBar;
		[SerializeField] private GameObject  HealthBarCanvas;

		[SerializeField] private ParticleSystem deadthParticles;
		[SerializeField] private Transform deadthParticlesSpawn;

		[Header("RayFire")]
		[SerializeField] private RayfireGun rayfireGun;
		[SerializeField] private Transform  rayfireTarget;

		[Header("Player")]
		[Tooltip("Move speed of the character in m/s")]
		public float MoveSpeed = 2.0f;

		[Header("Sprint")]
		[Tooltip("Sprint speed of the character in m/s")]
		public float SprintSpeed = 5.335f;

		[Header("Crouching")]
		[Tooltip("Crouch speed of the character in m/s")]
		public float crouchSpeed = 1.335f;
		public float crouchYScale;
		private float startYScale;
		
	
		[Tooltip("Acceleration and deceleration")]
		public float SpeedChangeRate = 10.0f;

		[Space(10)]
		[Tooltip("The height the player can jump")]
		public float JumpHeight = 1.2f;
		[Tooltip("The character uses its own gravity value. The engine default is -9.81f")]
		public float Gravity = -15.0f;

		[Space(10)]
		[Tooltip("Time required to pass before being able to jump again. Set to 0f to instantly jump again")]
		public float JumpTimeout = 0f;
		[Tooltip("Time required to pass before entering the fall state. Useful for walking down stairs")]
		public float FallTimeout = 0.15f;

		[Header("Player Grounded")]
		[Tooltip("If the character is grounded or not. Not part of the CharacterController built in grounded check")]
		public bool Grounded = true;
		[Tooltip("Useful for rough ground")]
		public float GroundedOffset = -0.14f;
		[Tooltip("The radius of the grounded check. Should match the radius of the CharacterController")]
		public float GroundedRadius = 0.28f;
		[Tooltip("What layers the character uses as ground")]
		public LayerMask GroundLayers;

		[Header("Cinemachine")]
		[Tooltip("The follow target set in the Cinemachine Virtual Camera that the camera will follow")]
		public GameObject CinemachineCameraTarget;
		[Tooltip("How far in degrees can you move the camera up")]
		public float TopClamp = 70.0f;
		[Tooltip("How far in degrees can you move the camera down")]
		public float BottomClamp = -30.0f;
		[Tooltip("Additional degress to override the camera. Useful for fine tuning camera position when locked")]
		public float CameraAngleOverride = 0.0f;
		[Tooltip("For locking the camera position on all axis")]
		public bool LockCameraPosition = false;

		// cinemachine
		private float _cinemachineTargetYaw;
		private float _cinemachineTargetPitch;
		private Transform cameraTransform;
	

		// player
		private PlayerInput playerInput;
		private float _speed;
		private float _animationBlend;
		private float _targetRotation = 0.0f;
		private float _rotationVelocity;
		private float _verticalVelocity;
		private float _terminalVelocity = 53.0f;

		// timeout deltatime
		private float _jumpTimeoutDelta;
		private float _fallTimeoutDelta;

		// animation IDs
		private int _animIDSpeed;
		private int _animIDGrounded;
		private int _animIDJump;
		private int _animIDFreeFall;
		private int _animIDMotionSpeed;

		private InputAction jumpAction;

		private Animator _animator;
		private CharacterController _controller;
		private bool groundedPlayer;
		private StarterAssetsInputs _input;
		private GameObject _mainCamera;
		

		private const float _threshold = 0.01f;

		private float combatTime = 3f;
		private float currentCombatTime = 0f;
		private bool combatStatus;
		private Vector3 playerVelocity;

		public LayerMask layerMask;

		


			// RIG
		[Space(10)]
		[Header("Rigs")]
		private float aimDuration = 0.2f;
	    public Rig  aimLayer;
		public Rig  aimHandsLayer;
		public Rig  weaponPoseLayer;
		public Rig  weaponBackLayer;
		
		public Rig  rightHandLayer;
		public Rig  leftHandLayer;
		

		public Transform aimObject;
		public Transform relativeYawObj;

		[SerializeField] private OverrideTransform WeaponRecoil;
		[SerializeField] private OverrideTransform BodyRecoil;

		public Rig sprintLayer;
		private float sprintDuration = 0.3f;

		[Header("Effects")]
		[SerializeField] private ParticleSystem Dash;
		[SerializeField] private GameObject DashTrail;
		[SerializeField] private GameObject DashLight;
		[SerializeField]
		private ParticleSystem HookSpeedAnim;


		private ParticleSystem generatedParticle;

		private bool _hasAnimator;

			//SPELLS


		// HOOK
		private InputAction hookAction;
		private InputAction crouchAction;

		private const float NORMAL_FOV = 40f;
		private const float SPRINT_FOV = 70f;
		private const float HOOKSHOT_FOV = 80f;
		float fovSpeed = 4f;
		private float targetFov;
        private float fov;
		public bool grapplingState;
		


		// DOUBLE JUMP
		private int jumpCount=0;

		[Header("Gun")]
		 [SerializeField]  private Gun Gun;


		 // Shake timer variables
		 private float shakeTimer;
		 private float shakeTimerTotal;
		 private float startingIntensity;
		 private CinemachineBasicMultiChannelPerlin cinemachineBasicMultiChannelPerlin;

		 Vector2 currentAnimationBlendVector ;
         Vector2 animationVelocity;
		int moveXAnimationParameterId;
    	int moveZAnimationParameterId;
		int jumpAnimation;

		 [SerializeField]
    	private float rotationSpeed = 5f;

		[SerializeField]
		private float animationSmoothTime = 0.1f;

		[SerializeField]
		private float animationPlayTransition = 0.10f;

		

		//[SerializeField] private Transform debugHitPoint;

		private State state;
		public Vector3 hookshotPosition;
		private enum State{
			Normal,
			HookshotFlyingPlayer,
			HookshotThrown,
			Disarmed,
			Inventory
		}
		private Vector3 characterMomentum;
		[SerializeField] private Transform hookshotTransform;
		private float hookshotSize;

		// FOOT CORRECT PLACEMENT ------------------------------------------------------------------------------------------------------------
		[Range(0,1f)]
		public float DistancetoGround;

		private void OnAnimatorIK(int layerIndex) {
			if(_animator){
				_animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot,_animator.GetFloat("IKLeftFootWeight"));
				_animator.SetIKRotationWeight(AvatarIKGoal.LeftFoot,_animator.GetFloat("IKLeftFootWeight"));

				_animator.SetIKPositionWeight(AvatarIKGoal.RightFoot,_animator.GetFloat("IKRightFootWeight"));
				_animator.SetIKRotationWeight(AvatarIKGoal.RightFoot,_animator.GetFloat("IKRightFootWeight"));
				//Debug.Log("Foot");
				RaycastHit hit;

				//left  foot
				Ray ray = new Ray(_animator.GetIKPosition(AvatarIKGoal.LeftFoot) + Vector3.up, Vector3.down);
				if(Physics.Raycast(ray, out hit,DistancetoGround + 1f, layerMask)){

					if(hit.transform.tag=="Walkable"){
						Vector3 footPosition = hit.point;
						footPosition.y  += DistancetoGround;
						_animator.SetIKPosition(AvatarIKGoal.LeftFoot,footPosition);
						_animator.SetIKRotation(AvatarIKGoal.LeftFoot,Quaternion.LookRotation(transform.forward,hit.normal));
					}
				}

				 ray = new Ray(_animator.GetIKPosition(AvatarIKGoal.RightFoot) + Vector3.up, Vector3.down);
				if(Physics.Raycast(ray, out hit,DistancetoGround + 1f, layerMask)){

					if(hit.transform.tag=="Walkable"){
						Vector3 footPosition = hit.point;
						footPosition.y  += DistancetoGround;
						_animator.SetIKPosition(AvatarIKGoal.RightFoot,footPosition);
						_animator.SetIKRotation(AvatarIKGoal.RightFoot,Quaternion.LookRotation(transform.forward,hit.normal));
					}
				}

			}	
		}

		private void Awake()
		{
			// get a reference to our main camera
			if (_mainCamera == null)
			{
				_mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
				cameraTransform = Camera.main.transform;
				//cameraFov = _mainCamera.GetComponent<CameraFOV>();
			}

			moveXAnimationParameterId = Animator.StringToHash("MoveX");
        	moveZAnimationParameterId = Animator.StringToHash("MoveZ");
			//jumpAnimation = Animator.StringToHash("Jump");
			
			playerInput = GetComponent<PlayerInput>();
			  hookAction = playerInput.actions["MiddleClick"];
			  crouchAction = playerInput.actions["Ctrl"];

			  hookshotTransform.gameObject.SetActive(false);
			 state = State.Normal;  //State.Disarmed;
			 fov = NORMAL_FOV;
			 targetFov = fov;
			 weaponBackLayer.weight = 0;

		}
		
		private void OnEnable(){
			
			hookAction.performed += _ => HandleHookshotStart();
			hookAction.canceled += _ => CancelHookshotStart();

			//crouchAction.performed += _ => HandleHookshotStart();
			//hookAction.canceled += _ => CancelHookshotStart();
			
      
    	}

		private void Start()
		{
			_hasAnimator = TryGetComponent(out _animator);
			_controller = GetComponent<CharacterController>();
			_input = GetComponent<StarterAssetsInputs>();

			AssignAnimationIDs();

			generatedParticle = Dash.GetComponent<ParticleSystem>();

			// reset our timeouts on start
			_jumpTimeoutDelta = JumpTimeout;
			_fallTimeoutDelta = FallTimeout;

			Cursor.lockState = CursorLockMode.Locked;
			Cursor.visible = false;
	

		}

		private void Update()
		{




			CheckObjectsI();
			
			switch (state){
				default:
				case State.Normal:
					JumpAndGravity();
					GroundedCheck();
					CombatCheck();
					Move();
					controlView();
					break;
				case State.HookshotThrown:
					HandleHookshotThrown();
					//GroundedCheck();
					JumpAndGravity();
					controlView();
					break;	
				case State.HookshotFlyingPlayer:
				    //GroundedCheck();
					JumpAndGravity();
					HandleHookshotMovement();
					controlView();
					break;
				case State.Disarmed:
					controlView();
					Move();
					JumpAndGravity();
					GroundedCheck();
					
					break;
				case State.Inventory:
					break;	


			}
			
			
		
		}

		// Check raycast objects in level
		private void CheckObjectsI(){
		
			/*if(_input.inventoryStatus){
			//	Debug.Log("Inventory Click");
				InventoryManager.instance.EnterInventory();
				PPManager.instance.setDOF(1.5f);
				state = State.Inventory;
				
			}else{
				//Debug.Log("lala");
				InventoryManager.instance.ExitInventory();
				PPManager.instance.setDOF(15f);
				state = State.Normal;
				
			}*/

			RaycastHit hit;
			int layerMask = 1 << 9;
			//layerMask = ~layerMask;

			if(Physics.Raycast(cameraTransform.position, cameraTransform.forward, out hit, Mathf.Infinity, layerMask,  QueryTriggerInteraction.Collide)){
					var obj = hit.collider.gameObject.GetComponent<IActivable>();
					if(obj!=null){
						obj.Activate();
					}
			}else{
				
			}

		

		}


		private void CombatCheck(){


			if(_input.spell1Status){
						//castSpell1();
						combatStatus = true;
					    rightHandLayer .weight = 0;
						int throwAnimation = Animator.StringToHash("Throw");
					  GetComponent<Animator>().CrossFade(throwAnimation,0.65f);
						DOTween.To(()=>1,(u)=>rightHandLayer.weight=u,0,.6f).OnComplete(()=>{
						castNade1();
						  DOTween.To(()=>1,(u)=>rightHandLayer.weight=u,1,.8f).SetDelay(.7f).OnComplete(()=>{
					  	});
					  });
						
						combatStatus = true;
			}

			if(_input.spell2Status){
				combatStatus = true;
					    rightHandLayer .weight = 0;
						int throwAnimation = Animator.StringToHash("Throw");
					  GetComponent<Animator>().CrossFade(throwAnimation,0.65f);
						 DOTween.To(()=>1,(u)=>rightHandLayer.weight=u,0,.6f).OnComplete(()=>{
						castMultiRocket();
						  DOTween.To(()=>1,(u)=>rightHandLayer.weight=u,1,.8f).SetDelay(.7f).OnComplete(()=>{
					  	});
					  });
						
						combatStatus = true;
			}
			if(_input.spell3Status){
						
						combatStatus = true;
					    rightHandLayer .weight = 0;
						int throwAnimation = Animator.StringToHash("Throw");
					  GetComponent<Animator>().CrossFade(throwAnimation,0.65f);
					  DOTween.To(()=>1,(u)=>rightHandLayer.weight=u,0,.6f).OnComplete(()=>{
						castMine();
						  DOTween.To(()=>1,(u)=>rightHandLayer.weight=u,1,.8f).SetDelay(.7f).OnComplete(()=>{
					  	});
					  });

					  
					  
			}
			if(_input.spell4Status){
						castUltimate();
						combatStatus = true;
			}else{
				stopUltimate();
			}

			if(_input.spell5Status){
					 disarm();
					 combatStatus = false;
					 //int macarenaAnimation = Animator.StringToHash("Macarena");
					 // GetComponent<Animator>().CrossFade(macarenaAnimation,0.15f);
			}

			if(_input.leftStatus || _input.rightStatus){		
					combatStatus = true;
					
					if(_input.leftStatus){
						shoot();
					}else{
						//timePressed = 0;
						
					}

				
					
					
			}

			if(currentCombatTime<combatTime && combatStatus ){		
				currentCombatTime += Time.deltaTime;
			}
			else{
					combatStatus = false;
					currentCombatTime = 0;
				//	Debug.Log("Exit Combat Mode");
					
			}

			
			
			
		}


		public void macarenaEnd(){
			
		}
	
	
		void shoot(){

        RaycastHit hit;
		int layerMask = 1 << 3;
		layerMask = ~layerMask;

         if(Physics.Raycast(cameraTransform.position, cameraTransform.forward, out hit, Mathf.Infinity, layerMask)){
//			 Debug.Log(hit.collider.gameObject.name);
            Gun.target = hit;

			var rayFireColl = hit.collider.gameObject.GetComponent<Rigidbody>();

			if(rayFireColl){
				rayfireTarget.position = hit.point;
				rayfireGun.Shoot();
            
			}
			
			
			
			Gun.hit = true;
			shakeCamera(0.4f,0.6f);
        }else{
            Gun.target = hit;
          //  Gun.target = cameraTransform.position + cameraTransform.forward * bulletHitMissDistance ;
            Gun.hit = false;
        }
            Gun.Shoot();
			GenerateRecoil();
    	}

		private bool effectUlt;
		[SerializeField] private GameObject ultimateEffect;
		
		void castUltimate(){
		
       RaycastHit hit;
		int layerMask = 1 << 3;
		layerMask = ~layerMask;

	if(!effectUlt){
		GameObject ultPrefab = Instantiate(ultimateEffect,  castPoint.position, castPoint.rotation , castPoint) as GameObject;
        ultPrefab.transform.localScale = new Vector3(.2f, .2f, .7f);
        effectUlt = true; 

	}

        if(Physics.Raycast(cameraTransform.position, cameraTransform.forward, out hit, Mathf.Infinity, layerMask)){


          	Ultimate.target = hit;
            Ultimate.hit = true;
			shakeCamera(0.4f,0.6f);
        }else{
            Ultimate.target = hit;
            Ultimate.hit = false;
        }
     		 Ultimate.Shoot();
			 combatStatus = true;

			
    	}

		void stopUltimate(){
			Ultimate.stop();
		}

           

		// RECOIL SYSTEM
		  public float verticalRecoilMin = 0.1f;
		  public float verticalRecoilMax = 0.2f;
		  public float horizontalRecoilMin = -0.14f;
		  public float horizontalRecoilMax = 0.14f;

		private void GenerateRecoil(){
			var brain = CinemachineCore.Instance.GetActiveBrain(0);
			var Vcam = brain.ActiveVirtualCamera.VirtualCameraGameObject.GetComponent<CinemachineVirtualCamera>();

			if(Vcam!=null ){


			CinemachinePOV pov = Vcam.GetCinemachineComponent<CinemachinePOV>();
			
			try {
				pov.m_VerticalAxis.Value -= Random.Range(verticalRecoilMin,verticalRecoilMax);
			pov.m_HorizontalAxis.Value -= Random.Range(horizontalRecoilMin,horizontalRecoilMax);
			/*DOTween.To(() =>0, (u) => WeaponRecoil.weight = u,1f,.1f).OnComplete(() =>{
					DOTween.To(() => 1 , (u) => WeaponRecoil.weight = u ,0f,.1f);
			});*/
			DOTween.To(() =>0, (u) => BodyRecoil.weight = u,1f,.1f).OnComplete(() =>{
					DOTween.To(() => 1 , (u) => BodyRecoil.weight = u ,0f,.1f);
			});
			} catch{}
			


			//DOTween.To(currBodyRecoil,currBodyRecoil,1,.3f).SetLoops(1,LoopType.Yoyo);
			
			
			//WeaponRecoil.weight = currWeaponRecoil;
			//BodyRecoil.weight = currBodyRecoil ;
			}
		

		}


		private void AssignAnimationIDs()
		{
			_animIDSpeed = Animator.StringToHash("Speed");
			_animIDGrounded = Animator.StringToHash("Grounded");
			_animIDJump = Animator.StringToHash("Jump");
			_animIDFreeFall = Animator.StringToHash("FreeFall");
			_animIDMotionSpeed = Animator.StringToHash("MotionSpeed");
		}

		private void GroundedCheck()
		{
			// set sphere position, with offset
			Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z);
			Grounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers, QueryTriggerInteraction.Ignore);

			// update animator if using character
			if (_hasAnimator)
			{
				_animator.SetBool(_animIDGrounded, Grounded);
			}
		}

		
		private void shakeCamera(float intensity, float time){


			
			var brain = CinemachineCore.Instance.GetActiveBrain(0);
 			var Vcam = brain.ActiveVirtualCamera.VirtualCameraGameObject.GetComponent<CinemachineVirtualCamera>();

			if(Vcam!=null){
			cinemachineBasicMultiChannelPerlin = Vcam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
			cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = intensity;
			cinemachineBasicMultiChannelPerlin.m_FrequencyGain = 23f;
			startingIntensity = intensity;
			shakeTimerTotal = time;
			shakeTimer = time;
			} 
		}
		
		private void arm(){
			state = State.Normal;
			aimLayer.weight = 1;
			 rightHandLayer.weight = 1;
			 leftHandLayer.weight = 1;
			 
			//weaponBackLayer.weight = 1;
			weaponPoseLayer.weight = 1;
		}

		private void disarm(){
			state = State.Disarmed;
			aimLayer.weight = 0;
			 leftHandLayer.weight = 0;
			 rightHandLayer.weight = 0;
			weaponBackLayer.weight = 1;
			weaponPoseLayer.weight = 0;
		}

		private void Move()
		{

			// Camera shake smoother
			if(shakeTimer > 0){
				shakeTimer -= Time.deltaTime;
				cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = Mathf.Lerp(startingIntensity,.5f, 0.5f - (shakeTimer / shakeTimerTotal));
				
			} else{

					try{
						cinemachineBasicMultiChannelPerlin.m_FrequencyGain = .5f;
					}catch{

					}

				
			}

			// Aim rigging animation
			if(combatStatus){
				aimLayer.weight += Time.deltaTime / aimDuration;	
			}else{
				aimLayer.weight -= Time.deltaTime / aimDuration;	
			}
			//float yawCamera = _mainCamera.transform.rotation.eulerAngles.y;
			
			aimObject.position = new Vector3(aimObject.position.x,relativeYawObj.position.y,aimObject.position.z);
			//Quaternion.Euler(new Vector3(0f,yawCamera,0f));
			//

			//
			if(_input.sprint){
				//Dash.Stop();
			 	 //generatedParticle.enableEmission = true;
				 sprintLayer.weight += Time.deltaTime / sprintDuration;	
				 DashTrail.GetComponent<TrailRenderer>().emitting =true; 
				 DashLight.GetComponent<Light>().intensity = .3f;
				 SetCameraFov(SPRINT_FOV);
				 
			}else{
				//generatedParticle.enableEmission = false;
				sprintLayer.weight -= Time.deltaTime / sprintDuration;	
				DashTrail.GetComponent<TrailRenderer>().emitting=false; 
				DashLight.GetComponent<Light>().intensity = 0;
				SetCameraFov(NORMAL_FOV);
			}
			// set target speed based on move speed, sprint speed and if sprint is pressed
			float targetSpeed = _input.sprint ? SprintSpeed : MoveSpeed;

		
        Vector2 Input = _input.move;
        currentAnimationBlendVector = Vector2.SmoothDamp(currentAnimationBlendVector, Input, ref animationVelocity, animationSmoothTime);


        Vector3 move = new Vector3(currentAnimationBlendVector.x, 0, currentAnimationBlendVector.y);
        move = move.x * cameraTransform.right.normalized + move.z * cameraTransform.forward.normalized ;
        move.y = _verticalVelocity;

		// Apply Momentum
	    move += characterMomentum;

        _controller.Move(move * Time.deltaTime * targetSpeed);

		// Dampen Momentum
		if(characterMomentum.magnitude > 0f){
			float momentumDrag = 3f;
			characterMomentum -= characterMomentum * momentumDrag * Time.deltaTime;
			if(characterMomentum.magnitude < 0f){
				characterMomentum = Vector3.zero;
			}
		}

       _animator.SetFloat(moveXAnimationParameterId, currentAnimationBlendVector.x);
       _animator.SetFloat(moveZAnimationParameterId, currentAnimationBlendVector.y);

    
		}

		private void controlView(){

		// Set FOV
///////////////// OPTIMIZAR
		 var brain = CinemachineCore.Instance.GetActiveBrain(0);
  	     var Vcam = brain.ActiveVirtualCamera.VirtualCameraGameObject.GetComponent<CinemachineVirtualCamera>();


			fov = Mathf.Lerp(fov,targetFov, Time.deltaTime * fovSpeed) ;
        	Vcam.m_Lens.FieldOfView = fov;

			//Debug.Log(_input.move.y + " CAMERA SPEED " );
			if(combatStatus || _input.move.y > 0f || _input.move.x > 0f){
				Quaternion targetRotation = Quaternion.Euler(0,cameraTransform.eulerAngles.y,0);
        	transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime ); //
			}
			 


		}

		private void JumpAndGravity()
		{
			if (Grounded){
				// reset the fall timeout timer
				_fallTimeoutDelta = FallTimeout;
				// update animator if using character
				if (_hasAnimator){
					_animator.SetBool(_animIDJump, false);
					_animator.SetBool(_animIDFreeFall, false);
				}
				// stop our velocity dropping infinitely when grounded
				if (_verticalVelocity < 0.0f)
				{
					_verticalVelocity = -2f;
				}
				// Jump
				if (_input.jump ) //&& _jumpTimeoutDelta <= 0.0f
				{
					// the square root of H * -2 * G = how much velocity needed to reach desired height
					_verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity);
					// update animator if using character
					if (_hasAnimator){
						//Debug.Log("enviando animcion salto");
						_animator.SetBool(_animIDJump, true);
					}
				}
				// jump timeout
				if (_jumpTimeoutDelta >= 0.0f){
					_jumpTimeoutDelta -= Time.deltaTime;
				}
				jumpCount++;
			}
			else{
				// reset the jump timeout timer
				_jumpTimeoutDelta = JumpTimeout;
				// fall timeout
				if (_fallTimeoutDelta >= 0.0f){
					_fallTimeoutDelta -= Time.deltaTime;
				}
				else{
					// update animator if using character
					if (_hasAnimator){
						_animator.SetBool(_animIDFreeFall, true);
					}
				}

				// if we are not grounded, do not jump
				_input.jump = false;
				jumpCount=0;
			}
			// apply gravity over time if under terminal (multiply by delta time twice to linearly speed up over time)
			if (_verticalVelocity < _terminalVelocity)
			{
				_verticalVelocity += Gravity * Time.deltaTime;
			}
		}


	private void HandleHookshotStart(){
		  RaycastHit hit;
		  Debug.Log("trro");
		int layerMask = 1 << 3;
		layerMask = ~layerMask;

         if(Physics.Raycast(cameraTransform.position, cameraTransform.forward, out hit, Mathf.Infinity, layerMask)){
			 	//debugHitPoint.position = hit.point;
				hookshotPosition = hit.point;
				hookshotSize = 0f;
				hookshotTransform.gameObject.SetActive(true);
				hookshotTransform.localScale = Vector3.zero;
				state = State.HookshotThrown;
				grapplingState = true;
	
        }else{
            
        }
	}

	 public Vector3 GetGrapplePoint() {
        return hookshotPosition;
    }
	public bool IsGrappling() {
        return grapplingState;
    }
	

	private void HandleHookshotThrown(){
		hookshotTransform.LookAt(hookshotPosition);
		float hookshotThrownSpeed = 40f;
		hookshotSize += hookshotThrownSpeed * Time.deltaTime;
		hookshotTransform.localScale = new Vector3(1,1,hookshotSize);
		//Debug.Log("pasando trw" + hookshotSize);
		if(hookshotSize >= Vector3.Distance(transform.position,hookshotPosition)){
			state = State.HookshotFlyingPlayer;
			SetCameraFov(HOOKSHOT_FOV);
			HookSpeedAnim.Play();
			grapplingState = true;

		}

	}

	private void CancelHookshotStart(){
		state = State.Normal;
		grapplingState = false;
		ResetGravityEffect();
		hookshotTransform.gameObject.SetActive(false);
	}

	private void HandleHookshotMovement(){
		hookshotTransform.LookAt(hookshotPosition);
		Vector3 hookshotDir = (hookshotPosition - transform.position).normalized;

		float hookshotSpeedMin = 10f;
		float hookshotSpeedMax = 40f;
		float hookshotSpeed = Mathf.Clamp(Vector3.Distance(transform.position, hookshotPosition),hookshotSpeedMin, hookshotSpeedMax);
		float hookshotSpeedMultiplier = 2f;

		_controller.Move(hookshotDir * hookshotSpeed * hookshotSpeedMultiplier *  Time.deltaTime);
		// ANIM
		//_animator.SetBool(_animIDFreeFall, true);

		float reachedHookDistance = 1f;
		if(Vector3.Distance(transform.position,hookshotPosition)<reachedHookDistance){
			 stopHookShot();
		}

		if(_input.jump){
			float momentumExtraSpeed = .4f;
		    characterMomentum = hookshotDir * hookshotSpeed *  momentumExtraSpeed;
			characterMomentum += Vector3.up * 2f;
			stopHookShot();
		}
		
	}

	private void stopHookShot(){
			state = State.Normal;
			ResetGravityEffect();
			hookshotTransform.gameObject.SetActive(false);
			SetCameraFov(NORMAL_FOV);
			HookSpeedAnim.Stop();
			//.SetBool(_animIDFreeFall, false);
	}

	private void ResetGravityEffect(){
		_verticalVelocity = 0f;
	}


	// Spell Casting 
	[SerializeField] private Spell spellToCast_1; 
	[SerializeField] private Spell spellToCast_2; 
	[SerializeField] private Spell spellToCast_3; 
	[SerializeField] private Spell spellToCast_4; 
	[SerializeField] private Ultimate_CoreBeam Ultimate; 
	[SerializeField] private Transform castPoint;

	private float LastCastSpell_1;
	private float ShootDelay_1 = 0.5f;

	private void castSpell1(){
		
		  if (LastCastSpell_1 + ShootDelay_1 < Time.time)
        {

        Spell spellClon;
         _animator.SetBool("Spell1", true);
         spellClon = Instantiate(spellToCast_1,castPoint.position, cameraTransform.rotation);

		 LastCastSpell_1 = Time.time;
         //spellClon.GetComponent<Rigidbody>().AddRelativeForce(Vector3.forward * 500);
         //spellClon.GetComponent<Rigidbody>().velocity = 30 * cameraTransform.forward;
		 }
    }


	
	public GameObject grenadePrefab;
	private float LastCastSpell_2;
	private float ShootDelay_2 = 1f;

	private void castNade1(){

		  if (LastCastSpell_2 + ShootDelay_2 < Time.time)
        {
	
		
		GameObject grenade = Instantiate(grenadePrefab,castPoint.position, cameraTransform.rotation);
		//Rigidbody rb = grenade.GetComponent<Rigidbody>();
		//rb.AddForce(cameraTransform.forward * throwForce, ForceMode.Impulse);

		LastCastSpell_2 = Time.time;

		}
    }

	// MINE ------------------------------------------------
	public GameObject minePrefab;
	private float LastCastSpell_4;
	private float ShootDelay_4 = 1f;

	private void castMine(){

		  if (LastCastSpell_4 + ShootDelay_4 < Time.time)
        {

			
				 RaycastHit hit;
			int layerMask = 1 << 3;
			layerMask = ~layerMask;
			
			Debug.Log("Entrando");
		 if(Physics.Raycast(cameraTransform.position, cameraTransform.forward, out hit, Mathf.Infinity, layerMask)){
			 GameObject	mine = Instantiate(minePrefab,castPoint.position, cameraTransform.rotation);
			    Mine mineScript = mine.GetComponent<Mine>();
				mineScript.target = hit;
		 		mineScript.StartMovement();
				 LastCastSpell_4 = Time.time;
		 }else{
            
            //mine.target = cameraTransform.position + cameraTransform.forward * 100 ;
            //mine.hit = false;
        }

		
		//Rigidbody rb = grenade.GetComponent<Rigidbody>();
		//rb.AddForce(cameraTransform.forward * throwForce, ForceMode.Impulse);

		

		}
    }

	// MultiRocket --------------------------------------------------------------------------------
	
	public GameObject multiPrefab;
	private float LastCastSpell_3;
	private float ShootDelay_3 = 1f;
	
	private void castMultiRocket(){

		  if (LastCastSpell_3 + ShootDelay_3 < Time.time)
        {
		
		GameObject multiR = Instantiate(multiPrefab,castPoint.position, cameraTransform.rotation);
		//Rigidbody rb = grenade.GetComponent<Rigidbody>();
		//rb.AddForce(cameraTransform.forward * throwForce, ForceMode.Impulse);
		LastCastSpell_3 = Time.time;

		}
    }


	  public void SetCameraFov(float targetFov){
        this.targetFov = targetFov;
    }


	// DAMAGE ------------------------------------------------------------------------------------------------------------
	private GameObject _effectParticles;
    public void ApplyEffect(StatusEffectData _data)
    {
        
        RemoveEffect();
        this._data = _data;
      /*  if (_data.MovementPenalty > 0) _currentMoveSpeed = _moveSpeed / _data.MovementPenalty;
//        Debug.Log(_data.hitpoint);
       _effectParticles = Instantiate(_data.EffectParticles,  _data.hitpoint.point +  _data.hitpoint.normal*0.1f, Quaternion.LookRotation(_data.hitpoint.normal) );
	   */
    }

    private float _currentEffectTime = 0f;
    private float _nextTickTime = 0f;
    public void RemoveEffect() 
    {
        _data = null;
        _currentEffectTime = 0;
        _nextTickTime = 0;
      //  _currentMoveSpeed = _moveSpeed;
        if (_effectParticles != null) Destroy(_effectParticles);
    }

    
    public void HandleEffect()
    {
        _currentEffectTime += Time.deltaTime;
        
        if (_currentEffectTime >= _data.Lifetime) RemoveEffect();

        if (_data == null) return;
        
        if (_data.DOTAmount != 0 && _currentEffectTime > _nextTickTime)
        {
            _nextTickTime += _data.TickSpeed;
            currentHealth -= _data.DOTAmount;
            bool isCriticalHit = Random.Range(0,100) < 30;
            damagePop(_data.DOTAmount,isCriticalHit);

            // Damage Animation
            int damageAnimation = Animator.StringToHash("KidneyHit");
            _animator.CrossFade(damageAnimation,0.15f);


            currentHealth = Mathf.Clamp(currentHealth, 0, _maxHealth);

           HealthBar.value = currentHealth;
           
           if(currentHealth<=0){
               live = false;
               die();
               ArenaManager.instance.StartCoroutine(ArenaManager.instance.SlowMotionSequence());
               //HealthBar.setEnabled(false);
               
               //Instantiate(deadthParticles, deadthParticlesSpawn.position,deadthParticlesSpawn.rotation);
               //Destroy(gameObject);
           }
        }
    }

    void die(){
        //
        int dieAnimation = Animator.StringToHash("Die");
        _animator.CrossFade(dieAnimation,0.15f);
        HealthBarCanvas.SetActive(false);
       
        //var capsuleCollider = GetComponent<CapsuleCollider>(); 

        //capsuleCollider.height = .2f;
        //capsuleCollider.radius = 0.2f;
        //setRigidBodyState(false);
        //setColliderState(true);
        //GetComponent<Animator>().enabled = false;
        
    }
	 private void damagePop(int damage, bool isCriticalHit){
        //var look = _data.hitpoint.normal*-1;
        //look.y = 0.01f;

       // var brain = CinemachineCore.Instance.GetActiveBrain(0);
        //var Vcam = brain.ActiveVirtualCamera.VirtualCameraGameObject.GetComponent<CinemachineVirtualCamera>();
        
    //+  _data.hitpoint.normal*0.1f
        Transform damagePopupTransform = Instantiate(pfDamagePopup, pfDamageSpawn.position , Camera.main.transform.rotation);
        DamagePopup damagePopup = damagePopupTransform.GetComponent<DamagePopup>();
        damagePopup.Setup(damage,isCriticalHit);
    }
   

}