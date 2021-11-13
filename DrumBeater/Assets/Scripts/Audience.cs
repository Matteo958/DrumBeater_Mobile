using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Audience: MonoBehaviour
{
    public float jumpPower;

    private bool _canJump;

    void Start()
    {        
        jumpPower = Random.Range(1.8f, 2.3f);        
    }

    public void Jump()
    {
        _canJump = true;
        transform.GetComponent<Rigidbody>().AddForce(transform.up * jumpPower, ForceMode.Impulse);
    }

    private void OnCollisionEnter(Collision collision)
    {        
        if (collision.gameObject.layer == 9 && !GameManager.instance.gamePaused && GameManager.instance.levelStarted)        
            Jump();
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
