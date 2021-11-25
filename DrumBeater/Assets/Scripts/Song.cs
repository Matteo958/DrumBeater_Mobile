using System.Collections.Generic;
using UnityEngine;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Interaction;

public class Song : MonoBehaviour
{
    public int bpm;
    public int spawnGapInBeat;
    public Audio.AudioType type;
    public MidiFile midiFile;
    public float songDelayInSeconds;
    public bool tutorial;

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
        var notes = midiFile.GetNotes();
        Note[] array = new Melanchall.DryWetMidi.Interaction.Note[notes.Count];
        notes.CopyTo(array, 0);
        NoteSpawner.instance.setNotes(midiFile.GetTempoMap(), array, bpm);

        Invoke(nameof(StartSong), songDelayInSeconds);
    }
    public void StartSong()
    {
        NoteSpawner.instance.startSong(this);
    }
}
