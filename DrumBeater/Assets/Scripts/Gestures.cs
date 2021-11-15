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

    public int _powerUp1Active = 0;

    private bool _handOpenLeft, _handOpenRight, _fistLeft, _fistRight, _rotationStarted;

    private Vector3 _palmRightStartPos;
    private Vector3 _palmLeftStartPos;

    #region Clapping PowerUp
    public void ActivateHandsLookingToEachOther()
    {
        _powerUp1Active = _powerUp1Active == 2 ? _powerUp1Active : _powerUp1Active + 1;

        if (_powerUp1Active == 2 && GameManager.instance.hasAutoMode)
            StartCoroutine(CheckHandsClapping());
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
            yield return null;
        }
    }

    private void UpdatePalmPosHandsClapping(float destination)
    {
        if (Vector3.Distance(_palmLeft.localPosition, _palmRight.localPosition) < destination)
            GameManager.instance.activateAutoMode();
    }

    #endregion

    #region Left Fist
    public void ActivateFistLeft()
    {        
        _fistLeft = true;
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
        while (_fistLeft)
        {
            UpdatePalmPosFistLeft();
            yield return new WaitForSeconds(0.01f);
        }
    }

    private void UpdatePalmPosFistLeft()
    {
        if ((Vector3.Distance(_palmLeft.position, _thumbLeftEnd.transform.position) < 0.06f
            && Vector3.Distance(_palmLeft.position, _indexLeftEnd.transform.position) < 0.05f
            && Vector3.Distance(_palmLeft.position, _middleLeftEnd.transform.position) < 0.05f
            && Vector3.Distance(_palmLeft.position, _ringLeftEnd.transform.position) < 0.05f
            && Vector3.Distance(_palmLeft.position, _pinkyLeftEnd.transform.position) < 0.05f))
        {
            //Debug.Log("Power Activated by Left Hand");
            _handOpenLeft = false;

            if (!GameManager.instance.gamePaused && GameManager.instance.levelStarted)
                GameManager.instance.pause();
        }
    }
    #endregion

    #region Right Fist
    public void ActivateFistRight()
    {
        _fistRight = true;

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
    }

    // Richiamare in update in un if con condizioni !handopenright && canactivatepower2 per fare in modo che venga controllato continuamente e non solo con mano verso camera
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
        while (_fistRight)
        {
            UpdatePalmPosFistRight();
            yield return new WaitForSeconds(0.01f);
        }
    }

    private void UpdatePalmPosFistRight()
    {
        if ((Vector3.Distance(_palmRight.position, _thumbRightEnd.transform.position) < 0.06f
            && Vector3.Distance(_palmRight.position, _indexRightEnd.transform.position) < 0.05f
            && Vector3.Distance(_palmRight.position, _middleRightEnd.transform.position) < 0.05f
            && Vector3.Distance(_palmRight.position, _ringRightEnd.transform.position) < 0.05f
            && Vector3.Distance(_palmRight.position, _pinkyRightEnd.transform.position) < 0.05f))
        {
            _handOpenRight = false;
            if (!GameManager.instance.gamePaused && GameManager.instance.levelStarted)
                GameManager.instance.pause();
        }
    }

    #endregion

    #region Left Rotation

    public void ActivateRotationLeft()
    {
        if (GameManager.instance.canRotateLeft)
        {
            _palmRightStartPos = _palmRight.transform.localPosition;
            _rotationStarted = true;
            StartCoroutine(RotateLeft());
        }
    }

    public void DeactivateRotationLeft()
    {
        _rotationStarted = false;
    }

    IEnumerator RotateLeft()
    {
        while (_rotationStarted && GameManager.instance.canRotateLeft)
        {
            UpdatePalmPosRightToLeft(_dispForBaseRotation);
            yield return new WaitForSeconds(0);
        }
    }

    private void UpdatePalmPosRightToLeft(float displacement)
    {
        if (_palmRight.transform.localPosition.x < _palmRightStartPos.x - displacement)
            GameManager.instance.rotateTrack(true);
    }
    #endregion

    #region Right Rotation
    public void ActivateRotationRight()
    {
        if (GameManager.instance.canRotateRight)
        {
            _palmLeftStartPos = _palmLeft.transform.localPosition;
            _rotationStarted = true;
            StartCoroutine(RotateRight());
        }
    }

    public void DeactivateRotationRight()
    {
        _rotationStarted = false;
    }

    IEnumerator RotateRight()
    {
        while (_rotationStarted && GameManager.instance.canRotateRight)
        {
            UpdatePalmPosLeftToRight(_dispForBaseRotation);
            yield return new WaitForSeconds(0);
        }
    }

    private void UpdatePalmPosLeftToRight(float displacement)
    {
        if (_palmLeft.transform.localPosition.x > _palmLeftStartPos.x + displacement)
            GameManager.instance.rotateTrack(false);        
    }
    #endregion
}
