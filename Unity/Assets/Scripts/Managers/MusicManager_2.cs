﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum AudioCueType { UNDEFINED, BaseTrack, Foreshadow_Short, Foreshadow_Long };

[RequireComponent(typeof(AudioSource))]
public class MusicManager_2 : MonoBehaviour {

    public float musicDelay = 0.5f;

    private int beginTrackIndex = 0;
    public List<MusicWithInformation> beginTracks;
    private int endTrackIndex = 0;
    public List<MusicWithInformation> endTracks;

    public List<MusicWithInformation> trackList;
    public List<MusicWithInformation> foreshadowTracksLong;
    public List<MusicWithInformation> foreshadowTracksShort;

    MusicWithInformation currentlyPlayingTrack;
    List<MusicWithInformation> currentTrackQueue = new List<MusicWithInformation>();

    List<AudioSource> sources = new List<AudioSource>();
    List<AudioSourceController> foreshadowSourceControllers = new List<AudioSourceController>();

    double initOfNextClip = 0.0;

    int beatNumber = 0;
    double lastBeatTime = 0;
    int barNumber = 0;

    int foreshadowID = 0;

    Coroutine newClipWaitingRoutine;

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

        EventManager.OnMusic_StartNewClip += (double syncTime, double clipLength) => { newClipWaitingRoutine = StartCoroutine(OnNewClip(syncTime, clipLength)); };
        EventManager.OnGameEnd += ChangeFastToEndingTracks;
        EventManager.OnTerminateLevel += OnTerminateLevel;

        double initTime = AudioSettings.dspTime;
        EventManager.Music_NewClip(initTime, 0); // Declare that we are starting queuing

        yield break;
    }

    IEnumerator OnNewClip(double syncTime, double clipLength) {
        double initTime = syncTime + clipLength;
        yield return new WaitForSeconds((float)(initTime - AudioSettings.dspTime - Time.fixedDeltaTime * 1.5f));
        MusicWithInformation newTrack = GetNextBaseTrack();
        if (newTrack != null) {

            AudioSource newSource = sources[0];
            newSource.clip = newTrack.clip;
            newSource.volume = newTrack.volume;
            newSource.PlayScheduled(initTime);
            newSource.SetScheduledStartTime(initTime);
            newSource.SetScheduledEndTime(initTime + newSource.clip.length);
            newTrack.initTime = initTime;

            Debug.Log("New clip queued! Name: " + newTrack.name);

            CycleList(ref sources);

            initOfNextClip = initTime;

            beatNumber = 0;
            currentlyPlayingTrack = newTrack;

            EventManager.Music_NewClip(initTime, newTrack.clip.length);
            yield break;
        } else {
            Debug.Log("Returned null track");
            Application.LoadLevel("StartMenu");
        }
    }

    double GetTimeUntilNextClip() {
        return initOfNextClip - AudioSettings.dspTime - Time.fixedDeltaTime;
    }

    MusicWithInformation GetNextBaseTrack() {
        MusicWithInformation result = null;

        switch (StateManager.State) {
            case GameState.Beginning:
                Debug.Log("Flags here: " + StateManager.Flags);
                if (StateManager.Flags == StateFlags.ReadyForPlay) {
                    EventManager.OnMusic_StartNewClip += QueuedEventSetStatePlaying;
                    result = trackList[0];
                    CycleList(ref trackList);
                } else {
                    if (beginTrackIndex > beginTracks.Count - 1) {
                        beginTrackIndex = 0;
                    }
                    result = beginTracks[beginTrackIndex];
                    beginTrackIndex++;
                }
                break;

            case GameState.Playing:
                result = trackList[0];
                CycleList(ref trackList);
                break;

            case GameState.Ended:
                if (endTrackIndex > endTracks.Count - 1) {
                    result = null;
                } else {
                    result = endTracks[endTrackIndex];
                    endTrackIndex++;
                }
                break;
        }

        if (result == null) {
            return null;
        }

        return result.Copy();
    }

    double GetTimeOfNextBar() {
        double currentBeatDuration = 60.0 / (currentlyPlayingTrack.BPM);
        double initTime = currentlyPlayingTrack.initTime + (beatNumber + (4 - beatNumber % 4)) * currentBeatDuration;
        return initTime;
    }

    void ChangeFastToEndingTracks() {
        StopCoroutine(newClipWaitingRoutine);

        double initTime = GetTimeOfNextBar();
        MusicWithInformation newTrack = GetNextBaseTrack();

        foreach (AudioSource source in sources) {
            source.SetScheduledEndTime(initTime);
        }

        AudioSource newSource = sources[0];
        newSource.clip = newTrack.clip;
        newSource.volume = newTrack.volume;
        newSource.PlayScheduled(initTime);
        newSource.SetScheduledStartTime(initTime);



        newSource.SetScheduledEndTime(initTime + newSource.clip.length);
        newTrack.initTime = initTime;

        Debug.Log("New clip interrupt! Name: " + newTrack.name);

        initOfNextClip = initTime;

        beatNumber = 0;
        currentlyPlayingTrack = newTrack;

        EventManager.Music_NewClip(initTime, newTrack.clip.length);
    }

    public void StartForeshadowing(AudioCueType cueType) {
        if (StateManager.State == GameState.Playing) {
            AudioSource foreshadow = gameObject.AddComponent<AudioSource>();

            SyncSourceSettings(audio, ref foreshadow);
            MusicWithInformation foreshadowTrack;

            if (cueType == AudioCueType.Foreshadow_Long) {
                foreshadowTrack = foreshadowTracksLong[0];
                CycleList(ref foreshadowTracksLong);
            } else if (cueType == AudioCueType.Foreshadow_Short) {
                foreshadowTrack = foreshadowTracksShort[0];
                CycleList(ref foreshadowTracksShort);
            } else {
                return;
            }

            foreshadow.clip = foreshadowTrack.clip;
            foreshadow.volume = foreshadowTrack.volume;

            double initTime = GetTimeOfNextBar();
            foreshadow.PlayScheduled(initTime);
            foreshadow.SetScheduledStartTime(initTime);
            AudioSourceController controller = new AudioSourceController(initTime, foreshadow, foreshadowTracksLong[0].cueType);
            foreshadowSourceControllers.Add(controller);

            int id = foreshadowID++;

            //EventManager.Music_ForeshadowBegin(id, foreshadow.clip.length);

            double currentBeatDuration = 60.0 / (currentlyPlayingTrack.BPM);

            if (cueType == AudioCueType.Foreshadow_Long) {
                StartCoroutine(CallForeshadowBegin(initTime - AudioSettings.dspTime, id, currentBeatDuration * 8));
                StartCoroutine(CallForeshadowEvent(initTime - AudioSettings.dspTime + currentBeatDuration * 8, id, foreshadow.clip.length));
            } else if (cueType == AudioCueType.Foreshadow_Short) {
                StartCoroutine(CallForeshadowBegin(initTime - AudioSettings.dspTime, id, currentBeatDuration * 4));
                StartCoroutine(CallForeshadowEvent(initTime - AudioSettings.dspTime + currentBeatDuration * 4, id, foreshadow.clip.length));
            } else if (controller.cueType == AudioCueType.UNDEFINED) {
                Debug.LogWarning("Tried to activate a foreshadow countdown but the clip had the UNDEFINED cue type");
            }
        }
    }

    // DEBUG
    void Update() {
        TrimForeshadowSourceControllers();

        if (StateManager.State != GameState.Ended) {
            if (Input.GetButtonDown("Back")) {
                StateManager.State = GameState.Ended;
            }
        }
		else
		if (Input.GetButtonDown("Back") || Input.GetButtonDown("Start")) {
			//it really sucks to wait after the game ends -- particularly if 
			//you don't know that the game will auto-restart in a few seconds
			Debug.Log("Force-Closed Game!");
			Application.LoadLevel("StartMenu");
		}

    }

    IEnumerator CallForeshadowBegin(double countdown, int id, double durationOfClip) {
        yield return new WaitForSeconds((float)countdown);
        EventManager.Music_ForeshadowBegin(id, durationOfClip);
        yield break;
    }

    IEnumerator CallForeshadowEvent(double countdown, int id, double durationOfClip) {
        yield return new WaitForSeconds((float)countdown);
        EventManager.Music_ForeshadowConclusion(id, durationOfClip);
        yield break;
    }

    void FixedUpdate() {
        if (currentlyPlayingTrack != null) {
            double currentBeatDuration = 60.0 / (currentlyPlayingTrack.BPM);
            if (AudioSettings.dspTime - 0.05 > currentlyPlayingTrack.initTime + currentBeatDuration * beatNumber) {
                beatNumber++;
                if (beatNumber % 4 == 1) {
                    EventManager.Music_Bar();
                }
                EventManager.Music_Beat((beatNumber - 1) % 4);
                lastBeatTime = currentlyPlayingTrack.initTime + currentBeatDuration * beatNumber;
            }
        }
    }

    void TrimForeshadowSourceControllers() {
        List<AudioSourceController> removeList = new List<AudioSourceController>();

        foreach (AudioSourceController asc in foreshadowSourceControllers) {
            if (AudioSettings.dspTime > asc.initTime + Time.fixedDeltaTime) {
                if (!asc.source.isPlaying) {
                    removeList.Add(asc);
                }
            }
        }

        foreach (AudioSourceController asc in removeList) {
            foreshadowSourceControllers.Remove(asc);
            Destroy(asc.source);
        }

    }

    double GetNextBeatTime() {
        double result = 0.0;
        double currentBeatDuration = 60.0 / (currentlyPlayingTrack.BPM);
        result = lastBeatTime + currentBeatDuration;
        return result;
    }

    void QueuedEventSetStatePlaying(double time, double clipLength) {
        if (StateManager.State == GameState.Beginning) {
            StateManager.State = GameState.Playing;
            EventManager.OnMusic_StartNewClip -= QueuedEventSetStatePlaying;
        }
    }

    public static void CycleList<T>(ref List<T> list) {
        list.Add(list[0]);
        list.RemoveAt(0);
    }

    void OnTerminateLevel() {
        StopAllCoroutines();
        Destroy(this);
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
    public float BPM = 144f;
    public float volume = 1f;

    public float chanceWeight = 1f;

    [HideInInspector]
    public double initTime;
    [HideInInspector]
    public AudioCueType cueType;

    public MusicWithInformation Copy() {
        MusicWithInformation result = new MusicWithInformation();
        result.name = name;
        result.clip = clip;
        result.BPM = BPM;
        result.volume = volume;
        result.chanceWeight = chanceWeight;
        result.cueType = cueType;

        return result;
    }
}

public class AudioSourceController {
    public AudioSource source;
    public double initTime;

    [HideInInspector]
    public AudioCueType cueType;

    public AudioSourceController(double initTime, AudioSource source, AudioCueType type) {
        this.source = source;
        this.initTime = initTime;
        cueType = type;
    }

    public AudioSourceController(double initTime, AudioClip clip, AudioSource copySettings, AudioCueType type) {
        MusicManager_2.SyncSourceSettings(copySettings, ref source);
        source.clip = clip;
        this.initTime = initTime;
        cueType = type;
    }
}
