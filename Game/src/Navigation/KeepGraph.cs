using System.Numerics;
using KeepLordWarriors;

namespace Navigation;

public static class KeepGraph
{
    public class Node
    {
        public Node(uint id)
        {
            this.KeepId = id;
            Next = new(0);
        }

        public uint KeepId;
        public HashSet<Node> Next;
        public int DistanceFromFrontline;
    }

    public static Dictionary<uint, Node> Calculate(uint[,] keepOwnership)
    {
        Dictionary<uint, Node> nodes = new();

        for (int y = 0; y < keepOwnership.GetLength(0) - 1; y++)
        {
            for (int x = 0; x < keepOwnership.GetLength(1) - 1; x++)
            {
                MaybeConnectNodes(nodes, keepOwnership[y, x], keepOwnership[y, x + 1]);
                MaybeConnectNodes(nodes, keepOwnership[y, x], keepOwnership[y + 1, x + 1]);
                MaybeConnectNodes(nodes, keepOwnership[y, x], keepOwnership[y + 1, x]);
                MaybeConnectNodes(nodes, keepOwnership[y + 1, x], keepOwnership[y, x + 1]);
                MaybeConnectNodes(nodes, keepOwnership[y, x + 1], keepOwnership[y + 1, x + 1]);
                MaybeConnectNodes(nodes, keepOwnership[y + 1, x], keepOwnership[y + 1, x + 1]);
            }
        }

        return nodes;
    }

    private struct NodeWithDistance
    {
        public Node Node;
        public int Distance;

        public NodeWithDistance(Node node, int distance)
        {
            Node = node;
            Distance = distance;
        }
    }

    public static void CalculateDistancesFromFrontline(Dictionary<uint, Keep> keeps, Dictionary<uint, Node> graph)
    {
        foreach (uint keepId in graph.Keys)
        {
            graph[keepId].DistanceFromFrontline = int.MaxValue;
        }

        Queue<NodeWithDistance> q =
            new Queue<NodeWithDistance>(graph.Values
                .Where(n => IsKeepOnFrontline(keeps, n))
                .Select(n => new NodeWithDistance(n, 0)));

        while (q.Count > 0)
        {
            NodeWithDistance curr = q.Dequeue();
            if (graph[curr.Node.KeepId].DistanceFromFrontline > curr.Distance)
            {
                graph[curr.Node.KeepId].DistanceFromFrontline = curr.Distance;
                foreach (Node n in curr.Node.Next)
                {
                    q.Enqueue(new NodeWithDistance(n, curr.Distance + 1));
                }
            }
        }
    }

    private static bool IsKeepOnFrontline(Dictionary<uint, Keep> keeps, Node n)
    {
        return n.Next.Any(nextNode => keeps[n.KeepId].Alliance != keeps[nextNode.KeepId].Alliance);
    }

    private static void MaybeConnectNodes(Dictionary<uint, Node> nodes, uint keep1, uint keep2)
    {
        if (keep1 != keep2)
        {
            AddNode(nodes, keep1, keep2);
        }
    }

    private static void AddNode(Dictionary<uint, Node> nodes, uint keep1, uint keep2)
    {
        if (!nodes.ContainsKey(keep1))
            nodes.Add(keep1, new Node(keep1));

        if (!nodes.ContainsKey(keep2))
            nodes.Add(keep2, new Node(keep2));

        nodes[keep1].Next.Add(nodes[keep2]);
        nodes[keep2].Next.Add(nodes[keep1]);
    }
}