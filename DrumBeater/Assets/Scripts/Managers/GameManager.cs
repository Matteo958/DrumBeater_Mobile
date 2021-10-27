using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Material _buttonTrackNotPressed = default;
    [SerializeField] private Material _buttonTrackPressed = default;
    [SerializeField] private GameObject _forceField = default;

    [SerializeField] private GameObject _PowerUpIcosphere = default;


    [HideInInspector] public bool autoMode = false;
    [HideInInspector] public bool hasAutoMode = false;
    [HideInInspector] public bool hasFinalBonus = false;
    [HideInInspector] public bool gamePaused = false;

    [Tooltip("Duration of the auto mode in seconds")]
    [SerializeField] private float autoModeTime = 15;

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
        //TODO
    }

    public void unpause()
    {
        Time.timeScale = 1;
        AudioListener.pause = false;
        gamePaused = false;
        //TODO
    }

    private void Update()
    {
        if(_PowerUpIcosphere.transform.GetChild(0).GetComponent<Renderer>().material.GetFloat("Fill") == 1.0)
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
    }
}
