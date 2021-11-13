using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Slider : MonoBehaviour
{
    [SerializeField] private Image _fillAreaTarget = default;
    [SerializeField] private Image _handleTarget = default;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        _fillAreaTarget.GetComponent<RectTransform>().sizeDelta = new Vector2(_handleTarget.rectTransform.localPosition.x + 500, 20);
        
    }
}
