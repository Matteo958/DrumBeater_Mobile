using System.Collections;
using System.Collections.Generic;
using UnityEngine;

    public class UIManager : MonoBehaviour
    {
    [SerializeField] private GameObject _panelTutorial = default;
    [SerializeField] private GameObject _panelOptions = default;
    [SerializeField] private GameObject _panelCredits = default;
    [SerializeField] private GameObject _panelQuit = default;

        private static UIManager _instance;

        public static UIManager instance { get => _instance; }

    private float _panelStartScaleX;
    private Vector3 _panelArrivingScale;

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
        _panelStartScaleX = _panelQuit.transform.localScale.x;
    }

    public void openPanel(string _menuChoice)
        {
            switch (_menuChoice)
            {
                case "Songs":
                    break;
                case "Tutorial":
                _panelTutorial.SetActive(true);
                StartCoroutine(open(_panelTutorial));
                break;
                case "Options":
                _panelOptions.SetActive(true);
                StartCoroutine(open(_panelOptions));
                break;
                case "Credits":
                _panelCredits.SetActive(true);
                StartCoroutine(open(_panelCredits));
                break;
                case "Quit":
                _panelQuit.SetActive(true);
                StartCoroutine(open(_panelQuit));
                    break;
            }
        }

    public void closePanel(string _menuChoice)
    {
        switch (_menuChoice)
        {
            case "Songs":
                break;
            case "Tutorial":
                StartCoroutine(close(_panelTutorial));
                break;
            case "Options":
                StartCoroutine(close(_panelOptions));
                break;
            case "Credits":
                StartCoroutine(close(_panelCredits));
                break;
            case "Quit":
                StartCoroutine(close(_panelQuit));
                break;
        }
    }

    IEnumerator open(GameObject _panel)
    {
        float t = 0;
        _panelArrivingScale = new Vector3(1, _panel.transform.localScale.y, _panel.transform.localScale.z);
        while (Mathf.Abs(_panel.transform.localScale.x - _panelArrivingScale.x) > 0.001f)
        {
            _panel.transform.localScale = Vector3.Lerp(_panel.transform.localScale, _panelArrivingScale, t);
            t += Time.deltaTime * 0.1f;
            yield return null;
        }
    }

    IEnumerator close(GameObject _panel)
    {
        float t = 0;
        _panelArrivingScale = new Vector3(_panelStartScaleX, _panel.transform.localScale.y, _panel.transform.localScale.z);
        while (Mathf.Abs(_panel.transform.localScale.x - _panelArrivingScale.x) > 0.001f)
        {
            _panel.transform.localScale = Vector3.Lerp(_panel.transform.localScale, _panelArrivingScale, t);
            t += Time.deltaTime * 0.1f;
            yield return null;
        }
        _panel.SetActive(false);
    }
}

