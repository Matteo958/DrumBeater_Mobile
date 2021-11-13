using Leap.Unity.Interaction;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject _video = default;

    [SerializeField] private Text _pointsText;
    [SerializeField] private Text _percentageText;
    [SerializeField] private Text _comboText;
    [SerializeField] private Text _maxComboText;
    [SerializeField] private Text _comboMultiplierText;
    [SerializeField] private GameObject _soloText;
    [SerializeField] private GameObject _pausePanel;
    [SerializeField] private GameObject _endGamePanel;

    [SerializeField] private Material _buttonTrackNotPressed = default;
    [SerializeField] private Material _buttonTrackPressed = default;
    [SerializeField] private GameObject _forceField = default;

    [SerializeField] private GameObject _powerUpIcosphere = default;
    [SerializeField] private GameObject _panelSongs = default;
    [SerializeField] private GameObject _panelTutorial = default;
    [SerializeField] private GameObject _panelCredits = default;
    [SerializeField] private GameObject _panelQuit = default;

    [SerializeField] private Transform _songChoice = default;
    [SerializeField] private Transform _tutorialChoice = default;
    [SerializeField] private Transform _songChoiceStartPos = default;
    [SerializeField] private Transform _tutorialChoiceStartPos = default;

    [SerializeField] private Transform _buttonQuit = default;
    [SerializeField] private Transform _buttonCredits = default;
    [SerializeField] private Transform _knobMusicSlider = default;
    [SerializeField] private Transform _knobSFXSlider = default;

    [SerializeField] private GameObject _starterAnchor = default;

    [SerializeField] private Transform _audience = default;

    [SerializeField] private Transform _pauseContinueText = default;
    [SerializeField] private Transform _pauseRestartText = default;
    [SerializeField] private Transform _pauseReturnText = default;

    //[SerializeField] private float _panelsOffset = default;

    // Animations
    [SerializeField] private Transform _pauseContainer = default;
    [SerializeField] private Transform _tracks = default;
    
    [SerializeField] private Transform _console = default;

    [SerializeField] private Transform _manhole = default;
    [SerializeField] private Transform _manhole2 = default;
    [SerializeField] private Transform _manhole3 = default;

    [SerializeField] private Light _dirLight = default;
    [SerializeField] private Color _pauseDirLightColor = default;
    [SerializeField] private Color _songDirLightColor = default;
    [SerializeField] private Camera _mainCamera = default;
    [SerializeField] private Transform _fog = default;
    [SerializeField] private Image _halo = default;
    private Vector3 _fogStartPos;
    private bool _isDarkening;
    private bool _showingHalo = false;

    private static UIManager _instance;

    public static UIManager instance { get => _instance; }

    //private float _panelStartScaleX;
    //private Vector3 _panelArrivingScale;

    private List<GameObject> _panelsSongs = new List<GameObject>();

    [System.Serializable]
    public enum PauseOption
    {
        None, Continue, Restart, Return
    }

    public PauseOption Option;

    // Check if player is pressing button play
    private bool _buttonPressed;

    private bool _panelOpening;

    private Transform _choice, _choiceStartPos;
    private Vector3 _choiceStartScale;

    private GameObject _panelContainerActive;

    private bool _songChoiceIsFloating, _tutorialChoiceIsFloating;

    //// List of songs with their difficulties
    //Dictionary<int, int> Songs = new Dictionary<int, int>();

    //// What song to play at what difficulty 
    //public int[] SongToPlay;
    //public int TutorialToPlay;

    //// Which container is active; Songs or Tutorial
    //private GameObject _panelContainerActive;
    //// Panel song or panel tutorial that is in front of camera
    //private Transform _panelActive;

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
        _songChoiceIsFloating = _tutorialChoiceIsFloating = false;

        _choiceStartScale = _songChoice.transform.localScale;
        _panelOpening = false;

        _fogStartPos = _fog.position;
        //// Firstly all difficulties are set to 1 --> Medium
        //for (int i = 0; i < 3; i++)
        //{
        //    Songs[i] = 1;
        //}

        _buttonPressed = false;
        //_panelStartScaleX = _panelQuit.transform.localScale.x;
        _panelQuit.SetActive(false);
        _panelCredits.SetActive(false);
    }

    public void fillIcosphere(float fill)
    {
        _powerUpIcosphere.transform.GetChild(0).GetComponent<Renderer>().material.SetFloat("Fill", fill);

        if (fill == 1)
            _powerUpIcosphere.GetComponent<Animator>().SetBool("PowerUpActive", true);
    }


    public void openPanel()
    {
        _panelOpening = true;

        switch (_starterAnchor.GetComponent<Anchor>().choice)
        {
            case "Songs":
                _panelSongs.SetActive(true);
                _panelContainerActive = _panelSongs;
                _choice = _songChoice;
                _choiceStartPos = _songChoiceStartPos;
                _tutorialChoice.GetComponent<Collider>().enabled = false;
                StartCoroutine(open(_panelSongs, true));
                break;
            case "Tutorial":
                _panelTutorial.SetActive(true);
                _panelContainerActive = _panelTutorial;
                _choice = _tutorialChoice;
                _choiceStartPos = _tutorialChoiceStartPos;
                _songChoice.GetComponent<Collider>().enabled = false;
                StartCoroutine(open(_panelTutorial, false));
                break;
        }
    }

    public void ClosePanel()
    {
        _panelOpening = false;
        //_buttonCredits.GetComponent<Collider>().enabled = true;
        //_buttonQuit.GetComponent<Collider>().enabled = true;
        //foreach (Transform panel in _panelContainerActive.transform)
        //{
        //    panel.GetComponent<Collider>().enabled = false;
        //    if (_panelContainerActive == _panelSongs)
        //    {
        //        foreach (Transform text in panel.GetChild(3))
        //        {
        //            text.GetComponent<Collider>().enabled = false;
        //        }
        //    }
        //}

        //_panelContainerActive.transform.GetChild(0).transform.GetChild(1).GetComponent<Collider>().enabled = false;
        if (_panelContainerActive == _panelSongs)
        {
            foreach (Transform t in _panelContainerActive.transform.GetChild(0).transform.GetChild(3))
            {
                t.GetComponent<Collider>().enabled = false;
            }
        }
        _tutorialChoice.GetComponent<Collider>().enabled = true;
        _songChoice.GetComponent<Collider>().enabled = true;
        StartCoroutine(close(_panelContainerActive));
    }

    IEnumerator open(GameObject _panel, bool songs)
    {
        float t = 0;
        Vector3 _panelArrivingScale = new Vector3(_panel.transform.localScale.x, _panel.transform.localScale.y, 12);
        while (Mathf.Abs(_panel.transform.localScale.z - _panelArrivingScale.z) > 0.5f && _panelOpening)
        {
            _panel.transform.localScale = Vector3.Lerp(_panel.transform.localScale, _panelArrivingScale, t);
            t += Time.deltaTime * 0.2f;
            yield return null;
        }

        if (songs)
        {
            _video.GetComponent<UnityEngine.Video.VideoPlayer>().Play();
            _panel.transform.GetChild(0).transform.GetChild(1).GetComponent<Collider>().enabled = true;
            foreach (Transform p in _panel.transform.GetChild(0).transform.GetChild(3))
            {
                p.GetComponent<Collider>().enabled = true;

            }
        }

    }

    IEnumerator close(GameObject _panel)
    {
        float t = 0;
        Vector3 _panelArrivingScale = new Vector3(_panel.transform.localScale.x, _panel.transform.localScale.y, 0);
        while (Mathf.Abs(_panel.transform.localScale.z - _panelArrivingScale.z) > 0.5f && !_panelOpening)
        {
            _panel.transform.localScale = Vector3.Lerp(_panel.transform.localScale, _panelArrivingScale, t);
            t += Time.deltaTime * 0.2f;
            yield return null;
        }

        //_panel.transform.position = new Vector3(0, _panel.transform.position.y, _panel.transform.position.z);
        _panel.SetActive(false);

    }

    //Change directional light and main camera color
    IEnumerator PauseDirLightColor(Color colorEnd, Vector3 fogPosStart, Vector3 fogPosEnd)
    {
        float t = 0;
        while ((Mathf.Abs(_dirLight.GetComponent<Light>().color.r - colorEnd.r) > 0.05 &&
               Mathf.Abs(_dirLight.GetComponent<Light>().color.g - colorEnd.g) > 0.05 &&
               Mathf.Abs(_dirLight.GetComponent<Light>().color.b - colorEnd.b) > 0.05) ||
               (Mathf.Abs(_fog.position.y - fogPosEnd.y) > 0.1) && _isDarkening)
        {
            _dirLight.GetComponent<Light>().color = Color.Lerp(_dirLight.GetComponent<Light>().color, colorEnd, t);
            _mainCamera.GetComponent<Camera>().backgroundColor = Color.Lerp(_dirLight.GetComponent<Light>().color, colorEnd, t);
            _fog.position = Vector3.Lerp(fogPosStart, fogPosEnd, t);
            t += Time.deltaTime;
            yield return null;
        }
    }

    IEnumerator UnpauseDirLightColor(Color colorEnd, Vector3 fogPosStart, Vector3 fogPosEnd)
    {
        float t = 0;
        while ((Mathf.Abs(_dirLight.GetComponent<Light>().color.r - colorEnd.r) > 0.01 &&
               Mathf.Abs(_dirLight.GetComponent<Light>().color.g - colorEnd.g) > 0.01 &&
               Mathf.Abs(_dirLight.GetComponent<Light>().color.b - colorEnd.b) > 0.01) ||
               (Mathf.Abs(_fog.position.y - fogPosEnd.y) > 0.01) && !_isDarkening)
        {
            _dirLight.GetComponent<Light>().color = Color.Lerp(_dirLight.GetComponent<Light>().color, colorEnd, t);
            _mainCamera.GetComponent<Camera>().backgroundColor = Color.Lerp(_mainCamera.GetComponent<Camera>().backgroundColor, colorEnd, t);
            _fog.position = Vector3.Lerp(fogPosStart, fogPosEnd, t);
            t += Time.deltaTime;
            yield return null;
        }
    }

    #region SCROLL PANELS
    // If I release the panel without giving it a speed to move it, it returns in front of the camera
    //public void ContactEnd()
    //{
    //    if (_panelContainerActive.GetComponent<Rigidbody>().velocity.x >= -1f && _panelContainerActive.GetComponent<Rigidbody>().velocity.x <= 1f && _panelContainerActive.GetComponent<Rigidbody>().velocity.x != 0)
    //    {
    //        _panelContainerActive.GetComponent<Rigidbody>().velocity = Vector3.zero;
    //        _panelActive.GetComponent<Collider>().enabled = false;

    //        StartCoroutine(NextPos(-(_panelActive.GetSiblingIndex() * _panelsOffset)));
    //    }
    //}

    //IEnumerator NextPos(float destination)
    //{
    //    float t = 0;
    //    while (Mathf.Abs(_panelContainerActive.transform.position.x - destination) > 0.01f)
    //    {
    //        _panelContainerActive.transform.position = Vector3.Lerp(_panelContainerActive.transform.position, new Vector3(destination, _panelContainerActive.transform.position.y, _panelContainerActive.transform.position.z), t);
    //        t += Time.deltaTime * 0.5f;
    //        yield return null;
    //    }

    //    _panelContainerActive.transform.position = new Vector3(destination, _panelContainerActive.transform.position.y, _panelContainerActive.transform.position.z);
    //    _panelActive.transform.GetComponent<Collider>().enabled = true;
    //}

    //// When a panel passes in front of the camera, it is set as the current active panel.
    //private void OnTriggerEnter(Collider col)
    //{
    //    _panelActive = col.gameObject.transform;
    //    // If when passing in front of the camera the panel speed is within a certain range, its position is set equal to the camera.
    //    if (_panelContainerActive.GetComponent<Rigidbody>().velocity.x >= -2f && _panelContainerActive.GetComponent<Rigidbody>().velocity.x <= 2f && _panelContainerActive.GetComponent<Rigidbody>().velocity.x != 0)
    //    {
    //        _panelContainerActive.GetComponent<Rigidbody>().velocity = Vector3.zero;
    //        _panelActive.GetComponent<Collider>().enabled = false;

    //        StartCoroutine(NextPos(-(_panelActive.GetSiblingIndex() * _panelsOffset)));
    //    }

    //    if (col.gameObject.tag == "PanelSong")
    //    {

    //        // The song to play is set as the active panel song; its difficulty is taken from the dictionary
    //        SongToPlay[0] = _panelActive.GetSiblingIndex();
    //        SongToPlay[1] = Songs[SongToPlay[0]];

    //        _panelActive.GetChild(3).GetChild(SongToPlay[1]).GetComponent<Text>().color = new Color(0, 1, 0, 1);



    //    }
    //    else if (col.gameObject.tag == "PanelTutorial")
    //    {
    //        TutorialToPlay = _panelActive.GetSiblingIndex();
    //    }
    //}

    //// When a panel leaves the camera view, the current panel becomes the next one
    //private void OnTriggerExit(Collider col)
    //{
    //    // If the current panel is the first one, I can't move to the left
    //    if (col.transform.GetSiblingIndex() == 0)
    //    {

    //        if (_panelActive.transform.position.x > 0.1f)
    //        {
    //            _panelContainerActive.GetComponent<Rigidbody>().velocity = Vector3.zero;
    //            _panelActive.GetComponent<Collider>().enabled = false;

    //            StartCoroutine(NextPos(0.0f));
    //        }
    //    }
    //    // If the current panel is the last one, I can't move to the right
    //    else if (col.transform.GetSiblingIndex() == _panelSongs.transform.childCount - 1)
    //    {

    //        if (_panelActive.transform.position.x < -0.1f)
    //        {
    //            _panelContainerActive.GetComponent<Rigidbody>().velocity = Vector3.zero;
    //            _panelActive.GetComponent<Collider>().enabled = false;

    //            StartCoroutine(NextPos(-(_panelActive.GetSiblingIndex() * _panelsOffset)));
    //        }
    //    }
    //}
    #endregion

    public void LongPressPlayButton()
    {
        _buttonPressed = true;
        // while I'm pressing the play button, I can't change the difficulty


        foreach (Transform t in _panelSongs.transform.GetChild(0).transform.GetChild(3))
        {
            t.GetComponent<Collider>().enabled = false;
        }
        StartCoroutine(FillButtonRange("Play", _panelSongs.transform.GetChild(0).transform.GetChild(1)));
    }

    public void StopPressPlayButton()
    {
        _buttonPressed = false;

        foreach (Transform t in _panelSongs.transform.GetChild(0).transform.GetChild(3))
        {
            t.GetComponent<Collider>().enabled = true;
        }
        _panelSongs.transform.GetChild(0).transform.GetChild(1).GetComponent<Image>().fillAmount = 0;
    }

    public void LongPressQuitGame()
    {
        _buttonPressed = true;
        _panelQuit.transform.GetChild(0).transform.GetChild(1).GetComponent<Collider>().enabled = false;
        StartCoroutine(FillButtonRange("Quit", _panelQuit.transform.GetChild(0).transform.GetChild(3)));
    }

    public void StopPressQuitGame()
    {
        _buttonPressed = false;
        _panelQuit.transform.GetChild(0).transform.GetChild(1).GetComponent<Collider>().enabled = true;
        _panelQuit.transform.GetChild(0).transform.GetChild(3).GetComponent<Image>().fillAmount = 0;
    }

    public void LongPressBackQuitButton()
    {
        _buttonPressed = true;
        _panelQuit.transform.GetChild(0).transform.GetChild(3).GetComponent<Collider>().enabled = false;
        StartCoroutine(FillButtonRange("BackQuit", _panelQuit.transform.GetChild(0).transform.GetChild(1)));
    }

    public void StopPressBackQuitButton()
    {
        _buttonPressed = false;
        _panelQuit.transform.GetChild(0).transform.GetChild(3).GetComponent<Collider>().enabled = true;
        _panelQuit.transform.GetChild(0).transform.GetChild(1).GetComponent<Image>().fillAmount = 0;
    }

    public void LongPressBackCreditsButton()
    {
        _buttonPressed = true;

        StartCoroutine(FillButtonRange("BackCredits", _panelCredits.transform.GetChild(0).transform.GetChild(1)));
    }

    public void StopPressBackCreditsButton()
    {
        _buttonPressed = false;
        _panelCredits.transform.GetChild(0).transform.GetChild(1).GetComponent<Image>().fillAmount = 0;
    }

    IEnumerator FillButtonRange(string button, Transform obj)
    {
        float t = 0;
        while (obj.GetComponent<Image>().fillAmount < 0.98f && _buttonPressed)
        {
            obj.GetComponent<Image>().fillAmount = Mathf.Lerp(obj.GetComponent<Image>().fillAmount, 1, t);
            t += Time.deltaTime * 0.1f;
            yield return null;
        }
        if (obj.GetComponent<Image>().fillAmount >= 0.98)
        {
            switch (button)
            {
                case "Play":
                    ClosePanel();
                    
                    _buttonCredits.GetComponent<Collider>().enabled = false;
                    _buttonQuit.GetComponent<Collider>().enabled = false;
                    _knobMusicSlider.GetComponent<Collider>().enabled = false;
                    _knobSFXSlider.GetComponent<Collider>().enabled = false;
                    _songChoice.GetComponent<Collider>().enabled = false;
                    //_tutorialChoice.GetComponent<Collider>().enabled = false;

                    _tracks.gameObject.SetActive(true);

                    _manhole.GetComponent<Animator>().SetBool("Manhole", true);
                    _manhole2.GetComponent<Animator>().SetBool("Manhole", true);
                    _manhole3.GetComponent<Animator>().SetBool("Manhole", true);

                    _isDarkening = false;
                    StartCoroutine(UnpauseDirLightColor(_songDirLightColor, _fog.position, _fogStartPos + (2 * Vector3.up)));
                    StartCoroutine(ConsoleDown(-542.5f, 0.01f, 0.05f));
                    StartCoroutine(TracksUp(-540.33f, 0.01f, 0.05f, true));

                    break;
                case "Quit":
                    Application.Quit();
                    break;
                case "BackQuit":
                    ClosePanel();

                    _songChoice.GetComponent<Collider>().enabled = true;
                    _tutorialChoice.GetComponent<Collider>().enabled = true;
                    _buttonCredits.GetComponent<Collider>().enabled = true;
                    break;
                case "BackCredits":
                    ClosePanel();

                    _songChoice.GetComponent<Collider>().enabled = true;
                    _tutorialChoice.GetComponent<Collider>().enabled = true;
                    _buttonQuit.GetComponent<Collider>().enabled = true;
                    break;
            }
        }
    }

    IEnumerator FillButtonRangePause(string button, Transform obj)
    {
        Debug.Log("Coroutine avviata");
        float t = 0;
        while (obj.GetComponent<Image>().fillAmount < 0.98f && _buttonPressed)
        {
            obj.GetComponent<Image>().fillAmount = Mathf.Lerp(obj.GetComponent<Image>().fillAmount, 1, t);
            t += Time.deltaTime * 0.1f;
            yield return null;
        }
        Debug.Log("Coroutine terminata senza premere il tasto");
        if (obj.GetComponent<Image>().fillAmount >= 0.98)
        {
            Debug.Log("Coroutine terminata premendo il tasto");
            _buttonPressed = false;
            switch (button)
            {

                case "Continue":
                    Option = PauseOption.Continue;
                    _pauseRestartText.GetComponent<Collider>().enabled = true;
                    _pauseReturnText.GetComponent<Collider>().enabled = true;

                    GameManager.instance.unpause();
                    break;
                case "Restart":
                    Option = PauseOption.Restart;

                    _pauseContinueText.GetComponent<Collider>().enabled = true;
                    _pauseReturnText.GetComponent<Collider>().enabled = true;

                    GameManager.instance.unpause();
                    break;
                case "Return":
                    Option = PauseOption.Return;

                    _pauseContinueText.GetComponent<Collider>().enabled = true;
                    _pauseRestartText.GetComponent<Collider>().enabled = true;

                    GameManager.instance.unpause();
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
                GameManager.instance.difficulty = 0;
                break;
            case "Medium":
                GameManager.instance.difficulty = 1;
                break;
            case "Hard":
                GameManager.instance.difficulty = 2;
                break;
        }

        //Songs[_panelActive.GetSiblingIndex()] = SongToPlay[1];

        foreach (Transform t in _panelSongs.transform.GetChild(0).transform.GetChild(3))
        {
            if (t == button)
                t.GetComponent<Text>().color = new Color(0, 1, 0, 1);
            else
                t.GetComponent<Text>().color = new Color(1f, 1f, 1f, 1);
        }
    }


    public void updateGameUI()
    {
        _comboText.text = "COMBO\n" + PointsManager.instance.comboHits;
        _comboMultiplierText.text = "x " + PointsManager.instance.comboMultiplier;
    }

    public void showEndGame()
    {
        //pointsText.text = "POINTS: " + PointsManager.instance.points;
        //percentageText.text = PointsManager.instance.hitsPercentage + "%";
        //maxComboText.text = "MAX COMBO: " + PointsManager.instance.maxComboHits;
        //endGamePanel.SetActive(true);
    }

    public void showPausePanel()
    {
        _manhole.GetComponent<Animator>().SetBool("Manhole", true);
        _manhole2.GetComponent<Animator>().SetBool("Manhole", true);
        _manhole3.GetComponent<Animator>().SetBool("Manhole", true);

        _pauseContainer.gameObject.SetActive(true);
        _isDarkening = true;

        StartCoroutine(PauseDirLightColor(_pauseDirLightColor, _fog.position, _fogStartPos - (2 * Vector3.up)));
        StartCoroutine(TracksDown(-542.5f, 0.01f, 0.1f));
        StartCoroutine(PauseContainerUp(-540.27f, 0.01f, 0.1f));

    }

    public void closePausePanel()
    {
        _manhole.GetComponent<Animator>().SetBool("Manhole", true);


        switch (Option)
        {
            case PauseOption.Continue:
                _manhole2.GetComponent<Animator>().SetBool("Manhole", true);
                _manhole3.GetComponent<Animator>().SetBool("Manhole", true);

                _tracks.gameObject.SetActive(true);
                _isDarkening = false;
                StartCoroutine(UnpauseDirLightColor(_songDirLightColor, _fog.position, _fogStartPos + (2 * Vector3.up)));
                StartCoroutine(PauseContainerDown(-542.5f, 0.01f, 0.1f));
                StartCoroutine(TracksUp(-540.33f, 0.01f, 0.1f, false));

                break;
            case PauseOption.Restart:
                break;
            case PauseOption.Return:
                _console.gameObject.SetActive(true);

                GameManager.instance.levelStarted = false;
                GameManager.instance.gamePaused = false;

                _choice.GetComponent<AnchorableBehaviour>().Detach();
                _choice.position = _choiceStartPos.position;
                _choice.GetComponent<AnchorableBehaviour>().TryAttach();

                StartCoroutine(PauseDirLightColor(_pauseDirLightColor, _fog.position, _fogStartPos - (2 * Vector3.up)));
                StartCoroutine(PauseContainerDown(-542.5f, 0.01f, 0.1f));
                StartCoroutine(ConsoleUp(-540.38f, 0.01f, 0.1f));

                break;
        }
    }


    public void OnContinueText(Transform text)
    {
        _pauseRestartText.GetComponent<Collider>().enabled = false;
        _pauseReturnText.GetComponent<Collider>().enabled = false;

        _buttonPressed = true;
        Debug.Log("ButtonPressedTRUE");
        text.GetComponent<Text>().color = new Color(0, 1, 0, 1);

        StartCoroutine(FillButtonRangePause("Continue", text.GetChild(1)));
    }

    public void OnRestartText(Transform text)
    {
        _pauseContinueText.GetComponent<Collider>().enabled = false;
        _pauseReturnText.GetComponent<Collider>().enabled = false;

        _buttonPressed = true;
        text.GetComponent<Text>().color = new Color(0, 1, 0, 1);

        StartCoroutine(FillButtonRangePause("Restart", text.GetChild(1)));
    }

    public void OnReturnText(Transform text)
    {
        _pauseContinueText.GetComponent<Collider>().enabled = false;
        _pauseRestartText.GetComponent<Collider>().enabled = false;

        _buttonPressed = true;
        text.GetComponent<Text>().color = new Color(0, 1, 0, 1);

        StartCoroutine(FillButtonRangePause("Return", text.GetChild(1)));
    }

    public void OutContinueText(Transform text)
    {
        _buttonPressed = false;
        Debug.Log("ButtonPressedFALSE");
        text.GetComponent<Text>().color = new Color(1, 1, 1, 1);

        _pauseRestartText.GetComponent<Collider>().enabled = true;
        _pauseReturnText.GetComponent<Collider>().enabled = true;
        text.GetChild(1).GetComponent<Image>().fillAmount = 0;
    }

    public void OutRestartText(Transform text)
    {
        _buttonPressed = false;
        text.GetComponent<Text>().color = new Color(1, 1, 1, 1);

        _pauseContinueText.GetComponent<Collider>().enabled = true;
        _pauseReturnText.GetComponent<Collider>().enabled = true;
        text.GetChild(1).GetComponent<Image>().fillAmount = 0;
    }

    public void OutReturnText(Transform text)
    {
        _buttonPressed = false;
        text.GetComponent<Text>().color = new Color(1, 1, 1, 1);

        _pauseContinueText.GetComponent<Collider>().enabled = true;
        _pauseRestartText.GetComponent<Collider>().enabled = true;
        text.GetChild(1).GetComponent<Image>().fillAmount = 0;
    }

    IEnumerator ConsoleUp(float endPosY, float threshold, float speed)
    {
        float t = 0;
        while (Mathf.Abs(_console.localPosition.y - endPosY) > threshold)
        {
            _console.localPosition = Vector3.Lerp(_console.localPosition, new Vector3(_console.localPosition.x, endPosY, _console.localPosition.z), t);
            t += Time.deltaTime * speed;
            yield return null;
        }
        _manhole.GetComponent<Animator>().SetBool("Manhole", false);

        _choice.GetComponent<Collider>().enabled = true;
        _buttonCredits.GetComponent<Collider>().enabled = true;
        _buttonQuit.GetComponent<Collider>().enabled = true;
        _knobMusicSlider.GetComponent<Collider>().enabled = true;
        _knobSFXSlider.GetComponent<Collider>().enabled = true;
    }

    IEnumerator ConsoleDown(float endPosY, float threshold, float speed)
    {
        float t = 0;
        while (Mathf.Abs(_console.localPosition.y - endPosY) > threshold)
        {
            _console.localPosition = Vector3.Lerp(_console.localPosition, new Vector3(_console.localPosition.x, endPosY, _console.localPosition.z), t);
            t += Time.deltaTime * speed;
            yield return null;
        }

        _console.gameObject.SetActive(false);
    }

    IEnumerator TracksUp(float endPosY, float threshold, float speed, bool levelStarted)
    {
        float t = 0;
        while (Mathf.Abs(_tracks.localPosition.y - endPosY) > threshold)
        {
            _tracks.localPosition = Vector3.Lerp(_tracks.localPosition, new Vector3(_tracks.localPosition.x, endPosY, _tracks.localPosition.z), t);
            t += Time.deltaTime * speed;
            yield return null;
        }

        _manhole.GetComponent<Animator>().SetBool("Manhole", false);
        _manhole2.GetComponent<Animator>().SetBool("Manhole", false);
        _manhole3.GetComponent<Animator>().SetBool("Manhole", false);

        if (levelStarted)
        {
            GameManager.instance.levelStarted = true;
            GameManager.instance.startSong();
        }
        else
            GameManager.instance.gamePaused = false;

        foreach (Transform a in _audience)        
            a.GetComponent<Audience>().Jump();

        Debug.Log("TRACKS UP");
    }

    IEnumerator TracksDown(float endPosY, float threshold, float speed)
    {
        float t = 0;
        while (Mathf.Abs(_tracks.localPosition.y - endPosY) > threshold)
        {
            _tracks.localPosition = Vector3.Lerp(_tracks.localPosition, new Vector3(_tracks.localPosition.x, endPosY, _tracks.localPosition.z), t);
            t += Time.deltaTime * speed;
            yield return null;
        }

        _tracks.gameObject.SetActive(false);
    }

    IEnumerator PauseContainerUp(float endPosY, float threshold, float speed)
    {
        float t = 0;
        while (Mathf.Abs(_pauseContainer.localPosition.y - endPosY) > threshold)
        {
            _pauseContainer.localPosition = Vector3.Lerp(_pauseContainer.localPosition, new Vector3(_pauseContainer.localPosition.x, endPosY, _pauseContainer.localPosition.z), t);
            t += Time.deltaTime * speed;
            yield return null;
        }
        _manhole.GetComponent<Animator>().SetBool("Manhole", false);
        _manhole2.GetComponent<Animator>().SetBool("Manhole", false);
        _manhole3.GetComponent<Animator>().SetBool("Manhole", false);
    }

    IEnumerator PauseContainerDown(float endPosY, float threshold, float speed)
    {
        float t = 0;
        while (Mathf.Abs(_pauseContainer.localPosition.y - endPosY) > threshold)
        {
            _pauseContainer.localPosition = Vector3.Lerp(_pauseContainer.localPosition, new Vector3(_pauseContainer.localPosition.x, endPosY, _pauseContainer.localPosition.z), t);
            t += Time.deltaTime * speed;
            yield return null;
        }

        _pauseContinueText.GetComponent<Image>().fillAmount = 0;
        _pauseRestartText.GetComponent<Image>().fillAmount = 0;
        _pauseReturnText.GetComponent<Image>().fillAmount = 0;

        _pauseContainer.gameObject.SetActive(false);
    }


    IEnumerator CheckSongChoiceHeight(Transform choice)
    {
        while (_songChoiceIsFloating)
        {
            UpdateSongChoiceHeight(choice);
            yield return new WaitForSeconds(0.01f);
        }
    }

    private void UpdateSongChoiceHeight(Transform choice)
    {
        if (choice.position.y < -1f)
        {
            _songChoiceIsFloating = false;
            choice.transform.localScale = new Vector3(0, 0, 0);
            choice.GetComponent<Rigidbody>().useGravity = false;
            choice.GetComponent<Rigidbody>().isKinematic = true;
            choice.GetComponent<Collider>().enabled = false;
            if (choice.tag == "SongChoice")
            {
                choice.position = _songChoiceStartPos.position;
            }
            else
                choice.position = _tutorialChoiceStartPos.position;

            StartCoroutine(MenuChoiceScale(choice));
        }
    }

    IEnumerator CheckTutorialChoiceHeight(Transform choice)
    {
        while (_tutorialChoiceIsFloating)
        {
            UpdateTutorialChoiceHeight(choice);
            yield return new WaitForSeconds(0.01f);
        }
    }

    private void UpdateTutorialChoiceHeight(Transform choice)
    {
        if (choice.position.y < -1f)
        {
            _tutorialChoiceIsFloating = false;
            choice.transform.localScale = new Vector3(0, 0, 0);
            choice.GetComponent<Rigidbody>().useGravity = false;
            choice.GetComponent<Rigidbody>().isKinematic = true;
            choice.GetComponent<Collider>().enabled = false;
            if (choice.tag == "SongChoice")
            {
                choice.position = _songChoiceStartPos.position;
            }
            else
                choice.position = _tutorialChoiceStartPos.position;

            StartCoroutine(MenuChoiceScale(choice));
        }
    }

    IEnumerator MenuChoiceScale(Transform choice)
    {
        float t = 0;

        while (Mathf.Abs(choice.localScale.x - _choiceStartScale.x) > 0.1f)
        {
            choice.localScale = Vector3.Lerp(choice.localScale, _choiceStartScale, t);
            t += Time.deltaTime * 0.05f;
            yield return null;
        }

        choice.localScale = _choiceStartScale;
        choice.GetComponent<Collider>().enabled = true;
    }

    public void AnchorAttached(Transform anch)
    {
        switch (anch.tag)
        {
            case "SongChoice":
                if ((anch.position - _songChoiceStartPos.position).magnitude < 0.06f)
                {
                    anch.GetComponent<Rigidbody>().useGravity = false;
                    anch.GetComponent<Rigidbody>().isKinematic = true;
                    _buttonCredits.GetComponent<Collider>().enabled = true;
                    _buttonQuit.GetComponent<Collider>().enabled = true;
                }
                else if ((anch.position - _starterAnchor.transform.position).magnitude < 0.06f)
                {

                    anch.GetComponent<Rigidbody>().useGravity = false;
                    anch.GetComponent<Rigidbody>().isKinematic = true;
                }
                else
                {
                    anch.GetComponent<Rigidbody>().isKinematic = false;
                    _songChoiceIsFloating = true;

                    _buttonCredits.GetComponent<Collider>().enabled = true;
                    _buttonQuit.GetComponent<Collider>().enabled = true;
                    StartCoroutine(CheckSongChoiceHeight(anch));
                }
                break;
            case "TutorialChoice":
                if ((anch.position - _tutorialChoiceStartPos.position).magnitude < 0.06f)
                {
                    anch.GetComponent<Rigidbody>().useGravity = false;
                    anch.GetComponent<Rigidbody>().isKinematic = true;
                    _buttonCredits.GetComponent<Collider>().enabled = true;
                    _buttonQuit.GetComponent<Collider>().enabled = true;

                }
                else if ((anch.position - _starterAnchor.transform.position).magnitude < 0.06f)
                {
                    anch.GetComponent<Rigidbody>().useGravity = false;
                    anch.GetComponent<Rigidbody>().isKinematic = true;


                }
                else
                {
                    anch.GetComponent<Rigidbody>().isKinematic = false;
                    _tutorialChoiceIsFloating = true;

                    _buttonCredits.GetComponent<Collider>().enabled = true;
                    _buttonQuit.GetComponent<Collider>().enabled = true;
                    StartCoroutine(CheckTutorialChoiceHeight(anch));
                }
                break;
        }


    }

    public void AnchorDetached(Transform anch)
    {
        anch.GetComponent<Rigidbody>().useGravity = true;
        anch.GetComponent<Rigidbody>().isKinematic = false;

        _buttonCredits.GetComponent<Collider>().enabled = false;
        _buttonQuit.GetComponent<Collider>().enabled = false;
    }

    public void OnPressButtonCredits()
    {

        _panelContainerActive = _panelCredits;
        _panelOpening = true;
        _panelCredits.SetActive(true);
        _buttonQuit.GetComponent<Collider>().enabled = false;
        _songChoice.GetComponent<Collider>().enabled = false;
        _tutorialChoice.GetComponent<Collider>().enabled = false;


        StartCoroutine(open(_panelCredits, false));
    }


    public void OnPressButtonQuit()
    {
        _panelOpening = true;
        _panelQuit.SetActive(true);
        _panelContainerActive = _panelQuit;
        _buttonCredits.GetComponent<Collider>().enabled = false;
        _songChoice.GetComponent<Collider>().enabled = false;
        _tutorialChoice.GetComponent<Collider>().enabled = false;

        StartCoroutine(open(_panelQuit, false));
    }

    public void showHalo(bool left = false) {
        if (!_showingHalo)
            StartCoroutine(showHaloRoutine(left));
    }

    private IEnumerator showHaloRoutine(bool left)
    {
        _showingHalo = true;

        if (left)        
            _halo.rectTransform.localScale = new Vector3(-1,1,1);        
        else
            _halo.rectTransform.localScale = new Vector3(1,1,1);

        float t = 0;

        Color tempColor = _halo.color;

        while (t < 1)
        {
            t += Time.deltaTime * 5;
            tempColor.a = Mathf.Lerp(0, 1, t);
            _halo.color = tempColor;
            yield return null;
        }
    }

    public void hideHalo()
    {
        _showingHalo = false;
        float t = 0;

        Color tempColor = _halo.color;
        tempColor.a = 0;
        _halo.color = tempColor;
    }
}

