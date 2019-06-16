using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetMarker : MonoBehaviour {
    public Agent target;
    private Vector3 position;
    // Update is called once per frame
    void FixedUpdate()
    {
        if (target != null)
        {
            transform.position = new Vector3(target.transform.position.x, 2f, target.transform.position.z);
        }
    }
    public void EnableTarget(Agent obj)
    {
        target = obj;
        gameObject.SetActive(true);
    }

    public void DisableTarget()
    {
        gameObject.SetActive(false);
        target = null;
    }
}
