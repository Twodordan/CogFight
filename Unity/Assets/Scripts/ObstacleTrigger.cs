using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ObstacleTrigger : MonoBehaviour {


	//public Spike spike;

    public List<GameObject> spikePrefabs;

	public Material tileMaterial;
	public Material warningMaterial;

	public List<float> spikeStabDurations;// = 0.4f;
    public float spikeRetractDuration = 1.7f;
	public List<float> spikeDistances;
    public AnimationCurve spikeMovementCurve = new AnimationCurve();
    //Coroutine spikeCoroutine = null;

	private float spikeStealthedY = -7.91f;

	private GameObject[] tileArrayFloor;
	private GameObject[] tileArrayWallL;
	private GameObject[] tileArrayWallR;
	private int activeTile;

	private float i = 0f;
	private float rate;
	private int animation = 0;

	private List<GameObjectIDPair> objectIDPairs = new List<GameObjectIDPair>();

    private List<GameObject> activeTiles = new List<GameObject>();

	void Start() {
		//Find all the tiles tagged with tiles (the ones beneath the players)
		tileArrayFloor = GameObject.FindGameObjectsWithTag("Tile");
		tileArrayWallL = GameObject.FindGameObjectsWithTag("TileL");
		tileArrayWallR = GameObject.FindGameObjectsWithTag("TileR");

		if (tileArrayFloor.Length == 0) {
			Debug.Log("No game objects are tagged with Tile");
		}
		if (tileArrayWallL.Length == 0) {
			Debug.Log("No game objects are tagged with TileL");
		}
		if (tileArrayWallR.Length == 0) {
			Debug.Log("No game objects are tagged with TileR");
		}

        //spikeStealthedY = spike.transform.position.y;
	}

	void Awake (){
		EventManager.OnMusic_ForeshadowBegin += ForeshadowBegin;
		EventManager.OnMusic_ForeshadowConclusion += ForeshadowConclusion;
        EventManager.OnPlayerDeath += (int id) => { StartCoroutine(PlayerDeathTimer(id)); };
	}

	void ForeshadowBegin (int id, double duration){
		handleForeshadowBegin(tileArrayFloor, id, duration);
		float rand = Random.Range(0.0f,1.0f);
		Debug.Log(rand);
		if(rand>0.5f){
			handleForeshadowBegin(tileArrayWallL, id, duration);
		}
		else{
			handleForeshadowBegin(tileArrayWallR, id, duration);
		}
	}

	void handleForeshadowBegin(GameObject[] tileArray, int id, double duration){

		GameObject chosenGameobject = tileArray[Random.Range(0,tileArray.Length-1)];
		
		while (activeTiles.Contains(chosenGameobject)) {
			chosenGameobject = tileArray[Random.Range(0, tileArray.Length-1)];
		}
		
		StartCoroutine(AnimateColor((float)duration, chosenGameobject));
		objectIDPairs.Add (new GameObjectIDPair(chosenGameobject, id));
	}

    bool playerDeath = false;

    IEnumerator PlayerDeathTimer(int id) {
        objectIDPairs.Clear();
        playerDeath = true;
        yield return new WaitForSeconds(2f);
        playerDeath = false;
        yield break;
    }

	IEnumerator AnimateColor(float duration, GameObject objectToColor) {
		float startTime = Time.time;
        activeTiles.Add(objectToColor);
        Color colorAtBeginning = objectToColor.renderer.material.color;

		// Color more
		while(Time.time < startTime + duration && !playerDeath) {
			float degree = (Time.time - startTime) / duration;
            objectToColor.renderer.material.color = Color.Lerp(colorAtBeginning, warningMaterial.color, degree);

            if (playerDeath) {
                break;
            }

			yield return null;
		}

		// Color less
		startTime = Time.time;
		while(Time.time < startTime + 1.5f && !playerDeath) {
			float degree = (Time.time - startTime) / 1.5f;
            objectToColor.renderer.material.color = Color.Lerp(warningMaterial.color, colorAtBeginning, degree);

            if (playerDeath) {
                break;
            }

			yield return null;
		}

        objectToColor.renderer.material.color = colorAtBeginning;
        activeTiles.Remove(objectToColor);
		yield break;
	}

	void ForeshadowConclusion (int id, double duration){
        if (!playerDeath) {
            GameObject intendedGameObject = null;
			int wallCounter = 3;
            foreach (GameObjectIDPair objIDPair in objectIDPairs) {
                if (objIDPair.id == id) {
					wallCounter--;
                    intendedGameObject = objIDPair.obj;
                    
					//this is ..inelegant
					if(intendedGameObject.transform.position.x == tileArrayWallL[0].transform.position.x){
						
						StartCoroutine(AnimateSpike(intendedGameObject.transform.position, Vector3.right, 
						                            spikePrefabs[1], spikeDistances[1], spikeStabDurations[1], false));
					}
					else if(intendedGameObject.transform.position.x == tileArrayWallR[0].transform.position.x){
						
						StartCoroutine(AnimateSpike(intendedGameObject.transform.position, Vector3.left, 
						                            spikePrefabs[1], spikeDistances[1], spikeStabDurations[1], false));
					}
					else{
						StartCoroutine(AnimateSpike(intendedGameObject.transform.position, Vector3.up, 
						                            spikePrefabs[0], spikeDistances[0], spikeStabDurations[0], true));
					}

					if(wallCounter<0){
						break;
					}
                }
            }

			/*
            if (intendedGameObject != null) {
                //spike.ActivateStab(intendedGameObject.transform.position.x);
            }*/
        }
	}

	IEnumerator AnimateSpike(Vector3 spikeOrigin, Vector3 spikeDirection, 
	                         GameObject spikePrefab, float spikeDistance, float spikeStabDuration, bool returns ) {
		GameObject spike = (GameObject)Instantiate(spikePrefab, spikeOrigin, Quaternion.LookRotation(spikeDirection));
        //spike.transform.position = new Vector3(x, spikeStealthedY);

        Vector3 startPosition = spike.transform.position;
		Vector3 topPosition = startPosition + spikeDirection * spikeDistance;

        float startTimeStamp = Time.time;

        while (Time.time < startTimeStamp + spikeStabDuration && !playerDeath) {
            spike.transform.position = Vector3.Lerp(startPosition, topPosition, spikeMovementCurve.Evaluate((Time.time - startTimeStamp) / spikeStabDuration));
            yield return null;
        }

        startTimeStamp = Time.time;

		if(returns)
	        while (Time.time < startTimeStamp + spikeRetractDuration && !playerDeath) {
	            spike.transform.position = Vector3.Lerp(startPosition, topPosition, spikeMovementCurve.Evaluate(1 - (Time.time - startTimeStamp) / spikeRetractDuration));
	            yield return null;
	        }

        spike.transform.position = startPosition;

        Destroy(spike);

        //spikeCoroutine = null;
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
