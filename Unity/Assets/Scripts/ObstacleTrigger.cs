using UnityEngine;
using System.Collections;

public class ObstacleTrigger : MonoBehaviour {

	private GameObject[] tileArray;
	public Spike spike;
	public Material tileMaterial;
	public Material warningMaterial;
	private int activeTile;

	void Start() {
		//Find all the tiles tagged with tiles (the ones beneath the players)
		tileArray = GameObject.FindGameObjectsWithTag("Tile");

		if (tileArray.Length == 0) {
			Debug.Log("No game objects are tagged with Tile");
		}
	}
	

	void Awake (){
		
		EventManager.OnMusic_ForeshadowBegin += ForeshadowBegin;
		EventManager.OnMusic_ForeshadowConclusion += ForeshadowConclusion;
			
	}
	
	
	void ForeshadowBegin (int n, double m){
		//Select a random Tile
		activeTile = Random.Range(0,tileArray.Length-1);

		warnTile((float)m);

	}

	void warnTile(float time){
		float i = 0.0f;
		float rate = 1f/time;
		while (i<1.0f){
			i += Time.deltaTime*rate;
			tileArray[activeTile].renderer.material.color = Color.Lerp(tileMaterial.color, warningMaterial.color, i);
		}
	}

	void ForeshadowConclusion (int n, double m){

		tileArray[activeTile].renderer.material = tileMaterial;

		spike.ActivateStab(tileArray[activeTile].transform.position.x);
		
	}

}
