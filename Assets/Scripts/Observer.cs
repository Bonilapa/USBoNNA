using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;

public class Observer : MonoBehaviour {

    /// <summary>
    /// Number of Resource on playfield and any agent has
    /// </summary>
    public uint SimulationNumber = 0;
    public string Path;
    public ulong WorldAge = 0;
    public Agent AgentSample;
    public int AgentsNumber = 10;
    public ResourceController ResourceSample;
    public ulong AgentIdCounter = 0;
    public ulong ResourceIdCounter = 0;
    public Agent[] agents;
    public ResourceController[] resources;
    public SafeArea[] safeAreas;
    public Timer Timer;
    public bool Started = false;

    private int OverallResourceAmount;

    // Use this for initialization
    void Start ()
    {
        StartNewSimulation();
    }
	
    public void InitializeResource()
    {
        int i = 0;
        int Overall = CalculateOverallResource();

        while (Overall < OverallResourceAmount && i < resources.Length)
        {
            if (resources[i] == null)
            {
                resources[i] = Instantiate(ResourceSample).gameObject.GetComponent<ResourceController>();
                resources[i].Id = ResourceIdCounter;
                ResourceIdCounter++;
                resources[i].transform.position = GetPositionForResource(resources[i]);
                resources[i].gameObject.SetActive(true);
                Overall++;
            }
            i++;
        }
    }

    public void InitializeAgents()
    {
        int i = 0;
        if (agents[i] == null)
        {
            while (i < AgentsNumber)
            {
                {
                    agents[i] = Instantiate(AgentSample).gameObject.GetComponent<Agent>();
                    agents[i].Name = AgentIdCounter;
                    AgentIdCounter++;
                    agents[i].gameObject.name = "Agent_" + agents[i].Name;
                    agents[i].transform.position = GetPositionForAgent(agents[i]);
                    agents[i].gameObject.SetActive(true);
                    //agents[i].GetComponent<NNAI>().Begin();
                    i++;
                }
            }
        }
    }

    /// <summary>
    /// Полностью удаляет обьект ресурса
    /// </summary>
    /// <param name="other"></param>
    public void RemoveResourceUnit(GameObject other)
    {
        for (int i = 0; i < resources.Length; i++)
        {
            if (resources[i] != null && resources[i].Id == other.GetComponent<ResourceController>().Id)
            {
                resources[i] = null;
            }
        }

        for (int i = 0; i < safeAreas.Length; i++)
        {
            for (int j = 0; j < safeAreas[i].resources.Length; j++)
            {
                if (safeAreas[i].resources[j] != null
                    && safeAreas[i].resources[j].Id == other.GetComponent<ResourceController>().Id)
                {
                    safeAreas[i].resources[j] = null;
                }
            }
        }
        foreach(SafeArea area in safeAreas)
        {
            for(int i = 0; i < area.resources.Length; i++)
            {
                if(area.resources[i] == null)
                {
                    area.Full = false;
                }
            }
        }
        Destroy(other);
    }

    public void RemoveAgent(GameObject other)
    {
        for (int i = 0; i < agents.Length; i++)
        {
            if (agents[i] != null && agents[i].Name == other.GetComponent<Agent>().Name)
            {
                agents[i] = null;
            }
        }
        Destroy(other);
    }

    public void EndSimulation()
    {
        foreach(Agent agent in agents)
        {
            if (agent != null)
            {
                if (agent.ResourceAmount > 0)
                {
                    agent.ResourceAmount = 1;
                    agent.ReduceResource();
                }
            }
        }
        
    }
    public void PreStore()
    {
        string path = Path + "StartInfo";
        if (!File.Exists(path))
        {
            // Create a file to write to.
            using (StreamWriter sw = new StreamWriter(path, false))
            {
                //Root.RecursivelyStore(sw);
                sw.WriteLine("StartAgetnsNumber: " + AgentsNumber);
                sw.WriteLine("AgetnsLimit: " + agents.Length);
                sw.WriteLine("AgentStartResourceAmount: " + agents[0].ResourceAmount);
                sw.WriteLine("SpreadBorder: " + agents[0].SpreadBorder);
                sw.WriteLine("CycleLength: " + Timer.Value);
            }
        }
    }
    public void PostStore()
    {

        string path = Path + "EndInfo";
        if (!File.Exists(path))
        {
            // Create a file to write to.
            using (StreamWriter sw = new StreamWriter(path, false))
            {
                //Root.RecursivelyStore(sw);
                Debug.Log("Stored worldAge: " + WorldAge);
                sw.WriteLine("WorldAge: " + WorldAge);
                sw.WriteLine("AgetnsCounter: " + AgentIdCounter);
            }
        }
    }

    public void StartNewSimulation()
    {
        //simulation preparation
        //remove resources
        foreach (ResourceController res in resources)
        {
            if (res != null)
            {
                RemoveResourceUnit(res.gameObject);
            }
        }
        //renew parameters

        WorldAge = 0;
        AgentIdCounter = 0;
        ResourceIdCounter = 0;

        //renew agentSample parameters
        AgentSample.SpreadBorder = 20;
        AgentSample.ResourceAmount = 15;
        NNAI nnai = AgentSample.GetComponent<NNAI>();

        //HiddenLayerLength;
        //DecisionLearningRate = 1.0f;
        //DirectionLearningRate = 1.0f;
        //MutationRate = 0.1f;
        nnai.HiddenLayerLength = (uint)Random.Range(2, 20);
        nnai.DecisionLearningRate = Random.Range(0.01f, 10f);
        nnai.DirectionLearningRate = Random.Range(0.01f, 10f);
        nnai.MutationRate = Random.Range(0.1f, 0.9f);
        foreach (SafeArea area in safeAreas)
        {
            area.Owner = null;

        }


        Path = @"c:\USBoNNA\" + SimulationNumber + @"\";
        if (Directory.Exists(Path))
        {
            SimulationNumber++;
            Start();
            //throw new MissingComponentException("There is such simulation cathalog. Choose another simulation number.");
        }
        else
        {
            Directory.CreateDirectory(Path);
            OverallResourceAmount = resources.Length;
            InitializeAgents();
            if (CalculateOverallResource() > resources.Length)
            {
                throw new MissingComponentException("There is more resources then intended");
            }

            InitializeResource();
            foreach (Agent n in agents)
            {
                if (n != null)
                    n.GetComponent<NNAI>().Begin();
            }
            PreStore();
        }

        Started = true;
        Timer.Begin();
    }

    int CalculateOverallResource()
    {
        int Overall = 0;
        foreach(ResourceController resource in resources)
        {
            if(resource != null)
            {
                Overall++;
            }
        }
        foreach (Agent agent in agents)
        {
            if (agent != null)
            {
                Overall += agent.ResourceAmount;
            }
        }
        return Overall;
    }

    public void DropResourceUnit(Agent agent)
    {
        int i = 0;
        while (i < resources.Length)
        {
            if (resources[i] == null)
            {
                resources[i] = Instantiate(ResourceSample).gameObject.GetComponent<ResourceController>();
                agent.ReduceResource();
                agent.DTTResourceChange++;
                resources[i].Id = ResourceIdCounter;
                ResourceIdCounter++;
                resources[i].transform.position = agent.transform.position - agent.Direction;
                resources[i].gameObject.SetActive(true);
                break;
            }
            i++;
        }
    }

    public Vector3 GetPositionForResource(ResourceController newResource)
    {
        foreach (SafeArea area in safeAreas)
        {
            if (!area.Full)
            {
                area.AddResourceUnit(newResource);
                return new Vector3(Random.Range(area.transform.position.x - 4f, area.transform.position.x + 4f),
                                0.5f,
                                Random.Range(area.transform.position.z - 4f, area.transform.position.z + 4f));
            }

        }
        return new Vector3(Random.Range(13f, 87f),
                                0.5f,
                                Random.Range(1f, 49f));
    }

    public Vector3 GetPositionForAgent(Agent newAgent)
    {
        foreach (SafeArea area in safeAreas)
        {
            if (area.Owner == null)
            {
                area.Owner = newAgent;
                return new Vector3(area.transform.position.x,
                                0.5f,
                                area.transform.position.z);
            }

        }
        return new Vector3(Random.Range(13f, 87f),
                                0.5f,
                                Random.Range(1f, 49f));
    }
    //public void SetResourceUnit()
    //{
    //    InitializeResource();
    //}
}
