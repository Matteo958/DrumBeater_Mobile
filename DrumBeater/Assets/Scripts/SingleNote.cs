using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleNote : MonoBehaviour
{
    private bool _pressed;

    public bool CorrectZone;
    // Start is called before the first frame update
    void Start()
    {
        _pressed = false;
        CorrectZone = false;
    }

    public void NotePressed()
    {
        _pressed = true;
        transform.parent.GetComponent<Notes>().NotePressed = true;
        if (CorrectZone)
            Debug.Log("YES");
        else
            Debug.Log("NO");
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
