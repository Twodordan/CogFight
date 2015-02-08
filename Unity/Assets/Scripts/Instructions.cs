using UnityEngine;
using System.Collections;

public class Instructions : MonoBehaviour {

	public Texture backgroundTexture;


	void OnGUI() {
		if( StateManager.State==GameState.Beginning) GUI.DrawTexture (new Rect (Screen.width / 3f, Screen.height / 3f, Screen.width/3f, Screen.height/3f), backgroundTexture);
	}
	

}
