using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class TreeOfDecisions
{
    public Node Root { get; set; }
    public Node Current { get; set; }
    public enum Type
    {
        Drop, Wait, Throw, Punch, Root
    }

    private ulong idCounter = 0;
    private float rebalanceValue = 0;

    public float RebalanceValue
    {
        get
        {
            return rebalanceValue;
        }
        set
        {
            rebalanceValue = 0;
        }
    }

    public class Node
    {
        public ulong id { get; set; }
        public Node Parent { get; set; }
        public Type MyType { get; set; }
        public uint positiveOutcomes { get; set; }
        public uint negativeOutcomes { get; set; }
        public List<Node> Children { get; set; }


        public Node(Type type, ulong newId)
        {
            id = newId;
            MyType = type;
            Children = new List<Node>();
        }

        public void addChild(Node newChild)
        {
            newChild.Parent = this;
            Children.Add(newChild);
        }

        public void ShowNode()
        {
            uint Pid;
            if (Parent != null)
                Pid = Convert.ToUInt32(Parent.id);
            else
                Pid = 0;
            Debug.Log("[Id: " + id + "] [Parent: " + Pid + "] [Type: " + MyType + "] [+: " + positiveOutcomes + "] [-: " + negativeOutcomes + "] Children: { ");
            foreach (Node node in Children)
            {
                Debug.Log(" " + node.id + " ");
            }
            Debug.Log("}");

        }

        public Node GetChildWith(Type type)
        {
            foreach (Node node in Children)
            {
                if (node.MyType == type)
                {
                    return node;
                }
            }
            return null;
        }

        public void RecursivelyShow()
        {

            Debug.Log("[Id: " + id + "] [Parent: " + Parent.id + "] [Type: " + MyType + "] [+: " + positiveOutcomes + "] [-: " + negativeOutcomes + "] Children: { ");
            foreach (Node node in Children)
            {
                Debug.Log(" " + node.id + " ");
            }
            Debug.Log("}");

            if (Children.Count == 0)
                return;

            foreach (Node temp in Children)
            {
                temp.RecursivelyShow();
            }
        }

        public void RecursivelyStore(StreamWriter streamWriter)
        {
            if (Parent != null)
            {
                streamWriter.WriteLine(id + " " + Parent.id + " " + MyType + " " + positiveOutcomes + " " + negativeOutcomes);
            }else
            {
                streamWriter.WriteLine(id + " null " + MyType + " " + positiveOutcomes + " " + negativeOutcomes);

            }
            if (Children.Count == 0)
                return;

            foreach (Node temp in Children)
            {
                temp.RecursivelyStore(streamWriter);
            }
        }
    }
    
    

    /// <summary>
    /// To show tree during simulation in user interface
    /// </summary>
    public void ShowTree()
    {
        Root.ShowNode();
        foreach (Node temp in Root.Children)
        {
            temp.RecursivelyShow();
        }

    }

    public float WalkWith(Type type)
    {
        Node temp = Current.GetChildWith(type);
        if (temp == null)
        {
            addItem(type);
        }
        else
        {
            moveTo(temp);
        }
        
        return RebalanceValue;
    }

    public void moveTo(Node child)
    {
        Current = child;
    }

    private void addItem(Type childType)
    {
        Node node = new Node(childType, idCounter++);
        Current.addChild(node);
        moveTo(node);
    }

    /// <summary>
    /// positive value is a positive outcome,
    /// negative value is a negative outcome;
    /// 0 is simple walk through without changing outcomes
    /// </summary>
    /// <param name="outcome"></param>
    public void ChangeOutcomes(short positive, short negative)
    {
        Current.positiveOutcomes += Convert.ToUInt32(positive);
        Current.negativeOutcomes += Convert.ToUInt32(negative);

        rebalanceValue =
            (Convert.ToInt32(Current.positiveOutcomes) - Current.negativeOutcomes)
            /
            (Convert.ToInt32(Current.positiveOutcomes) + Current.negativeOutcomes);
    }

    public void addNode(Node node)
    {
        Current.addChild(node);
    }

    public Node Find(int parent_id)
    {
        if((int)Current.id == parent_id)
        {
            return Current;
        }
        Node n = null;
        if (Current.Children != null)
        {
            foreach (Node node in Current.Children)
            {
                Current = node;
                n = Find(parent_id);
            }
        }
        return n;
    }

    public int MaxID()
    {
        int i = (int)Root.id;
        foreach (Node node in Root.Children)
        {
            int i1 = calcNodes();
            if (i1 > i)
            {
                i = i1;
            }
        }
        return i;
    }
    int calcNodes()
    {
        int i = (int)Current.id;
        if(Current.Children == null)
        {
            return i;
        }
        foreach (Node node in Current.Children)
        {
            Current = node;
            int i1 = calcNodes();
            if (i1 > i)
            {
                i = i1;
            }
        }
        return i;
    }
    public string s;
    public void catchDrop(int id, int simulation, int found)
    {
        if (Current.Children == null)
        {
            return;
        }
        if (Current.MyType == Type.Drop)
            {
                found++;
                s = toStr(Type.Drop);
            }
        if (found > 0)
        {
            s += "-"+toStr(Current.MyType);
            //Debug.Log("found: " + found);
            //Debug.Log(Current.positiveOutcomes + " " + Current.negativeOutcomes);
            if (Current.positiveOutcomes > 1 || Current.negativeOutcomes > 1)
            {
                using (StreamWriter sw = new StreamWriter(@"C:\USBoNNA\Results.txt", true, System.Text.Encoding.Default))
                {
                    sw.WriteLine("id: " + id + "; simulation: " + simulation + "; p: " + Current.positiveOutcomes + "; n: " + Current.negativeOutcomes+"; found: " + found);
                    sw.WriteLine(s);
                    s = null;
                }
            }
        }
        foreach (Node node in Current.Children)
        {
            Current = node;
            //Debug.Log("I am " + Current.MyType);
            
            catchDrop(id, simulation, found);
            //showPattern();
        }
    }
    public string toStr(Type t)
    {
        if (t == Type.Drop) return "Drop";
        else if (t == Type.Punch) return "Punch";
        else if (t == Type.Throw) return "Throw";
        else if (t == Type.Wait) return "Stay";
        else  return "Root";
    }

    public int Depth(int depth)
    {
        depth++;
        //Debug.Log("I am " + Current.MyType + "; depth is: " + depth);
        int i = depth;
        //Debug.Log(Current.Children);
        if (Current.Children == null)
        {
            //Debug.Log("returns " + depth);
            return depth;
        }
        foreach (Node node in Current.Children)
        {
            Current = node;
            int i1 = Depth(depth);
            if (i1 > i)
            {
                i = i1;
        //Debug.Log(i);
            }
        }
        return i;
    }
}