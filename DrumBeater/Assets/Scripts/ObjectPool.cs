using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    [SerializeField] private GameObject objToPool;
    [SerializeField] private int amountToPool = 20;
    [HideInInspector] public List<GameObject> pooledObjs;

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
            tmp.name = "Note" + i;
            tmp.SetActive(false);
            pooledObjs.Add(tmp);
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
