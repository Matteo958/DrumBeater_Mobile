using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Song _song = default;
    [SerializeField] private Gestures _gestures = default;

    [SerializeField] private Material _buttonTrackNotPressed = default;
    [SerializeField] private Material _buttonTrackPressed = default;
    [SerializeField] private GameObject _forceField = default;

    [SerializeField] private GameObject _PowerUpIcosphere = default;
    [SerializeField] private GameObject _tracks = default;
    [SerializeField] private GameObject _soloText = default;

    [SerializeField] private Light _dirLight = default;
    [SerializeField] private Color _pauseDirLightColor = default;
    [SerializeField] private Camera _mainCamera = default;
    [SerializeField] private Transform _fog = default;


    [HideInInspector] public bool autoMode = false;
    [HideInInspector] public bool hasAutoMode = false;
    [HideInInspector] public bool gamePaused = false;
    [HideInInspector] public int activeTrack = 2;

    [Tooltip("Duration of the auto mode in seconds")]
    [SerializeField] private float autoModeTime = 10;
    [SerializeField] private int comboHitsToBonus = 150;

    // Check if the level is already started
    public bool levelStarted = false;
    private bool soloIsActive = false;
    private int rightTrack = 2;


    private Color _startDirLightColor;
    private Vector3 _fogStartPos;

    private bool _isPausing;


    public static GameManager instance { get; private set; }

    private void Awake()
    {
        if (instance != null && instance != this)
            Destroy(this.gameObject);
        else
            instance = this;
    }

    public void StartSong()
    {
        Debug.Log("StartLevel");
        _song.GetDataFromMidi();
    }

    //public void LevelFinished()
    //{
    //    LevelStarted = false;
    //    UIManager.instance.closePausePanel();
    //    StartCoroutine(PauseDirLightColor(_pauseDirLightColor, _fog.position, _fogStartPos - (2 * Vector3.up)));
    //}

    public void activateAutoMode()
    {
        if (hasAutoMode)
            StartCoroutine(autoModeCoroutine());
    }

    private IEnumerator autoModeCoroutine()
    {
        autoMode = true;
        hasAutoMode = false;
        NoteSpawner.instance.activateAutoMode();

        yield return new WaitForSeconds(autoModeTime);

        autoMode = false;
        NoteSpawner.instance.deactivateAutoMode();
    }

    public void pause()
    {
        //Time.timeScale = 0;

        AudioListener.pause = true;
        gamePaused = true;
        _isPausing = true;

        UIManager.instance.showPausePanel();
    }

    public void unpause()
    {
        //Time.timeScale = 1;

        _isPausing = false;

        UIManager.instance.closePausePanel();
    }

    private void Update()
    {
        if (_PowerUpIcosphere.transform.GetChild(0).GetComponent<Renderer>().material.GetFloat("Fill") >= 1.0)
            _PowerUpIcosphere.GetComponent<Animator>().SetBool("PowerUpActive", true);
        else
            _PowerUpIcosphere.GetComponent<Animator>().SetBool("PowerUpActive", false);

    }

    public void OnPressButtonTrack(Transform button)
    {
        button.GetChild(0).GetComponent<Renderer>().material = _buttonTrackPressed;
        Instantiate(_forceField, new Vector3(Random.Range(-3.0f, 3.0f), -2.2f, Random.Range(0f, 4.5f)), Quaternion.identity);
        _PowerUpIcosphere.transform.GetChild(0).GetComponent<Renderer>().material.SetFloat("Fill", _PowerUpIcosphere.transform.GetChild(0).GetComponent<Renderer>().material.GetFloat("Fill") + 0.0075f);

        //button.GetChild(0).GetComponent<Renderer>().material = _buttonTrackPressed;
        
        if (soloIsActive)
        {
            PointsManager.instance.finalBonusHit();
            Instantiate(_forceField, new Vector3(Random.Range(-3.0f, 3.0f), -2.2f, Random.Range(0f, 4.5f)), Quaternion.identity);
        }
        else if (button.GetChild(1).GetComponent<NoteController>().press())
            Instantiate(_forceField, new Vector3(Random.Range(-3.0f, 3.0f), -2.2f, Random.Range(0f, 4.5f)), Quaternion.identity);
    }

    public void OnUnpressButtonTrack(Transform button)
    {
        button.GetChild(0).GetComponent<Renderer>().material = _buttonTrackNotPressed;
    }

    public void verifyTrack(int buttonID)
    {
        rightTrack = buttonID < 3 ? 1 : (buttonID < 8 ? 2 : 3);

        if (rightTrack > activeTrack)
            UIManager.instance.showHalo();
        else if (rightTrack < activeTrack)
            UIManager.instance.showHalo(true);
    }

    public void rotateTrack(bool clockwise = true)
    {
        if (clockwise)
        {
            if (activeTrack == 3)
                return;

            if (rightTrack == 3 && activeTrack == 1)
            {
                activeTrack = 3;
                StartCoroutine(rotateTrackRoutine(-180, 0.5f));
            }
            else
            {
                activeTrack = activeTrack == 1 ? 2 : 3;
                StartCoroutine(rotateTrackRoutine(-90, 0.5f));
            }
        }
        else
        {
            if (activeTrack == 1)
                return;

            if (rightTrack == 1 && activeTrack == 3)
            {
                activeTrack = 1;
                StartCoroutine(rotateTrackRoutine(180, 0.5f));
            }
            else
            {
                activeTrack = activeTrack == 3 ? 2 : 1;
                StartCoroutine(rotateTrackRoutine(90, 0.5f));
            }

        }

        if (rightTrack == activeTrack)
            UIManager.instance.hideHalo();
    }

    public void finishSong()
    {
        StartCoroutine(finish());
    }

    private IEnumerator finish()
    {
        if (PointsManager.instance.maxComboHits > comboHitsToBonus)
        {
            _soloText.GetComponent<RotateText>().activate();

            yield return new WaitForSeconds(2);

            soloIsActive = true;
            NoteSpawner.instance.activateSolo();
        }

        yield return new WaitForSeconds(20);

        _soloText.GetComponent<RotateText>().deactivate();
        soloIsActive = false;
        rightTrack = 2;
        activeTrack = 2;


        NoteSpawner.instance.songHasStarted = false;
        PointsManager.instance.calculatePercentage();
        UIManager.instance.showEndGame();
    }

    IEnumerator rotateTrackRoutine(float angle, float duration)
    {
        float startRotation = _tracks.transform.eulerAngles.y;
        float endRotation = startRotation + angle;
        float t = 0;
        while (t < duration)
        {
            t += Time.deltaTime;
            float yRotation = Mathf.Lerp(startRotation, endRotation, t / duration) % 360.0f;
            _tracks.transform.eulerAngles = new Vector3(_tracks.transform.eulerAngles.x, yRotation, _tracks.transform.eulerAngles.z);
            yield return null;
        }
        _gestures.canRotateLeft = true;
        _gestures.canRotateRight = true;
    }
}