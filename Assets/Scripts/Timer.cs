using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour {

    public Observer ObserverInstance;
    public InnerWall InnerWalls;
    public float Value = 0.0f;
    public int seconds;

    private float timer;
    public void Begin()
    {

        InnerWalls.LiveLength = InnerWalls.StartLiveLength;
        InnerWalls.gameObject.SetActive(true);
        timer = Value;
    }
    // Update is called once per frame
    void Update()
    {
        if (ObserverInstance.Started && !SimulationIsFinished())
        {
            timer -= Time.deltaTime;
            seconds = (int)timer % 60;

            if (timer <= 0)
            {
                for (int i = 0; i < ObserverInstance.agents.Length; i++)
                {
                    Agent agent = ObserverInstance.agents[i];
                    if (agent != null)
                    {
                        agent.ReduceResource();
                        agent.IncreaseResource();//unlimited life
                        agent.DeadEndReset();
                        agent.DTTResourceChange++;
                        agent.Age++;
                    }
                }
                ObserverInstance.InitializeResource();
                ObserverInstance.WorldAge++;
                if (InnerWalls.LiveLength > 0)
                    InnerWalls.DecrementLiveLength();

                timer = Value;
            }
        }
        else
        {
            ObserverInstance.Started = false;
            ObserverInstance.PostStore();
            ObserverInstance.StartNewSimulation();
        }
    }

    private bool SimulationIsFinished()
    {
        bool temp = true;
        for (int i = 0; i < ObserverInstance.agents.Length; i++)
        {
            Agent agent = ObserverInstance.agents[i];
            if (agent != null)
            {
                temp = false;
                break;
            }
        }
        return temp;
    }
}
