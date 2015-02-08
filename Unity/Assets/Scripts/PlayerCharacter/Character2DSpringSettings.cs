using UnityEngine;
using System.Collections;

public class Character2DSpringSettings:MonoBehaviour{
	[Space(10)]
	[Range(1f,20f)]
	public float rangeBetweenChars = 5;

	//uncomment if you need independent intervals or a middle interval
	//[Range(0.0f,2.0f)]
	//public float repelPercentOnRange = 1.0f;//0.8f;
	//[Range(1f,20f)]
	//public float attractPercentOnRange = 1.0f;//1.2f;
	[Space(10)]
	public AnimationCurve springCurve;

	[Space(10)]
	[Range(0.0f,10.0f)]
	public float scaleSpringForce = 5.0f;

}