using UnityEngine;
using System.Collections;

public class ScoreManager : MonoBehaviour {

    private static ScoreManager singleton;

    public int playerLivesAtStart = 5;
    [HideInInspector]
    public int player1Lives, player2Lives;

    public AudioClip playerDeathSound;
    public AudioClip playerSwitchSound;
    private AudioSource playerDeathSource;
    private AudioSource playerSwitchSource;

    public AnimationCurve switchAnimationCurve = new AnimationCurve();

    GameObject player1Object, player2Object;

    void Awake() {
        singleton = this;

        playerDeathSource = gameObject.AddComponent<AudioSource>();
        playerDeathSource.loop = false;
        playerDeathSource.playOnAwake = false;
        playerDeathSource.bypassEffects = true;
        playerDeathSource.bypassListenerEffects = true;
        playerDeathSource.bypassReverbZones = true;
        playerDeathSource.volume = 0.2f;

        playerSwitchSource = gameObject.AddComponent<AudioSource>();
        playerSwitchSource.loop = false;
        playerSwitchSource.playOnAwake = false;
        playerSwitchSource.bypassEffects = true;
        playerSwitchSource.bypassListenerEffects = true;
        playerSwitchSource.bypassReverbZones = true;
        playerSwitchSource.volume = 0.35f;
    }

    void Start() {
        player1Lives = playerLivesAtStart;
        player2Lives = playerLivesAtStart;

        player1Object = GameObject.Find("PlayerChar2D_P1");
        player2Object = GameObject.Find("PlayerChar2D_P2");

        if (player1Object == null || player2Object == null) {
            Debug.LogError("Could not find one or more players! Player 1 mus be named 'PlayerChar2D_P1', and player 2 must be named 'PlayerChar2D_P2'");
        }

        EventManager.OnPlayerDeath += OnPlayerDeath;
        EventManager.OnPlayerSwitch += OnPlayerSwitch;
    }

    void OnPlayerDeath(int playerID) {
        if (playerID == 1) {
            player1Lives--;
        } else {
            player2Lives--;
        }

        playerDeathSource.PlayOneShot(playerDeathSound);

        if (player1Lives < 1 || player2Lives < 1) {
            StateManager.State = GameState.Ended;
        }
    }

    void OnPlayerSwitch() {
        playerSwitchSource.PlayOneShot(playerSwitchSound);

        StartCoroutine(SwitchAnimation());
    }

    IEnumerator SwitchAnimation() {
        //Debug.Log("SWITCH");

        float startTime = Time.time;

        Transform player1Pivot = player1Object.transform.GetChild(0);
        Vector3 player1StartScale = player1Pivot.localScale;
        Transform player2Pivot = player2Object.transform.GetChild(0);
        Vector3 player2StartScale = player2Pivot.localScale;

        while (Time.time < startTime + 0.3f) {
            float t = (Time.time - startTime) / 0.3f;
            player1Pivot.localScale = player1StartScale * (1 + 0.5f * switchAnimationCurve.Evaluate(t));
            player2Pivot.localScale = player2StartScale * (1 + 0.5f * switchAnimationCurve.Evaluate(t));
            yield return null;
        }

        player1Pivot.localScale = player1StartScale;
        player2Pivot.localScale = player2StartScale;
    }


    Rect player1ScoreBox = new Rect(Screen.width / 2f - 130, 0, 120, 40);
    Rect player2ScoreBox = new Rect(Screen.width / 2f + 10, 0, 120, 40);

    Rect centerRect = new Rect(Screen.width / 5f, Screen.height / 5f, Screen.width - 2 * Screen.width / 5f, Screen.height - 2 * Screen.height / 5f);

    void OnGUI() {
        if (StateManager.State != GameState.Ended) {
            GUI.Box(player1ScoreBox, "P1 (Red) lives: " + player1Lives.ToString());
            GUI.Box(player2ScoreBox, "P2 (Blue) lives: " + player2Lives.ToString());
        } else {
            if (player1Lives > player2Lives) {
                GUI.Box(centerRect, "Player 1 (Red) wins!");
            } else if (player1Lives == player2Lives) {
                GUI.Box(centerRect, "Draw!");
            } else {
                GUI.Box(centerRect, "Player 2 (Blue) wins!");
            }
        }
    }
}
