using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class NeuralNetwork
{
    #region PUBLICS
    #endregion
    public float DecisionLearningRate;
    public float DirectionLearningRate;
    public float MutationRate;

    public class Neuron
    {
        public float output;
        public bool visited = false;
        public List<Neuron> inputs;
        public List<float> weights;

        public Neuron()
        {
            //Debug.Log("Neruon created");
        }
        public Neuron(List<Neuron> input, List<float> newWeights)
        {
            if (input.Count != newWeights.Count)
            {
                throw new System.Exception("Weights and input lists have different size.");
            }
            weights = newWeights;
            foreach (float weight in weights)
            {
                if (weight > 1 || weight < 0)
                {
                    throw new UnassignedReferenceException("Weight is not assigned");
                }
            }
            inputs = input;
        }

        public void RebalanceWeights(float rebalanceValue, float learningRate)
        {
            if (inputs == null || weights == null || visited)
            {
                return;
            }
            //Debug.Log("Internal " + counter);
            //TODO think here. weight can be negative or > 1

            RecalWeights(rebalanceValue, learningRate);
            foreach (Neuron neuron in inputs)
            {
                neuron.RebalanceWeights(rebalanceValue, learningRate);
            }

            visited = true;
        }
        private void RecalWeights(float rebalanceValue, float learningRate)
        {
            //Debug.Log("-------------------------------");

            for (int i = 0; i < weights.Count; i++)
            {
                //Debug.Log(inputs[i].output);
                //Debug.Log(weights[i]);
                //Debug.Log("Rebalance: " + rebalanceValue + " __ old: "+ inputs[i].output + " " + weights[i]);
                weights[i] += learningRate * inputs[i].output * weights[i] * rebalanceValue;
                //Debug.Log("value to rebalance " + learningRate * inputs[i].output * weights[i] * rebalanceValue);
                //if(weights[i] > 1)
                //{
                //    weights[i] = 1;
                //}
                if (weights[i] < 0)
                {
                    weights[i] = 1e-5f;
                }
                //Debug.Log("New: " + inputs[i].output + " " + weights[i]);
            }
        }
        /// <summary>
        /// To scale negative and positive numbers to [0..1]
        /// atan(val/2) + 1/2
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static float atanActivationFunction(float val)
        {
            return Mathf.Atan(val / 2) / Mathf.PI + 0.5f;
        }
        /// <summary>
        /// 2 * atan(0.01*x)/pi. 
        /// For big non-negative numbers.
        /// Grows very slowly. 
        /// enough to scale [1..100] to [0..1]
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static float atanActivationFunction2(float val)
        {
            if (val > 0)
                return 2 * Mathf.Atan(0.01f * val) / Mathf.PI;
            else
                return 0;
        }
        /// <summary>
        /// To scale positive numbers to [0..1]
        /// 2*atan(val/10)
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static float atanActivationFunction3(float val)
        {
            return 2 * Mathf.Atan(val / 10) / Mathf.PI;
        }
        public void Compute()
        {
            if (inputs == null || weights == null || visited)
            {
                return;
            }

            //Debug.Log("=================");
            output = 0;
            for (int i = 0; i < inputs.Count; i++)
            {
                inputs[i].Compute();
                //Debug.Log(inputs[i].output+" * "+weights[i]);
                output += inputs[i].output * weights[i];
                //Debug.Log(output);
            }
            output = atanActivationFunction3(output);
            visited = true;
            //Debug.Log("neuron output = " + output);
        }
        //ebanii/ do not use
        public float RecursiveCompute(List<Neuron> neurons)
        {
            foreach (Neuron neuron in neurons)
            {
                if (neuron.weights == null || neuron.inputs == null)
                {
                    return neuron.output;
                }
            }
            output = 0;
            int i = 0;
            foreach (Neuron neuron in neurons)
            {
                output += RecursiveCompute(neuron.inputs) * weights[i];
                i++;
            }
            output = atanActivationFunction(output);
            return output;
        }
    }

    public List<Neuron> InputLayer;
    public List<Neuron> DecisionHiddenLayer;
    public List<Neuron> DecisionOutputLayer;
    public List<Neuron> DirectionHiddenLayer;
    public List<Neuron> DirectionOutputLayer;

    public NeuralNetwork(float DecisionRate, float DirectionRate)
    {
        InputLayer = new List<Neuron>();
        DecisionHiddenLayer = new List<Neuron>();
        DecisionHiddenLayer = new List<Neuron>();
        DecisionOutputLayer = new List<Neuron>();
        DecisionLearningRate = DecisionRate;
        DirectionLearningRate = DirectionRate;

    }
    public NeuralNetwork(uint inN, uint hiN, uint decOuN, uint dirOuN, float DecisionRate, float DirectionRate, float MutRate)
    {
        InputLayer = new List<Neuron>();
        for (int i = 0; i < inN; i++)
        {
            InputLayer.Add(new Neuron());
        }
        DecisionHiddenLayer = new List<Neuron>();
        for (int i = 0; i < hiN; i++)
        {
            DecisionHiddenLayer.Add(new Neuron());
        }
        DirectionHiddenLayer = new List<Neuron>();
        for (int i = 0; i < hiN; i++)
        {
            DirectionHiddenLayer.Add(new Neuron());
        }
        DecisionOutputLayer = new List<Neuron>();
        for (int i = 0; i < decOuN; i++)
        {
            DecisionOutputLayer.Add(new Neuron());
        }
        DirectionOutputLayer = new List<Neuron>();
        for (int i = 0; i < dirOuN; i++)
        {
            DirectionOutputLayer.Add(new Neuron());
        }
        DecisionLearningRate = DecisionRate;
        DirectionLearningRate = DirectionRate;
        MutationRate = MutRate;

    }
    public void SetWeightToAll(float commonWeight)
    {
        SetWeight(DecisionOutputLayer, commonWeight);
    }

    private void SetWeight(List<Neuron> neurons, float common)
    {
        foreach (Neuron neuron in neurons)
        {
            if (neuron.weights != null)
            {
                for (int i = 0; i < neuron.weights.Count; i++)
                {
                    neuron.weights[i] = common;
                }
            }
            if (neuron.inputs != null)
            {
                SetWeight(neuron.inputs, common);
            }
        }
    }
    public void AssignInputLayer(List<float> input)
    {
        if (input.Count > InputLayer.Count)
        {
            //Debug.Log(input.Count + " " + InputLayer.Count);
            throw new System.Exception("Nowhere to assign weights. List in parameter is too long.");
        }
        else if (input.Count < InputLayer.Count)
        {
            throw new System.Exception("Not enough items in list in parameter.");
        }
        else
        {
            for (int i = 0; i < InputLayer.Count; i++)
            {
                InputLayer[i].output = input[i];
                //Debug.Log(input[i]);
            }
        }
    }
    public void ReAssignDecisionHiddenLayer()
    {
        foreach (Neuron hiddenNeuron in DecisionHiddenLayer)
        {
            hiddenNeuron.inputs = InputLayer;
        }

    }
    public void ReAssignDirectionHiddenLayer()
    {
        foreach (Neuron hiddenNeuron in DirectionHiddenLayer)
        {
            hiddenNeuron.inputs = InputLayer;
        }

    }
    public void RandAssignDecisionHiddenLayer()
    {
        foreach (Neuron neuron in DecisionHiddenLayer)
        {
            neuron.weights = new List<float>();
            for (int i = 0; i < InputLayer.Count; i++)
            {
                neuron.weights.Add(Random.value);
                neuron.inputs = InputLayer;
            }
        }
    }
    public void RandAssignDirectionHiddenLayer()
    {
        foreach (Neuron neuron in DirectionHiddenLayer)
        {
            neuron.weights = new List<float>();
            for (int i = 0; i < InputLayer.Count; i++)
            {
                neuron.weights.Add(Random.value);
                neuron.inputs = InputLayer;
            }
        }
    }
    public void RandAssignDecisionOutputLayer()
    {
        foreach (Neuron neuron in DecisionOutputLayer)
        {
            neuron.weights = new List<float>();
            for (int i = 0; i < DecisionHiddenLayer.Count; i++)
            {
                neuron.weights.Add(Random.value);
            }
            neuron.inputs = DecisionHiddenLayer;
            neuron.Compute();
            //Debug.Log(neuron.output);
        }
    }
    public void RandAssignDirectionOutputLayer()
    {
        foreach (Neuron neuron in DirectionOutputLayer)
        {
            neuron.weights = new List<float>();
            for (int i = 0; i < DirectionHiddenLayer.Count; i++)
            {
                neuron.weights.Add(Random.value);
            }
            neuron.inputs = DirectionHiddenLayer;
            neuron.Compute();
            //Debug.Log(neuron.output);
        }
        //DirectionOutput.weights = new List<float>();
        //    for (int i = 0; i < DirectionHiddenLayer.Count; i++)
        //    {
        //        DirectionOutput.weights.Add(Random.value);
        //        DirectionOutput.inputs = DirectionHiddenLayer;
        //    }
        //    DirectionOutput.Compute();
        //    //Debug.Log(neuron.output);
    }
    public void AssignOutputLayer(List<float> weights)
    {
        if (weights.Count > DecisionHiddenLayer.Count)
        {
            throw new System.Exception("Nowhere to assign weights. List in parameter is too long.");
        }
        else if (weights.Count < DecisionHiddenLayer.Count)
        {
            throw new System.Exception("Not enough items in list in parameter.");
        }
        else
        {
            foreach (Neuron outputNeuron in DecisionOutputLayer)
            {
                outputNeuron.inputs = DecisionHiddenLayer;
                outputNeuron.weights = weights;
                //foreach (Neuron hiddenNeuron in HiddenLayer)
                //{
                //    //strict connection of neuron's output from input 
                //    //layer to input list in every hidden layer's neuron
                //    outputNeuron.inputs.Add(hiddenNeuron);
                //}
            }
            //for (int i = 0; i < weights.Count; i++)
            //{

            //    OutputLayer[i].weights = weights;
            //}
        }
    }
    public void ResetVisit(List<Neuron> list)
    {
        foreach (Neuron i in list)
        {
            i.visited = false;
        }
    }
    public void RebalanceDecisionBranch(float value, TreeOfDecisions.Type decision)
    {
        ResetVisit(DecisionOutputLayer);
        ResetVisit(DecisionHiddenLayer);
        ResetVisit(InputLayer);
        if (DecisionOutputLayer == null || DecisionHiddenLayer == null || InputLayer == null)
        {
            return;
        }
        switch (decision)
        {
            case TreeOfDecisions.Type.Drop:
                {
                    //foreach(Neuron neuron in OutputLayer)
                    //{
                    //    if (neuron == null)
                    //        Debug.Log("null");
                    //    else
                    //        Debug.Log(neuron.output);
                    //}
                    DecisionOutputLayer[0].RebalanceWeights(value, DecisionLearningRate);
                    break;
                }
            case TreeOfDecisions.Type.Punch:
                {
                    DecisionOutputLayer[1].RebalanceWeights(value, DecisionLearningRate);
                    break;
                }
            case TreeOfDecisions.Type.Throw:
                {
                    DecisionOutputLayer[2].RebalanceWeights(value, DecisionLearningRate);
                    break;
                }
            case TreeOfDecisions.Type.Wait:
                {
                    DecisionOutputLayer[3].RebalanceWeights(value, DecisionLearningRate);
                    break;
                }
        }

        //foreach (Neuron neuron in OutputLayer)
        //{
        //    neuron.RebalanceWeights(value, learningRate);
        //}
    }
    private void Rebalance(List<Neuron> neurons, float value, float rate)
    {
        foreach (Neuron neuron in neurons)
        {
            if (neuron.weights != null && neuron.inputs != null)
            {
                neuron.RebalanceWeights(value, rate);
                Rebalance(neuron.inputs, value, rate);
            }
            else
            {
                return;
            }
        }
    }
    public TreeOfDecisions.Type GetDecision()
    {
        List<float> outputs = new List<float>();
        ResetVisit(DecisionHiddenLayer);
        ResetVisit(DecisionOutputLayer);
        ResetVisit(InputLayer);
        ReAssignDecisionHiddenLayer();

        foreach (Neuron neuron in DecisionOutputLayer)
        {
            neuron.Compute();
            outputs.Add(neuron.output);
        }
        int maxIndex = 0;
        for (int i = 1; i < outputs.Count; i++)
        {
            if (outputs[maxIndex] < outputs[i])
            {
                maxIndex = i;
            }
        }
        switch (maxIndex)
        {
            case 0:
                {
                    return TreeOfDecisions.Type.Drop;
                }
            case 1:
                {
                    return TreeOfDecisions.Type.Punch;
                }
            case 2:
                {
                    return TreeOfDecisions.Type.Throw;
                }
            case 3:
                {
                    return TreeOfDecisions.Type.Wait;
                }
            default:
                {
                    return TreeOfDecisions.Type.Root;
                }
        }
    }
    public Vector3 GetDirection()
    {
        List<float> outputs = new List<float>();
        ResetVisit(DirectionHiddenLayer);
        ResetVisit(DirectionOutputLayer);
        ResetVisit(InputLayer);
        ReAssignDirectionHiddenLayer();


        foreach (Neuron neuron in DirectionOutputLayer)
        {
            neuron.Compute();
            outputs.Add(neuron.output);
        }
        int maxIndex = 0;
        for (int i = 1; i < outputs.Count; i++)
        {
            if (outputs[maxIndex] < outputs[i])
            {
                maxIndex = i;
            }
        }
        switch (maxIndex)
        {
            case 0:
                {
                    return new Vector3(1.0f, 0.0f, 0.0f).normalized;
                }
            case 1:
                {
                    return new Vector3(1.0f, 0.0f, 1.0f).normalized;
                }
            case 2:
                {
                    return new Vector3(0.0f, 0.0f, 1.0f).normalized;
                }
            case 3:
                {
                    return new Vector3(-1.0f, 0.0f, 1.0f).normalized;
                }
            case 4:
                {
                    return new Vector3(-1.0f, 0.0f, 0.0f).normalized;
                }
            case 5:
                {
                    return new Vector3(-1.0f, 0.0f, -1.0f).normalized;
                }
            case 6:
                {
                    return new Vector3(0.0f, 0.0f, -1.0f).normalized;
                }
            case 7:
                {
                    return new Vector3(1.0f, 0.0f, -1.0f).normalized;
                }
            default:
                {
                    return new Vector3(0.0f, 0.0f, 0.0f);
                }
        }
    }
    public void RebalanceDirectionBranch(float value, Vector3 direction)
    {
        ResetVisit(DirectionOutputLayer);
        ResetVisit(DirectionHiddenLayer);
        ResetVisit(InputLayer);
        if (DirectionOutputLayer == null || DirectionHiddenLayer == null || InputLayer == null)
        {
            return;
        }

        if (direction.x > 0 && direction.z == 0)
        {
            //Debug.Log("__[1 0]__"+value);
            DirectionOutputLayer[0].RebalanceWeights(value, DirectionLearningRate);
        }
        else if (direction.x > 0 && direction.z > 0)
        {
            //Debug.Log("__[1 1]__" + value);
            DirectionOutputLayer[1].RebalanceWeights(value, DirectionLearningRate);
        }
        else if (direction.x == 0 && direction.z > 0)
        {
            //Debug.Log("__[0 1]__" + value);
            DirectionOutputLayer[2].RebalanceWeights(value, DirectionLearningRate);
        }
        else if (direction.x < 0 && direction.z > 0)
        {
            //Debug.Log("__[-1 1]__" + value);
            DirectionOutputLayer[3].RebalanceWeights(value, DirectionLearningRate);
        }
        else if (direction.x < 0 && direction.z == 0)
        {
            //Debug.Log("__[-1 0]__" + value);
            DirectionOutputLayer[4].RebalanceWeights(value, DirectionLearningRate);
        }
        else if (direction.x < 0 && direction.z < 0)
        {
            //Debug.Log("__[-1 -1]__" + value);
            DirectionOutputLayer[5].RebalanceWeights(value, DirectionLearningRate);
        }
        else if (direction.x == 0 && direction.z < 0)
        {
            //Debug.Log("__[0 -1]__" + value);
            DirectionOutputLayer[6].RebalanceWeights(value, DirectionLearningRate);
        }
        else if (direction.x > 0 && direction.z < 0)
        {
            //Debug.Log("__[1 -1]__" + value);
            DirectionOutputLayer[7].RebalanceWeights(value, DirectionLearningRate);
        }
        //DirectionOutput.RebalanceWeights(value, DirectionLearningRate);
    }
    public void Mutate()
    {
        if (Random.value <= MutationRate)
        {
            float temp = Random.value;
            if (temp >= 0.66f)
            {
                //Remove neuron
                if (Random.value >= 0.5f)
                {
                    int index = Random.Range(0, DecisionHiddenLayer.Count);
                    DecisionHiddenLayer.RemoveAt(index);
                    for (int i = 0; i < DecisionOutputLayer.Count; i++)
                    {
                        DecisionOutputLayer[i].weights.RemoveAt(index);
                    }
                }
                else
                {
                    int index = Random.Range(0, DirectionHiddenLayer.Count);
                    DirectionHiddenLayer.RemoveAt(index);
                    for (int i = 0; i < DirectionOutputLayer.Count; i++)
                    {
                        DirectionOutputLayer[i].weights.RemoveAt(index);
                    }

                }
            }
            else if (temp >= 0.33f)
            {
                //Add neuron
                if (Random.value >= 0.5f)
                {
                    List<float> l = new List<float>();
                    for (int i = 0; i < InputLayer.Count; i++)
                    {
                        l.Add(Random.value);
                    }
                    Neuron neur = new Neuron(InputLayer, l);
                    DecisionHiddenLayer.Add(neur);
                    foreach (Neuron n in DecisionOutputLayer)
                    {
                        n.weights.Add(Random.value);
                    }
                }
                else
                {
                    List<float> l = new List<float>();
                    for (int i = 0; i < InputLayer.Count; i++)
                    {
                        l.Add(Random.value);
                    }
                    Neuron neur = new Neuron(InputLayer, l);
                    DirectionHiddenLayer.Add(neur);
                    foreach (Neuron n in DirectionOutputLayer)
                    {
                        n.weights.Add(Random.value);
                    }
                }
            }
            else
            {
                //weights random change for random neuron
                if (Random.value >= 0.5f)
                {
                    int index = Random.Range(0, DecisionHiddenLayer.Count);
                    
                    for(int i = 0; i < DecisionHiddenLayer[index].weights.Count; i++)
                    {
                        DecisionHiddenLayer[index].weights[i] = Random.value;
                    }
                }
                else
                {
                    int index = Random.Range(0, DirectionHiddenLayer.Count);

                    for (int i = 0; i < DirectionHiddenLayer[index].weights.Count; i++)
                    {
                        DirectionHiddenLayer[index].weights[i] = Random.value;
                    }
                }
            }
        }
    }
    public void StoreNetwork(string path)
    {
        if (!File.Exists(path))
        {
            // Create a file to write to.
            using (StreamWriter sw = new StreamWriter(path, false))
            {
                //Root.RecursivelyStore(sw);
                sw.WriteLine("DirectionLearningRate: " + DirectionLearningRate);
                sw.WriteLine("DecisionLearningRate: " + DecisionLearningRate);
                sw.WriteLine("Input: " + InputLayer.Count);
                StoreLayer(sw, InputLayer);
                sw.WriteLine("DirectionHidden: " + DirectionHiddenLayer.Count);
                StoreLayer(sw, DirectionHiddenLayer);
                sw.WriteLine("DirectionOutput: " + DirectionOutputLayer.Count);
                StoreLayer(sw, DirectionOutputLayer);
                sw.WriteLine("DecisionHidden: " + DecisionHiddenLayer.Count);
                StoreLayer(sw, DecisionHiddenLayer);
                sw.WriteLine("DecisionOutput: " + DecisionOutputLayer.Count);
                StoreLayer(sw, DecisionOutputLayer);
            }
        }
    }
    public void StoreLayer(StreamWriter streamWriter, List<Neuron> ns)
    {
        foreach (Neuron n in ns)
        {
            if(n.weights != null)
            {
                streamWriter.WriteLine("{");
                int c = 5;
                foreach (float w in n.weights)
                {
                    if(c <= 0)
                    {
                        c = 5;
                        streamWriter.WriteLine();
                    }
                    streamWriter.Write(w + " ");
                    c--;
                }
                streamWriter.WriteLine("}");
            }
        }
    }
}