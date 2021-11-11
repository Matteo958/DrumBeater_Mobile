using Melanchall.DryWetMidi.Interaction;
using System.Collections;
using System.Collections.Generic;
//using System.Security.Cryptography;
using UnityEngine;

public class NoteSpawner : MonoBehaviour
{
    public List<float> timeStamps = new List<float>();
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
    private List<int> notesPosition = new List<int>();

    // The number of seconds for each song beat
    private float secPerBeat;
    // Current song position, in seconds
    public float songPosition;
    // Current song position, in beats
    [HideInInspector] public float songPosInBeats;
    // Check if the song is already started
    [HideInInspector] public bool songHasStarted = false;
    // How many seconds have passed since the song started
    private float dspSongTime;
    // The offset to the first beat of the song in seconds
    private float firstBeatOffset;
    // The index of the note to spawn
    private int spawnIndex = 0;
    // Spawned note
    private GameObject spawnedNote;

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
        spawnIndex = 0;

        // Record the time when the music starts
        dspSongTime = (float)AudioSettings.dspTime;

        AudioManager.instance.playAudio(song.type);

        StartCoroutine(wait());
    }

    private IEnumerator wait()
    {
        yield return new WaitForSeconds(1);
        songHasStarted = true;
    }

    void Update()
    {
        if (!songHasStarted || spawnIndex == timeStamps.Count)
            return;

        // Determine how many seconds since the song started
        songPosition = (float)(AudioSettings.dspTime - dspSongTime - firstBeatOffset);
        // Determine how many beats since the song started
        songPosInBeats = songPosition / secPerBeat;

        // Check if there are still notes for the song and if the note has to be spawned yet
        if (songPosInBeats >= timeStamps[spawnIndex] - noteSpawnGapInBeats)
        {
            spawnNote();
            spawnIndex++;
        }
    }

    private void spawnNote()
    {
        spawnedNote = ObjectPool.instance.getPooledObj();
        if (spawnedNote != null)
        {
            spawnedNote.transform.SetParent(trackRemoversMap[notesPosition[spawnIndex]].transform);
            spawnedNote.transform.localRotation = Quaternion.identity;
            spawnedNote.transform.localPosition = Vector3.zero + transform.up * noteHeight;
            spawnedNote.SetActive(true);

            if (spawnIndex == timeStamps.Count - 1)
                spawnedNote.GetComponent<NoteController>().isLastNote = true;

            spawnedNote.GetComponent<NoteController>().bpmTime = timeStamps[spawnIndex];
            if (GameManager.instance.autoMode)
                spawnedNote.GetComponent<Renderer>().material = autoModeNoteMaterial;
            else
                spawnedNote.GetComponent<Renderer>().material = noteMaterials[notesPosition[spawnIndex]];
        }
        else
            Debug.LogError("Increase object pool number");

        GameManager.instance.verifyTrack(notesPosition[spawnIndex + 1]);
    }

    public void setNotes(Melanchall.DryWetMidi.Interaction.Note[] array, int bpm)
    {
        // Calculate the number of seconds in each beat
        secPerBeat = 60f / bpm;

        foreach (Note note in array)
        {
            MetricTimeSpan metricTimeSpan = TimeConverter.ConvertTo<MetricTimeSpan>(note.Time, Song.midiFile.GetTempoMap());
            timeStamps.Add((metricTimeSpan.Minutes * 60f + metricTimeSpan.Seconds + metricTimeSpan.Milliseconds / 1000f) / secPerBeat);
            notesPosition.Add((int)note.NoteName);
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