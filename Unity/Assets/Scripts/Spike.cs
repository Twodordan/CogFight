using UnityEngine;
using System.Collections;

public class Spike : MonoBehaviour {
	
	private float stabDistance = 12; //How long does the Spike travel?
	private float stabSpeedUp = 30; //Speed going up
	private float stabSpeedDown = 7; //Speed going down
	private bool stabbed = false;
	private float distanceTravelled = 0;
	private float stealthedY; //Stealthed position, for resetting the y position.
	public bool animated = false; //For activating the animation

	void Start (){

		//The script assumes the Spike is placed in the scene where it's stealthed
		stealthedY = transform.position.y;

	}

	void Update () {

		if(animated){
			Vector3 currentPos = transform.position;

			if (!stabbed){
				transform.Translate(Vector3.up * Time.deltaTime * stabSpeedUp);
			} else {
				transform.Translate(Vector3.down * Time.deltaTime * stabSpeedDown);
			}

			distanceTravelled += Vector3.Distance(currentPos,transform.position);
			
			if (Mathf.Abs (distanceTravelled) >= stabDistance){
				distanceTravelled = 0;
				if (!stabbed){
					stabbed = true;
				} else if (stabbed){
					stabbed = false;
					animated = false;
					transform.position = new Vector3(transform.position.x,stealthedY);
				}
			}
		}
		
	}

	public void ActivateStab(float x){

		if (!animated){
			transform.position = new Vector3(x,stealthedY);
			animated = true;
		} else {
			Debug.Log("Spike is already activated! Cannot activate until action is done!");
		}

	}

}
