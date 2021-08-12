using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplodeExample : MonoBehaviour
{
    private Vector3 _startPos;
    private Quaternion _startRot;
    // Start is called before the first frame update
    void Start()
    {
        _startPos = transform.localPosition;
        _startRot = transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Explode()
    {
        transform.GetComponent<Rigidbody>().AddForce(new Vector3(transform.position.x * Random.Range(-15, 15), transform.position.y * Random.Range(0, 4), transform.position.z * Random.Range(-10, 10)) * 5f);
    }

    public void ReturnToStartPos()
    {
        StartCoroutine(Return());
    }

    IEnumerator Return()
    {
        transform.rotation = _startRot;
        float t = 0;
        while (Mathf.Abs(Vector3.Distance(transform.localPosition, _startPos)) > 0.00001)
        {
            
            transform.localPosition = Vector3.Lerp(transform.localPosition, _startPos, t);
            t += Time.deltaTime * 0.05f;
            yield return null;
        }

        
        transform.localPosition = _startPos;
    }
}
