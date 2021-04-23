﻿using System.Collections.Generic;
using UnityEngine;

public class NoteSpawner : MonoBehaviour
{
    [Tooltip("The gap in beats between the note's spawn and its actual beat time")]
    public float noteSpawnGapInBeats;

    [Tooltip("Heights of the note")]
    [SerializeField] private float noteHeight = 0.6f;
    [Tooltip("The first track")]
    [SerializeField] private Transform firstTrack;
    [Tooltip("The second track")]
    [SerializeField] private Transform secondTrack;
    [Tooltip("The third track")]
    [SerializeField] private Transform thirdTrack;
    [Tooltip("The prefab of the note")]
    [SerializeField] private GameObject notePrefab;
    [Tooltip("List of possible color for the note")]
    [SerializeField] private List<Material> noteMaterials;
    [Tooltip("The color of the note during auto perfect")]
    [SerializeField] private Material autoModeNoteMaterial;

    private Dictionary<int, Remover> firstTrackRemoversMap = new Dictionary<int, Remover>();
    private Dictionary<int, Remover> secondTrackRemoversMap = new Dictionary<int, Remover>();
    private Dictionary<int, Remover> thirdTrackRemoversMap = new Dictionary<int, Remover>();

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

    private static NoteSpawner _instance;
    public static NoteSpawner instance { get => _instance; }

    private void Awake()
    {
        if (_instance != null && _instance != this)
            Destroy(this.gameObject);
        else
        {
            Remover tmpRemover;
            for (int i = 0; i < firstTrack.childCount - 1; i++)
            {
                tmpRemover = firstTrack.GetChild(i).GetComponent<Remover>();
                firstTrackRemoversMap.Add(tmpRemover.id, tmpRemover);
            }

            for (int i = 0; i < secondTrack.childCount; i++)
            {
                tmpRemover = secondTrack.GetChild(i).GetComponent<Remover>();
                secondTrackRemoversMap.Add(tmpRemover.id, tmpRemover);
            }

            for (int i = 0; i < thirdTrack.childCount; i++)
            {
                tmpRemover = thirdTrack.GetChild(i).GetComponent<Remover>();
                thirdTrackRemoversMap.Add(tmpRemover.id, tmpRemover);
            }

            _instance = this;
        }
    }

    public void startSong(Song song)
    {
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
            return;

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
        spawnedNote = ObjectPool.instance.getPooledObj();

        if (spawnedNote != null)
        {
            switch (note.track)
            {
                case 1:
                    spawnedNote.transform.position = firstTrackRemoversMap[note.removerID].transform.position + Vector3.up * noteHeight;
                    spawnedNote.transform.SetParent(firstTrackRemoversMap[note.removerID].transform);
                    //firstTrackRemoversMap[note.line].notesBeatQueue.Enqueue(note.bpmTime);
                    //    firstTrackRemoversMap[note.line].isLastNote = true;

                    break;
                case 2:
                    spawnedNote.transform.position = secondTrackRemoversMap[note.removerID].transform.position + Vector3.up * noteHeight;
                    spawnedNote.transform.SetParent(secondTrackRemoversMap[note.removerID].transform);

                    //secondTrackRemoversMap[note.line].notesBeatQueue.Enqueue(note.bpmTime);
                    //    secondTrackRemoversMap[note.line].isLastNote = true;

                    break;
                case 3:
                    spawnedNote.transform.position = thirdTrackRemoversMap[note.removerID].transform.position + Vector3.up * noteHeight;
                    spawnedNote.transform.SetParent(thirdTrackRemoversMap[note.removerID].transform);

                    //thirdTrackRemoversMap[note.line].notesBeatQueue.Enqueue(note.bpmTime);
                    //    thirdTrackRemoversMap[note.line].isLastNote = true;

                    break;
            }


            spawnedNote.SetActive(true);

            if (nextNoteIndex == notes.Count - 1)
                spawnedNote.GetComponent<NoteController>().isLastNote = true;

            //Set the time in bpm of the note
            spawnedNote.GetComponent<NoteController>().bpmTime = note.bpmTime;
            //Set note color
            if (GameManager.instance.autoMode)
                spawnedNote.GetComponent<Renderer>().material = autoModeNoteMaterial;
            else
                spawnedNote.GetComponent<Renderer>().material = noteMaterials[Random.Range(0, noteMaterials.Count - 1)];
        }
        else
        {
            Debug.LogError("No more notes available to activate. Add more notes to the pool");
        }
    }

    public void activateFinalBonus()
    {
        // TODO
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