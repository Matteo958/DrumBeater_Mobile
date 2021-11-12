using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{

    private static InputManager _instance;

    public static InputManager instance { get => _instance; }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    void Update()
    {
        if (!NoteSpawner.instance.songHasStarted)
            return;

        if (Input.GetKeyDown(KeyCode.LeftArrow))
            GameManager.instance.rotateTrack(false);
        else if (Input.GetKeyDown(KeyCode.RightArrow))
            GameManager.instance.rotateTrack(true);
        else if (Input.GetKeyDown(KeyCode.Q))
            switch (GameManager.instance.activeTrack)
            {
                case 1:
                    GameManager.instance.OnPressButtonTrackNoVR(0);
                    break;
                case 2:
                    GameManager.instance.OnPressButtonTrackNoVR(3);
                    break;
                case 3:
                    GameManager.instance.OnPressButtonTrackNoVR(8);
                    break;
            }
        else if (Input.GetKeyDown(KeyCode.A))
            switch (GameManager.instance.activeTrack)
            {
                case 1:
                    break;
                case 2:
                    GameManager.instance.OnPressButtonTrackNoVR(4);
                    break;
                case 3:
                    GameManager.instance.OnPressButtonTrackNoVR(9);
                    break;
            }
        else if (Input.GetKeyDown(KeyCode.W))
            switch (GameManager.instance.activeTrack)
            {
                case 1:
                    GameManager.instance.OnPressButtonTrackNoVR(1);
                    break;
                case 2:
                    GameManager.instance.OnPressButtonTrackNoVR(5);
                    break;
                case 3:
                    break;
            }
        else if (Input.GetKeyDown(KeyCode.S))
            switch (GameManager.instance.activeTrack)
            {
                case 1:
                    break;
                case 2:
                    GameManager.instance.OnPressButtonTrackNoVR(6);
                    break;
                case 3:
                    GameManager.instance.OnPressButtonTrackNoVR(10);
                    break;
            }
        else if (Input.GetKeyDown(KeyCode.E))
            switch (GameManager.instance.activeTrack)
            {
                case 1:
                    GameManager.instance.OnPressButtonTrackNoVR(2);
                    break;
                case 2:
                    GameManager.instance.OnPressButtonTrackNoVR(7);
                    break;
                case 3:
                    GameManager.instance.OnPressButtonTrackNoVR(11);
                    break;
            }
    }
}
