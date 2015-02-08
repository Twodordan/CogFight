using UnityEngine;
using System.Collections;

public class QuitGame : MonoBehaviour {

    void Start() {
        DontDestroyOnLoad(this.gameObject);
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            Application.Quit();
        }
    }
}
