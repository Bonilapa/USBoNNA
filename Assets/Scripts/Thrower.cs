using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Thrower : MonoBehaviour {


    public GameObject ParticleSample;
    public uint ParticleSpeed = 5;
    public void act()
    {
        GameObject throwElement = Instantiate(ParticleSample);
        GetComponent<Agent>().ReduceResource();
        GetComponent<Agent>().DTTResourceChange++;
        throwElement.transform.position = transform.position;
        throwElement.GetComponent<Particle>().Source = gameObject;
        throwElement.SetActive(true);
        throwElement.GetComponent<Particle>().enabled = true;
        throwElement.GetComponent<Rigidbody>().AddForce(
            GetComponent<Rigidbody>().velocity.normalized * ParticleSpeed * 100);
    }
}
