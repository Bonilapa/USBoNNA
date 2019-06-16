using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallCollisionController : MonoBehaviour {
    public Observer ObserverInstance;

    // Update is called once per frame
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<ResourceController>())
        {
            other.transform.position = ObserverInstance.GetPositionForResource(other.GetComponent<ResourceController>());
        }
    }
}
