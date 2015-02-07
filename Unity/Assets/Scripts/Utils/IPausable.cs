using UnityEngine;
using System.Collections;

public class IPausable : MonoBehaviour
{
	protected bool isPaused;
	
	public virtual void Pause(bool pause)
	{
		isPaused = pause;
	}
}
