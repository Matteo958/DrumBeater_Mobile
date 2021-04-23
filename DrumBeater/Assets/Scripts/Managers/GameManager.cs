using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [HideInInspector] public bool autoMode = false;
    [HideInInspector] public bool hasAutoMode = false;
    [HideInInspector] public bool hasFinalBonus = false;

    [Tooltip("Duration of the auto mode in seconds")]
    [SerializeField] private float autoModeTime = 15;

    private static GameManager _instance;
    public static GameManager instance { get => _instance; }

    private void Awake()
    {
        if (_instance != null && _instance != this)
            Destroy(this.gameObject);
        else
            _instance = this;
    }

    public void activateAutoMode()
    {
        StartCoroutine(autoModeCoroutine());
    }

    private IEnumerator autoModeCoroutine()
    {
        autoMode = true;
        hasAutoMode = false;
        NoteSpawner.instance.activateAutoMode();

        //TODO rallentare il tempo?

        yield return new WaitForSeconds(autoModeTime);

        autoMode = false;
        NoteSpawner.instance.deactivateAutoMode();
    }
}
