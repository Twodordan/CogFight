using UnityEngine;
using System.Collections;

public class line : MonoBehaviour {

	private LineRenderer renderer;
	public Transform[] transforms;

	// Use this for initialization
	void Start () {
		renderer = gameObject.GetComponent<LineRenderer>();
		renderer.SetVertexCount(transforms.Length);
	}
	
	// Update is called once per frame
	void Update () {
		renderer.SetPosition(0,transforms[0].position);
		renderer.SetPosition(1,(transforms[0].position-transforms[1].position)*0.5f+transforms[1].position);


		float distance = Vector3.Distance(transforms[0].position,transforms[1].position);
		renderer.SetWidth(0.7f, 1.2f-(distance/10));
	}
}