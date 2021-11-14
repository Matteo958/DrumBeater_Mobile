using System.Collections.Generic;
using UnityEngine;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Interaction;

public class Song : MonoBehaviour
{
    public int bpm;
    public Audio.AudioType type;
    public MidiFile midiFile;
    public float songDelayInSeconds;
    
    public string fileLocation;
    
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
        Debug.Log(midiFile != null);
        var notes = midiFile.GetNotes();
        Note[] array = new Melanchall.DryWetMidi.Interaction.Note[notes.Count];
        notes.CopyTo(array, 0);
        Debug.Log(Application.streamingAssetsPath + "/" + fileLocation);
        Debug.Log(array.Length);
        NoteSpawner.instance.setNotes(midiFile.GetTempoMap(), array,bpm);

        Invoke(nameof(StartSong), songDelayInSeconds);
    }
    public void StartSong()
    {
        NoteSpawner.instance.startSong(this);
    }
}
