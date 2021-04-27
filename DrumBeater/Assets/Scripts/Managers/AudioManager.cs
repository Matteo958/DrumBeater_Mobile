using AudioType = Audio.AudioType;
using UnityEngine;
using System.Collections;
using System;

public class AudioManager : MonoBehaviour
{
    private static AudioManager _instance;
    public static AudioManager instance { get => _instance; }

    // table who contains relationship between audiotypes(key) and tracks (value)
    private Hashtable _audioTable;
    // table who contains relationship between audiotypes(key) and jobs (value) (Coroutine)
    private Hashtable _jobTable;

    public AudioTrack[] tracks;

    private enum AudioAction
    {
        START,
        STOP,
        PAUSE,
        UNPAUSE,
        RESTART
    }

    [Serializable]
    public class AudioObject
    {
        public AudioType type;
        public AudioClip clip;
    }

    [Serializable]
    public class AudioTrack
    {
        public AudioSource source;
        public AudioObject[] audio;
    }

    private class AudioJob
    {
        public AudioAction action;
        public AudioType type;
        public bool fade;
        public float delay;

        public AudioJob(AudioAction action, AudioType type, bool fade, float delay)
        {
            this.action = action;
            this.type = type;
            this.fade = fade;
            this.delay = delay;
        }
    }


    private void Awake()
    {
        if (_instance != null && _instance != this)
            Destroy(this.gameObject);
        else
        {
            _instance = this;
            _audioTable = new Hashtable();
            _jobTable = new Hashtable();
            populateAudioTable();
        }
    }

    public void playAudio(AudioType type, bool fade = false, float delay = 0.0F)
    {
        addJob(new AudioJob(AudioAction.START, type, fade, delay));
    }

    public void restartAudio(AudioType type, bool fade = false, float delay = 0.0F)
    {
        addJob(new AudioJob(AudioAction.RESTART, type, fade, delay));
    }

    public void stopAudio(AudioType type, bool fade = false, float delay = 0.0F)
    {
        addJob(new AudioJob(AudioAction.STOP, type, fade, delay));
    }

    public void pauseAudio(AudioType type, bool fade = false, float delay = 0.0F)
    {
        addJob(new AudioJob(AudioAction.PAUSE, type, fade, delay));
    }

    public void unpauseAudio(AudioType type, bool fade = false, float delay = 0.0F)
    {
        addJob(new AudioJob(AudioAction.UNPAUSE, type, fade, delay));
    }

    private void populateAudioTable()
    {
        foreach (AudioTrack track in tracks)
        {
            foreach (AudioObject obj in track.audio)
            {
                // do not duplicate keys
                if (!_audioTable.ContainsKey(obj.type))
                    _audioTable.Add(obj.type, track);
            }
        }
    }

    private void addJob(AudioJob job)
    {
        RemoveConflictingJobs(job.type);

        IEnumerator jobRunner = RunAudioJob(job);
        _jobTable.Add(job.type, jobRunner);
        StartCoroutine(jobRunner);
    }

    private void RemoveJob(AudioType type)
    {
        if (!_jobTable.ContainsKey(type))
            return;
        
        IEnumerator runningJob = (IEnumerator)_jobTable[type];
        StopCoroutine(runningJob);
        _jobTable.Remove(type);
    }

    private void RemoveConflictingJobs(AudioType type)
    {
        // cancel the job if one exists with the same type
        if (_jobTable.ContainsKey(type))        
            RemoveJob(type);

        // cancel jobs that share the same audio track
        AudioType conflictAudio = AudioType.None;
        foreach (DictionaryEntry entry in _jobTable)
        {
            AudioType audioType = (AudioType)entry.Key;
            AudioTrack audioTrackInUse = GetAudioTrack(audioType, "Get Audio Track In Use");
            AudioTrack audioTrackNeeded = GetAudioTrack(type, "Get Audio Track Needed");
            if (audioTrackInUse.source == audioTrackNeeded.source)            
                conflictAudio = audioType;
            
        }
        if (conflictAudio != AudioType.None)        
            RemoveJob(conflictAudio);
        
    }

    private IEnumerator RunAudioJob(AudioJob job)
    {
        yield return new WaitForSeconds(job.delay);

        AudioTrack track = GetAudioTrack(job.type); // track existence should be verified by now
        track.source.clip = GetAudioClipFromAudioTrack(job.type, track);

        switch (job.action)
        {
            case AudioAction.START:
                track.source.Play();
                break;

            case AudioAction.STOP:
                if (!job.fade)
                    track.source.Stop();
                break;

            case AudioAction.RESTART:
                track.source.Stop();
                track.source.Play();
                break;

            case AudioAction.PAUSE:
                if (!job.fade)
                    track.source.Pause();
                break;

            case AudioAction.UNPAUSE:
                if (!job.fade)
                    track.source.UnPause();
                break;
        }

        // fade volume
        if (job.fade)
        {
            float initial = job.action == AudioAction.START || job.action == AudioAction.RESTART ? 0 : 1;
            float target = initial == 0 ? 1 : 0;
            float duration = 1.0f;
            float timer = 0.0f;

            while (timer < duration)
            {
                track.source.volume = Mathf.Lerp(initial, target, timer / duration);
                timer += Time.deltaTime;
                yield return null;
            }

            switch (job.action)
            {
                case AudioAction.STOP:
                    track.source.Stop();
                    break;

                case AudioAction.PAUSE:
                    track.source.Pause();
                    break;

                case AudioAction.UNPAUSE:
                    track.source.UnPause();
                    break;
            }
        }

        _jobTable.Remove(job.type);
        Debug.Log("Job count: " + _jobTable.Count);
    }

    private AudioTrack GetAudioTrack(AudioType type, string job = "")
    {
        if (!_audioTable.ContainsKey(type))
        {
            Debug.LogError("You are trying to <color=#fff>" + job + "</color> for [" + type + "] but no track was found supporting this audio type.");
            return null;
        }
        return (AudioTrack)_audioTable[type];
    }

    private AudioClip GetAudioClipFromAudioTrack(AudioType type, AudioTrack track)
    {
        foreach (AudioObject obj in track.audio)
        {
            if (obj.type == type)            
                return obj.clip;            
        }
        return null;
    }
}
