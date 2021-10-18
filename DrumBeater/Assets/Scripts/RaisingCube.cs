using System.Collections;
using UnityEngine;

public class RaisingCube : MonoBehaviour
{
    public float speed;
    public float finalYPos;

    Vector3 startPos;
    Vector3 finalPos;

    void Start()
    {
        startPos = transform.position;
        finalPos = new Vector3(startPos.x, finalYPos, startPos.z);
        StartCoroutine(raise());
    }

    IEnumerator raise()
    {
        float i = 0;
        
        while (i <= 1)
        {
            i += Time.deltaTime * speed;
            transform.position = Vector3.Lerp(startPos, finalPos, i);
            yield return null;
        }
    }

    IEnumerator deraise()
    {
        float i = 0;

        while (i <= 1)
        {
            i += Time.deltaTime * speed;
            transform.position = Vector3.Lerp(finalPos, startPos, i);
            yield return null;
        }
    }

    public void changeColor()
    {
        Material mat = GetComponent<Renderer>().material;
        mat.color = Color.green;
    }

}
