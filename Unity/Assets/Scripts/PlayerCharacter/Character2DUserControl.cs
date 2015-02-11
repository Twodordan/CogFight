using UnityEngine;
using System.Collections;

public class Character2DUserControl{




	public void solveInput(PlayerNumber playerNmber, 
	                       Transform transform, bool looksAtTarget, 
	                       Transform lookAtTarget, 
	                       out Vector3 lookPos, out Vector3 move, out bool jump
	                       )
	{


		//maybe we don't have crouch
		//bool crouch = Input.GetKey(KeyCode.LeftControl) || Input.GetButton("Crouch");
		
		
		//jump = Input.GetButton("Jump_"+(int)playerNmber);
		float h = Input.GetAxis("Horizontal_"+(int)playerNmber);
		float v = Input.GetAxis("Vertical_"+(int)playerNmber);

		if(v > 0){
			jump = true;
		}
		else{
			jump = false;
		}


		move = v * Vector3.up + h * Vector3.right;
		
		
		if (move.magnitude > 1) move.Normalize();
		
		
		//calculate the head look target position, if we want the character to look at the other char
		lookPos = looksAtTarget && lookAtTarget != null
				? lookAtTarget.position
				: transform.position + transform.forward * 100;
		

	}
		


}
