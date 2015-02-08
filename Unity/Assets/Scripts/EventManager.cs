using UnityEngine;
using System.Collections;

public class EventManager : MonoBehaviour {

    public delegate void BasicAction();
    public static event BasicAction OnGameStart;
    public static event BasicAction OnGameEnd;
    public static event BasicAction OnGamePause;
    public static event BasicAction OnDifficultyChange;

    public delegate void IntegerAction(int n);
    public static event IntegerAction OnMusic_Beat;
    public static event BasicAction OnMusic_Bar;

    public delegate void ForeshadowAction(int id, double timeToConclusion);
    public static event ForeshadowAction OnMusic_ForeshadowBegin;
    public static event ForeshadowAction OnMusic_ForeshadowConclusion;

    public delegate void PlayerDeathAction(int playerID);
    public static event PlayerDeathAction OnPlayerDeath;

    public delegate void AudioStartAction(double syncTime, double clipLength);
    public static event AudioStartAction OnMusic_StartNewClip;

    public static void StartGame() {
        if (OnGameStart != null) {
            OnGameStart();
        }
    }

    public static void EndGame() {
        if (OnGameEnd != null) {
            OnGameEnd();
        }
    }

    public static void PauseGame() {
        if (OnGamePause != null) {
            OnGamePause();
        }
    }

    public static void DifficultyChanged() {
        if (OnDifficultyChange != null) {
            OnDifficultyChange();
        }
    }

    public static void Music_NewClip(double syncTime, double clipLength) {
        if (OnMusic_StartNewClip != null) {
            OnMusic_StartNewClip(syncTime, clipLength);
        }
    }

    public static void Music_Beat(int n) {
        if (StateManager.State == GameState.Playing) {
            if (OnMusic_Beat != null) {
                OnMusic_Beat(n);
            }
        }
    }

    public static void Music_Bar() {
        if (StateManager.State == GameState.Playing) {
            if (OnMusic_Bar != null) {
                OnMusic_Bar();
            }
        }
    }

    public static void Music_ForeshadowBegin(int id, double time) {
        if (OnMusic_ForeshadowBegin != null) {
            OnMusic_ForeshadowBegin(id, time);
        }
    }

    public static void Music_ForeshadowConclusion(int id, double time) {
        if (OnMusic_ForeshadowConclusion != null) {
            OnMusic_ForeshadowConclusion(id, time);
        }
    }

    public static void PlayerDeath(int playerID) {
        if (OnPlayerDeath != null) {
            OnPlayerDeath(playerID);
        }
    }
}
