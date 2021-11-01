using Leap.Unity.Interaction;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private Text pointsText;
    [SerializeField] private Text percentageText;
    [SerializeField] private Text comboText;
    [SerializeField] private Text maxComboText;
    [SerializeField] private Text comboMultiplierText;
    [SerializeField] private GameObject soloText;
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject endGamePanel;

    [SerializeField] private Material _buttonTrackNotPressed = default;
    [SerializeField] private Material _buttonTrackPressed = default;
    [SerializeField] private GameObject _forceField = default;

    [SerializeField] private GameObject _panelSongs = default;
    [SerializeField] private GameObject _panelTutorial = default;
    [SerializeField] private GameObject _panelCredits = default;
    [SerializeField] private GameObject _panelQuit = default;

    [SerializeField] private Transform _songChoice = default;
    [SerializeField] private Transform _tutorialChoice = default;

    [SerializeField] private Transform _buttonQuit = default;
    [SerializeField] private Transform _buttonCredits = default;

    [SerializeField] private GameObject _starterAnchor = default;


    [SerializeField] private float _panelsOffset = default;

    private static UIManager _instance;

    public static UIManager instance { get => _instance; }

    private float _panelStartScaleX;
    private Vector3 _panelArrivingScale;

    private List<GameObject> _panelsSongs = new List<GameObject>();


    // Check if player is pressing button play
    private bool _buttonPressed;

    // List of songs with their difficulties
    Dictionary<int, int> Songs = new Dictionary<int, int>();

    // What song to play at what difficulty 
    public int[] SongToPlay;
    public int TutorialToPlay;

    // Which container is active; Songs or Tutorial
    private GameObject _panelContainerActive;
    // Panel song or panel tutorial that is in front of camera
    private Transform _panelActive;

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

    private void Start()
    {
        // Firstly all difficulties are set to 1 --> Medium
        for (int i = 0; i < 3; i++)
        {
            Songs[i] = 1;
        }

        _buttonPressed = false;
        _panelStartScaleX = _panelQuit.transform.localScale.x;
        _panelQuit.SetActive(false);
        _panelCredits.SetActive(false);
    }

    public void openPanel()
    {
        _buttonCredits.GetComponent<Collider>().enabled = false;
        _buttonQuit.GetComponent<Collider>().enabled = false;
        switch (_starterAnchor.GetComponent<Anchor>().choice)
        {
            case "Songs":
                _panelSongs.SetActive(true);
                _panelContainerActive = _panelSongs;
                StartCoroutine(open(_panelContainerActive));
                break;
            case "Tutorial":
                _panelTutorial.SetActive(true);
                _panelContainerActive = _panelTutorial;
                StartCoroutine(open(_panelContainerActive));
                break;
        }
    }

    public void closePanel()
    {
        _buttonCredits.GetComponent<Collider>().enabled = true;
        _buttonQuit.GetComponent<Collider>().enabled = true;
        foreach (Transform panel in _panelContainerActive.transform)
        {
            panel.GetComponent<Collider>().enabled = false;
            if (_panelContainerActive == _panelSongs)
            {
                foreach (Transform text in panel.GetChild(3))
                {
                    text.GetComponent<Collider>().enabled = false;
                }
            }
        }
        StartCoroutine(close(_panelContainerActive));
    }

    IEnumerator open(GameObject _panel)
    {
        float t = 0;
        _panelArrivingScale = new Vector3(_panel.transform.localScale.x, 0.03f, _panel.transform.localScale.z);
        while (Mathf.Abs(_panel.transform.localScale.y - _panelArrivingScale.y) > 0.001f)
        {
            _panel.transform.localScale = Vector3.Lerp(_panel.transform.localScale, _panelArrivingScale, t);
            t += Time.deltaTime * 0.05f;
            yield return null;
        }
        foreach (Transform p in _panel.transform)
        {
            p.GetComponent<Collider>().enabled = true;
            if (_panelContainerActive == _panelSongs)
            {
                foreach (Transform text in p.GetChild(3))
                {
                    text.GetComponent<Collider>().enabled = true;
                }
            }
        }
    }

    IEnumerator close(GameObject _panel)
    {
        float t = 0;
        _panelArrivingScale = new Vector3(_panel.transform.localScale.x, 0f, _panel.transform.localScale.z);
        while (Mathf.Abs(_panel.transform.localScale.y - _panelArrivingScale.y) > 0.001f)
        {
            _panel.transform.localScale = Vector3.Lerp(_panel.transform.localScale, _panelArrivingScale, t);
            t += Time.deltaTime * 0.1f;
            yield return null;
        }

        _panel.transform.position = new Vector3(0, _panel.transform.position.y, _panel.transform.position.z);
        _panel.SetActive(false);
    }

    // If I release the panel without giving it a speed to move it, it returns in front of the camera
    public void ContactEnd()
    {
        if (_panelContainerActive.GetComponent<Rigidbody>().velocity.x >= -1f && _panelContainerActive.GetComponent<Rigidbody>().velocity.x <= 1f && _panelContainerActive.GetComponent<Rigidbody>().velocity.x != 0)
        {
            _panelContainerActive.GetComponent<Rigidbody>().velocity = Vector3.zero;
            _panelActive.GetComponent<Collider>().enabled = false;

            StartCoroutine(NextPos(-(_panelActive.GetSiblingIndex() * _panelsOffset)));
        }
    }

    IEnumerator NextPos(float destination)
    {
        float t = 0;
        while (Mathf.Abs(_panelContainerActive.transform.position.x - destination) > 0.01f)
        {
            _panelContainerActive.transform.position = Vector3.Lerp(_panelContainerActive.transform.position, new Vector3(destination, _panelContainerActive.transform.position.y, _panelContainerActive.transform.position.z), t);
            t += Time.deltaTime * 0.5f;
            yield return null;
        }

        _panelContainerActive.transform.position = new Vector3(destination, _panelContainerActive.transform.position.y, _panelContainerActive.transform.position.z);
        _panelActive.transform.GetComponent<Collider>().enabled = true;
    }

    // When a panel passes in front of the camera, it is set as the current active panel.
    private void OnTriggerEnter(Collider col)
    {
        _panelActive = col.gameObject.transform;
        // If when passing in front of the camera the panel speed is within a certain range, its position is set equal to the camera.
        if (_panelContainerActive.GetComponent<Rigidbody>().velocity.x >= -2f && _panelContainerActive.GetComponent<Rigidbody>().velocity.x <= 2f && _panelContainerActive.GetComponent<Rigidbody>().velocity.x != 0)
        {
            _panelContainerActive.GetComponent<Rigidbody>().velocity = Vector3.zero;
            _panelActive.GetComponent<Collider>().enabled = false;

            StartCoroutine(NextPos(-(_panelActive.GetSiblingIndex() * _panelsOffset)));
        }

        if (col.gameObject.tag == "PanelSong")
        {

            // The song to play is set as the active panel song; its difficulty is taken from the dictionary
            SongToPlay[0] = _panelActive.GetSiblingIndex();
            SongToPlay[1] = Songs[SongToPlay[0]];

            _panelActive.GetChild(3).GetChild(SongToPlay[1]).GetComponent<Text>().color = new Color(0, 1, 0, 1);



        }
        else if (col.gameObject.tag == "PanelTutorial")
        {
            TutorialToPlay = _panelActive.GetSiblingIndex();
        }
    }

    // When a panel leaves the camera view, the current panel becomes the next one
    private void OnTriggerExit(Collider col)
    {
        // If the current panel is the first one, I can't move to the left
        if (col.transform.GetSiblingIndex() == 0)
        {

            if (_panelActive.transform.position.x > 0.1f)
            {
                _panelContainerActive.GetComponent<Rigidbody>().velocity = Vector3.zero;
                _panelActive.GetComponent<Collider>().enabled = false;

                StartCoroutine(NextPos(0.0f));
            }
        }
        // If the current panel is the last one, I can't move to the right
        else if (col.transform.GetSiblingIndex() == _panelSongs.transform.childCount - 1)
        {

            if (_panelActive.transform.position.x < -0.1f)
            {
                _panelContainerActive.GetComponent<Rigidbody>().velocity = Vector3.zero;
                _panelActive.GetComponent<Collider>().enabled = false;

                StartCoroutine(NextPos(-(_panelActive.GetSiblingIndex() * _panelsOffset)));
            }
        }
    }

    public void LongPressPlayButton()
    {
        _buttonPressed = true;
        // while I'm pressing the play button, I can't change the difficulty
        if (_panelContainerActive == _panelSongs)
        {
            foreach (Transform t in _panelActive.GetChild(3))
            {
                t.GetComponent<Collider>().enabled = false;
            }
        }

        StartCoroutine(FillButtonRange("Play", _panelActive.GetChild(0)));
    }

    public void StopPressPlayButton()
    {
        _buttonPressed = false;
        if (_panelContainerActive == _panelSongs)
        {
            foreach (Transform t in _panelActive.GetChild(3))
            {
                t.GetComponent<Collider>().enabled = true;
            }
        }

        _panelActive.GetChild(0).GetComponent<Image>().fillAmount = 0;
    }

    public void LongPressQuitGame()
    {
        _buttonPressed = true;
        _panelQuit.transform.GetChild(3).GetComponent<Collider>().enabled = false;
        StartCoroutine(FillButtonRange("Quit", _panelQuit.transform.GetChild(1)));
    }

    public void StopPressQuitGame()
    {
        _buttonPressed = false;
        _panelQuit.transform.GetChild(3).GetComponent<Collider>().enabled = true;
        _panelQuit.transform.GetChild(1).GetComponent<Image>().fillAmount = 0;
    }

    public void LongPressBackQuitButton()
    {
        _buttonPressed = true;
        _panelQuit.transform.GetChild(1).GetComponent<Collider>().enabled = false;
        StartCoroutine(FillButtonRange("BackQuit", _panelQuit.transform.GetChild(3)));
    }

    public void LongPressBackCreditsButton()
    {
        _buttonPressed = true;

        StartCoroutine(FillButtonRange("BackCredits", _panelCredits.transform.GetChild(0)));
    }

    public void StopPressBackQuitButton()
    {
        _buttonPressed = false;
        _panelQuit.transform.GetChild(1).GetComponent<Collider>().enabled = true;
        _panelQuit.transform.GetChild(3).GetComponent<Image>().fillAmount = 0;
    }

    public void StopPressBackCreditsButton()
    {
        _buttonPressed = false;
        _panelCredits.transform.GetChild(0).GetComponent<Image>().fillAmount = 0;
    }

    IEnumerator FillButtonRange(string button, Transform obj)
    {
        float t = 0;
        while (obj.GetComponent<Image>().fillAmount < 0.98f && _buttonPressed)
        {
            obj.GetComponent<Image>().fillAmount = Mathf.Lerp(obj.GetComponent<Image>().fillAmount, 1, t);
            t += Time.deltaTime * 0.005f;
            yield return null;
        }
        if (obj.GetComponent<Image>().fillAmount >= 0.98)
        {
            switch (button)
            {
                case "Play":
                    //TO-DO --> Play song or tutorial
                    break;
                case "Quit":
                    Application.Quit();
                    break;
                case "BackQuit":
                    _panelQuit.SetActive(false);
                    _songChoice.GetComponent<Collider>().enabled = true;
                    _tutorialChoice.GetComponent<Collider>().enabled = true;
                    _buttonCredits.GetComponent<Collider>().enabled = true;
                    break;
                case "BackCredits":
                    _panelCredits.SetActive(false);
                    _songChoice.GetComponent<Collider>().enabled = true;
                    _tutorialChoice.GetComponent<Collider>().enabled = true;
                    _buttonQuit.GetComponent<Collider>().enabled = true;
                    break;
            }
        }
    }

    // The difficulty level is set for the current active panel song;
    // 0 --> Easy; 1 --> Medium; 2 --> Hard
    public void SetDifficulty(Transform button)
    {
        switch (button.tag)
        {
            case "Easy":
                SongToPlay[1] = 0;
                break;
            case "Medium":
                SongToPlay[1] = 1;
                break;
            case "Hard":
                SongToPlay[1] = 2;
                break;
        }

        Songs[_panelActive.GetSiblingIndex()] = SongToPlay[1];

        foreach (Transform t in _panelActive.GetChild(3))
        {
            if (t == button)
                t.GetComponent<Text>().color = new Color(0, 1, 0, 1);
            else
                t.GetComponent<Text>().color = new Color(1f, 1f, 1f, 1);
        }
    }

    public void OpenPanelMenu(string panel)
    {
        switch (panel)
        {
            case "Quit":
                _panelQuit.SetActive(true);
                _songChoice.GetComponent<Collider>().enabled = false;
                _tutorialChoice.GetComponent<Collider>().enabled = false;
                _buttonCredits.GetComponent<Collider>().enabled = false;
                break;
            case "Credits":
                _panelCredits.SetActive(true);
                _songChoice.GetComponent<Collider>().enabled = false;
                _tutorialChoice.GetComponent<Collider>().enabled = false;
                _buttonQuit.GetComponent<Collider>().enabled = false;
                break;
        }
    }

    public void updateGameUI()
    {
        //comboText.text = "COMBO\n" + PointsManager.instance.comboHits;
        //comboMultiplierText.text = "x " + PointsManager.instance.comboMultiplier;
    }

    public void showEndGame()
    {
        //pointsText.text = "POINTS: " + PointsManager.instance.points;
        //percentageText.text = PointsManager.instance.hitsPercentage + "%";
        //maxComboText.text = "MAX COMBO: " + PointsManager.instance.maxComboHits;
        //endGamePanel.SetActive(true);
    }

    public void showPausePanel(bool active)
    {
        //pausePanel.SetActive(active);
    }
}

