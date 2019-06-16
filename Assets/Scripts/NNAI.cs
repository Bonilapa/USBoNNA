using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NNAI : MonoBehaviour {
    public NeuralNetwork neuralNetwork;
    private uint DifferentDecisionAmount = 4;// Drop, Throw, Punch, Wait
    private uint DifferentDirectionAmount = 8;// 2 level of freedom
    public Observer ObserverInstance;
    public uint HiddenLayerLength;
    public float DecisionLearningRate = 1.0f;
    public float DirectionLearningRate = 1.0f;
    public float MutationRate = 0.1f;
    public bool NNExists = true;
    public void Begin()
    {
        //Resource amount
        //force angle
        //resource distance - M resources
        //{agent name, Agents distence, Agent force angle, Agent valocity angle} - N agents
        int inputLayerNeuronNumber = 3 + ObserverInstance.agents.Length * 5 - 5 + ObserverInstance.resources.Length;
        if (neuralNetwork == null)
        {
            neuralNetwork = new NeuralNetwork((uint)inputLayerNeuronNumber,
                (uint)HiddenLayerLength,
                DifferentDecisionAmount,
                DifferentDirectionAmount,
                DecisionLearningRate,
                DirectionLearningRate,
                MutationRate);
        }
        InputReset();
        neuralNetwork.RandAssignDecisionHiddenLayer();
        neuralNetwork.RandAssignDirectionHiddenLayer();
        neuralNetwork.RandAssignDecisionOutputLayer();
        neuralNetwork.RandAssignDirectionOutputLayer();
        //Debug.Log("\n");
        //foreach (NeuralNetwork.Neuron n in neuralNetwork.OutputLayer[0].inputs[0].inputs)
        //{
        //    Debug.Log(n.output);
        //}
        //foreach (float i in n.input)
        //Debug.Log(i);

    }

    public void ShowOutput()
    {
        Debug.Log("\n");
        foreach (NeuralNetwork.Neuron n in neuralNetwork.DecisionOutputLayer)
        {
            Debug.Log(n.output);
        }
    }

    public void InputReset()
    {
        if (neuralNetwork != null)
        {
            neuralNetwork.AssignInputLayer(GetInputs());
        }
        else
        {
            Debug.Log(gameObject.name + "network is dissapired");

            NNExists = false;
        }
        //foreach (NeuralNetwork.Neuron n in neuralNetwork.InputLayer)
        //{
        //    Debug.Log(n.output);
        //}
    }

    public float scaleInput(float val)
    {
        return NeuralNetwork.Neuron.atanActivationFunction2(val);
        //return NeuralNetwork.Neuron.atanActivationFunction(val);
    }
    private float distanceCoefficient(float val)
    {
        return 2 * (Mathf.Atan( -val / 10) / Mathf.PI) + 1;
    }
    /// <summary>
    /// Assign values if there were no such
    /// or assign difference between curent and previous state
    /// </summary>
    /// <returns></returns>
    public List<float> GetInputs()
    {

        List<float> inp = new List<float>();
        inp.Add(GetComponent<Agent>().ResourceAmount);
        if (GetComponent<Rigidbody>().velocity.magnitude != 0)
        {
            inp.Add(scaleInput(RadianToDegree(CalcAngle(GetComponent<Rigidbody>().velocity.normalized))));
        }
        else
        {
            inp.Add(0);
        }
        if (GetComponent<Agent>().Direction.magnitude != 0)
        {
            inp.Add(scaleInput(RadianToDegree(CalcAngle(GetComponent<Agent>().Direction))));
        }
        else
        {
            inp.Add(0);
        }
        foreach (ResourceController resource in ObserverInstance.resources)
        {
            if (resource != null)
            {
                float temp = Vector3.Distance(transform.position, resource.transform.position);
                inp.Add(scaleInput(temp) * distanceCoefficient(temp));
            }
            else
            {
                inp.Add(0);
            }
            //Debug.Log(inp[inp.Count - 1]);
        }
        foreach (Agent agent in ObserverInstance.agents)
        {
            if (agent != GetComponent<Agent>())
            {
                if (agent != null)
                {
                    float temp = agent.Name;
                    inp.Add(scaleInput(temp) * distanceCoefficient(temp));
                    //Debug.Log(inp[inp.Count - 1]);
                    temp = agent.ResourceAmount;
                    inp.Add(scaleInput(temp) * distanceCoefficient(temp));
                    //Debug.Log(inp[inp.Count - 1]);
                    temp = Vector3.Distance(transform.position, agent.transform.position);
                    inp.Add(scaleInput(temp) * distanceCoefficient(temp));
                    //Debug.Log(inp[inp.Count - 1]);
                    if (GetComponent<Rigidbody>().velocity.magnitude != 0)
                    {
                        temp = RadianToDegree(CalcAngle(GetComponent<Rigidbody>().velocity.normalized));
                        inp.Add(scaleInput(temp) * distanceCoefficient(temp));
                    }
                    else
                    {
                        inp.Add(0);
                    }
                    if (GetComponent<Agent>().Direction.magnitude != 0)
                    {
                        temp = RadianToDegree(CalcAngle(GetComponent<Agent>().Direction));
                        inp.Add(scaleInput(temp) * distanceCoefficient(temp));
                    }
                    else
                    {
                        inp.Add(0);
                    }
                }
                else
                {
                    for (int i = 0; i < 5; i++)
                    {
                        inp.Add(0);
                        //Debug.Log(inp[inp.Count - 1]);
                    }
                }
            }
        }
        return inp;
    }
    public TreeOfDecisions.Type GetDecision()
    {
        //Debug.Log("<<<" + decision + ">>>");
        //Debug.Log(neuralNetwork);
        if (neuralNetwork == null)
        {
            return TreeOfDecisions.Type.Root;
        }
        return neuralNetwork.GetDecision();
    }

    public void DecisionBackPropagation(float value, TreeOfDecisions.Type decision)
    {
        //Debug.Log(neuralNetwork);
        neuralNetwork.RebalanceDecisionBranch(value, decision);
    }
    public void DirectionBackPropagation(float value, Vector3 direction)
    {
        neuralNetwork.RebalanceDirectionBranch(value, direction);
    }

    public Vector3 GetDirection()
    {
        //Vector3 closest = new Vector3(0, 0, 0);
        //foreach (ResourceController resource in GetComponent<Agent>().ObserverInstance.resources)
        //{
        //    if (resource != null)
        //    {
        //        closest = resource.transform.position - transform.position;
        //        break;
        //    }
        //}
        //foreach (ResourceController resource in GetComponent<Agent>().ObserverInstance.resources)
        //{
        //    if (resource != null)
        //    {
        //        Vector3 temp = resource.transform.position - transform.position;
        //        if (closest.magnitude > temp.magnitude)
        //        {
        //            closest = temp;
        //        }
        //    }
        //}
        //return closest.normalized;
        if (neuralNetwork == null)
        {
            return new Vector3(0, 0, 0);
        }
        return neuralNetwork.GetDirection();
    }
    private float AngleMapping(float v)
    {
        return v * 360;
    }
    private Vector3 CalcAngle(float v)
    {
        return new Vector3(Mathf.Cos(v), 0.0f, Mathf.Sin(v)).normalized;
    }
    private float CalcAngle(Vector3 v)
    {
        Vector2 x1 = new Vector2(1f, 0f);
        Vector2 x2 = new Vector2(v.x, v.z);
        //Debug.Log(x1 + " " + x2);
        float answer = 0;
        answer += Mathf.Acos((x1.x * x2.x + x1.y * x2.y) / (x1.magnitude * x2.magnitude));

        if (x2.y < 0)
        {
            answer = 2 * Mathf.PI - answer;
        }

        return answer;
    }

    float RadianToDegree(float radian)
    {
        return (radian * 180f / (float)Mathf.PI);
    }
    float DegreeToRadians(float degree)
    {
        return (degree * (float)Mathf.PI / 180f);
    }
    public void Mutate()
    {
        neuralNetwork.Mutate();
    }
    public void StoreNetwork(string path)
    {
        neuralNetwork.StoreNetwork(path);
    }
}
