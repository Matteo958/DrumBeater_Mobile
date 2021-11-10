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
        if (active)
            transform.Rotate(0, speed * Time.deltaTime, 0);
    }

    public void activate()
    {
        active = true;
        GetComponent<Renderer>().material = activeMaterial;
        spotLight.SetActive(false);
    }

    public void deactivate()
    {
        active = false;
        GetComponent<Renderer>().material = originalMaterial;
        spotLight.SetActive(true);
    }
}
