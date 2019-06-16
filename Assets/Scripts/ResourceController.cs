using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResourceController : MonoBehaviour {

    public Observer ObserverInstance;
    public ulong Id;

	// Update is called once per frame
	void Update () {
        transform.Rotate(new Vector3(45, 45, 45) * Time.deltaTime);
	}

    //void OnTriggerEnter(Collider other)
    //{
    //    if (other.gameObject.CompareTag("Agent"))
    //    {
    //        other.gameObject.SetActive(false);
    //        //observer.
    //    }
    //}
}
