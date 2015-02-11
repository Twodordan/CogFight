using UnityEngine;
using System.Collections;

public class QuickCameraScaleWithWindow : MonoBehaviour {
	[Range(0.0f, 10)]
	public float windowResizeCheckInterval = 0.2f;

	public float gameFrameWidth = 28;
	Vector2 wh;
	bool run = true;
	
	void Start(){
		OnResize();
		StartCoroutine(checkWindowResize());
	}
	
	IEnumerator checkWindowResize(){
		wh = new Vector2(Screen.width, Screen.height);
		//fWidth = Camera.main.transform.position.z;

		while(run){
			if(wh.x != Screen.width || wh.y != Screen.height){
				OnResize();
				wh = new Vector2(Screen.width, Screen.height);
			}
			yield return new WaitForSeconds(windowResizeCheckInterval);
		}
	}
	
	void OnDestroy(){
		run = false;
		StopAllCoroutines();
	}

	void OnResize(){
		float fT = gameFrameWidth / Screen.width * Screen.height;
		fT /= 2.0f * Mathf.Tan (0.5f * Camera.main.fieldOfView * Mathf.Deg2Rad);
		Vector3 v3T = Camera.main.transform.position;
		v3T.z = -fT;
		transform.position = v3T;
	}
}
