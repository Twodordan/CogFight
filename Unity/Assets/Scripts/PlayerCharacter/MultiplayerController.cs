using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum PlayerNumber{
	One = 1,
	Two = 2
}

public class MultiplayerController : MonoBehaviour {

	public List<Character2DController> players;

	void Awale () {

	}

	// Use this for initialization
	void Start () {
		EventManager.OnGameStart += unpauseChars;
		EventManager.OnGameEnd += pauseChars;
		pauseChars();

	}

	// Update is called once per frame
	void Update () {
		//TESTING TODO: remove

		if(Input.GetButtonDown("Start")){
			EventManager.StartGame();
		}

		if(Input.GetKeyDown(KeyCode.Space)){
			EventManager.StartGame();
			//pauseChars();
			//switchCharacters(1);
			//pauseCharacters();
		}

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
	

	public void switchCharacters(float delaySeconds){
		//TODO: changge to loop if we have more than 2 chars.

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
