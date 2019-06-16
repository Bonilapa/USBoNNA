using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Particle : MonoBehaviour {
    public GameObject Source;
    public short ParticleDamage = 2;

    public int Life = 10;

    private void FixedUpdate()
    {
        if (GetComponent<Particle>().enabled && Life > 0)
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
        if (other.gameObject.CompareTag("Wall"))
        {
            Destroy(gameObject);
        }
        else if (other.gameObject.CompareTag("Agent") && other.gameObject != Source)
        {
            other.gameObject.GetComponent<Agent>().ReduceResource();
            other.gameObject.GetComponent<Agent>().ReduceResource();
            other.gameObject.GetComponent<Agent>().negativeChange += ParticleDamage;
            Destroy(gameObject);
        }
    }
}
