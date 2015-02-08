using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class StartMenuManager : MonoBehaviour {

    Rect startGameButton = new Rect(Screen.width / 5f, Screen.height / 5f, Screen.width - 2 * Screen.width / 5f, Screen.height - 2 * Screen.height / 5f);

    public float audioTimer = 1f;

    void Awake() {
        audio.playOnAwake = false;
        audio.loop = false;
    }

    void OnGUI() {
        if (GUI.Button(startGameButton, "THERE IS ONLY ONE BUTTON\nAND IT STARTS THE GAME")) {
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
