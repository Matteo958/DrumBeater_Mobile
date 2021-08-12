using System.Collections;
using UnityEngine;

public class Gestures : MonoBehaviour
{
    [SerializeField] private Transform _palmRight = default;
    [SerializeField] private Transform _palmLeft = default;
    [SerializeField] private Transform _cubeExample;

    [SerializeField] private GameObject _thumbLeftEnd = default;
    [SerializeField] private GameObject _indexLeftEnd = default;
    [SerializeField] private GameObject _middleLeftEnd = default;
    [SerializeField] private GameObject _ringLeftEnd = default;
    [SerializeField] private GameObject _pinkyLeftEnd = default;

    [SerializeField] private GameObject _thumbRightEnd = default;
    [SerializeField] private GameObject _indexRightEnd = default;
    [SerializeField] private GameObject _middleRightEnd = default;
    [SerializeField] private GameObject _ringRightEnd = default;
    [SerializeField] private GameObject _pinkyRightEnd = default;

    [SerializeField] private float _dispForBaseRotation = default;

    public int _powerUp1Active;

    public bool _canActivatePower1, _canActivatePower2, _handOpenLeft, _handOpenRight, _fistLeft, _fistRight, _canRotateRight, _canRotateLeft, _rotationStarted;

    private Vector3 _palmRightStartPos;
    private Vector3 _palmLeftStartPos;

    // Start is called before the first frame update
    void Start()
    {
        _powerUp1Active = 0;

        _canActivatePower1 = _canActivatePower2 = false;
        _handOpenLeft = _handOpenRight = false;
        _fistLeft = _fistRight = false;
        _canRotateLeft = _canRotateRight = false;
        _rotationStarted = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("PowerUp1"))
        {
            _canActivatePower1 = true;
        }else if (Input.GetButtonDown("PowerUp2"))
        {
            _canActivatePower2 = true;
        }
        else if (Input.GetButtonDown("RotateLeft"))
        {
            _canRotateLeft = true;
        }
        else if (Input.GetButtonDown("RotateRight"))
        {
            _canRotateRight = true;
        }
        
    }
    #region Clapping PowerUp
    public void ActivateHandsLookingToEachOther()
    {
        _powerUp1Active = _powerUp1Active == 2? _powerUp1Active:_powerUp1Active+1;

        if(_powerUp1Active == 2 && _canActivatePower1)
        {
            StartCoroutine(CheckHandsClapping());
        }
    }

    public void DeactivateHandsLookingToEachOther()
    {
        _powerUp1Active = _powerUp1Active == 0 ? _powerUp1Active : _powerUp1Active - 1;
    }

    IEnumerator CheckHandsClapping()
    {
        while (_powerUp1Active == 2)
        {
            UpdatePalmPosHandsClapping(0.02f);

            yield return new WaitForSeconds(0.01f);
        }

    }

    private void UpdatePalmPosHandsClapping(float destination)
    {

        if (Vector3.Distance(_palmLeft.localPosition, _palmRight.localPosition) < destination)
        {
            _canActivatePower1 = false;
            foreach (Transform cube in _cubeExample.transform)
            {
                cube.GetComponent<ExplodeExample>().Explode();
            }
            //TO-DO: Power Up Hands Up
        }
    }

    #endregion

    #region Left Fist
    public void ActivateFistLeft()
    {
        _fistLeft = true;

        if(_canActivatePower2)
            StartCoroutine(CheckHandOpenLeft());
    }

    public void DeactivateFistLeft()
    {
        
        _handOpenLeft = false;
        _fistLeft = false;
    }

    IEnumerator CheckHandOpenLeft()
    {
        while (!_handOpenLeft && _fistLeft)
        {
            UpdateHandOpenLeft();

            yield return new WaitForSeconds(0.01f);
        }
        Debug.Log("LeftHand Open");
    }

    private void UpdateHandOpenLeft()
    {
        if ((Vector3.Distance(_palmLeft.position, _thumbLeftEnd.transform.position) > 0.07f
            && Vector3.Distance(_palmLeft.position, _indexLeftEnd.transform.position) > 0.06f
            && Vector3.Distance(_palmLeft.position, _middleLeftEnd.transform.position) > 0.06f
            && Vector3.Distance(_palmLeft.position, _ringLeftEnd.transform.position) > 0.06f
            && Vector3.Distance(_palmLeft.position, _pinkyLeftEnd.transform.position) > 0.06f))
        {
            _handOpenLeft = true;
            StartCoroutine(CheckFistLeft());
        }
    }

    IEnumerator CheckFistLeft()
    {
        while (_canActivatePower2 && _fistLeft)
        {
            UpdatePalmPosFistLeft();
            yield return new WaitForSeconds(0.01f);
        }
        Debug.Log("Left Fist");
    }

    private void UpdatePalmPosFistLeft()
    {
        if ((Vector3.Distance(_palmLeft.position, _thumbLeftEnd.transform.position) < 0.06f
            && Vector3.Distance(_palmLeft.position, _indexLeftEnd.transform.position) < 0.05f
            && Vector3.Distance(_palmLeft.position, _middleLeftEnd.transform.position) < 0.05f
            && Vector3.Distance(_palmLeft.position, _ringLeftEnd.transform.position) < 0.05f
            && Vector3.Distance(_palmLeft.position, _pinkyLeftEnd.transform.position) < 0.05f))
        {
            Debug.Log("Power Activated by Left Hand");
            _handOpenLeft = false;

            _canActivatePower2 = false;
            foreach (Transform cube in _cubeExample.transform)
            {
                cube.GetComponent<Rigidbody>().velocity = Vector3.zero;
                cube.GetComponent<Collider>().enabled = false;
                cube.GetComponent<ExplodeExample>().ReturnToStartPos();
            }
        }
    }
    #endregion

    #region Right Fist
    public void ActivateFistRight()
    {
       
        _fistRight = true;

        if (_canActivatePower2)
            StartCoroutine(CheckHandOpenRight());
    }

    public void DeactivateFistRight()
    {
        
        _handOpenRight = false;
        _fistRight = false;
    }

    IEnumerator CheckHandOpenRight()
    {
        while (!_handOpenRight && _fistRight)
        {
            UpdateHandOpenRight();

            yield return new WaitForSeconds(0.01f);
        }
        Debug.Log("RightHand Open");
    }

    private void UpdateHandOpenRight()
    {
        if ((Vector3.Distance(_palmRight.position, _thumbRightEnd.transform.position) > 0.07f
            && Vector3.Distance(_palmRight.position, _indexRightEnd.transform.position) > 0.06f
            && Vector3.Distance(_palmRight.position, _middleRightEnd.transform.position) > 0.06f
            && Vector3.Distance(_palmRight.position, _ringRightEnd.transform.position) > 0.06f
            && Vector3.Distance(_palmRight.position, _pinkyRightEnd.transform.position) > 0.06f))
        {
            _handOpenRight = true;
            StartCoroutine(CheckFistRight());
        }
    }

    IEnumerator CheckFistRight()
    {
        while (_canActivatePower2 && _fistRight)
        {
            UpdatePalmPosFistRight();
            yield return new WaitForSeconds(0.01f);
        }
        Debug.Log("Right Fist");
    }

    private void UpdatePalmPosFistRight()
    {


        if ((Vector3.Distance(_palmRight.position, _thumbRightEnd.transform.position) < 0.06f
            && Vector3.Distance(_palmRight.position, _indexRightEnd.transform.position) < 0.05f
            && Vector3.Distance(_palmRight.position, _middleRightEnd.transform.position) < 0.05f
            && Vector3.Distance(_palmRight.position, _ringRightEnd.transform.position) < 0.05f
            && Vector3.Distance(_palmRight.position, _pinkyRightEnd.transform.position) < 0.05f))
        {
            Debug.Log("Power Activated by Right Hand");
            _handOpenRight = false;

            _canActivatePower2 = false;
            foreach (Transform cube in _cubeExample.transform)
            {
                cube.GetComponent<Rigidbody>().velocity = Vector3.zero;
                cube.GetComponent<Collider>().enabled = false;
                cube.GetComponent<ExplodeExample>().ReturnToStartPos();
            }
        }
    }

    #endregion

    #region Left Rotation

    public void ActivateRotationLeft()
    {
        _palmRightStartPos = _palmRight.transform.localPosition;
        _rotationStarted = true;
        if (_canRotateLeft)
        {
            StartCoroutine(RotateLeft());
        }
    }

    public void DeactivateRotationLeft()
    {
        _rotationStarted = false;
    }

    IEnumerator RotateLeft()
    {
        while (_rotationStarted && _canRotateLeft)
        {
            UpdatePalmPosRightToLeft(_dispForBaseRotation);
            yield return new WaitForSeconds(0.01f);
        }
    }

    private void UpdatePalmPosRightToLeft(float displacement)
    {

        if (_palmRight.transform.localPosition.x < _palmRightStartPos.x - displacement)
        {
            _canRotateLeft = false;
            Debug.Log("Left rotation");
            // Rotate to Left
        }
    }
    #endregion

    #region Right Rotation
    public void ActivateRotationRight()
    {
        _palmLeftStartPos = _palmLeft.transform.localPosition;
        _rotationStarted = true;
        if (_canRotateRight)
        {
            StartCoroutine(RotateRight());
        }
    }

    public void DeactivateRotationRight()
    {
        _rotationStarted = false;
    }
    
    IEnumerator RotateRight()
    {
        while (_rotationStarted && _canRotateRight)
        {
            UpdatePalmPosLeftToRight(_dispForBaseRotation);
            yield return new WaitForSeconds(0.01f);
        }
    }

    private void UpdatePalmPosLeftToRight(float displacement)
    {
        
        if (_palmLeft.transform.localPosition.x > _palmLeftStartPos.x + displacement)
        {
            _canRotateRight = false;
            Debug.Log("Right rotation");
            // Rotate to Left
        }
    }
    #endregion
}
