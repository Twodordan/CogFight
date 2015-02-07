using UnityEngine;
using System.Collections;

public enum GameState { Beginning, Playing, Paused, Ended };

public class StateManager : MonoBehaviour {

    static StateManager singleton;
    GameState state = GameState.Beginning;
    int difficulty = 0;

    void Awake() {
        singleton = this;
        EventManager.OnGameStart += SetStateToPlaying;
        EventManager.OnGameEnd += SetStateToEnded;
    }

    public static GameState State {
        get { return singleton.state; ; }
        set { singleton.state = value; Debug.Log("New game state: " + singleton.state); }
    }

    public static int CurrentDifficulty {
        get { return singleton.difficulty; }
        set { singleton.difficulty = value; EventManager.DifficultyChanged(); Debug.Log("New difficulty set: " + singleton.difficulty); }
    }

    void SetStateToPlaying() {
        State = GameState.Playing;
    }

    void SetStateToEnded() {
        State = GameState.Ended;
    }
}
