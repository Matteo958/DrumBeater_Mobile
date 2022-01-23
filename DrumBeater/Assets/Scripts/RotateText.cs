using UnityEngine;

public class RotateText : MonoBehaviour
{
    public float speed;
    private bool active;
    public Material activeMaterial;
    private Material originalMaterial;
    public GameObject spotLight;

    private void Start()
    {
        originalMaterial = GetComponent<Renderer>().material;
    }

    // Update is called once per frame
    void Update()
    {
        if (active && !GameManager.instance.gamePaused)
            transform.Rotate(0, speed * Time.deltaTime, 0);
    }

    public void activate()
    {
        active = true;
        GetComponent<Renderer>().material = activeMaterial;
        spotLight.SetActive(true);
    }

    public void deactivate()
    {
        active = false;
        GetComponent<Renderer>().material = originalMaterial;
        spotLight.SetActive(false);
    }
}
