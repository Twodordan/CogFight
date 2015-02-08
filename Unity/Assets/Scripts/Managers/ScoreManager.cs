using UnityEngine;
using System.Collections;

public class ScoreManager : MonoBehaviour {

    private static ScoreManager singleton;

    public int playerLivesAtStart = 5;
    [HideInInspector]
    public int player1Lives, player2Lives;

    public AudioClip playerDeathSound;
    public AudioClip playerSwitchSound;
    private AudioSource managerSoundSource;

    GameObject player1Object, player2Object;

    void Awake() {
        singleton = this;
        managerSoundSource = gameObject.AddComponent<AudioSource>();
        managerSoundSource.loop = false;
        managerSoundSource.playOnAwake = false;
        managerSoundSource.bypassEffects = true;
        managerSoundSource.bypassListenerEffects = true;
        managerSoundSource.bypassReverbZones = true;
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

        managerSoundSource.PlayOneShot(playerDeathSound);
    }

    void OnPlayerSwitch() {
        managerSoundSource.PlayOneShot(playerSwitchSound);
    }

    Rect player1ScoreBox = new Rect(0, 0, 100, 40);
    Rect player2ScoreBox = new Rect(Screen.width - 100, 0, 100, 40);

    void OnGUI() {
        GUI.Label(player1ScoreBox, "Player 1 lives: " + player1Lives.ToString());
        GUI.Label(player2ScoreBox, "Player 2 lives: " + player2Lives.ToString());
    }
}
