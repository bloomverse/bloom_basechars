using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Animations.Rigging;
using Cinemachine;
using DG.Tweening;
using RayFire;
using Fusion;
using UnityEngine.UI;

	[OrderBefore(typeof(NetworkTransform))]

	public class ThirdPersonController :  NetworkTransform
	{
		public float gravity       = -20.0f;
		public float acceleration  = 10.0f;
		public float braking       = 10.0f;
		public float maxSpeed      = 2.0f;
		public float viewUpDownRotationSpeed = 50.0f;

		[Tooltip("Useful for rough ground")]
		public float GroundedOffset = -0.14f;
		[Tooltip("The radius of the grounded check. Should match the radius of the CharacterController")]
		public float GroundedRadius = 0.28f;
		[Tooltip("What layers the character uses as ground")]
		public LayerMask GroundLayers;

		[Header("Sprint")]
		[Tooltip("Sprint speed of the character in m/s")]
		public float SprintSpeed = 5.335f;
		
		[Networked]
		[HideInInspector]
		public bool IsGrounded { get; set; }

		[Networked]
		[HideInInspector]
		public Vector3 Velocity { get; set; }

		protected override Vector3 DefaultTeleportInterpolationVelocity => Velocity;
		protected override Vector3 DefaultTeleportInterpolationAngularVelocity => new Vector3(0f, 0f, rotationSpeed);

  		public CharacterController Controller { get; private set; }

		[Header("RayFire")]
		[SerializeField] private RayfireGun rayfireGun;
		[SerializeField] private Transform  rayfireTarget;

		// cinemachine
		private Transform cameraTransform;

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

		
		private NetworkInputData networkInputData;

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
		[SerializeField] private ParticleSystem HookSpeedAnim;
		[SerializeField] private ParticleSystem RocketParticles;


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


			// player
		private PlayerInput playerInput;
		private float _speed;
		private float _animationBlend;
		private float _targetRotation = 0.0f;
		private float _rotationVelocity;
		private float _verticalVelocity;
		private float _terminalVelocity = 53.0f;

		// DOUBLE JUMP
		private int jumpCount=0;

		
		 Vector2 currentAnimationBlendVector ;
         Vector2 animationVelocity;
		int moveXAnimationParameterId;
    	int moveZAnimationParameterId;
		int jumpAnimation;

		 [SerializeField]
    	public float rotationSpeed = 5f;

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

	/* IK FLOOR
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
		*/

		private void Awake()
		{
			base.Awake();
    		CacheController();
			// get a reference to our main camera
			if (_mainCamera == null)
			{
				_mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
				//cameraTransform = Camera.main.transform;
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

		
  public override void Spawned() {
    base.Spawned();
    CacheController();

    // Caveat: this is needed to initialize the Controller's state and avoid unwanted spikes in its perceived velocity
    Controller.Move(transform.position);
  }

  private void CacheController() {
    if (Controller == null) {
      Controller = GetComponent<CharacterController>();

      Assert.Check(Controller != null, $"An object with {nameof(ThirdPersonController)} must also have a {nameof(CharacterController)} component.");
    }
  }

  protected override void CopyFromBufferToEngine() {
    // Trick: CC must be disabled before resetting the transform state
    Controller.enabled = false;
    // Pull base (NetworkTransform) state from networked data buffer
    base.CopyFromBufferToEngine();
    // Re-enable CC
    Controller.enabled = true;
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

		private void AssignAnimationIDs()
	{
		_animIDSpeed = Animator.StringToHash("Speed");
		_animIDGrounded = Animator.StringToHash("Grounded");
		_animIDJump = Animator.StringToHash("Jump");
		_animIDFreeFall = Animator.StringToHash("FreeFall");
		_animIDMotionSpeed = Animator.StringToHash("MotionSpeed");
	}

		public void updateController()
		{

  if (Object.HasStateAuthority)
        {
			//CheckObjectsI();
			
			switch (state){
				default:
				case State.Normal:
					//JumpAndGravity();
					GroundedCheck();
					CombatCheck();
					//Move();
					controlView();
					break;
				case State.HookshotThrown:
					HandleHookshotThrown();
					//GroundedCheck();
					//JumpAndGravity();
					//controlView();
					break;	
				case State.HookshotFlyingPlayer:
				    //GroundedCheck();
					//JumpAndGravity();
					HandleHookshotMovement();
					//controlView();
					break;
				case State.Disarmed:
					//controlView();
					//Move();
					//JumpAndGravity();
					//GroundedCheck();
					
					break;
				case State.Inventory:
					break;	


			}
			
		}
		
		}


	private void controlView(){

		// Set FOV
		///////////////// OPTIMIZAR
		 var brain = CinemachineCore.Instance.GetActiveBrain(0);
  	     var Vcam = brain.ActiveVirtualCamera.VirtualCameraGameObject.GetComponent<CinemachineVirtualCamera>();
		 fov = Mathf.Lerp(fov,targetFov, Time.deltaTime * fovSpeed) ;
         Vcam.m_Lens.FieldOfView = fov;

			//Debug.Log(_input.move.y + " CAMERA SPEED " );
			/*if(combatStatus || _input.move.y > 0f || _input.move.x > 0f){
				Quaternion targetRotation = Quaternion.Euler(0,cameraTransform.eulerAngles.y,0);
        	transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime ); //
			}*/
			
		}

	public void GroundedCheck(){

		// set sphere position, with offset
			Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z);
			IsGrounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers, QueryTriggerInteraction.Ignore);

		_animator.SetBool(_animIDGrounded, IsGrounded);

		if (IsGrounded) {
			// reset the fall timeout timer
			_fallTimeoutDelta = FallTimeout;
			RocketParticles.Stop();
			
			_animator.SetBool(_animIDJump, false);
			_animator.SetBool(_animIDFreeFall, false);

			// jump timeout
				if (_jumpTimeoutDelta >= 0.0f){
					_jumpTimeoutDelta -= Time.deltaTime;
				}
			
		}else{

			// fall timeout
				_jumpTimeoutDelta = JumpTimeout;
				if (_fallTimeoutDelta >= 0.0f){
					_fallTimeoutDelta -= Time.deltaTime;
				}
				else{
					// update animator if using character
					if (_hasAnimator){
						_animator.SetBool(_animIDFreeFall, true);
					}
				}
		}

		if(currentGas<gasoline && IsGrounded){
			StartCoroutine(replenishGasoline());
		}  

		currentAnimationBlendVector = Vector2.SmoothDamp(currentAnimationBlendVector, _input.move, ref animationVelocity, animationSmoothTime);

		_animator.SetFloat(moveXAnimationParameterId, currentAnimationBlendVector.x);
		_animator.SetFloat(moveZAnimationParameterId, currentAnimationBlendVector.y);


		// Status animation
		if(combatStatus){
				aimLayer.weight += Time.deltaTime / aimDuration;	
			}else{
				aimLayer.weight -= Time.deltaTime / aimDuration;	
			}

			aimObject.position = new Vector3(aimObject.position.x,relativeYawObj.position.y,aimObject.position.z);

	}	

	private float jumpImpulse   = 5.0f;
	private float RocketImpulse = .5f;
	public bool candoublejump= false;
	public float gasoline = 10f;
	private float currentGas = 10f;
	private float gasDeploySpeed = 0.1f;
	private float gasRecoverySpeed = 0.5f;
	private float jumpDoubleTimer= 0.3f;
	private float jumpTimer = 0f;
	private float JumpTimeout = 0f;
	[Tooltip("Time required to pass before entering the fall state. Useful for walking down stairs")]
		public float FallTimeout = 0.15f;


	public virtual void Jump(float? overrideImpulse = null) {

		if (IsGrounded ) {
			var newVel = Velocity;
			newVel.y += jumpImpulse;
			Velocity =  newVel;
			_animator.SetBool(_animIDJump, true);
			SetCameraFov(NORMAL_FOV);
		}

		if(!IsGrounded && currentGas>0){
			
			var newVel = Velocity;
			newVel.y +=  RocketImpulse;
			currentGas -= gasDeploySpeed;
			Velocity =  newVel;
			SetCameraFov(SPRINT_FOV);
			RocketParticles.Play();
			UIController.instance.setRocketGasolineValue(  (currentGas / gasoline) * 100);
		
		}

		

	}

	IEnumerator replenishGasoline(){
		while(currentGas<gasoline){
			currentGas += gasRecoverySpeed;
			//Debug.Log(currentGas + "Current gas");
			UIController.instance.setRocketGasolineValue(  (currentGas / gasoline) * 100);
			yield  return new WaitForSeconds(1f);
			
		}

		if(currentGas>=gasoline){
			//currentGas = gasoline;
			//UIController.instance.setRocketGasolineValue(100);
		}

	}

	public virtual void Move(Vector3 direction) {
		var deltaTime    = Runner.DeltaTime;
		var previousPos  = transform.position;
		var moveVelocity = Velocity;

		direction = direction.normalized;

		moveVelocity.y += gravity * Runner.DeltaTime;
		

		if(moveVelocity.y < 0) {
				moveVelocity.y = 0f;
			}

		var horizontalVel = default(Vector3);
		horizontalVel.x = moveVelocity.x;
		horizontalVel.z = moveVelocity.z;


		float targetSpeed = _input.sprint ? SprintSpeed  : maxSpeed;

		if (direction == default) {
		horizontalVel = Vector3.Lerp(horizontalVel, default, braking * deltaTime);
		} else {
		horizontalVel      = Vector3.ClampMagnitude(horizontalVel + direction * acceleration * deltaTime, targetSpeed);
		}

		moveVelocity.x = horizontalVel.x;
		moveVelocity.z = horizontalVel.z;

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


		

		Controller.Move(moveVelocity * deltaTime);

		Velocity   = (transform.position - previousPos) * Runner.Simulation.Config.TickRate;

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


		public void CombatCheck(){


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
						//shoot();
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
			//shakeCamera(0.4f,0.6f);
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


	
    

}