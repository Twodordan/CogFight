using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum PlayerNumber{
	One = 1,
	Two = 2
}

public class MultiplayerController : MonoBehaviour {
	[Space(10)]
	[Header("1 bar =  4x beats")]
	[Range(1,8)]
	public int switchPlayersEveryXBar = 1;
	[Space(10)]

	[SerializeField]
	[Range(0,2)]
	private int whichSceneToLoadOnReset = 0;

	[SerializeField]
	private List<Character2DController> players;

	private int beatsCountedSinceSwitch = 0;

	void Awake () {
		//EventManager.OnGameStart += unpauseChars;
		//EventManager.OnGameEnd += pauseChars;
		EventManager.OnMusic_Bar += switchCharactersByEvent;
		EventManager.OnTerminateLevel += cleanUpBeforeTerminate;
	}

	// Use this for initialization
	void Start () {

		pauseChars();

	}

	// Update is called once per frame
	void Update () {


		//TESTING
		/*
		if(Input.GetButtonDown("Start")){
			//bad
			//EventManager.StartGame();
			//StateManager.State = GameState.Playing;

			StateManager.Flags = StateFlags.ReadyForPlay;
		}
		else if(Input.GetButtonDown("Back")){

			EventManager.TerminateLevel();
			Debug.Log("_________________ TerminatingLevel");
			StartCoroutine(delayLoadLevel(0));


			//Application.loa
		}*/


		/*
		if(Input.GetKeyDown(KeyCode.Space)){
			//StateManager.State = GameState.Playing;
			StateManager.Flags = StateFlags.ReadyForPlay;
			//pauseChars();
			//switchCharacters(1);
			//pauseCharacters();
		}
		*/

	}

	private void cleanUpBeforeTerminate(){
		StopAllCoroutines();
	}

	private IEnumerator delayLoadLevel(float secs){

		yield return new WaitForSeconds(secs);
		Debug.Log("_________________ delayload");
		Application.LoadLevel(whichSceneToLoadOnReset);
	}

	public void unpauseChars(){
		setPauseCharacters(false);
	}
	public void pauseChars(){
		setPauseCharacters(true);
	}

	public void setPauseCharacters(bool val){
		for(int i=0; i< players.Count; i++){
			players[i].Pause(val); 
		}
	}

	public void switchCharactersByEvent(){

		beatsCountedSinceSwitch++;

		if(beatsCountedSinceSwitch >= switchPlayersEveryXBar){

			beatsCountedSinceSwitch = 0;
			switchCharacters(0);
		}

	}

	public void switchCharacters(float delaySeconds){
		//Note: changge to loop if we have more than 2 chars.

		bool success = players[(int)PlayerNumber.One-1].switchToPlayer(PlayerNumber.Two, 
		                                                               players[(int)PlayerNumber.Two-1],
		                                                               delaySeconds);

		if(success){
			Character2DController tmp = players[(int)PlayerNumber.One-1];
			players[(int)PlayerNumber.One-1] = players[(int)PlayerNumber.Two-1];
			players[(int)PlayerNumber.Two-1] = tmp;
		}


	}
}
