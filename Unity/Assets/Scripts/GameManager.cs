using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {

    public float secondsPerDifficultyIncrease = 20f;
    public float currentDifficulty = 0f;

    float startTime = 0f;

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
        currentDifficulty = (Time.time - startTime) / secondsPerDifficultyIncrease;
    }

    void SetStartTime() {
        startTime = Time.time;
    }
}
