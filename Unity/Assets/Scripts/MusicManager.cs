using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MusicManager : MonoBehaviour {

    public float countdownToStart = 2f;

    List<AudioClip> musicTracks;


	void Start () {
        StartCoroutine(CountdownToStartOfGame(countdownToStart));
	}

    void FixedUpdate() {

    }

	void Update () {
	    
	}

    IEnumerator CountdownToStartOfGame(float time) {
        yield return new WaitForSeconds(time);

        EventManager.StartGame();

        yield break;
    }
}
