using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Puncher : MonoBehaviour {
    public GameObject PunchAreaSample;

    public void Punch()
    {
        GameObject punch = Instantiate(PunchAreaSample);
        punch.transform.position = transform.position;
        punch.GetComponent<PunchArea>().Source = gameObject;
        punch.SetActive(true);
    }
}
