using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum PlayerNumber{
	One = 1,
	Two = 2
}

public class MultiplayerController : MonoBehaviour {

	public List<Character2DController> players;

	// Use this for initialization
	void Start () {
	
	}

	// Update is called once per frame
	void Update () {
		//TESTING TODO: remove
		if(Input.GetKeyDown(KeyCode.Space)){
			//switchCharacters();
			pauseCharacters();
		}
	}

	public void pauseCharacters(){
		for(int i=0; i< players.Count; i++){
			players[i].Pause(true); 
		}
	}

	public void switchCharacters(){
		//TODO: changge to loop if we have more than 2 chars.

		bool success = players[(int)PlayerNumber.One-1].switchToPlayer(PlayerNumber.Two, players[(int)PlayerNumber.Two-1]);

		if(success){
			Character2DController tmp = players[(int)PlayerNumber.One-1];
			players[(int)PlayerNumber.One-1] = players[(int)PlayerNumber.Two-1];
			players[(int)PlayerNumber.Two-1] = tmp;
		}


	}
}
