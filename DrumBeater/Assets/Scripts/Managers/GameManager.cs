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

    [HideInInspector] public bool autoMode = false;
    [HideInInspector] public bool hasAutoMode = false;
    [HideInInspector] public bool gamePaused = false;
    private bool soloIsActive = false;

    [Tooltip("Duration of the auto mode in seconds")]
    [SerializeField] private float autoModeTime = 15;
    [SerializeField] private float finalBonusTime = 15;
    [SerializeField] private int comboHitsToBonus = 150;

    public static GameManager instance { get; private set; }

    private void Awake()
    {
        if (instance != null && instance != this)
            Destroy(this.gameObject);
        else
            instance = this;
    }

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
        Time.timeScale = 0;
        AudioListener.pause = true;
        gamePaused = true;
        UIManager.instance.showPausePanel(true);
    }

    public void unpause()
    {
        UIManager.instance.showPausePanel(false);
        Time.timeScale = 1;
        AudioListener.pause = false;
        gamePaused = false;
    }

    private void Update()
    {
        if (_PowerUpIcosphere.transform.GetChild(0).GetComponent<Renderer>().material.GetFloat("Fill") == 1.0)
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
        //button.GetChild(0).GetComponent<Renderer>().material = _buttonTrackPressed;
        //for (int i = 1; i < button.transform.childCount; i++)
        //{
        //    if (button.GetChild(i).GetComponent<NoteController>().press())
        //        return;
        //}

        if (soloIsActive)
        {
            PointsManager.instance.finalBonusHit();
            Instantiate(_forceField, new Vector3(Random.Range(-3.0f, 3.0f), -2.2f, Random.Range(0f, 4.5f)), Quaternion.identity);
        }
        else if (button.GetChild(1).GetComponent<NoteController>().press())
            Instantiate(_forceField, new Vector3(Random.Range(-3.0f, 3.0f), -2.2f, Random.Range(0f, 4.5f)), Quaternion.identity);
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
