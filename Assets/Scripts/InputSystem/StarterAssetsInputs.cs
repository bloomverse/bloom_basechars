using UnityEngine;
using UnityEngine.InputSystem;


	public class StarterAssetsInputs : MonoBehaviour
	{
		[Header("Character Input Values")]
		public Vector2 move;
		public Vector2 look;
		public bool jump;
		public bool sprint;
		public bool combatMode;
	
		public bool rightStatus;
		public bool leftStatus;

		public bool spell1Status;
		public bool spell2Status;
		public bool spell3Status;
		public bool spell4Status;
		public bool spell5Status;
		
		public bool inventoryStatus;

		private InputAction leftAction;
		private InputAction rightAction;
		private InputAction spellAction1;
    	private InputAction spellAction2;
    	private InputAction spellAction3;
    	private InputAction spellAction4;
		private InputAction spellAction5;
		private InputAction jumpAction;

		

		private PlayerInput playerInput;

		private InputAction inventory;

		private float toogleDelay = 1;
		private float toogleTime = 0;
		

		[Header("Movement Settings")]
		public bool analogMovement;

		// MOBILE
		#if !UNITY_IOS || !UNITY_ANDROID
		[Header("Mouse Cursor Settings")]
		public bool cursorLocked = true;
		public bool cursorInputForLook = true;
		#endif



		private void Awake(){
			playerInput = GetComponent<PlayerInput>();
			leftAction = playerInput.actions["LeftClick"];
			rightAction = playerInput.actions["RightClick"];
			spellAction1 = playerInput.actions["CastSpell1"];
			spellAction2 = playerInput.actions["CastSpell2"];
			spellAction3 = playerInput.actions["CastSpell3"];
			spellAction4 = playerInput.actions["CastSpell4"];
			spellAction5 = playerInput.actions["CastSpell5"];
			jumpAction = playerInput.actions["Jump"];
			inventory = playerInput.actions["Inventory"];
		}
		private void OnEnable(){
		
		leftAction.performed += _ => leftClickStart();
		leftAction.canceled += _ => leftClickStop();

		rightAction.performed += _ => rightClickStart();
		rightAction.canceled += _ => rightClickStop();

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

		jumpAction.performed += _ => {jump=true; };
        jumpAction.canceled += _ => {jump=false; };


		inventory.performed += _ => toogleInventory();


	    }

		private void castSpell1(){
			Debug.Log("casting 1");
			spell1Status = true;
		}
		private void stopCast1(){
			spell1Status = false;
		}
		private void castSpell2(){
			spell2Status = true;
		}
		private void stopCast2(){
			spell2Status = false;
		}
		private void castSpell3(){
			spell3Status = true;
		}
		private void stopCast3(){
			spell3Status = false;
		}
		private void castSpell4(){
			spell4Status = true;
		}
		private void stopCast4(){
			spell4Status = false;
		}


		private void castSpell5(){
			spell5Status = true;
		}
		private void stopCast5(){
			spell5Status = false;
		}

		private void leftClickStart(){
			leftStatus = true;
		}
		private void leftClickStop(){
			leftStatus = false;
		}
		private void rightClickStart(){
			rightStatus = true;
		}
		private void rightClickStop(){
			rightStatus = false;
		}

		// Inventory Controolls
		private void toogleInventory(){

		  
		  if(!inventoryStatus  && toogleTime<toogleDelay){
			toogleTime += Time.deltaTime;
			inventoryStatus = true;

		  }else{
			toogleTime = 0;
			inventoryStatus = false;
		  }
		}
		

		public void OnMove(InputValue value)
		{
			MoveInput(value.Get<Vector2>());
		}

		public void OnLook(InputValue value)
		{
			if(cursorInputForLook)
			{
				LookInput(value.Get<Vector2>());
			}
		}

		public void OnJump(InputValue value)
		{
			//Debug.Log(value.isPressed + "isPressed");
			//JumpInput(value.isPressed);
		}

		public void OnSprint(InputValue value)
		{
			SprintInput(value.isPressed);
		}

		//-------------------------------------------- END LISTENERS



		public void MoveInput(Vector2 newMoveDirection)
		{
			move = newMoveDirection;
		} 

		public void LookInput(Vector2 newLookDirection)
		{
			look = newLookDirection;
		}

		public void JumpInput(bool newJumpState)
		{
			jump = newJumpState;
		}

		public void SprintInput(bool newSprintState)
		{
			sprint = newSprintState;
		}

		

#if !UNITY_IOS || !UNITY_ANDROID

		private void OnApplicationFocus(bool hasFocus)
		{
			SetCursorState(cursorLocked);
		}

		private void SetCursorState(bool newState)
		{
			Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;
		}

#endif


	
}