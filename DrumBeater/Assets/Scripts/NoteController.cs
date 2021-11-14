﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteController : MonoBehaviour
{
    private float _perfectGap = 0.2f;
    private float _goodGap = 0.4f;
    private float _okGap = 0.5f;
    private float _fill = 0;

    public bool auto = false;

    [HideInInspector] public float bpmTime;
    [HideInInspector] public bool isLastNote = false;

    void Update()
    {
        _fill = (NoteSpawner.instance.noteSpawnGapInBeats - (bpmTime - NoteSpawner.instance.songPosInBeats)) / NoteSpawner.instance.noteSpawnGapInBeats;

        transform.localScale = Vector3.Lerp(new Vector3(0.001f, 1, 0.001f), new Vector3(1, 1, 1), _fill);

        if (auto && _fill >= 0.97)
            hit(PointsManager.Precision.PERFECT);
        else if (_fill > 1.1)
            miss();
    }

    public bool press()
    {
        if (Mathf.Abs(1 - _fill) <= _perfectGap)
        {
            hit(PointsManager.Precision.PERFECT);
            return true;
        }   
        else if (Mathf.Abs(1 - _fill) <= _goodGap)
        {
            hit(PointsManager.Precision.GOOD);
            return true;
        }
        else if (Mathf.Abs(1 - _fill) <= _okGap)
        {
            hit(PointsManager.Precision.OK);
            return true;
        }

        return false;
    }

    private void hit(PointsManager.Precision p)
    {
        PointsManager.instance.hitNote(p);

        if (isLastNote)        
            GameManager.instance.finishSong();

        transform.parent = null;
        gameObject.SetActive(false);
    }

    private void miss()
    {
        PointsManager.instance.missNote();
        if (isLastNote)
            GameManager.instance.finishSong();

        transform.parent = null;
        gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        isLastNote = false;
        transform.localScale = new Vector3(0.001f, 1, 0.001f);
    }
}