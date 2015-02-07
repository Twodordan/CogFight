using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MusicManager : MonoBehaviour {

    public float countdownToStart = 2f;

    public MusicWithInformation beginningTrack;
    public MusicWithInformation endingTrack;
    
    [SerializeField]
    public List<MusicWithInformation> musicTracks;


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

[System.Serializable]
public class MusicWithInformation {
    public string name = "";
    public AudioClip musicTrack;
    public float BPM = 120f;

}
