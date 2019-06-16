using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class Agent : MonoBehaviour
{

    public Observer ObserverInstance;
    public float Step;
    public int ResourceAmount = 10;
    public ulong Name;
    public uint SpreadBorder;
    public int DeadEndAgentCounterValue = 15;
    public Vector3 Direction;
    public ulong Age = 0;
    public uint generation = 1;
    public short DTTResourceChange = 0;
    public short positiveChange = 0;
    public short negativeChange = 0;
    public TreeOfDecisions tree;
    public float Angle;


    private Rigidbody rb;
    private int deadEndAgentCounter;
    private NNAI nnai;
    private TreeOfDecisions.Type decision;
    private TreeOfDecisions.Type oldDecision;
    private Vector3 oldDirection;
    // Use this for initialization
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        nnai = GetComponent<NNAI>();
        //Direction = new Vector3(UnityEngine.Random.Range(-1, 2) * Step, 0.0f, UnityEngine.Random.Range(-1,2) * Step);
        tree = new TreeOfDecisions(this);
        decision = nnai.GetDecision();
        tree.WalkWith(decision);
        Direction = nnai.GetDirection();
        deadEndAgentCounter = DeadEndAgentCounterValue;


        //Vector3 j = new Vector3(-0.5f, 0, 0.5f);
        //j = j.normalized;
        //proc(j.x, j.z);
        //j = new Vector3(-0.5f, 0, -0.5f);
        //j = j.normalized;
        //proc(j.x, j.z);
        //j = new Vector3(0.5f, 0, 0.5f);
        //j = j.normalized;
        //proc(j.x, j.z);
        //j = new Vector3(0.5f, 0, -0.5f);
        //j = j.normalized;
        //proc(j.x, j.z);
    }

    void proc(double x, double y)
    {

        double acos, asin, asin90, acos90;

        acos = Math.Acos(x);
        asin = Math.Asin(y);
        if (asin < 0)
        {
            //TODO заменить везде углы на радианы, 
            //чтобы не тратить процессорное время
            asin = (2*Math.PI) + asin;
        }
        asin90 = (Math.PI - Math.Asin(y));
        acos90 = (2 * Math.PI - Math.Acos(x));
        Debug.Log("asin90 " + asin90);
        Debug.Log("acos90 " + acos90);
        Debug.Log("asin " + asin);
        Debug.Log("acos " + acos);
        if(asin != acos)
        {
            Debug.Log("hihuhuhuhuhu"+ asin+" "+acos);
        }
        if (asin == acos)
        {
            Debug.Log("Common1 ======" + asin);
        }
        else if (asin == acos90)
        {
            Debug.Log("Common2 ======" + asin);
        }
        else if (asin90 == acos)
        {
            Debug.Log("Common3 ======" + asin90);
        }
        else if (asin90 == acos90)
        {
            Debug.Log("Common4 ======" + asin90);
        }
    }


    // Update is called once per frame
    void FixedUpdate()
    {
        //Debug.Log(UnityEngine.Random.value);
        //Debug.Log(positiveChange + " " + negativeChange +" "+ DTTResourceChange);

        if (tree == null || nnai.neuralNetwork == null || deadEndAgentCounter <= 0)
        {
            Debug.Log("Memory bug. " + ObserverInstance.SimulationNumber + " simulation is inconsisnent.");

            
            //TODO add DeadEng label into agent stored data


            ObserverInstance.RemoveAgent(gameObject);
        }
        nnai.InputReset();
        if (positiveChange > 0 || negativeChange > 0 || DTTResourceChange > 0)
        {

            tree.ChangeOutcomes(positiveChange, (short)(negativeChange + DTTResourceChange));
            nnai.DecisionBackPropagation(tree.RebalanceValue, decision);
            nnai.DirectionBackPropagation(tree.RebalanceValue, Direction);
            tree.RebalanceValue = 0;
            //Debug.Log("Rebalance of " + decision);

            if (negativeChange > 0 || positiveChange > 0)
            {
                tree.moveTo(tree.Root);
                deadEndAgentCounter = DeadEndAgentCounterValue;
            }
            resetResourceChange();

            decision = nnai.GetDecision();
            if (decision != oldDecision)
            {
                DeadEndReset();
                oldDecision = decision;
            }
            processDecision(decision);
            tree.WalkWith(decision);

        }
        Direction = nnai.GetDirection();
        if (Direction != oldDirection)
        {
            DeadEndReset();
            oldDirection = Direction;
        }
        //Debug.Log("Direction: " + RadianToDegree(CalcAngle(Direction.normalized)));
        //Debug.Log("Velocity: " + RadianToDegree(CalcAngle(rb.velocity.normalized)));
        rb.AddForce(Direction * Step);
    }

    public void DeadEndReset()
    {
        deadEndAgentCounter = DeadEndAgentCounterValue;
    }

    private void processDecision(TreeOfDecisions.Type type)
    {
        switch (type)
        {
            case TreeOfDecisions.Type.Wait:
                {
                    GetComponent<Waiter>().act();
                    break;
                }
            case TreeOfDecisions.Type.Drop:
                {
                    GetComponent<Dropper>().act();
                    break;
                }
            case TreeOfDecisions.Type.Throw:
                {
                    GetComponent<Thrower>().act();
                    break;
                }
            case TreeOfDecisions.Type.Punch:
                {
                    GetComponent<Puncher>().Punch();
                    break;
                }
        }
    }
    private void resetResourceChange()
    {
        positiveChange = 0;
        negativeChange = 0;
        DTTResourceChange = 0;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Resource"))
        {
            IncreaseResource();
            positiveChange++;
            ObserverInstance.RemoveResourceUnit(other.gameObject);
        }
    }

    public void ReduceResource()
    {
        ResourceAmount--;
        if (ResourceAmount <= 0)
        {
            string fileName = "ToD_" + Name;
            tree.StoreTree(ObserverInstance.Path + fileName);
            fileName = "NN_" + Name;
            nnai.StoreNetwork(ObserverInstance.Path + fileName);
            ObserverInstance.RemoveAgent(gameObject);
        }
    }

    public void IncreaseResource()
    {
        ResourceAmount++;
        
        if(ResourceAmount == SpreadBorder)
        {
            Spread();
        }
    }

    private Vector3 Manual()
    {
        int t1 = 0, t2 = 0;
        if (Input.GetAxis("Vertical") > 0)
            t2 = 1;
        else if (Input.GetAxis("Vertical") < 0)
            t2 = -1;
        if (Input.GetAxis("Horizontal") > 0)
            t1 = 1;
        else if (Input.GetAxis("Horizontal") < 0)
            t1 = -1;
        if (Input.GetKeyDown(KeyCode.Space))
        {
            GetComponent<Thrower>().act();
        }
        return new Vector3(t1, 0.0f, t2);
        
    }

    public void Spread()
    {
        int i = 0;
        while (i < ObserverInstance.agents.Length)
        {
            if (ObserverInstance.agents[i] == null)
            {
                ObserverInstance.agents[i] = Instantiate(gameObject).GetComponent<Agent>();
                ResourceAmount /= 2;
                ObserverInstance.agents[i].ResourceAmount = ResourceAmount;
                ObserverInstance.agents[i].Name = ObserverInstance.AgentIdCounter;
                ObserverInstance.AgentIdCounter++;
                ObserverInstance.agents[i].gameObject.name = "Agent_" + ObserverInstance.agents[i].Name;
                ObserverInstance.agents[i].transform.position = transform.position - Direction;
                ObserverInstance.agents[i].Age = 0;
                ObserverInstance.agents[i].generation ++;
                ObserverInstance.agents[i].GetComponent<NNAI>().Begin();
                ObserverInstance.agents[i].GetComponent<NNAI>().Mutate();
                break;
            }
            i++;
        }
    }
}