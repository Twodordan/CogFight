using UnityEngine;
using System.Collections;

public class QuitGame : MonoBehaviour {

    static QuitGame singleton = null;

    void Awake() {
        if (singleton == null) {
            singleton = this;
        }
    }

    void Start() {
        if (singleton == this) {
            DontDestroyOnLoad(this.gameObject);
        } else {
            Destroy(this.gameObject);
        }
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            Application.Quit();
        }
    }
}
