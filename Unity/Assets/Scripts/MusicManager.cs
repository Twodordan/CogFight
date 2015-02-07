using UnityEngine;
using System.Collections;
using System.Collections.Generic;



[RequireComponent(typeof(AudioSource))]
public class MusicManager : MonoBehaviour {

    public float musicDelay = 1f;
    public float countdownToStart = 3f;

    public MusicWithInformation beginningTrack;
    public MusicWithInformation endingTrack;

    [SerializeField]
    public List<MusicWithInformation> musicTracks;

    List<AudioSource> baseTrackQueue = new List<AudioSource>();
    //List<AudioSource> variationTrackQueue;

    double currentBaseTrackStartTime = -1f;
    double currentBaseTrackBPM = 120f;

    void Awake() {
        audio.Stop();
        audio.playOnAwake = false;
    }

	IEnumerator Start () {
        yield return new WaitForSeconds(musicDelay);

        double initTime = AudioSettings.dspTime;
        EventManager.OnMusic_StartNewClip += OnNewClipDebug;
        EventManager.OnMusic_StartNewClip += (double syncTime, double clipLength) => { StartCoroutine(OnNewClipStart(syncTime, clipLength)); };
        EventManager.OnMusic_Beat += OnBeatDebug;

        audio.Stop();
        audio.loop = false;
        audio.playOnAwake = false;
        
        baseTrackQueue.Add(audio);

        yield return null;

        //AudioSource startTrackSource = AudioSource.Instantiate(originalSource) as AudioSource;
        AudioSource startTrackSource = CurrentBaseSource();
        startTrackSource.clip = beginningTrack.clip;
        startTrackSource.Play();

        currentBaseTrackStartTime = initTime;
        currentBaseTrackBPM = beginningTrack.BPM;
        EventManager.Music_NewClip(initTime, startTrackSource.clip.length);
	}

    void OnNewClipDebug(double syncTime, double clipLength) {
        Debug.Log("MUSIC START! SyncTime: " + syncTime + ", Clip length: " + clipLength);
    }

    int beatNumber = 0;

    void OnBeatDebug() {
        Debug.Log("--> BEAT <--");
        beatNumber++;
    }

    void OnGUI() {
        GUI.Label(new Rect(0, 0, 100, 100), beatNumber.ToString());
    }

    IEnumerator OnNewClipStart(double syncTime, double clipLength) {
        // Create new audio source and set its options
        AudioSource newTrackSource = (AudioSource)gameObject.AddComponent<AudioSource>();
        newTrackSource.Stop();
        newTrackSource.playOnAwake = false;
        SyncSourceSettings(CurrentBaseSource(), ref newTrackSource);

        // Link the right clip and add to queue
        MusicWithInformation newMusic = ChooseTrackFromList(musicTracks);
        newTrackSource.clip = newMusic.clip;
        baseTrackQueue.Add(newTrackSource);

        // Schedule the play time
        newTrackSource.PlayScheduled(syncTime + clipLength);

        // Wait until the end of this clip...
        yield return new WaitForSeconds((float)clipLength);

        // ... and tell the system that a new clip is starting.
        currentBaseTrackStartTime = syncTime + clipLength;
        currentBaseTrackBPM = newMusic.BPM;
        //lastBeatTime = syncTime + clipLength;
        EventManager.Music_NewClip(syncTime + clipLength, newTrackSource.clip.length);
        //TrimBaseTrackQueue();
    }

    void FixedUpdate() {
         // Remove audio sources that have played out

        //if (BeatThisFixedUpdate()) {
        //    EventManager.Music_Beat();
        //}

        //if (BarThisFixedUpdate()) {
        //    EventManager.Music_Bar();
        //}
    }

    //double lastBeatTime = -1f;

    //bool BeatThisFixedUpdate() {
    //    double beatInterval = 60.0 / currentBaseTrackBPM;

    //    if (AudioSettings.dspTime > lastBeatTime + beatInterval) {
    //        lastBeatTime = AudioSettings.dspTime;
    //        return true;
    //    }
    //    return false;
    //}

    //bool BarThisFixedUpdate() {

    //}

    MusicWithInformation ChooseTrackFromList(List<MusicWithInformation> trackList) {
        if (trackList.Count == 1) {
            return trackList[0];
        }

        float sumOfChances = 0;
        foreach (MusicWithInformation mwi in trackList) {
            sumOfChances += mwi.chanceWeight;
        }

        float randomChoice = Random.Range(0f, sumOfChances);
        float reachedSum = 0;
        MusicWithInformation result = null;

        for (int i = 0; i < trackList.Count; i++) {
            reachedSum += trackList[i].chanceWeight;

            if (reachedSum >= randomChoice) {
                result = trackList[i];
                break;
            }
        }

        return result;
    }

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

//[System.Serializable]
//public class MusicWithInformation {
//    public string name = "";
//    public AudioClip clip;
//    public float BPM = 120f;
//    public float chanceWeight = 1f;

    //float lastBeatCheck = -1f;
    //float lastBarCheck = -1f;

    //public bool CheckForBeat(float window, AudioSource player) {

    //}

    //public bool CheckForBar(float window, AudioSource player) {
        
    //}
//}
