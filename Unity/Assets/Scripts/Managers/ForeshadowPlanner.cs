using UnityEngine;
using System.Collections;

[RequireComponent(typeof(MusicManager_2))]
public class ForeshadowPlanner : MonoBehaviour {

    public float avgTimeBetweenForeshadows = 0.2f;
    public float varyTimeBetweenForeshadows = 0.075f;

    public float shortToLongForeshadowWeight01 = 0.25f;

    float startTimeStamp = 0f;

    float lastForeshadow = 0f;
    float nextForeshadow = 0f;

    MusicManager_2 musicManager;

    void Start() {
        EventManager.OnGameStart += Initialize;
    }

    void Initialize() {
        startTimeStamp = Time.time;
        lastForeshadow = Time.time;
        nextForeshadow = Time.time;

        musicManager = GetComponent<MusicManager_2>();
    }

    void FixedUpdate() {
        if (StateManager.State == GameState.Playing) {
            if (Time.time > nextForeshadow) {
                lastForeshadow = Time.time;

                nextForeshadow = Time.time + avgTimeBetweenForeshadows - 0.5f * varyTimeBetweenForeshadows + Random.Range(0f, 1f) * varyTimeBetweenForeshadows;

                if (Random.Range(0f, 1f) < shortToLongForeshadowWeight01) {
                    musicManager.StartForeshadowing(AudioCueType.Foreshadow_Long);
                } else {
                    musicManager.StartForeshadowing(AudioCueType.Foreshadow_Short);
                }
            }
        }
    }
}
