using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ObstacleTrigger : MonoBehaviour {

	private GameObject[] tileArray;
	public Spike spike;
	public Material tileMaterial;
	public Material warningMaterial;

    public float spikeStabDuration = 0.4f;
    public float spikeRetractDuration = 1.7f;
    public float spikeDistance = 12f;
    public float spikeStealthedY;
    public AnimationCurve spikeMovementCurve = new AnimationCurve();
    Coroutine spikeCoroutine = null;

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

        spikeStealthedY = spike.transform.position.y;
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

        Color colorAtBeginning = objectToColor.renderer.material.color;

		// Color more
		while(Time.time < startTime + duration) {
			float degree = (Time.time - startTime) / duration;
            objectToColor.renderer.material.color = Color.Lerp(colorAtBeginning, warningMaterial.color, degree);
			yield return null;
		}

		// Color less
		startTime = Time.time;
		while(Time.time < startTime + 1.5f) {
			float degree = (Time.time - startTime) / 1.5f;
            objectToColor.renderer.material.color = Color.Lerp(warningMaterial.color, colorAtBeginning, degree);
			yield return null;
		}

        objectToColor.renderer.material.color = colorAtBeginning;
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

		if (intendedGameObject != null && spikeCoroutine == null) {
			//spike.ActivateStab(intendedGameObject.transform.position.x);
            spikeCoroutine = StartCoroutine(AnimateSpike(intendedGameObject.transform.position.x));
		}
	}

    IEnumerator AnimateSpike(float x) {

        spike.transform.position = new Vector3(x, spikeStealthedY);

        Vector3 startPosition = spike.transform.position;
        Vector3 topPosition = startPosition + Vector3.up * spikeDistance;

        float startTimeStamp = Time.time;

        while (Time.time < startTimeStamp + spikeStabDuration) {
            spike.transform.position = Vector3.Lerp(startPosition, topPosition, spikeMovementCurve.Evaluate((Time.time - startTimeStamp) / spikeStabDuration));
            yield return null;
        }

        startTimeStamp = Time.time;

        while (Time.time < startTimeStamp + spikeRetractDuration) {
            spike.transform.position = Vector3.Lerp(startPosition, topPosition, spikeMovementCurve.Evaluate(1 - (Time.time - startTimeStamp) / spikeRetractDuration));
            yield return null;
        }

        spike.transform.position = startPosition;

        spikeCoroutine = null;
        yield break;
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
