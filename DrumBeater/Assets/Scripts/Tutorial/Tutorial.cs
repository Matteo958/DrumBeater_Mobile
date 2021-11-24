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
        }
    }

    IEnumerator menuTutorialCoroutine()
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

    

    void Start()
    {
        tutorialState = 0;
    }
}
