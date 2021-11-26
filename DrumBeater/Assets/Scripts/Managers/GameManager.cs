using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Song _songEasy = default;
    [SerializeField] private Song _songMedium = default;
    [SerializeField] private Song _songExpert = default;
    [SerializeField] private Song _tutorial = default;

    [SerializeField] private Material _buttonTrackNotPressed = default;
    [SerializeField] private Material _buttonTrackPressed = default;
    [SerializeField] private GameObject _forceField = default;

    [SerializeField] public GameObject soloText = default;

    [SerializeField] private Light _dirLight = default;
    [SerializeField] private Color _pauseDirLightColor = default;
    [SerializeField] private Camera _mainCamera = default;
    [SerializeField] private Transform _fog = default;

    [HideInInspector] public bool canRotateRight = false;
    [HideInInspector] public bool canRotateLeft = false;
    [HideInInspector] public bool autoMode = false;
    [HideInInspector] public bool hasAutoMode = false;
    [HideInInspector] public bool gamePaused = false;
    [HideInInspector] public int activeTrack = 2;
    [HideInInspector] public int difficulty;

    [Tooltip("Duration of the auto mode in seconds")]
    public float autoModeTime = 10;
    [SerializeField] private int percentageToSolo = 90;
    [SerializeField] private GameObject button;
    [SerializeField] private GameObject button2;
    [SerializeField] private GameObject button3;

    public GameObject tracks;

    // Check if the level is already started
    public bool levelStarted = false;
    public bool soloIsActive = false;
    private int rightTrack = 2;
    private bool _rotating = false;
    public bool tutorial = false;

    public Audio.AudioType buttonSFX = Audio.AudioType.HitNote1;


    private Color _startDirLightColor;
    private Vector3 _fogStartPos;

    public static GameManager instance { get; private set; }

    private void Awake()
    {
        if (instance != null && instance != this)
            Destroy(this.gameObject);
        else
            instance = this;
    }


    public void startSong()
    {
        AudioManager.instance.stopAudio(Audio.AudioType.MT_2, true);
        gamePaused = false;
        AudioListener.pause = false;

        switch (difficulty)
        {
            case 0:
                _songEasy.GetDataFromMidi();
                break;
            case 1:
                _songMedium.GetDataFromMidi();
                break;
            case 2:
                _songExpert.GetDataFromMidi();
                break;
        }
    }

    public void startEasy()
    {
        _songEasy.GetDataFromMidi();
        button.SetActive(false);
        button2.SetActive(false);
        button3.SetActive(false);
    }

    public void startMedium()
    {
        _songMedium.GetDataFromMidi();
        button.SetActive(false);
        button2.SetActive(false);
        button3.SetActive(false);
    }

    public void startExpert()
    {
        _songExpert.GetDataFromMidi();
        button.SetActive(false);
        button2.SetActive(false);
        button3.SetActive(false);
    }

    private void Start()
    {
        AudioManager.instance.playAudio(Audio.AudioType.MT_2, true);
    }

    public void activateAutoMode()
    {
        if (hasAutoMode)
        {
            StartCoroutine(autoModeCoroutine());
            StartCoroutine(UIManager.instance.emptyIcosphere());
        }
    }

    private IEnumerator autoModeCoroutine()
    {
        autoMode = true;
        hasAutoMode = false;

        if (!tutorial)        
            NoteSpawner.instance.activateAutoMode();                   
        else        
            TutorialNoteSpawner.instance.activateAuto();
        
        yield return new WaitForSeconds(autoModeTime);

        autoMode = false;
        PointsManager.instance.comboHitsAuto = 0;

        if (tutorial)
            UIManager.instance.fillIcosphere(1);
    }

    public void pause()
    {
        if (_rotating)
            return;

        AudioListener.pause = true;
        gamePaused = true;

        UIManager.instance.showPausePanel();
    }

    public void unpause()
    {
        gamePaused = false;
        AudioListener.pause = false;
    }

    public void OnPressButtonTrack(Transform button)
    {
        AudioManager.instance.playAudio(buttonSFX);
        if (soloIsActive)
        {
            PointsManager.instance.finalBonusHit();
            Instantiate(_forceField, new Vector3(Random.Range(-3.0f, 3.0f), -2.2f, Random.Range(0f, 4.5f)), Quaternion.identity);
            button.GetComponent<Renderer>().material = _buttonTrackPressed;
        }
        else if (tutorial ? button.GetChild(1).GetComponent<TutorialNote>().press() : button.GetChild(1).GetComponent<NoteController>().press())
        {
            
            Instantiate(_forceField, new Vector3(Random.Range(-3.0f, 3.0f), -2.2f, Random.Range(0f, 4.5f)), Quaternion.identity);
            button.GetComponentInParent<ParticleSystem>().Play();
        }
    }

    public void OnPressButtonTrackNoVR(int buttonPressed)
    {
        Transform button = NoteSpawner.instance.getButton(buttonPressed);

        if (soloIsActive)
        {
            PointsManager.instance.finalBonusHit();
            Instantiate(_forceField, new Vector3(Random.Range(-3.0f, 3.0f), -2.2f, Random.Range(0f, 4.5f)), Quaternion.identity);
        }
        else if (button.childCount > 1 && button.GetChild(1).GetComponent<NoteController>().press())
        {
            AudioManager.instance.playAudio(buttonSFX);
            Instantiate(_forceField, new Vector3(Random.Range(-3.0f, 3.0f), -2.2f, Random.Range(0f, 4.5f)), Quaternion.identity);
        }
    }

    public void OnUnpressButtonTrack(Transform button)
    {
        if (soloIsActive)
            button.GetComponent<Renderer>().material = _buttonTrackNotPressed;
    }

    public IEnumerator verifyTrack(int buttonID)
    {
        yield return new WaitForSeconds(0.2f);

        rightTrack = buttonID < 3 ? 1 : (buttonID < 8 ? 2 : 3);

        if (rightTrack > activeTrack)
        {
            if (rightTrack - activeTrack > 1)
            {
                canRotateRight = true;
                UIManager.instance.showHalo(true);
            }
            else
            {
                canRotateLeft = true;
                UIManager.instance.showHalo();
            }
        }
        else if (rightTrack < activeTrack)
        {
            if (activeTrack - rightTrack > 1)
            {
                canRotateLeft = true;
                UIManager.instance.showHalo();
            }
            else
            {
                canRotateRight = true;
                UIManager.instance.showHalo(true);
            }
        }
    }

    public void rotateTrack(bool clockwise)
    {
        if (clockwise)
        {
            if (activeTrack - rightTrack > 1)
            {
                activeTrack = 1;
                StartCoroutine(rotateTrackRoutine(-180, 0.5f));
            }
            else
            {
                activeTrack = activeTrack == 1 ? 2 : 3;
                StartCoroutine(rotateTrackRoutine(-90, 0.5f));
            }
        }
        else if (!clockwise)
        {
            if (rightTrack - activeTrack > 1)
            {
                activeTrack = 3;
                StartCoroutine(rotateTrackRoutine(180, 0.5f));
            }
            else
            {
                activeTrack = activeTrack == 3 ? 2 : 1;
                StartCoroutine(rotateTrackRoutine(90, 0.5f));
            }

        }

        UIManager.instance.hideHalo();
    }

    public void rotateToCenter()
    {
        if (activeTrack == 1)
            rotateTrack(true);
        else if (activeTrack == 3)
            rotateTrack(false);
    }

    public void finishSong()
    {
        StartCoroutine(finish());
    }

    private IEnumerator finish()
    {
        PointsManager.instance.calculatePercentage();
        float t = 0;

        if (PointsManager.instance.hitsPercentage > percentageToSolo)
        {
            soloText.GetComponent<RotateText>().activate();

            while (t < 2)
            {
                if (!gamePaused)
                    t += Time.deltaTime;
                yield return null;
            }

            rotateToCenter();
            soloIsActive = true;

            t = 0;
        }

        while (t < 20)
        {
            if (!gamePaused)
                t += Time.deltaTime;
            yield return null;
        }

        soloText.GetComponent<RotateText>().deactivate();
        soloText.transform.localEulerAngles = new Vector3(0, 18, 0);
        soloIsActive = false;

        NoteSpawner.instance.songHasStarted = false;
        UIManager.instance.showEndGame();
    }

    IEnumerator rotateTrackRoutine(float angle, float duration)
    {
        canRotateRight = false;
        canRotateLeft = false;
        _rotating = true;
        float startRotation = tracks.transform.eulerAngles.y;
        float endRotation = startRotation + angle;
        float t = 0;

        while (t < duration)
        {
            t += Time.deltaTime;
            float yRotation = Mathf.Lerp(startRotation, endRotation, t / duration) % 360.0f;
            tracks.transform.eulerAngles = new Vector3(tracks.transform.eulerAngles.x, yRotation, tracks.transform.eulerAngles.z);
            yield return null;
        }

        _rotating = false;

        if (tutorial && Tutorial.instance.tutorialState < 7)
        {
            Tutorial.instance.tutorialState++;
            Tutorial.instance.CheckTutorialState();
        }
    }
}