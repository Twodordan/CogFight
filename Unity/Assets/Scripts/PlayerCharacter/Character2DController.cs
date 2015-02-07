using UnityEngine;
using System.Collections;

public class Character2DController : IPausable {


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
	private Character2DScriptControl scriptControl;
	[SerializeField]
	private Character2D character;

	[SerializeField]
	private PlayerNumber playerNumber_;
	 



	void Start () {
		userControl = new Character2DUserControl();
		scriptControl = new Character2DScriptControl();

		if(playerNumber_ == 0){
			Debug.LogError("You must set the Player Number in the Inspector!");
		}
	}
	

	void Update () {

	}

	public override void Pause(bool pause){

		character.Pause();

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

		//scriptControl.rubberBanding(out lookPos, out move);


		character.Move(lookPos, move, jump);



	}

	public bool switchToPlayer(PlayerNumber number, Character2DController other){
		Debug.Log("SWITCH; number: "+number+"; != playerNumber: "+playerNumber);
		if(number != playerNumber){

			//handle necessary swaps

			//swap mesh TODO: do it properly once we have chars
			Transform charMeshParent = characterMesh.transform.parent;
			other.setNewCharacterMesh(characterMesh, other.characterMesh.transform.parent);
			setNewCharacterMesh(other.characterMesh, charMeshParent);


			//TODO:play swap animation (delay the swap too)

			other.playerNumber = this.playerNumber;
			playerNumber = number;

			return true;
		}
		else
			return false;
	}

	public void setNewCharacterMesh(GameObject characterMesh, Transform parent){
		//moving this GO to other GO
		characterMesh.transform.SetParent(parent);
		characterMesh.transform.localPosition = Vector3.zero;
		characterMesh.transform.localRotation = Quaternion.identity;

	}

}
