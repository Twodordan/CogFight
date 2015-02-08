using UnityEngine;
using System.Collections;

public class Hover : MonoBehaviour {

	public float hoverDistance;
	public float hoverSpeed;
	private bool direction = true;
	private float distanceTravelled = 0;

	// Use this for initialization
	void Start () {

		transform.Translate(Vector3.down*hoverDistance*0.5f);
	
	}
	
	// Update is called once per frame
	void Update () {

		Vector3 currentPos = transform.position;
		if (direction){
			transform.Translate(Vector3.up * Time.deltaTime * hoverSpeed);
		} else {
			transform.Translate(Vector3.down * Time.deltaTime * hoverSpeed);
		}
		distanceTravelled += Vector3.Distance(currentPos,transform.position);

		if (Mathf.Abs (distanceTravelled) >= hoverDistance){
			distanceTravelled = 0;
			if (direction) direction = false;else direction = true;
		}
	
	}
}
