using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial : MonoBehaviour
{
    [SerializeField] private GameObject _startText = default;
    [SerializeField] private GameObject _mainMenuText = default;
    [SerializeField] private GameObject _mainMenuText2 = default;
    [SerializeField] private GameObject _mainMenuText3 = default;
    [SerializeField] private GameObject _mainMenuText4 = default;

    [SerializeField] private GameObject _noteText = default;
    [SerializeField] private GameObject _changeTrackText = default;
    [SerializeField] private GameObject _multipleNotesText = default;
    [SerializeField] private GameObject _pauseText = default;
    [SerializeField] private GameObject _autoText = default;
    [SerializeField] private GameObject _soloText = default;
    [SerializeField] private GameObject _finalText = default;

    [SerializeField] private Transform _gestures = default;
    public int tutorialState;

    private static Tutorial _instance;
    public static Tutorial instance { get => _instance; }

    private void Awake()
    {
        if (_instance != null && _instance != this)
            Destroy(this.gameObject);
        else
            _instance = this;
    }

    void Start()
    {
        tutorialState = 0;
    }

    public void CheckTutorialState()
    {
        switch (tutorialState)
        {
            case 0:
                _startText.SetActive(true);
                _mainMenuText.SetActive(false);
                _mainMenuText2.SetActive(false);
                _mainMenuText3.SetActive(false);
                _mainMenuText4.SetActive(false);
                Gestures.instance.canThumbUp = true;
                Gestures.instance.ThumbUp();
                break;
            case 1:
                _startText.SetActive(false);
                StartCoroutine(menuTutorialCoroutine());
                break;
            case 2:
                UIManager.instance.OnPressButtonOptions();
                break;
            case 3:
                //one note
                _startText.SetActive(false);
                _noteText.SetActive(true);
                StartCoroutine(spawnFirstNote());
                Gestures.instance.canThumbUp = true;
                Gestures.instance.ThumbUp();
                break;
            case 4:
                //change track
                _noteText.SetActive(false);
                _changeTrackText.SetActive(true);
                StartCoroutine(GameManager.instance.verifyTrack(1));
                break;
            case 5:
                //three notes
                _changeTrackText.SetActive(false);
                _multipleNotesText.SetActive(true);
                StartCoroutine(spawnThreeNotes());
                Gestures.instance.canThumbUp = true;
                Gestures.instance.ThumbUp();
                break;
            case 6:
                //pause
                _multipleNotesText.SetActive(false);
                _pauseText.SetActive(true);
                Gestures.instance.canCloseHand = true;
                //Gestures.instance.canThumbUp = true;
                //Gestures.instance.ThumbUp();
                break;
            case 7:
                //auto
                _pauseText.SetActive(false);
                _autoText.SetActive(true);
                StartCoroutine(spawnAutoNotes());
                Gestures.instance.canThumbUp = true;
                Gestures.instance.ThumbUp();
                break;
            case 8:
                //solo
                _autoText.SetActive(false);
                _soloText.SetActive(true);
                GameManager.instance.soloText.GetComponent<RotateText>().activate();
                GameManager.instance.soloIsActive = true;
                Gestures.instance.canThumbUp = true;
                Gestures.instance.ThumbUp();
                break;
            case 9:
                //end
                _soloText.SetActive(false);
                _finalText.SetActive(true);
                Gestures.instance.canThumbUp = true;
                Gestures.instance.ThumbUp();
                break;
            case 10:
                //close tutorial
                _finalText.SetActive(false);
                break;
        }
    }

    private IEnumerator menuTutorialCoroutine()
    {
        float t = 0;

        _mainMenuText.SetActive(true);
        while (t < 7)
        {
            t += Time.deltaTime;
            if (!UIManager.instance.tutorialGuideIsActive)
                yield break;
            yield return null;

        }
        _mainMenuText.SetActive(false);
        _mainMenuText2.SetActive(true);
        while (t < 17)
        {
            t += Time.deltaTime;
            if (!UIManager.instance.tutorialGuideIsActive)
                yield break;
            yield return null;
        }
        _mainMenuText2.SetActive(false);
        _mainMenuText3.SetActive(true);
        while (t < 27)
        {
            t += Time.deltaTime;
            if (!UIManager.instance.tutorialGuideIsActive)
                yield break;
            yield return null;
        }
        _mainMenuText3.SetActive(false);
        _mainMenuText4.SetActive(true);
        Gestures.instance.canThumbUp = true;
        Gestures.instance.ThumbUp();
    }

    private IEnumerator spawnFirstNote()
    {
        while (tutorialState == 3)
        {
            TutorialNoteSpawner.instance.spawnNote(4);
            yield return new WaitForSeconds(7);
        }
    }

    private IEnumerator spawnThreeNotes()
    {
        while (tutorialState == 5)
        {
            TutorialNoteSpawner.instance.spawnNote(0);
            yield return new WaitForSeconds(1);
            TutorialNoteSpawner.instance.spawnNote(1);
            yield return new WaitForSeconds(1);
            TutorialNoteSpawner.instance.spawnNote(2);
            yield return new WaitForSeconds(7);
        }
    }

    private IEnumerator spawnAutoNotes()
    {
        while (tutorialState == 7)
        {
            GameManager.instance.hasAutoMode = true;
            for (int i = 0; i < 10; i++)
            {
                if (tutorialState != 7)
                    yield break;

                TutorialNoteSpawner.instance.spawnNote((int)Random.Range(0, 2));
                yield return new WaitForSeconds(Random.Range(2, 3));
            }
            GameManager.instance.autoMode = false;
            PointsManager.instance.comboHitsAuto = 0;

            yield return new WaitForSeconds(15);
        }
    }
}
