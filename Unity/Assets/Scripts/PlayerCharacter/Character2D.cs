using UnityEngine;
using System.Collections;

public class Character2D : IPausable{

	[SerializeField] Transform characterPivot;
	[SerializeField] float jumpPower = 12;								//determines the jump force applied when jumping (and therefore the jump height)
	//[SerializeField] float airSpeed = 6;								
	[SerializeField] float minAirMomentum = 6;							
	[SerializeField] float airDecelerationRate = 1;						
	[SerializeField] float groundDecelerationRate = 1;					
	
	[SerializeField] float airControl = 2;								//determines the response speed of controlling the character while airborne
	[SerializeField] float groundControl = 2;							//determines the response speed of controlling the character while grounded
	//[SerializeField] float groundFriction = 1;
	//[SerializeField] float airControlDecelerationRate = 1;	
	//[SerializeField] float minAirControlMomentum = 6;
	[SerializeField] float terminalAirSpeed = 10;
	[SerializeField] float terminalThrustSpeed = 10;
	[SerializeField] float terminalDiveSpeed = 10;
	[SerializeField] float terminalGroundSpeed = 10;
	
	[Range(1,4)] [SerializeField] public float gravityMultiplier = 2;	//gravity modifier - often higher than natural gravity feels right for game characters

	[SerializeField] float jumpBoostMultiplier = 2;
	[SerializeField][Range(0.1f,3f)] float moveSpeedMultiplier = 1;	   //how much the move speed of the character will be multiplied by
	[SerializeField][Range(0.1f,3f)] float animSpeedMultiplier = 1;	   //how much the animation of the character will be multiplied by
	[SerializeField] public AdvancedSettings advancedSettings;                //Container for the advanced settings class , thiss allows the advanced settings to be in a foldout in the inspector
	
	
	[System.Serializable]
	public class AdvancedSettings
	{
		public int reverseRaycastDirMeshFix = -1;
		public float groundedRayDistance = 1;
		public float stationaryTurnSpeed = 180;				//additional turn speed added when the player is stationary (added to animation root rotation)
		public float movingTurnSpeed = 360;					//additional turn speed added when the player is moving (added to animation root rotation)
		public float headLookResponseSpeed = 2;				//speed at which head look follows its target
		public float autoTurnThresholdAngle = 100;			//character auto turns towards camera direction iffacing away by more than this angle
		public float autoTurnSpeed = 2;						//speed at which character auto-turns towards cam direction
		public PhysicMaterial zeroFrictionMaterial;			//used when in motion to enable smooth movement
		public PhysicMaterial highFrictionMaterial;			//used when stationary to avoid sliding down slopes
		//public PhysicMaterial bakZeroFrictionMaterial;
		//public PhysicMaterial bakHighFrictionMaterial;
		public float jumpRepeatDelayTime = 0.25f;			//amount of time that must elapse between landing and being able to jump again
		public float runCycleLegOffset = 0.2f;				//animation cycle offset (0-1) used for determining correct leg to jump off
		public float groundStickyEffect = 5f;				//power of 'stick to ground' effect - prevents bumping down slopes.
	}
	
	bool onGround;                                         //Is the character on the ground
	Vector3 currentLookPos;                                //The current position where the character is looking
	float originalHeight;                                  //Used for tracking the original height of the characters capsule collider
	Animator animator;                                     //The animator for the character
	float lastAirTime;                                     //USed for checking when the character was last in the air for controlling jumps
	//CapsuleCollider capsule;                               //The collider for the character
	const float half = 0.5f;                               //whats it says, it's a constant for a half
	Vector3 moveInput;
	bool falling = false;
	bool canJumpContinuous = true;
	bool jump;
	float turnAmount;
	float forwardAmount;
	Vector3 velocity;
	IComparer rayHitComparer;

	Vector3 savedVelocityPause;
	Vector3 savedAngVelocityPause;
	float savedAnimatorSpeed;

	private float jumpBoost = 1;
	private Vector3 jumpSpeedCurve = Vector3.zero;
	private Vector3 terminalAirVelocity = Vector3.zero;//* rigidbody.drag;
	private Vector3 terminalGroundVelocity = Vector3.zero;//* rigidbody.drag;
	//private Vector3 terminalAirControlVelocity = Vector3.zero;//* rigidbody.drag;
	//private float aAirMom = 0;
	
	//Use this for initialization
	void Start(){
		animator = GetComponentInChildren<Animator>();

		//capsule = collider as CapsuleCollider;

		/*
		//as can return null so we need to make sure thats its not before assigning to it
		if(capsule != null){
			originalHeight = capsule.height;
			capsule.center = Vector3.up * originalHeight * half;
		} else{
			Debug.LogError(" collider cannot be cast to CapsuleCollider");
		}
		*/
		rayHitComparer = new RayHitComparer ();
		
		SetUpAnimator();


	}
	
	public override void Pause(bool paused){
		if(paused == isPaused)
			return;

		base.Pause(paused);

		if(isPaused){
			savedVelocityPause = rigidbody.velocity;
			savedAngVelocityPause = rigidbody.angularVelocity;

			//rigidbody.velocity = Vector3.zero;
			//rigidbody.angularVelocity = Vector3.zero;
			rigidbody.isKinematic = true;

			if(animator){
				savedAnimatorSpeed = animator.speed;
				animator.speed = 0;
			}
			else{
				Debug.LogWarning("animator.avatar is null. animations will not be frozen on pause.");
			}
		}
		else{

			rigidbody.isKinematic = false;
			rigidbody.AddForce( savedVelocityPause, ForceMode.VelocityChange );
			rigidbody.AddTorque( savedAngVelocityPause, ForceMode.VelocityChange );

			if(animator){
				animator.speed = savedAnimatorSpeed;
			}
			else{
				Debug.LogWarning("animator.avatar is null.");
			}
		}
	}


	public void Move(Vector3 lookPos, Vector3 move, bool jump){
		if(move.magnitude > 1) move.Normalize();

		//transfer input parameters to member variables.
		this.moveInput = move;
		this.jump = jump;
		if(jump && !falling){
			canJumpContinuous = true;
		}
		else
		if(!jump){
			falling = true;
			canJumpContinuous = false;
		}
		//Debug.Log("canJumpContinuous: "+canJumpContinuous+"; jump: "+jump);


		this.currentLookPos = lookPos;

		velocity = rigidbody.velocity;

		//ConvertMoveInput();//converts the relative move vector into local turn & fwd values
		//TODO
		TurnTowardsTarget();//makes the character face the way the camera is looking

		GroundCheck();//detect and stick to ground
		
		SetFriction();//use low or high friction values depending on the current state


		//control and velocity handling is different when grounded and airborne:
		if(onGround){
			HandleGroundedVelocities();
		} else{
			HandleAirborneVelocities();
		}
		//HandleAirborneVelocities();

		

		//TODO: our own animations
		//UpdateAnimator ();//send input and other state parameters to the animator
	
		rigidbody.velocity = velocity;
	}



	
	void ConvertMoveInput()
	{
		//convert the world relative moveInput vector into a local-relative
		//turn amount and forward amount required to head in the desired
		//direction. 
		Vector3 localMove = transform.InverseTransformDirection(moveInput);
		turnAmount = Mathf.Atan2 (localMove.y, localMove.x);
		forwardAmount = localMove.x;
	}
	
	void TurnTowardsTarget()
	{
		/*
		//automatically turn to face camera direction,
		//when not moving, and beyond the specified angle threshold
		if(Mathf.Abs (forwardAmount) < .01f){
			Vector3 lookDelta = transform.InverseTransformDirection (currentLookPos - transform.position);
			float lookAngle = Mathf.Atan2 (lookDelta.y, lookDelta.x) * Mathf.Rad2Deg;
		
			//are we beyond the threshold of where need to turn to face the camera?
			if(Mathf.Abs (lookAngle) > advancedSettings.autoTurnThresholdAngle){
				turnAmount += lookAngle * advancedSettings.autoTurnSpeed * .001f;
			}
		}
		*/
		characterPivot.LookAt(currentLookPos);
	}
	


	void ApplyExtraTurnRotation()
	{
		//help the character turn faster (this is in addition to root rotation in the animation)
		float turnSpeed = Mathf.Lerp (advancedSettings.stationaryTurnSpeed, advancedSettings.movingTurnSpeed, forwardAmount);
		transform.Rotate (0, turnAmount * turnSpeed * Time.deltaTime, 0);
	}
	
	void GroundCheck()
	{
		Ray ray = new Ray (transform.position + Vector3.up * .1f * advancedSettings.reverseRaycastDirMeshFix, 
		                   -Vector3.up );
		//Debug.DrawRay(transform.position + Vector3.up * advancedSettings.reverseRaycastDirMeshFix
		//              , -Vector3.up );
		RaycastHit[] hits = Physics.RaycastAll (ray, advancedSettings.groundedRayDistance);//.5f
		System.Array.Sort (hits, rayHitComparer);

		if(velocity.y < jumpPower * .5f){
			onGround = false;
			rigidbody.useGravity = true;

			foreach (var hit in hits){

				//check whether we hit a non-trigger collider (and not the character itself)
				if(!hit.collider.isTrigger){

					//this counts as being on ground.
					/*
					//stick to surface - helps character stick to ground - specially when running down slopes
					if(velocity.y <= 0){
						rigidbody.position = Vector3.MoveTowards (rigidbody.position, hit.point, Time.deltaTime * advancedSettings.groundStickyEffect);
					}
					*/
					onGround = true;
					rigidbody.useGravity = false;
					falling = false;
					canJumpContinuous = true;

					//terminalAirVelocity = terminalAirControlVelocity = Vector3.zero;
					terminalAirVelocity = Vector3.zero;
					jumpBoost = 1;
					
					break;
				}
			}
		}
		
		//remember when we were last in air, for jump delay
		if(!onGround) lastAirTime = Time.time;
		
	}
	
	void SetFriction()
	{
		
		if(onGround){
			
			//set friction to low or high, depending on ifwe're moving
			if(moveInput.magnitude == 0){
				//when not moving this helps prevent sliding on slopes:
				collider.material = advancedSettings.highFrictionMaterial;
			} else{
				//but when moving, we want no friction:
				collider.material = advancedSettings.zeroFrictionMaterial;
			}
		} else{
			//while in air, we want no friction against surfaces (walls, ceilings, etc)
			collider.material = advancedSettings.zeroFrictionMaterial;
		}
	}
	
	void HandleGroundedVelocities()
	{
		
		velocity.z = 0;
		velocity.y = 0;

		if(moveInput.magnitude == 0){
			velocity = Vector3.Lerp (velocity, Vector3.zero, Time.deltaTime * groundDecelerationRate);
			//velocity.x = 0;
			//velocity.y = 0;
		}
		
		//check whether conditions are right to allow a jump:
		//TODO: change this
		bool animationGrounded = animator.GetCurrentAnimatorStateInfo (0).IsName ("Grounded");
		bool okToRepeatJump = Time.time > lastAirTime + advancedSettings.jumpRepeatDelayTime;


		float localGroundControl = groundControl;

		terminalGroundVelocity.x = velocity.x;
	
		Vector3 groundMove = new Vector3 (moveInput.x* localGroundControl, 0, 0 );

		velocity = Vector3.Lerp (velocity, terminalGroundVelocity, Time.deltaTime * groundDecelerationRate);
		
		velocity = velocity+groundMove;

		if(velocity.x > terminalGroundSpeed)
			velocity.x = terminalGroundSpeed;
		else
			if(velocity.x < -terminalGroundSpeed)
				velocity.x = -terminalGroundSpeed;
		
		if(velocity.y > terminalGroundSpeed){
			velocity.y = terminalGroundSpeed;
		}
		else
		if(velocity.y < -terminalGroundSpeed){
			velocity.y = -terminalGroundSpeed;
		}
	


		//this is OnJumpStart
		if(jump && canJumpContinuous && moveInput.y>0 && okToRepeatJump && animationGrounded){

			//jump!
			onGround = false;
			//velocity = moveInput * airSpeed;
			//aAirMom = airMomentum;
			//velocity = moveInput * airSpeed;
			
			velocity.y = jumpPower;
			jumpBoost = jumpBoostMultiplier;

			jumpSpeedCurve = velocity;
			jumpSpeedCurve.y = 0;
			terminalAirVelocity = jumpSpeedCurve.normalized * minAirMomentum * 1/rigidbody.drag;
			//terminalAirControlVelocity = jumpSpeedCurve.normalized * minAirControlMomentum * 1/rigidbody.drag;

			terminalAirVelocity.y=0;
			
		}


	}

	void HandleAirborneVelocities ()
	{ 
		float localGravityMultiplier = gravityMultiplier;
		float localAirControl = airControl;

		terminalAirVelocity.y = velocity.y;
		
	
		Vector3 airMove;
		if(canJumpContinuous || moveInput.y <0){
			//Debug.Log ("jump: "+jump+"; falling: "+falling+"; canJumpContinuous: "+canJumpContinuous);
			airMove = new Vector3 (moveInput.x* localAirControl, moveInput.y * localAirControl, 0 );
		}
		else{
			airMove = new Vector3 (moveInput.x* localAirControl, 0, 0 );
		}

		
		velocity = Vector3.Lerp (velocity, terminalAirVelocity, Time.deltaTime * airDecelerationRate);
		
		velocity = velocity+airMove;


		if(velocity.x > terminalAirSpeed)
			velocity.x = terminalAirSpeed;
		else
			if(velocity.x < -terminalAirSpeed)
				velocity.x = -terminalAirSpeed;
		
		if(velocity.y > terminalThrustSpeed){
			canJumpContinuous = false;
			falling = true;
			velocity.y = terminalThrustSpeed;
		}
		else
		if(velocity.y < -terminalDiveSpeed){
			canJumpContinuous = false;
			velocity.y = -terminalDiveSpeed;
		}

		if(velocity.y <=0){
			falling = true;
		}

		//velocity= velocity + airMove;
		
		
		rigidbody.useGravity = true;
		
		
		
		//apply extra gravity from multiplier:
		//Vector3 extraGravityForce = (Physics.gravity * localGravityMultiplier) - Physics.gravity;
		
		float extraGravityForce = (Physics.gravity.y * localGravityMultiplier) - Physics.gravity.y;
		
		jumpBoost = Mathf.Lerp(jumpBoost, 1, Time.deltaTime * airDecelerationRate);
		
		float antigravity = 0;
		if(canJumpContinuous && rigidbody.velocity.y >jumpPower*0.9f){//0.3f){
			antigravity = (Physics.gravity.y * jumpBoost) - Physics.gravity.y;
		}
		
		//Debug.Log("+++++++++++++============== rigidbody.velocity.y "+rigidbody.velocity.y);
		rigidbody.AddForce(new Vector3(velocity.x,extraGravityForce-antigravity,velocity.z));
		
	}
	
	void UpdateAnimator ()
	{
		//Here we tell the animator what to do based on the current states and inputs.
		
		//only use root motion when on ground:
		animator.applyRootMotion = onGround;
		
		//update the animator parameters
		animator.SetFloat ("Forward", forwardAmount, 0.1f, Time.deltaTime);
		animator.SetFloat ("Turn", turnAmount, 0.1f, Time.deltaTime);
		//animator.SetBool ("Crouch", crouchInput);
		animator.SetBool ("OnGround", onGround);
		if(!onGround){
			animator.SetFloat ("Jump", velocity.y);
		}
		
		//calculate which leg is behind, so as to leave that leg trailing in the jump animation
		//(This code is reliant on the specific run cycle offset in our animations,
		//and assumes one leg passes the other at the normalized clip times of 0.0 and 0.5)
		float runCycle = Mathf.Repeat (animator.GetCurrentAnimatorStateInfo (0).normalizedTime + advancedSettings.runCycleLegOffset, 1);
		float jumpLeg = (runCycle < half ? 1 : -1) * forwardAmount;
		if(onGround){
			animator.SetFloat ("JumpLeg", jumpLeg);
		}
		
		//the anim speed multiplier allows the overall speed of walking/running to be tweaked in the inspector,
		//which affects the movement speed because of the root motion.
		if(onGround && moveInput.magnitude > 0){
			animator.speed = animSpeedMultiplier;
		} else{
			//but we don't want to use that while airborne
			animator.speed = 1;
			//animator.speed = animSpeedMultiplier;
		}
	}
	
	
	
	void OnAnimatorIK(int layerIndex)
	{
		/*
		//we set the weight so most of the look-turn is done with the head, not the body.
		//animator.SetLookAtWeight(1, 0.2f, 2.5f);
		//animation lookpos look position look at target angle threshold
		animator.SetLookAtWeight(1, 0.225f, 0.828f);
		
		//ifa transform is assigned as a look target, it overrides the vector lookPos value
		if(lookTarget != null){
			currentLookPos = lookTarget.position;
		}
		
		//Used for the head look feature.
		animator.SetLookAtPosition( currentLookPos );
		*/
	}
	
	
	
	void SetUpAnimator()
	{
		//this is a ref to the animator component on the root.
		animator = GetComponent<Animator>();
		
		//we use avatar from a child animator component ifpresent
		//(this is to enable easy swapping of the character model as a child node)
		foreach (var childAnimator in GetComponentsInChildren<Animator>()){
			if(childAnimator != animator){
				animator.avatar = childAnimator.avatar;
				Destroy (childAnimator);
				break;
			}
		}
	}
	
	public void OnAnimatorMove()
	{
		//Debug.Log("~~~~~~~~!~~~~~~~~~~~~~~!~~~~~~~~~~~~~~~~~!~~~~~~~~~!");
		//we implement this function to override the default root motion.
		//this allows us to modify the positional speed before it's applied.
		rigidbody.rotation = animator.rootRotation;
		if(onGround && Time.deltaTime > 0){
			Vector3 v = (animator.deltaPosition * moveSpeedMultiplier) / Time.deltaTime;
			
			//we preserve the existing y part of the current velocity.
			v.y = rigidbody.velocity.y;
			rigidbody.velocity = v;
		}
	}
	
	
	//used for comparing distances
	class RayHitComparer: IComparer
	{
		public int Compare(object x, object y)
		{
			return ((RaycastHit)x).distance.CompareTo(((RaycastHit)y).distance);
		}	
	}
	

}
