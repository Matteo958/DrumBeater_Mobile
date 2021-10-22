using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceField : MonoBehaviour
{
    float endScale;
    // Start is called before the first frame update
    void Start()
    {
        endScale = Random.Range(1.5f, 3.0f);
        StartCoroutine(GrowForceField());
    }

    IEnumerator GrowForceField()
    {
        float t = 0;
        while (transform.localScale.x < (endScale - 0.1f))
        {
            transform.localScale = Vector3.Lerp(transform.localScale, new Vector3(endScale, endScale, endScale), t);
            t += Time.deltaTime * 0.05f;
            yield return null;
        }

        StartCoroutine(ShrinkForceField());
    }

    IEnumerator ShrinkForceField()
    {
        float t = 0;
        while (transform.localScale.x > 0.1f)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, new Vector3(0, 0, 0), t);
            t += Time.deltaTime * 0.08f;
            yield return null;
        }

        Destroy(this.gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
