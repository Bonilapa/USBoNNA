using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waiter : MonoBehaviour {
    public void act()
    {
        GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0);
        GetComponent<Rigidbody>().AddForce(new Vector3(0, 0, 0));
        //return new Vector3(transform.position.x + temp1, transform.position.y, transform.position.z + temp2);
    }
}
