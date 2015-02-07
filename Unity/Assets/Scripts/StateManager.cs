using UnityEngine;
using System.Collections;

public enum GameState { Load, Beginning, Playing, Ended };

public class StateManager : MonoBehaviour {

    static StateManager singleton;
    public GameState state = GameState.Load;

    void Awake() {
        singleton = this;
    }

    public static GameState State {
        get { return singleton.state; }
        set { singleton.state = value; }
    }
}
