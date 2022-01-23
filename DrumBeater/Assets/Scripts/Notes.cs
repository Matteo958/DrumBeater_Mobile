using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Notes : MonoBehaviour
{
    [SerializeField] private Material _startMaterial = default;
    [SerializeField] private Material _rightMaterial = default;
    [SerializeField] private Material _wrongMaterial = default;

    public bool NotePressed;
    // Start is called before the first frame update
    void Start()
    {
        NotePressed = false;
        foreach (Transform button in transform)
        {
            button.GetChild(0).GetComponent<MeshRenderer>().material = _startMaterial;
            button.GetComponent<SingleNote>().CorrectZone = false;
        }

        int rand = Random.Range(0, 4);

        StartCoroutine(startNote(rand));
    }

    IEnumerator startNote(int note)
    {
        float t = 0;
        while (transform.GetChild(note).transform.GetChild(0).localScale.x < 1.2f && !NotePressed)
        {
            transform.GetChild(note).transform.GetChild(0).localScale = Vector3.Lerp(transform.GetChild(note).transform.GetChild(0).localScale, new Vector3(1.3f, transform.GetChild(note).transform.GetChild(0).localScale.y, 1.3f), t);
            t += Time.deltaTime * 0.005f;
            if (transform.GetChild(note).transform.GetChild(0).localScale.x > 0.75f && transform.GetChild(note).transform.GetChild(0).localScale.x < 1f)
            {
                transform.GetChild(note).transform.GetChild(0).GetComponent<MeshRenderer>().material = _rightMaterial;
                transform.GetChild(note).GetComponent<SingleNote>().CorrectZone = true;
            }

            else if (transform.GetChild(note).transform.GetChild(0).localScale.x > 1f)
            {
                transform.GetChild(note).transform.GetChild(0).GetComponent<MeshRenderer>().material = _wrongMaterial;
                transform.GetChild(note).GetComponent<SingleNote>().CorrectZone = false;
            }
            yield return null;
        }
        transform.GetChild(note).transform.GetChild(0).GetComponent<MeshRenderer>().material = _startMaterial;
        transform.GetChild(note).transform.GetChild(0).localScale = new Vector3(0.1f, transform.GetChild(note).transform.GetChild(0).localScale.y, 0.1f);
        NotePressed = false;
        int rand = Random.Range(0, 4);

        StartCoroutine(startNote(rand));
    }
}
