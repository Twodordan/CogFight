using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(AudioSource))]
public class MusicManager : MonoBehaviour {

    public float musicDelay = 0.25f;
    public float countdownToStart = 2f;

    public MusicWithInformation beginningTrack;
    public MusicWithInformation endingTrack;

    [SerializeField]
    public List<MusicWithInformation> musicTracks;

    List<AudioSource> baseTrackQueue = new List<AudioSource>();
    //List<AudioSource> variationTrackQueue;

    void Awake() {
        audio.Stop();
        audio.playOnAwake = false;
    }

	IEnumerator Start () {
        yield return new WaitForSeconds(musicDelay);

        double initTime = AudioSettings.dspTime;
        EventManager.OnMusic_StartNewClip += OnNewClipDebug;
        EventManager.OnMusic_StartNewClip += (double syncTime, double clipLength) => { StartCoroutine(OnNewClipStart(syncTime, clipLength)); };

        audio.Stop();
        audio.loop = false;
        audio.playOnAwake = false;
        
        baseTrackQueue.Add(audio);

        yield return null;

        //AudioSource startTrackSource = AudioSource.Instantiate(originalSource) as AudioSource;
        AudioSource startTrackSource = CurrentBaseSource();
        startTrackSource.clip = beginningTrack.clip;
        startTrackSource.Play();

        EventManager.Music_NewClip(initTime, startTrackSource.clip.length);
	}

    void OnNewClipDebug(double syncTime, double clipLength) {
        Debug.Log("MUSIC START! SyncTime: " + syncTime + ", Clip length: " + clipLength);
    }

    IEnumerator OnNewClipStart(double syncTime, double clipLength) {
        AudioSource newTrackSource = (AudioSource)gameObject.AddComponent<AudioSource>();
        newTrackSource.Stop();
        newTrackSource.playOnAwake = false;
        SyncSourceSettings(CurrentBaseSource(), ref newTrackSource);

        newTrackSource.clip = beginningTrack.clip;
        baseTrackQueue.Add(newTrackSource);

        newTrackSource.PlayScheduled(syncTime + clipLength);

        yield return new WaitForSeconds((float)clipLength);

        EventManager.Music_NewClip(syncTime + clipLength, newTrackSource.clip.length);
    }

    void FixedUpdate() {
        TrimBaseTrackQueue(); // Remove audio sources that are no longer active
    }

    //void FixedUpdate() {
    //    if (BeatThisFixedUpdate()) {
    //        EventManager.Music_Beat();
    //    }

    //    if (BarThisFixedUpdate()) {
    //        EventManager.Music_Bar();
    //    }
    //}

    //bool BeatThisFixedUpdate() {

    //}

    //bool BarThisFixedUpdate() {

    //}

    AudioSource CurrentBaseSource() {
        return baseTrackQueue[0];
    }

    void TrimBaseTrackQueue() {
        if (baseTrackQueue.Count > 1) {
            if (CurrentBaseSource().isPlaying == false) {
                Destroy(CurrentBaseSource());
                baseTrackQueue.Remove(CurrentBaseSource());
            }
        }
    }

    void QueueTrack(MusicWithInformation track) {
        
    }

    public static void SyncSourceSettings(AudioSource original, ref AudioSource next) {
        next.loop = original.loop;
        next.volume = original.volume;
        next.playOnAwake = original.playOnAwake;
        next.ignoreListenerVolume = original.ignoreListenerVolume;
        next.ignoreListenerPause = original.ignoreListenerPause;
        next.bypassReverbZones = original.bypassReverbZones;
        next.bypassListenerEffects = original.bypassListenerEffects;
        next.bypassEffects = original.bypassEffects;
    }
}

[System.Serializable]
public class MusicWithInformation {
    public string name = "";
    public AudioClip clip;
    public float BPM = 120f;

    //float lastBeatCheck = -1f;
    //float lastBarCheck = -1f;

    //public bool CheckForBeat(float window, AudioSource player) {

    //}

    //public bool CheckForBar(float window, AudioSource player) {
        
    //}
}
