using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    [SerializeField] private GameObject objToPool;
    [SerializeField] private GameObject objToPoolTutorial;
    [SerializeField] private int amountToPool = 20;
    [HideInInspector] public List<GameObject> pooledObjs;
    [HideInInspector] public List<GameObject> pooledObjsTutorial;

    public static ObjectPool instance { get; private set; }

    private void Awake()
    {
        if (instance != null && instance != this)
            Destroy(this.gameObject);
        else
            instance = this;
    }

    private void Start()
    {
        pooledObjs = new List<GameObject>();
        GameObject tmp;
        for (int i = 0; i < amountToPool; i++)
        {
           
            tmp = Instantiate(objToPool);
            tmp.SetActive(false);
            pooledObjs.Add(tmp);

            tmp = Instantiate(objToPoolTutorial);
            tmp.SetActive(false);
            pooledObjsTutorial.Add(tmp);
        }
        
    }

    public GameObject getPooledObj()
    {
        for (int i = 0; i < amountToPool; i++)
        {
            if (!pooledObjs[i].activeInHierarchy)
                return pooledObjs[i];
        }

        return null;
    }

    public void activateAuto(Material mat)
    {
        for (int i = 0; i < amountToPool; i++)
        {
            if (pooledObjs[i].activeInHierarchy)
            {
                pooledObjs[i].GetComponent<Renderer>().material = mat;
                
                pooledObjs[i].GetComponent<NoteController>().auto = true;
            }
                
        }
    }

    public void reset()
    {
        for (int i = 0; i < amountToPool; i++)
        {
            if (pooledObjs[i].activeInHierarchy)
            {
                pooledObjs[i].transform.parent = null;
                pooledObjs[i].SetActive(false);
            }

        }
    }

    public GameObject getPooledObjTutorial()
    {
        for (int i = 0; i < amountToPool; i++)
        {
            if (!pooledObjsTutorial[i].activeInHierarchy)
                return pooledObjsTutorial[i];
        }

        return null;
    }

    public void activateAutoTutorial(Material mat)
    {
        for (int i = 0; i < amountToPool; i++)
        {
            if (pooledObjsTutorial[i].activeInHierarchy)
            {
                pooledObjsTutorial[i].GetComponent<Renderer>().material = mat;

                pooledObjsTutorial[i].GetComponent<TutorialNote>().auto = true;
            }

        }
    }

    public void resetTutorial()
    {
        for (int i = 0; i < amountToPool; i++)
        {
            if (pooledObjsTutorial[i].activeInHierarchy)
            {
                pooledObjsTutorial[i].transform.parent = null;
                pooledObjsTutorial[i].SetActive(false);
            }

        }
    }

    //public void deactivateAuto()
    //{
    //    for (int i = 0; i < amountToPool; i++)
    //    {
    //        if (pooledObjs[i].activeInHierarchy)
    //        {
    //            pooledObjs[i].GetComponent<NoteController>().auto = false;
    //        }
    //    }
    //}
}
