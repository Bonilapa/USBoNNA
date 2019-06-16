using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UserInterface : MonoBehaviour {
    public TargetMarker targetMarker;
    public Text ResourceField;
    public Timer Timer;
    public Text TimerField;

    private void FixedUpdate()
    {
        if (targetMarker.target != null)
        {
            ResourceField.text = targetMarker.target.ResourceAmount.ToString();
        }
        TimerField.text = Timer.seconds.ToString();
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if(Physics.Raycast(ray, out hit))
            {
                SphereCollider sphere = hit.collider as SphereCollider;
                if(sphere != null)
                {
                    targetMarker.EnableTarget(sphere.GetComponent<Agent>());
                }
            }
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            targetMarker.DisableTarget();
        }
    }
}
