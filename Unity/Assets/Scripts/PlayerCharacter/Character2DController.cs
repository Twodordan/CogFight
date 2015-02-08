using UnityEngine;
using System.Collections;

public class Character2DController : IPausable {

	public bool switchInProgress{
		get{
			return switchInProgress_;
		}
	}
	public PlayerNumber playerNumber{
		get{
			return playerNumber_;
		}
		set{
			playerNumber_ = value;
		}
	}

	public GameObject characterMesh;
	public bool moveOnlyIn2D = true;
	public bool looksAtTarget = true;           
	public Transform lookAtTarget;
	public Transform playerIsAttachedTo;


	/*
	public class CharacterParameters{
	}
	private CharacterParameters charParams;
	*/


	private Character2DUserControl userControl;
	[SerializeField]
	private Character2DScriptControl scriptControl;
	[SerializeField]
	private Character2D character;

	[SerializeField]
	private PlayerNumber playerNumber_;
	 
	private bool switchInProgress_ = false;

	void Awake(){
		EventManager.OnPlayerDeath += onAPlayersDeath;
	}

	void Start () {
		userControl = new Character2DUserControl();
		//scriptControl = new Character2DScriptControl();

		if(playerNumber_ == 0){
			Debug.LogError("You must set the Player Number in the Inspector!");
		}
	}
	

	void Update () {

	}



	public override void Pause(bool pause){

		character.Pause(pause);

		base.Pause(pause);
	}

	void FixedUpdate() {

		if(isPaused){
			return;
		}

		Vector3 lookPos;//The position that the character should be looking towards
		Vector3 move;//the world-relative desired move direction, calculated from the lookAtTarget and user input, and then the characterScriptControl
		bool jump;



		userControl.solveInput(playerNumber, transform, looksAtTarget, lookAtTarget, 
		                       out lookPos, out move, out jump);


		bool onGround;
		character.Move(lookPos, move, jump, out onGround);

		scriptControl.rubberBanding(transform, playerIsAttachedTo, onGround);


	}

	void onAPlayersDeath(int i){
		Pause(true);
	}

	void OnCollisionEnter(Collision collision) {

		if(isPaused)
			return;

		foreach (ContactPoint contact in collision.contacts) {
			//Debug.DrawRay(contact.point, contact.normal, Color.white);
			if(contact.otherCollider.tag == "Obstacle"){
				EventManager.PlayerDeath((int)playerNumber);
				Debug.Log("playerNumber died: "+playerNumber);

				return;
			}
		}
	
		
	}
	
	public bool switchToPlayer(PlayerNumber number, Character2DController other, float delaySeconds){

		if(number != playerNumber && !(switchInProgress || other.switchInProgress) && !isPaused){

			switchInProgress_ = true;

			//handle necessary swaps

			//TODO: start to play swap animation

			//swap the chars in the middle of the animation.
			object[] parms = new object[3]{number, other, delaySeconds};
			StartCoroutine(delaySwitch(parms));
			//delaySwitch();
			
			//note: this doesn't actually check if the swap was successful, but if the references are there it will certainly work.
			return true;
		}
		else
			return false;
	}

	//params PlayerNumber number, Character2DController other, float delaySeconds
	public IEnumerator delaySwitch(object[] parms){
		PlayerNumber number = (PlayerNumber)parms[0];
		Character2DController other = (Character2DController)parms[1];
		float delaySeconds = (float)parms[2];
		/*
		Debug.Log("SWITCH in progress; number: "+number+"; != playerNumber: "+
		          playerNumber+"; delaySeconds: "+delaySeconds+
		          ";");
		          */
		yield return new WaitForSeconds(delaySeconds);

		//swap mesh 
		Transform charMeshParent = characterMesh.transform.parent;
		other.setNewCharacterMesh(characterMesh, other.characterMesh.transform.parent);
		setNewCharacterMesh(other.characterMesh, charMeshParent);

		//swap IDs
		other.playerNumber = this.playerNumber;
		playerNumber = number;
	}

	public void setNewCharacterMesh(GameObject characterMesh, Transform parent){
		//moving this GO to other GO
		characterMesh.transform.SetParent(parent);
		characterMesh.transform.localPosition = Vector3.zero;
		characterMesh.transform.localRotation = Quaternion.identity;

		switchInProgress_ = false;
		//Debug.Log("SWITCH Complete (can switch again)");
	}

	private void getPlayerNumber(){

	}

}
