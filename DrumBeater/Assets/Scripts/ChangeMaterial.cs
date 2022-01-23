using Leap.Unity.Interaction;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeMaterial : MonoBehaviour
{
    [SerializeField] private Material _songsMaterial;
    [SerializeField] private Material _tutorialMaterial;
    [SerializeField] private Material _startMaterial;
    [SerializeField] private GameObject _starterAnchor;

    private bool _choiceIsAttached;
    private int cont = 0;


    private Material[] mats;
    // Start is called before the first frame update
    void Start()
    {
        mats = GetComponent<Renderer>().materials;
        _choiceIsAttached = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void changeMaterial()
    {
        switch (_starterAnchor.GetComponent<Anchor>().choice)
        {
            case "Songs":
                _choiceIsAttached = true;
                StartCoroutine(choiceMaterial(_songsMaterial));
                break;
            case "Tutorial":
                _choiceIsAttached = true;
                StartCoroutine(choiceMaterial(_tutorialMaterial));
                break;
            case "":
                _choiceIsAttached = false;
                StartCoroutine(returnStartMaterial(_startMaterial));
                break;
        }
    }

    IEnumerator choiceMaterial(Material arrivalMat)
    {
        for (int i = cont; i < transform.childCount; i++)
        {
            if (_choiceIsAttached)
            {
                cont++;
                transform.GetChild(i).gameObject.GetComponent<Renderer>().material = arrivalMat;
                yield return new WaitForSeconds(0.03f);
            }   
        }
        
       
    }

    IEnumerator returnStartMaterial(Material arrivalMat)
    {
        for (int i = cont - 1; i >= 0; i--)
        {
            if (!_choiceIsAttached)
            {
                cont--;
                transform.GetChild(i).gameObject.GetComponent<Renderer>().material = arrivalMat;
                yield return new WaitForSeconds(0.03f);
            }
        }
    }
}
