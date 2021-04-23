using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteController : MonoBehaviour
{

    [HideInInspector] public float bpmTime;
    [HideInInspector] public bool isLastNote = false;
    [HideInInspector] public bool special = false;
    private Material mat;
    private float fill = 0;

    private void Awake()
    {
        mat = GetComponent<Renderer>().material;
    }

    void Update()
    {
        fill = (NoteSpawner.instance.noteSpawnGapInBeats - (bpmTime - NoteSpawner.instance.songPosInBeats)) / NoteSpawner.instance.noteSpawnGapInBeats;
        transform.localScale = Vector3.Lerp(new Vector3(0.001f, 20, 0.001f), new Vector3(1, 20, 1), fill);

        if (GameManager.instance.autoMode && fill >= 0.97)
            hit(PointsManager.Precision.PERFECT);
        else if (fill > 1.1)
            miss();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name == "Hand")
        {
            if ((fill <= 1.1 && fill > 1.07) || (fill >= 0.9 && fill < 0.3))
                hit(PointsManager.Precision.OK);
            else if ((fill <= 1.07 && fill > 1.03) || (fill >= 0.93 && fill < 0.97))
                hit(PointsManager.Precision.GOOD);
            else if (fill <= 1.03 && fill >= 0.97)
                hit(PointsManager.Precision.PERFECT);
        }
    }
    
    private void hit(PointsManager.Precision p)
    {
        PointsManager.instance.hitNote(p);
        if (isLastNote)
        {
            NoteSpawner.instance.songHasStarted = false;
            PointsManager.instance.calculatePercentage();
        }
        gameObject.SetActive(false);
    }

    private void miss()
    {
        PointsManager.instance.missNote();
        if (isLastNote)
        {
            NoteSpawner.instance.songHasStarted = false;
            PointsManager.instance.calculatePercentage();
        }
        gameObject.SetActive(false);
    }
}