using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Remover : MonoBehaviour
{
    public int id = 0;
    //[HideInInspector] public float fill;
    //[HideInInspector] public Queue<float> notesBeatQueue;
    //[HideInInspector] public bool isLastNote = false;

    //private float currentNoteBeat;
    //private Material mat;
    public void callError()
    {
        StartCoroutine(error());
    }

    IEnumerator error()
    {
        float t = 1;
        while (t > 0)
        {
            transform.localPosition = new Vector3(Mathf.Sin(Time.time * 100f), Mathf.Cos(Time.time * 100f), transform.localPosition.z) * 0.005f;
            t -= Time.deltaTime * 7f;

            yield return null;
        }
    }

    private void Start()
    {
    }

    void Update()
    {
        
        //if (notesBeatQueue.Count == 0)
        //    return;

        //if (currentNoteBeat == 0)
        //    currentNoteBeat = notesBeatQueue.Peek();

        //fill = (Conductor2.instance.noteSpawnGapInBeats - (currentNoteBeat - Conductor2.instance.songPosInBeats)) / Conductor2.instance.noteSpawnGapInBeats;

        //if (fill > 1.05)
        //{
        //    currentNoteBeat = 0;
        //    fill = 0;
        //    mat.SetFloat("fillRate", 0);
        //    notesBeatQueue.Dequeue();
        //    PointsManager.instance.missNote();
        //    if (isLastNote)
        //    {
        //        Conductor2.instance.songHasStarted = false;
        //        PointsManager.instance.calculatePercentage();
        //    }
        //}
        //else
        //    mat.SetFloat("fillRate", fill > 1 ? 1 : fill);
    }



    private void OnCollisionEnter(Collision collision)
    {
        //if (collision.gameObject.name == "Hand")
        //{
        //    if (fill <= 1.05 && fill >= 0.95)
        //    {
        //        PointsManager.instance.hitNote();
        //        if (isLastNote)
        //        {
        //            Conductor2.instance.songHasStarted = false;
        //            PointsManager.instance.calculatePercentage();
        //        }
        //        currentNoteBeat = 0;
        //        fill = 0;
        //        notesBeatQueue.Dequeue();
        //    }
        //}
    }
}