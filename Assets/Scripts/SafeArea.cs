using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SafeArea : MonoBehaviour {

    public Observer ObserverInstance;
    public Agent Owner;
    public ResourceController[] resources;
    public bool Full = false;
    
    public void AddResourceUnit(ResourceController newResource) {

        for (int i = 0; i < resources.Length; i++)
        {
            if (resources[i] == null)
            {
                resources[i] = newResource;
                if (areaIsFull())
                {
                    Full = true;
                }

                return;
            }
        }
    }
    private bool areaIsFull()
    {
        for(int i = 0; i< resources.Length; i++)
        {
            if(resources[i] == null)
            {
                return false;
            }
        }
        return true;
    }
}
