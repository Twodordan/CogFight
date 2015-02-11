using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class StartMenuManager : MonoBehaviour {

    Rect startGameButton = new Rect(Screen.width / 3.5f, Screen.height / 2.5f, Screen.width - 2 * Screen.width / 3.5f, Screen.height - 2 * Screen.height / 2.5f);

    public float audioTimer = 1f;

	public Texture backgroundTexture;

    void Awake() {
        audio.playOnAwake = false;
        audio.loop = false;
    }

    void OnGUI() {
		GUI.DrawTexture (new Rect (0, 0, Screen.width, Screen.height), backgroundTexture);

        if (GUI.Button(startGameButton, "\nTHERE IS ONLY ONE BUTTON\nAND IT STARTS THE GAME\n\n" +
        	"..but your controller also has buttons.\nLike Enter or Start.")
		    || Input.GetButtonDown("Start")
		    ) {
            audio.Play();
            StartCoroutine(LoadLevelDelayed(audioTimer));
        }
    }

    IEnumerator LoadLevelDelayed(float delay) {
        yield return new WaitForSeconds(delay);
        Application.LoadLevel("MainGame");
    }

    void OnDestroy() {
        StopAllCoroutines();
    }
}
