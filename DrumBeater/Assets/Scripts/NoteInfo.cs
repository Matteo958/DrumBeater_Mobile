using System;
using UnityEngine;

[Serializable]
public class NoteInfo
{
    [Tooltip("The line in which the note is spawned")]
    [Range(1, 15)] public int removerID = 1;
    [Tooltip("Defines if this note counts for the endgame special action")]
    public bool special = false;
    [Tooltip("The minimum difficulty required to make sure the note is spawned")]
    [Range(1, 4)] public int difficulty = 1;
    [Tooltip("The time in BPM corresponding to the note")]
    public float bpmTime = 0;
}