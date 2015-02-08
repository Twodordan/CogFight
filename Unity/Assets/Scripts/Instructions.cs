using UnityEngine;
using System.Collections;

public class Instructions : MonoBehaviour {

	public Texture backgroundTexture;


	void OnGUI() {
		if( StateManager.State==GameState.Beginning) GUI.DrawTexture (new Rect (Screen.width / 4f, Screen.height / 4f, Screen.width/2f, Screen.height/2f), backgroundTexture);
	}
	

}
