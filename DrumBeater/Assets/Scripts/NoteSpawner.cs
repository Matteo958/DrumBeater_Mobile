﻿using System.Collections.Generic;
//using System.Security.Cryptography;
using UnityEngine;

public class NoteSpawner : MonoBehaviour
{
    [Tooltip("The gap in beats between the note's spawn and its actual beat time")]
    public float noteSpawnGapInBeats;

    [Tooltip("Heights of the note")]
    [SerializeField] private float noteHeight = 1.3f;
    [Tooltip("The first track")]
    [SerializeField] private Transform firstTrack;
    [Tooltip("The second track")]
    [SerializeField] private Transform secondTrack;
    [Tooltip("The third track")]
    [SerializeField] private Transform thirdTrack;
    [Tooltip("List of possible color for the note")]
    [SerializeField] private List<Material> noteMaterials;
    [Tooltip("The color of the note during auto perfect")]
    [SerializeField] private Material autoModeNoteMaterial;

    private Dictionary<int, Remover> trackRemoversMap = new Dictionary<int, Remover>();

    // Notes of the current song
    private List<NoteInfo> notes;
    // Song beats per minute
    private float songBpm;
    // The number of seconds for each song beat
    private float secPerBeat;
    // Current song position, in seconds
    private float songPosition;
    // Current song position, in beats
    [HideInInspector] public float songPosInBeats;
    // Check if the song is already started
    [HideInInspector] public bool songHasStarted = false;
    // How many seconds have passed since the song started
    private float dspSongTime;
    // The offset to the first beat of the song in seconds
    private float firstBeatOffset;
    // The index of the note to spawn
    private int nextNoteIndex = 0;
    // Spawned note
    private GameObject spawnedNote;
    int i = 0;

    public static NoteSpawner instance { get; private set; }

    private void Awake()
    {
        if (instance != null && instance != this)
            Destroy(this.gameObject);
        else
        {
            Remover tmpRemover;
            for (int i = 0; i < firstTrack.childCount; i++)
            {
                tmpRemover = firstTrack.GetChild(i).GetComponent<Remover>();
                trackRemoversMap.Add(tmpRemover.id, tmpRemover);
            }

            for (int i = 0; i < secondTrack.childCount; i++)
            {
                tmpRemover = secondTrack.GetChild(i).GetComponent<Remover>();
                trackRemoversMap.Add(tmpRemover.id, tmpRemover);
            }

            for (int i = 0; i < thirdTrack.childCount; i++)
            {
                tmpRemover = thirdTrack.GetChild(i).GetComponent<Remover>();
                trackRemoversMap.Add(tmpRemover.id, tmpRemover);
            }

            instance = this;
        }
    }

    public void startSong(Song song)
    {
        nextNoteIndex = 0;
        notes = song.notes;
        songBpm = song.bpm;

        // Calculate the number of seconds in each beat
        secPerBeat = 60f / songBpm;
        // Record the time when the music starts
        dspSongTime = (float)AudioSettings.dspTime;

        AudioManager.instance.playAudio(song.type);

        songHasStarted = true;
    }

    void Update()
    {
        if (!songHasStarted)
        {
            //Debug.Log("CIAO");
            return;
        }

        // Determine how many seconds since the song started
        songPosition = (float)(AudioSettings.dspTime - dspSongTime - firstBeatOffset);
        // Determine how many beats since the song started
        songPosInBeats = songPosition / secPerBeat;

        // Check if there are still notes for the song and if the note has to be spawned yet
        if (nextNoteIndex < notes.Count && notes[nextNoteIndex].bpmTime < songPosInBeats + noteSpawnGapInBeats)
        {
            spawnNote(notes[nextNoteIndex]);
            nextNoteIndex++;
        }
    }

    private void spawnNote(NoteInfo note)
    {
        //Get the note from the pool
        spawnedNote = ObjectPool.instance.getPooledObj();

        if (spawnedNote != null)
        {
            spawnedNote.transform.SetParent(trackRemoversMap[note.removerID].transform);
            spawnedNote.transform.localRotation = Quaternion.identity;
            spawnedNote.transform.localPosition = Vector3.zero + transform.up * noteHeight;

            //thirdTrackRemoversMap[note.line].notesBeatQueue.Enqueue(note.bpmTime);
            //    thirdTrackRemoversMap[note.line].isLastNote = true;

            spawnedNote.SetActive(true);

            if (nextNoteIndex == notes.Count - 1)
            {
                Debug.Log("index: " + nextNoteIndex);
                Debug.Log("notes count: " + nextNoteIndex);
                spawnedNote.GetComponent<NoteController>().isLastNote = true;
            }

            //Set the time in bpm of the note
            spawnedNote.GetComponent<NoteController>().bpmTime = note.bpmTime;
            //Set note color
            if (GameManager.instance.autoMode)
                spawnedNote.GetComponent<Renderer>().material = autoModeNoteMaterial;
            else
            {
                //Set a new seed for the random
                Random.InitState(System.DateTime.Now.Millisecond);
                spawnedNote.GetComponent<Renderer>().material = noteMaterials[Random.Range(0, noteMaterials.Count)];
            }
        }
        else
        {
            Debug.LogError("No more notes available to activate. Add more notes to the pool");
        }
    }

    public void activateSolo()
    {
        foreach (KeyValuePair<int, Remover> entry in trackRemoversMap)
        {
            entry.Value.GetComponent<Renderer>().material = autoModeNoteMaterial;
        }
    }

    public void activateAutoMode()
    {
        ObjectPool.instance.changeObjsMaterial(autoModeNoteMaterial);
    }

    public void deactivateAutoMode()
    {
        ObjectPool.instance.changeObjsMaterial(noteMaterials);
    }
}