using UnityEngine;
using System.Collections;

public class EventManager : MonoBehaviour {

    public delegate void BasicAction();
    public static event BasicAction OnGameStart;
    public static event BasicAction OnGameEnd;
    public static event BasicAction OnDifficultyChange;

    public static event BasicAction OnMusic_Beat;
    public static event BasicAction OnMusic_Bar;

    public delegate void PlayerDeathAction(int playerID);
    public static event PlayerDeathAction OnPlayerDeath;
    

    public static void StartGame() {
        OnGameStart();
    }

    public static void EndGame() {
        OnGameEnd();
    }

    public static void DifficultyChanged() {
        OnDifficultyChange();
    }

    public static void Music_Beat() {
        OnMusic_Beat();
    }

    public static void Music_Bar() {
        OnMusic_Bar();
    }

    public static void PlayerDeath(int playerID) {
        OnPlayerDeath(playerID);
    }
}
