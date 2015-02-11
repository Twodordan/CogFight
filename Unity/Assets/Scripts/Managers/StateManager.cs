using UnityEngine;
using System.Collections;

public enum GameState { Beginning, Playing, Paused, Ended };

public enum StateFlags { NONE, ReadyForPlay, ReadyForEnd, ReadyForQuit };

public class StateManager : MonoBehaviour {

    static StateManager singleton;
    GameState state = GameState.Beginning;
    StateFlags flags = StateFlags.NONE;
    int difficulty = 0;

    void Awake() {
        singleton = this;
    }

    public static GameState State {
        get { return singleton.state; }
        set { singleton.state = value; singleton.ActivateStateChangeEvents(); Debug.Log("New game state: " + singleton.state); }
    }

    public static StateFlags Flags {
        get { return singleton.flags; }
        set { singleton.flags = value; Debug.Log("State flag set: " + singleton.flags); }
    }

    public static int CurrentDifficulty {
        get { return singleton.difficulty; }
        set { singleton.difficulty = value; EventManager.DifficultyChanged(); Debug.Log("New difficulty set: " + singleton.difficulty); }
    }

    void ActivateStateChangeEvents() {
        switch (state) {
            case GameState.Playing:
                EventManager.StartGame();
                break;
            case GameState.Ended:
                EventManager.EndGame();
                break;
            case GameState.Paused:
                EventManager.PauseGame();
                break;
        }
    }

    void SetStateToEnded() {
        State = GameState.Ended;
    }
}
