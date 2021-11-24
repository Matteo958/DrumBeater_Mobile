using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial : MonoBehaviour
{
    [SerializeField] private GameObject _startText = default;
    [SerializeField] private GameObject _mainMenuText = default;

    [SerializeField] private Transform _gestures = default;
    public int tutorialState;

    private static Tutorial _instance;
    public static Tutorial instance { get => _instance; }

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

    public void CheckTutorialState()
    {
        switch (tutorialState)
        {
            case 0:
                _startText.SetActive(true);
                _mainMenuText.SetActive(false);
                _gestures.GetComponent<Gestures>().ThumbUp();
                break;
            case 1:
                _startText.SetActive(false);
                _mainMenuText.SetActive(true);
                break;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        tutorialState = 0;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
