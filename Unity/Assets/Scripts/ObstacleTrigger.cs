using UnityEngine;
using System.Collections;

public class ObstacleTrigger : MonoBehaviour {

	private GameObject[] tileArray;
	public Spike spike;
	public Material tileMaterial;
	public Material warningMaterial;
	private int activeTile;

	private float i = 0f;
	private float rate;
	private int animation = 0;


	void Start() {
		//Find all the tiles tagged with tiles (the ones beneath the players)
		tileArray = GameObject.FindGameObjectsWithTag("Tile");

		if (tileArray.Length == 0) {
			Debug.Log("No game objects are tagged with Tile");
		}
	}

	void Update(){

		if (animation == 1){
			i += Time.deltaTime*rate*2;
			tileArray[activeTile].renderer.material.color = Color.Lerp(tileMaterial.color, warningMaterial.color, i);
		
			if (tileArray[activeTile].renderer.material.color == warningMaterial.color){
				animation = 2;
				tileArray[activeTile].renderer.material.color = tileMaterial.color;
				i = 0f;
				rate = 1f/1.5f;
			}
		} else if (animation == 2){
			i += Time.deltaTime*rate*2;
			tileArray[activeTile].renderer.material.color = Color.Lerp(warningMaterial.color, tileMaterial.color, i);
		
			if (tileArray[activeTile].renderer.material.color == tileMaterial.color){
				animation = 0;
				i = 0f;
			}
		}
	}
	
	void Awake (){
		EventManager.OnMusic_ForeshadowBegin += ForeshadowBegin;
		EventManager.OnMusic_ForeshadowConclusion += ForeshadowConclusion;
	}
	
	
	void ForeshadowBegin (int n, double m){
		//Select a random Tile
		activeTile = Random.Range(0,tileArray.Length-1);
		rate = 1f/(float)m;
		animation = 1;
	}



	void ForeshadowConclusion (int n, double m){
		spike.ActivateStab(tileArray[activeTile].transform.position.x);
	}

}
