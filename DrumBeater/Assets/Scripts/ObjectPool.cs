using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    [SerializeField] private GameObject objToPool;
    [SerializeField] private int amountToPool = 20;
    [HideInInspector] public List<GameObject> pooledObjs;

    private static ObjectPool _instance;
    public static ObjectPool instance { get => _instance; }

    private void Awake()
    {
        if (_instance != null && _instance != this)
            Destroy(this.gameObject);
        else
            _instance = this;
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

    public void changeObjsMaterial(Material mat)
    {
        for (int i = 0; i < amountToPool; i++)
        {
            if (pooledObjs[i].activeInHierarchy)
                pooledObjs[i].GetComponent<Renderer>().material = mat;
        }
    }

    public void changeObjsMaterial(List<Material> mat)
    {
        for (int i = 0; i < amountToPool; i++)
        {
            if (pooledObjs[i].activeInHierarchy)
                pooledObjs[i].GetComponent<Renderer>().material = mat[Random.Range(0, mat.Count - 1)];
        }
    }
}
