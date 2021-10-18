using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
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
}
