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
    private  Agent owner;

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
    
    public TreeOfDecisions(Agent newOwner)
    {
        owner = newOwner;
        Root = new Node(Type.Root, idCounter++);
        Root.Parent = null;
        moveTo(Root);
    }

    /// <summary>
    /// Save the tree on hard-drive
    /// </summary>
    public void StoreTree(string path)
    {
        if (!File.Exists(path))
        {
            // Create a file to write to.
            using (StreamWriter sw = new StreamWriter(path, false))
            {
                sw.WriteLine("Age: " + owner.Age.ToString() + " Generation: " + owner.generation);
                sw.WriteLine("ID PARENTID ACTIONTYPE POSITIVEOUTCOMES NEGATIVEOUTCOMES");
                Root.RecursivelyStore(sw);
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
}