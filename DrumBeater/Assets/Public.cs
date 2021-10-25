using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Public : MonoBehaviour
{
    public float jumpPower;

    private bool _canJump;
    // Start is called before the first frame update
    void Start()
    {
        
        jumpPower = Random.Range(1.8f, 2.3f); ;
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        
        if (collision.gameObject.layer == 9)
        {
            _canJump = true;
            transform.GetComponent<Rigidbody>().AddForce(transform.up * jumpPower, ForceMode.Impulse);
        }
            
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "ForceField" && _canJump)
        {
            _canJump = false;
            transform.GetComponent<Rigidbody>().AddForce(transform.up * Random.Range(2.2f, 2.7f), ForceMode.Impulse);
            
        }
    }
}
