using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gestures : MonoBehaviour
{
    [SerializeField] private Transform _palmRight = default;
    [SerializeField] private Transform _palmLeft = default;
    [SerializeField] private Transform _cubeExample;

    private float _palmFaceUpStartHeightL, _palmFaceUpStartHeightR;

    public int _powerUp1Active, _powerUp2Active;
    // Start is called before the first frame update
    void Start()
    {
        _powerUp1Active = 0;
        _powerUp2Active = 0;
    }

    // Update is called once per frame
    void Update()
    {
       
    }

    public void ActivateHandsUp()
    {
        _powerUp1Active = _powerUp1Active == 2? _powerUp1Active:_powerUp1Active+1;

        if(_powerUp1Active == 2)
        {
            _palmFaceUpStartHeightL = _palmLeft.transform.localPosition.y;
            _palmFaceUpStartHeightR = _palmRight.transform.localPosition.y;
            StartCoroutine(CheckHandsUp());
        }
    }

    public void DeactivateHandsUp()
    {
        _powerUp1Active = _powerUp1Active == 0 ? _powerUp1Active : _powerUp1Active - 1;
    }

    public void ActivateHandsDown()
    {
        _powerUp2Active = _powerUp2Active == 2 ? _powerUp2Active : _powerUp2Active + 1;

        _palmFaceUpStartHeightL = _palmLeft.transform.localPosition.y;
        _palmFaceUpStartHeightR = _palmRight.transform.localPosition.y;
        StartCoroutine(CheckHandsDown());
    }

    public void DeactivateHandsDown()
    {
        _powerUp2Active = _powerUp2Active == 0 ? _powerUp2Active : _powerUp2Active - 1;
    }

    IEnumerator CheckHandsUp()
    {
        while (_powerUp1Active == 2)
        {
            UpdatePalmPosHandsUp(_palmFaceUpStartHeightL + 0.1f, _palmFaceUpStartHeightR + 0.1f);
            
            yield return new WaitForSeconds(0.01f);
        }
        
    }

    IEnumerator CheckHandsDown()
    {
        while (_powerUp2Active == 2)
        {
            UpdatePalmPosHandsDown(_palmFaceUpStartHeightL - 0.1f, _palmFaceUpStartHeightR - 0.1f);

            yield return new WaitForSeconds(0.01f);
        }

    }

    private void UpdatePalmPosHandsUp(float destinationL, float destinationR)
    {
        
        if((_palmLeft.transform.localPosition.y >= destinationL) && (_palmRight.transform.localPosition.y >= destinationR))
        {
            _powerUp1Active = 0;
            _cubeExample.GetComponent<Rigidbody>().useGravity = false;
            _cubeExample.GetComponent<Rigidbody>().AddForce(_cubeExample.transform.up * 15);
            //TO-DO: Power Up Hands Up
        }
    }

    private void UpdatePalmPosHandsDown(float destinationL, float destinationR)
    {

        if ((_palmLeft.transform.localPosition.y <= destinationL) && (_palmRight.transform.localPosition.y <= destinationR))
        {
            _powerUp2Active = 0;
            _cubeExample.GetComponent<Rigidbody>().useGravity = true;
            //TO-DO: Power Up Hands Down
        }
    }
}
