using System.Collections.Generic;
using UnityEngine;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Interaction;

public class Song : MonoBehaviour
{
    public int bpm;
    public AudioSource audioSource;
    public Audio.AudioType type;
    public static MidiFile midiFile;
    public float songDelayInSeconds;
    
    public string fileLocation;

    public static Song instance { get; private set; }

    private void Awake()
    {
        if (instance != null && instance != this)
            Destroy(this.gameObject);
        else        
            instance = this;
    }

    void Start()
    {
        ReadFromFile();
    }

    private void ReadFromFile()
    {
        midiFile = MidiFile.Read(Application.streamingAssetsPath + "/" + fileLocation);
    }
    public void GetDataFromMidi()
    {
        var notes = midiFile.GetNotes();
        Note[] array = new Melanchall.DryWetMidi.Interaction.Note[notes.Count];
        notes.CopyTo(array, 0);

        NoteSpawner.instance.setNotes(array,bpm);

        Invoke(nameof(StartSong), songDelayInSeconds);
    }
    public void StartSong()
    {
        NoteSpawner.instance.startSong(this);
    }
    public static float GetAudioSourceTime()
    {       
        return instance.audioSource.timeSamples / instance.audioSource.clip.frequency;
    }
}
