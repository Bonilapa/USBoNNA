using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using UnityEngine.UI;

public class PlotDraw : MonoBehaviour
{
    public GameObject Sample;
    public int DotsNumber;
    public Text TextField;
    public ToD toD;
    public NN nN;
    public Simulation simulation;
    public int OverallSimulations = 0;
    public int OverallAgents = 0;
    public int OverallWorldCycles = 0;
    public int OverallAge = 0;
    public int OverallNodes = 0;
    public List<GameObject> dots = new List<GameObject>();



    public class ToD
    {
        public int Age;
        public int Generation;
        public TreeOfDecisions Tree;
        public int MaxID;

    }
    public class NN {
        public float DirectionLearningRate;
        public float DecisionLearningRate;
        public int InputLayer;
        public int DirectionHidden;
        public int DecisionHidden;
    }
    public class Simulation
    {
        public int StartAgetnsNumber;
        public int AgetnsLimit;
        public int AgentStartResourceAmount;
        public int SpreadBorder;
        public int CycleLength;
        public int WorldAge;
        public int AgetnsCounter;
        public bool DeadEnd;
    }

    void DrawThePlot(int x, int y, int z)
    {

        dots.Add(Instantiate(Sample));
        
        dots[dots.Count-1].transform.position = new Vector3(x, y, z);
        dots[dots.Count - 1].SetActive(true);
    }

    
    void catchDrop(int id, int simulation, TreeOfDecisions tree, bool found)
    {
        if (found)
        {
            if(tree.Current.positiveOutcomes > 1 || tree.Current.negativeOutcomes > 1)
            {
                Debug.Log("id: " + id + "simulation: " + simulation + "p: " + tree.Current.positiveOutcomes + "n: " + tree.Current.negativeOutcomes);
            }
        }
        foreach(TreeOfDecisions.Node node in tree.Current.Children)
        {
            tree.Current = node;
            if(node.MyType == TreeOfDecisions.Type.Drop)
            {
                if (found)
                {

                }
                catchDrop(id, simulation, tree, true);
            }
            else
            {
                catchDrop(id, simulation, tree, false);
            }
            //showPattern();
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        //DrawThePlot();
        ScanTheData();
        //HowManyUniqueAgents();
    }

    void ScanTheData() {

        int SimulationNumber = 0;
        
        string Path;
        List<int> ages = new List<int>();
        int AgentID;
            int maximumDepth = 0;
            int OverallDepth = 0;
        while (SimulationNumber <= 500)
        {
            Path = @"c:\USBoNNA\" + SimulationNumber + @"\";
            //Console.WriteLine(SimulationNumber);
            AgentID = 0;
            simulation = new Simulation();
            OverallSimulations++;
            try
            {   // Open the text file using a stream reader.
                using (StreamReader sr = new StreamReader(Path + "StartInfo"))
                {
                    // Read the stream to a string, and write the string to the console.
                    string line = sr.ReadToEnd();
                    line.Remove(line.Length - 1, 1);
                    simulation.StartAgetnsNumber = GetFieldNumber(line, "StartAgetnsNumber");
                    //Debug.Log(simulation.StartAgetnsNumber);
                    simulation.AgetnsLimit = GetFieldNumber(line, "AgetnsLimit");
                    //Debug.Log(simulation.AgetnsLimit);
                    simulation.AgentStartResourceAmount = GetFieldNumber(line, "AgentStartResourceAmount");
                    //Debug.Log(simulation.AgentStartResourceAmount);
                    simulation.SpreadBorder = GetFieldNumber(line, "SpreadBorder");
                    //Debug.Log(simulation.SpreadBorder);
                    simulation.CycleLength = GetFieldNumber(line, "CycleLength");
                    //Debug.Log(simulation.CycleLength);
                }
            }
            catch (IOException e)
            {
                //throw new MissingComponentException("The file could not be read: ", e);
                SimulationNumber++;
                continue;
            }
            try
            {   // Open the text file using a stream reader.
                using (StreamReader sr = new StreamReader(Path + "EndInfo"))
                {
                    // Read the stream to a string, and write the string to the console.
                    string line = sr.ReadToEnd();
                    line.Remove(line.Length - 1, 1);
                    if (line.IndexOf("DeadEnd") >= 0)
                    {
                        simulation.DeadEnd = true;
                    }
                    else
                    {
                        simulation.DeadEnd = false;
                    }


                    //Debug.Log(simulation.DeadEnd);
                    simulation.WorldAge = GetFieldNumber(line, "WorldAge");
                    //if(simulation.WorldAge > maximumSim)
                    //{
                    //    maximumSim = simulation.WorldAge;
                    //    maximumSimSimulationNumber = SimulationNumber;
                    //}
                    //OverallWorldCycles += simulation.WorldAge;
                    //Debug.Log("Cycles "+OverallWorldCycles);
                    simulation.AgetnsCounter = GetFieldNumber(line, "AgetnsCounter");
                    //OverallAgents += simulation.AgetnsCounter;
                    //if (simulation.AgetnsCounter > maximumID)
                    //{
                    //    maximumID = simulation.AgetnsCounter;
                    //    maximumIDSimulationNumber = SimulationNumber;
                    //}
                    //Debug.Log(simulation.AgetnsCounter);
                }
            }
            catch (IOException e)
            {
                //throw new MissingComponentException("The file could not be read: ", e);

                SimulationNumber++;
                continue;
            }



            while (AgentID < simulation.AgetnsCounter)
            {
                try
                {   // Open the text file using a stream reader.
                    using (StreamReader sr = new StreamReader(Path + "ToD_" + AgentID))
                    {
                        // Read the stream to a string, and write the string to the console.
                        string line = sr.ReadToEnd();
                        line.Remove(line.Length - 1, 1);
                        toD = new ToD();
                        toD.Age = GetFieldNumber(line, "Age");
                        //if(toD.Age > longestLife)
                        //{
                        //    longestLife = toD.Age;
                        //    longestLifeAgentID = AgentID;
                        //    longestLifeSimulationNumber = SimulationNumber;
                        //}

                        //OverallAge += toD.Age;
                        //Debug.Log("Age "+OverallAge);
                        toD.Generation = GetFieldNumber(line, "Generation");
                        toD.Tree = BuildTree(line);
                        //toD.Tree.moveTo(toD.Tree.Root);
                        //int depth = toD.Tree.Depth(0);
                        //if(depth > maximumDepth)
                        //{
                        //    maximumDepth = depth;
                        //    deepestTreeAgentID = AgentID;
                        //    deepestTreeSimulationNumber = SimulationNumber;
                        //}
                        //OverallDepth += depth;
                        //toD.Tree.catchDrop(AgentID, SimulationNumber, 0);
                        //toD.Tree.Listout();
                        //int nodes = toD.Tree.MaxID();
                        //if(nodes> biggestTree)
                        //{
                        //    biggestTree = nodes;
                        //    biggestTreeAgentID = AgentID;
                        //    biggestTreeSimulationNumber = SimulationNumber;
                        //}
                        //Debug.Log(OverallNodes);
                        //Debug.Log("___________"+AgentID+"___________");
                        //toD.Tree.ShowTree();

                    }
                }
                catch (IOException e)
                {
                    throw new MissingComponentException("The file could not be read: ", e);
                }

                try
                {   // Open the text file using a stream reader.
                    StreamReader sr;
                    try
                    { sr = new StreamReader(Path + "NN_" + AgentID);
                        // Read the stream to a string, and write the string to the console.
                        string line = sr.ReadToEnd();
                        line.Remove(line.Length - 1, 1);
                        nN = new NN();
                        nN.DirectionLearningRate = GetFieldNumberFloat (line, "DirectionLearningRate");
                        //Debug.Log(nN.DirectionLearningRate);
                        nN.DecisionLearningRate = GetFieldNumberFloat(line, "DecisionLearningRate");
                        //Debug.Log(nN.DecisionLearningRate);
                        //nN.InputLayer = GetFieldNumber(line, "InputLayer");
                        //Debug.Log(nN.InputLayer);
                        nN.DirectionHidden = GetFieldNumber(line, "DirectionHidden");
                        //Debug.Log(nN.DirectionHidden);
                        nN.DecisionHidden = GetFieldNumber(line, "DecisionHidden");
                        //Debug.Log(nN.DecisionHidden);

                    }catch(Exception e)
                    {
                        AgentID++;
                        continue;
                    }
                }
                catch (IOException e)
                {
                    throw new MissingComponentException("The file could not be read: ", e);
                }

                DrawThePlot(AgentID, toD.Age, SimulationNumber);
                AgentID++;
            }

            //Debug.Log(simulation);
            //Debug.Log(nN);
            //Debug.Log(toD);
            SimulationNumber++;
        }

    }

    int GetFieldNumber(string input, string field)
    {
        //Debug.Log(field);
        int index = input.IndexOf(field);
        //Debug.Log(index);
        //Debug.Log(input);
        try
        {
            input = input.Remove(0, index);
        }
        catch (Exception e)
        {
            throw new MissingComponentException(OverallSimulations + " === " + index + " "+ field +" " + input + "_ === " + " ====== " + e);
        }
        input = input.Replace(field+": ", "");
        //Debug.Log(input);
        index = input.IndexOf('\n');
        int index2 = input.IndexOf(" ");
        if (index > 0 && index2 > 0) {
            index = Math.Min(index, index2);
        }else if(index < 0)
        {
            index = index2;
        }
        //Debug.Log(index);
        string s = input.Substring(0, index);
        //Debug.Log(s);
        //int length = field.Length + 1;
        //input.Substring(0, length);
        int p = 0;
        try
        {
            p = Int32.Parse(s);
        }
        catch (Exception e)
        {
            throw new MissingComponentException(OverallSimulations +"==="+ s + "======" + e);
        }
        return p;
    }
    float GetFieldNumberFloat(string input, string field)
    {
        //Debug.Log(field);
        int index = input.IndexOf(field);
        //Debug.Log(index);
        input = input.Remove(0, index);
        input = input.Replace(field + ": ", "");
        //Debug.Log(input);
        index = input.IndexOf('\n');
        int index2 = input.IndexOf(" ");
        if (index > 0 && index2 > 0)
        {
            index = Math.Min(index, index2);
        }
        else if (index < 0)
        {
            index = index2;
        }
        //Debug.Log(index);
        string s = input.Substring(0, index);
        //Debug.Log(s);
        //int length = field.Length + 1;
        //input.Substring(0, length);

        return float.Parse(s);
    }

    TreeOfDecisions BuildTree(string input)
    {
        TreeOfDecisions tree = new TreeOfDecisions();

        string trigger = "NEGATIVEOUTCOMES";
        input = input.Remove(0, input.IndexOf(trigger) + trigger.Length + 2);
        //Debug.Log(input);
        string[] ss = input.Split(new char[] { '\n' });
        tree.Root = new TreeOfDecisions.Node(TreeOfDecisions.Type.Root, 0);
        tree.Current = tree.Root;
        for (int i = 1; i < ss.Length; i++)
        {
            if (String.Compare(ss[i], "") == 0)
                continue;
            //Debug.Log(ss[i]);
            int id, parent_id, pos, neg;
            string[] words = ss[i].Split(new char[] { ' ' });


            id = Int32.Parse(words[0]);
           //Debug.Log("id "+id);
            parent_id = Int32.Parse(words[1]);
            //Debug.Log(                "parent id " + parent_id);
            pos = Int32.Parse(words[3]);
            //Debug.Log("pos "+pos);
            neg = Int32.Parse(words[4]);
            //Debug.Log("neg "+neg);
            //Debug.Log(GetNodeType(ss[i]));
            TreeOfDecisions.Node node = new TreeOfDecisions.Node(GetNodeType(ss[i]), (ulong)id);
            tree.moveTo(tree.Root);
            TreeOfDecisions.Node parent = tree.Find(parent_id);
            //Debug.Log(parent.id);
            tree.moveTo(parent);
            tree.addNode(node);
            tree.Current.positiveOutcomes = (uint)pos;
            tree.Current.negativeOutcomes = (uint)neg;

            //tree.ShowTree();

        }        

        return tree;
    }
    
    TreeOfDecisions.Type GetNodeType(string input)
    {
        if (input.IndexOf("Punch") > 0)
        {
            return TreeOfDecisions.Type.Punch;
        }
        else if (input.IndexOf("Throw") > 0)
        {
            return TreeOfDecisions.Type.Throw;
        } else if (input.IndexOf("Wait") > 0)
        {
            return TreeOfDecisions.Type.Wait;
        }
        else if (input.IndexOf("Drop") > 0)
        {
            return TreeOfDecisions.Type.Drop;
        }
        return TreeOfDecisions.Type.Root;
    }
    void HowManyUniqueAgents()
    {
        string line;
        using (StreamReader sr = new StreamReader(@"C:\USBoNNA\Results.txt"))
        {
            line = sr.ReadToEnd();
            //////take out id
            //////remove 2 lines


        }
        int i = 0;
        List<int> l = new List<int>();
        for (int p = 0; p < 501; p++)
        {
            for (int k = 0; k < 300; k++)
            {
                //Debug.Log("id: " + k + "; simulation: " + p);
                if (line.IndexOf("id: " + k + "; simulation: " + p + ";") >= 0)
                {
                    //Debug.Log(line.IndexOf("id: " + k + "; simulation: " + p)); 
                    i++;
                }

            }
        }
        Debug.Log(i);
    }
    void HowMans()
    {
        string line;
        using (StreamReader sr = new StreamReader(@"C:\USBoNNA\Results.txt"))
        {
            line = sr.ReadToEnd();
            //////take out id
            //////remove 2 lines


        }
        string[] words = line.Split(new char[] { '\n' });
        int i = 0;
        List<int> l = new List<int>();
        for (int p = 0; p < 501; p++)
        {
            for (int k = 0; k < 300; k++)
            {
                //Debug.Log("id: " + k + "; simulation: " + p);
                if (line.IndexOf("id: " + k + "; simulation: " + p + ";") >= 0)
                {
                    //Debug.Log(line.IndexOf("id: " + k + "; simulation: " + p)); 
                    i++;
                }

            }
        }
        Debug.Log(i);
    }
}
