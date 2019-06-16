using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PunchArea : MonoBehaviour {
    public GameObject Source;
    public int Life = 10;

    private void FixedUpdate()
    {
        if (Life > 0)
        {
            Life--;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Agent") && other.gameObject != Source)
        {
            other.gameObject.GetComponent<Agent>().ReduceResource();
            other.gameObject.GetComponent<Agent>().negativeChange++;
        }
    }
}
