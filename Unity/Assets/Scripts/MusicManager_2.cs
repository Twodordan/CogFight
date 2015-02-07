using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(AudioSource))]
public class MusicManager_2 : MonoBehaviour {

    public float musicDelay = 0.5f;

    public MusicWithInformation beginTrack;
    public MusicWithInformation endTrack;

    public List<MusicWithInformation> trackList;

    MusicWithInformation currentlyPlayingTrack;
    List<AudioSource> sources = new List<AudioSource>();

    int beatNumber = 0;
    double lastBeatTime = 0;


    IEnumerator Start() {
        

        yield return new WaitForSeconds(musicDelay);

        audio.playOnAwake = false;
        audio.loop = false;
        sources.Add(audio);

        while (sources.Count < 2) {
            AudioSource newAudio = gameObject.AddComponent<AudioSource>();
            SyncSourceSettings(audio, ref newAudio);
            sources.Add(newAudio);
        }

        EventManager.OnMusic_StartNewClip += (double syncTime, double clipLength) => { StartCoroutine(OnNewClip(syncTime, clipLength)); };
        EventManager.OnMusic_Beat += () => { beatNumber++; };

        
        double initTime = AudioSettings.dspTime;
        sources[0].clip = beginTrack.clip;
        sources[0].PlayScheduled(initTime);
        beginTrack.initTime = initTime;
        currentlyPlayingTrack = beginTrack;

        EventManager.Music_NewClip(initTime, sources[0].clip.length); // Declare that we are starting the beginTrack

        yield break;
    }

    IEnumerator OnNewClip(double syncTime, double clipLength) {
        double initTime = syncTime + clipLength;

        MusicWithInformation newTrack = trackList[0];
        AudioSource newSource = sources[1];
        newSource.clip = newTrack.clip;
        newSource.PlayScheduled(initTime);
        newTrack.initTime = initTime;

        Debug.Log("New clip queued! Name: " + newTrack.name);

        CycleList(ref sources);
        CycleList(ref trackList);

        yield return new WaitForSeconds((float)(initTime - AudioSettings.dspTime - Time.fixedDeltaTime));

        beatNumber = 0;
        currentlyPlayingTrack = newTrack;
        EventManager.Music_NewClip(initTime, newTrack.clip.length);

        yield break;
    }

    void FixedUpdate() {
        if (currentlyPlayingTrack != null) {
            double currentBeatDuration = 60.0 / (currentlyPlayingTrack.BPM);
            if (AudioSettings.dspTime - 0.05 > currentlyPlayingTrack.initTime + currentBeatDuration * beatNumber) {
                beatNumber++;
                if (beatNumber % 4 == 0) {
                    EventManager.Music_Bar();
                }
                EventManager.Music_Beat();
            }
        }
    }

    void OnGUI() {
        GUI.Label(new Rect(0, 0, 100, 100), ((beatNumber - 1) / 4 + 1).ToString());
    }

    public static void CycleList<T>(ref List<T> list) {
        list.Add(list[0]);
        list.RemoveAt(0);
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
    public float chanceWeight = 1f;

    public double initTime;
}
