using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleModifier : MonoBehaviour
{
    [SerializeField] private Vector3 _arrivingScale = default;
    [SerializeField] private float _speedScale = default;
    [SerializeField] private bool _isAnchoredToMenu = default;

    private bool _palmFacingCamera = false;
    private Vector3 _startScale;
    // Start is called before the first frame update
    void Start()
    {
        _startScale = transform.localScale;
    }

    public void increaseScale()
    {
        if (_isAnchoredToMenu)
        {
            _palmFacingCamera = true;
            StartCoroutine(increase());
            
        }
            
    }

    public void decreaseScale()
    {
        if (_isAnchoredToMenu)
        {
            _palmFacingCamera = false;
            StartCoroutine(decrease());
        }
            
    }

    public void anchoredTrue()
    {
        _isAnchoredToMenu = true;
    }

    public void anchoredFalse()
    {
        _isAnchoredToMenu = false;
    }

    IEnumerator increase()
    {
        float t = 0;
        while ((Mathf.Abs(transform.localScale.x - _arrivingScale.x) > 0.001f) &&
                (Mathf.Abs(transform.localScale.y - _arrivingScale.y) > 0.001f) &&
                (Mathf.Abs(transform.localScale.z - _arrivingScale.z) > 0.001f) &&
                _palmFacingCamera)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, _arrivingScale, t);
            t += Time.deltaTime * _speedScale;
            yield return null;
        }
    }

    IEnumerator decrease()
    {
        float t = 0;
        while ((Mathf.Abs(transform.localScale.x - _startScale.x) > 0.001f) &&
                (Mathf.Abs(transform.localScale.y - _startScale.y) > 0.001f) &&
                (Mathf.Abs(transform.localScale.z - _startScale.z) > 0.001f) &&
                !_palmFacingCamera)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, _startScale, t);
            t += Time.deltaTime * _speedScale;
            yield return null;
        }
    }
}
