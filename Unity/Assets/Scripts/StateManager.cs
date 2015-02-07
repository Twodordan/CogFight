using UnityEngine;
using System.Collections;

public enum GameState { Beginning, Playing, Ended };

public class StateManager : MonoBehaviour {

    static StateManager singleton;
    GameState state = GameState.Beginning;

    void Awake() {
        singleton = this;
        EventManager.OnGameStart += SetStateToPlaying;
        EventManager.OnGameEnd += SetStateToEnded;
    }

    public static GameState State {
        get { return singleton.state; ; }
        set { singleton.state = value; Debug.Log("New game state: " + singleton.state); }
    }

    void SetStateToPlaying() {
        State = GameState.Playing;
    }

    void SetStateToEnded() {
        State = GameState.Ended;
    }
}
