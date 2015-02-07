using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {

    public float secondsPerDifficultyIncrease = 20f;
    public float currentDifficulty = 0f;

    float startTime = 0f;

    void Start() {
        EventManager.OnGameStart += SetStartTime;
    }

    void FixedUpdate() {
        currentDifficulty = (Time.time - startTime) / secondsPerDifficultyIncrease;
    }

    void SetStartTime() {
        startTime = Time.time;
    }
}
