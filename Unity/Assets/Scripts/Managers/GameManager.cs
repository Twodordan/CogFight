using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {

    public static GameManager singleton;

    public float secondsPerDifficultyIncrease = 20f;
    public float currentDifficulty = 0f;
    public static int DifficultyLevel { get { return Mathf.FloorToInt(singleton.currentDifficulty); } }

    float startTime = 0f;

    void Awake() {
        singleton = this;
    }

    IEnumerator Start() {
        EventManager.OnGameStart += SetStartTime;

        //StateManager.Flags = StateFlags.ReadyForPlay;
        yield break;
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.Space)) {
            StateManager.Flags = StateFlags.ReadyForPlay;
        }
    }

    void FixedUpdate() {
        if (StateManager.State == GameState.Playing) {
            int diff = Mathf.FloorToInt(currentDifficulty);
            currentDifficulty = (Time.time - startTime) / secondsPerDifficultyIncrease;

            if (diff != Mathf.FloorToInt(currentDifficulty)) {
                EventManager.DifficultyChanged();
            }
        }
    }

    void SetStartTime() {
        startTime = Time.time;
    }
}
