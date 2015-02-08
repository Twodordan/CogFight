using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ObstacleTrigger : MonoBehaviour {

	private GameObject[] tileArray;
	public Spike spike;
	public Material tileMaterial;
	public Material warningMaterial;
	private int activeTile;

	private float i = 0f;
	private float rate;
	private int animation = 0;

	private List<GameObjectIDPair> objectIDPairs = new List<GameObjectIDPair>();

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
	
	
	void ForeshadowBegin (int id, double duration){
		GameObject chosenGameobject = tileArray[Random.Range(0,tileArray.Length-1)];
		StartCoroutine(AnimateColor((float)duration, chosenGameobject));
		objectIDPairs.Add (new GameObjectIDPair(chosenGameobject, id));
	}

	IEnumerator AnimateColor(float duration, GameObject objectToColor) {
		float startTime = Time.time;

		// Color more
		while(Time.time < startTime + duration) {
			float degree = (Time.time - startTime) / duration;
			objectToColor.renderer.material.color = Color.Lerp(tileMaterial.color, warningMaterial.color, degree);
			yield return null;
		}

		// Color less
		startTime = Time.time;
		while(Time.time < startTime + 1.5f) {
			float degree = (Time.time - startTime) / 1.5f;
			objectToColor.renderer.material.color = Color.Lerp(warningMaterial.color, tileMaterial.color, degree);
			yield return null;
		}

		objectToColor.renderer.material.color = tileMaterial.color;
		yield break;
	}

	void ForeshadowConclusion (int id, double duration){
		GameObject intendedGameObject = null;
		foreach (GameObjectIDPair objIDPair in objectIDPairs) {
			if (objIDPair.id == id) {
				intendedGameObject = objIDPair.obj;
				break;
			}
		}

		if (intendedGameObject != null) {
			spike.ActivateStab(intendedGameObject.transform.position.x);
		}
	}

}

class GameObjectIDPair {
	public GameObject obj;
	public int id;

	public GameObjectIDPair(GameObject gameObject, int id) {
		this.obj = gameObject;
		this.id = id;
	}
}
