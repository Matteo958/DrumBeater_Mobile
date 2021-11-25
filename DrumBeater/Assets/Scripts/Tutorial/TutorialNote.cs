using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialNote : MonoBehaviour
{
    private float _perfectGap = 0.2f;
    private float _goodGap = 0.3f;
    private float _okGap = 0.4f;
    private float _fill = 0;

    public bool auto = false;
    
    public void startNote()
    {
        Debug.Log("FILL: " + _fill);
        StartCoroutine(start());
    }

    private IEnumerator start()
    {
        while (true)
        {
            _fill += Time.deltaTime * 0.3f;
            transform.localScale = Vector3.Lerp(new Vector3(0.001f, 1, 0.001f), new Vector3(1, 1, 1), _fill);
            if (auto && _fill >= 0.93)
            {
                hit(Precision.PERFECT);
                break;
            }
            else if (_fill > 1.1)
            {
                miss();
                break;
            }
            yield return null;
        }
    }

    public bool press()
    {
        if (Mathf.Abs(1 - _fill) <= _perfectGap)
        {
            hit(Precision.PERFECT);
            return true;
        }
        else if (Mathf.Abs(1 - _fill) <= _goodGap)
        {
            hit(Precision.GOOD);
            return true;
        }
        else if (Mathf.Abs(1 - _fill) <= _okGap)
        {
            hit(Precision.OK);
            return true;
        }

        return false;
    }

    private void hit(Precision p)
    {
        PointsManager.instance.hitNote(p, auto);
        UIManager.instance.showPrecision(p);

        GetComponentInParent<ParticleSystemRenderer>().material = GetComponent<Renderer>().material;
        GetComponentInParent<ParticleSystem>().Play();
        transform.parent = null;
        gameObject.SetActive(false);
    }

    private void miss()
    {
        PointsManager.instance.missNote();
        UIManager.instance.showPrecision(Precision.MISS);

        GetComponentInParent<Remover>().callError();
        transform.parent = null;
        gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        _fill = 0;
        auto = false;
        transform.localScale = new Vector3(0.001f, 1, 0.001f);
    }
}