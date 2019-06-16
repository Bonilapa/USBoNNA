using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InnerWall : MonoBehaviour
{
    public int StartLiveLength = 10;
    public int LiveLength = 10;
    public void DecrementLiveLength()
    {
        LiveLength--;
        if (LiveLength <= 0)
        {
            RemoveWalls();
        }
    }

    private void RemoveWalls()
    {
        this.gameObject.SetActive(false);
    }
}
