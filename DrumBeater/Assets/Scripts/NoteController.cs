using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteController : MonoBehaviour
{
    [HideInInspector] public float bpmTime;
    [HideInInspector] public bool isLastNote = false;
    [HideInInspector] public bool special = false;
    private float fill = 0;

    void Update()
    {
        fill = (NoteSpawner.instance.noteSpawnGapInBeats - (bpmTime - NoteSpawner.instance.songPosInBeats)) / NoteSpawner.instance.noteSpawnGapInBeats;
        //Debug.Log(fill);
        transform.localScale = Vector3.Lerp(new Vector3(0.001f, 1, 0.001f), new Vector3(1, 1, 1), fill);

        if (GameManager.instance.autoMode && fill >= 0.97)
            hit(PointsManager.Precision.PERFECT);
        else if (fill > 1)
        {
            if(isLastNote)
            Debug.Log("MISS");
            miss();
        }
    }


    public bool press()
    {
        if ((fill <= 1.1 && fill > 1.07) || (fill >= 0.9 && fill < 0.3))
        {
            hit(PointsManager.Precision.OK);
            return true;
        }
        else if ((fill <= 1.07 && fill > 1.03) || (fill >= 0.93 && fill < 0.97))
        {
            hit(PointsManager.Precision.GOOD);
            return true;
        }
        else if (fill <= 1.03 && fill >= 0.97)
        {
            hit(PointsManager.Precision.PERFECT);
            return true;
        }

        return false;
    }
    
    private void hit(PointsManager.Precision p)
    {
        PointsManager.instance.hitNote(p);
        if (isLastNote)
        {
            GameManager.instance.finishSong();
        }
        gameObject.SetActive(false);
    }

    private void miss()
    {
        PointsManager.instance.missNote();
        if (isLastNote)
        {
            Debug.Log(gameObject.name);
            GameManager.instance.finishSong();
        }
        gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        isLastNote = false;
        transform.localScale = new Vector3(0.001f, 1, 0.001f);
    }
}