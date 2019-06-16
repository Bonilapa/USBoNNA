using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dropper : MonoBehaviour{
    public void act()
    {
        GetComponent<Agent>().ObserverInstance.DropResourceUnit(GetComponent<Agent>());
    }
}
