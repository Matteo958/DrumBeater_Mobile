using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Public : MonoBehaviour
{
    public float startJumpPower;
    public float jumpPower;
    // Start is called before the first frame update
    void Start()
    {
        startJumpPower = Random.Range(1.8f, 2.3f);
        jumpPower = startJumpPower;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        
        if (collision.gameObject.layer == 9)
        {
            transform.GetComponent<Rigidbody>().AddForce(transform.up * jumpPower, ForceMode.Impulse);
        }
            
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "ForceField")
        {
            jumpPower = Random.Range(4.0f, 5.5f);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "ForceField")
        {
            jumpPower = startJumpPower;
        }
    }
}
