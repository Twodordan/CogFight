using UnityEngine;
using System.Collections;


public class Character2DScriptControl:MonoBehaviour{

	public Character2DSpringSettings springSettings;


	public void rubberBanding(Transform myTransform, 
							  Transform targetTransform,
	                          bool onGround
							  )
	{
		Vector3 vectToTarget = targetTransform.position - myTransform.position;
		float length = vectToTarget.magnitude / (springSettings.rangeBetweenChars*2);


		//A length of 0.5 is exactly right on the correct distance between chars. 0 means the distance between chars is 0.

		float fResult = 0;
		/*
		if(length < springSettings.repelPercentOnRange){
			fResult = springSettings.springCurve.Evaluate(length);
		}
		else
		if(length > springSettings.attractPercentOnRange){
			fResult = springSettings.springCurve.Evaluate(length);
		}*/

		fResult = springSettings.springCurve.Evaluate(length);

		//Debug.Log("Length in "+myTransform.name+" is: "+length+"fresult: "+fResult);

		int dir = 1;
		if(length < 0.5)
			dir = -1;

		vectToTarget.z = 0;

		//TODO
		float tempHack = 1;
		if(!onGround){
			tempHack = 2.75f;
			//Debug.Log("Not On Goround ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~`");
		}

		myTransform.rigidbody.AddForce(vectToTarget.normalized * dir *
		                               fResult * springSettings.scaleSpringForce * tempHack, 
		                               ForceMode.VelocityChange);
	}
}
