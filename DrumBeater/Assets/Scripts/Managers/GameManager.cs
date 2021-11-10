using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Material _buttonTrackNotPressed = default;
    [SerializeField] private Material _buttonTrackPressed = default;
    [SerializeField] private GameObject _forceField = default;

    [SerializeField] private GameObject _PowerUpIcosphere = default;
    [SerializeField] private GameObject _tracks = default;
    [SerializeField] private GameObject _soloText = default;
    private int activeTrack = 2;

    [SerializeField] private Light _dirLight = default;
    [SerializeField] private Color _pauseDirLightColor = default;
    [SerializeField] private Camera _mainCamera = default;
    [SerializeField] private Transform _fog = default;


    [HideInInspector] public bool autoMode = false;
    [HideInInspector] public bool hasAutoMode = false;
    [HideInInspector] public bool gamePaused = false;
    private bool soloIsActive = false;

    [Tooltip("Duration of the auto mode in seconds")]
    [SerializeField] private float autoModeTime = 15;
    [SerializeField] private float finalBonusTime = 15;
    [SerializeField] private int comboHitsToBonus = 150;

    // Check if the level is already started
    public bool LevelStarted;


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

    private void Start()
    {
        LevelStarted = false;
        

    }

    //public void StartLevel()
    //{
    //    Debug.Log("StartLevel");
    //    StartCoroutine(UnpauseDirLightColor(_startDirLightColor, _fog.position, _fogStartPos));
    //}

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
        
        //TODO
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
        if(_PowerUpIcosphere.transform.GetChild(0).GetComponent<Renderer>().material.GetFloat("Fill") >= 1.0)
        {
            _PowerUpIcosphere.GetComponent<Animator>().SetBool("PowerUpActive", true);
        }
        else
        {
            _PowerUpIcosphere.GetComponent<Animator>().SetBool("PowerUpActive", false);
        }
    }

    public void OnPressButtonTrack(Transform button)
    {
        button.GetChild(0).GetComponent<Renderer>().material = _buttonTrackPressed;
        Instantiate(_forceField, new Vector3(Random.Range(-3.0f, 3.0f), -2.2f, Random.Range(0f, 4.5f)), Quaternion.identity);
        _PowerUpIcosphere.transform.GetChild(0).GetComponent<Renderer>().material.SetFloat("Fill", _PowerUpIcosphere.transform.GetChild(0).GetComponent<Renderer>().material.GetFloat("Fill") + 0.0075f);


        //button.GetChild(0).GetComponent<Renderer>().material = _buttonTrackPressed;
        //for (int i = 1; i < button.transform.childCount; i++)
        //{
        //    if (button.GetChild(i).GetComponent<NoteController>().press())
        //        return;
        //}

        //if (soloIsActive)
        //{
        //    PointsManager.instance.finalBonusHit();
        //    Instantiate(_forceField, new Vector3(Random.Range(-3.0f, 3.0f), -2.2f, Random.Range(0f, 4.5f)), Quaternion.identity);
        //}
        //else if (button.GetChild(1).GetComponent<NoteController>().press())
        //    Instantiate(_forceField, new Vector3(Random.Range(-3.0f, 3.0f), -2.2f, Random.Range(0f, 4.5f)), Quaternion.identity);
    }

    public void OnUnpressButtonTrack(Transform button)
    {
        button.GetChild(0).GetComponent<Renderer>().material = _buttonTrackNotPressed;
    }

    public void rotateTrack(bool left = true)
    {
        if (left)
        {
            if (activeTrack == 3)
                return;
            _tracks.transform.Rotate(0, -90, 0);
        }
        else
        {
            if (activeTrack == 1)
                return;
            _tracks.transform.Rotate(0, 90, 0);

        }
    }

    public void finishSong()
    {
        if (PointsManager.instance.maxComboHits > comboHitsToBonus)
        {
            StartCoroutine(activateFinalBonus());
        }
        else
        {
            NoteSpawner.instance.songHasStarted = false;
            PointsManager.instance.calculatePercentage();
            UIManager.instance.showEndGame();
        }
    }

    private IEnumerator activateFinalBonus()
    {
        soloIsActive = true;
        _soloText.GetComponent<RotateText>().activate();

        yield return new WaitForSeconds(2);

        NoteSpawner.instance.activateSolo();

        yield return new WaitForSeconds(finalBonusTime);

        soloIsActive = false;
        _soloText.GetComponent<RotateText>().deactivate();
        NoteSpawner.instance.songHasStarted = false;
        PointsManager.instance.calculatePercentage();
        UIManager.instance.showEndGame();
    }
}
