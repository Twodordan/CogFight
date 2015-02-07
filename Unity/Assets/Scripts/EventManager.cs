using UnityEngine;
using System.Collections;

public class EventManager : MonoBehaviour {

    public delegate void BasicAction();
    public static event BasicAction OnGameStart;
    public static event BasicAction OnGameEnd;

    public static void StartGame() {
        OnGameStart();
    }

    public static void EndGame() {
        OnGameEnd();
    }
}
