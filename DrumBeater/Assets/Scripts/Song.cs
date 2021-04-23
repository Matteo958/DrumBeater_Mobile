using System.Collections.Generic;
using UnityEngine;

public class Song : MonoBehaviour
{
    public float bpm;
    public Audio.AudioType type;
    public List<NoteInfo> notes;

    public void play()
    {
        NoteSpawner.instance.startSong(this);
    }
}
