using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class Graph<T> {

    [System.Flags]
    enum DirtyFlags
    {
        BFS = 1
    }

    public class Node
    {
        public T value;
        public List<Node> connected;
        public List<Edge> edges;
        public Node(T t)
        {
            value = t;
            connected = new List<Node>();
            edges = new List<Edge>();
        }

        public Edge Connect(Node to)
        {
            if (!connected.Contains(to))
            {
                connected.Add(to);
                to.connected.Add(this);
                var edge = new Edge(this, to);
                edges.Add(edge);
                to.edges.Add(edge);
                return edge;
            }
            else
            {
                return null;
            }
        }

        public void Disconnect(Node from)
        {
            if (connected.Remove(from))
            {
                from.connected.Remove(this);
                Edge tr = null;
                foreach (var e in edges)
                {
                    if(e.a == from || e.b == from) {
                        tr = e;
                        break;
                    }
                }
                if(tr!=null) {
                    edges.Remove(tr);
                    from.edges.Remove(tr);
                }
            }
        }

        public void DisconnectAll()
        {
            while (edges.Count > 0)
            {
                Disconnect(edges[0].GetConnected(this));
            }
        }

        public bool IsConnected(Node to)
        {
            return to == this || connected.Contains(to);
        }
    }

    public class Edge
    {
        public Node a, b;
        public Edge(Node a, Node b)
        {
            this.a = a;
            this.b = b;
        }
        public Node GetConnected(Node n)
        {
            if (n == a)
            {
                return b;
            }
            else
            {
                return a;
            }
        }
    }

    public List<Node> nodes;
    Dictionary<T, Node> lookup;
    List<T> worker;
    List<Node> worker2;
    public List<Edge> edges;
    public List<Graph<T>> forest;
    int version;
    bool dirty { get { return dirtyFlags == 0; } set { dirtyFlags = value ? (uint)DirtyFlags.BFS : 0; } }
    uint dirtyFlags;

    public Graph() {
        nodes = new List<Node>();
        worker = new List<T>();
        lookup = new Dictionary<T, Node>();
        worker2 = new List<Node>();
        edges = new List<Edge>();
        forest = new List<Graph<T>>();
        dirtyFlags = 1;
    }

    public Graph(IEnumerable<T> collection) : this()
    {
        nodes = new List<Node>();
        foreach (var t in collection)
        {
            var node = new Node(t);
            nodes.Add(node);
            lookup[t] = node;
        }
    }

    Graph(Graph<T> parent, List<Node> nodes)
    {
        this.nodes = nodes;
        worker = parent.worker;
        worker2 = parent.worker2;
        lookup = parent.lookup;
        Connect((t, l) =>
        {
            Node n;
            if (parent.lookup.TryGetValue(t, out n))
            {
                foreach (var x in n.connected)
                {
                    if (nodes.Contains(x))
                    {
                        l.Add(x.value);
                    }
                }
            }
        });
    }

    public bool Contains(T value)
    {
        return value!=null && lookup.ContainsKey(value);
    }

    public void BFS()
    {
        //if (!dirty) return;
        worker2.Clear();
        var q = new Queue<Node>();

        foreach (var n in nodes)
        {
            if (worker2.Contains(n)) continue;
            var sg = new List<Node>();
            q.Enqueue(n);
            sg.Add(n);
            while (q.Count > 0)
            {
                var d = q.Dequeue();
                worker2.Add(d);
                foreach (var dn in d.connected)
                {
                    if (!sg.Contains(dn))
                    {
                        sg.Add(dn);
                        q.Enqueue(dn);
                    }
                }
            }
            forest.Add(new Graph<T>(this, sg));
        }

        dirtyFlags |= (uint)DirtyFlags.BFS;
    }

    public void Filter(System.Func<Node, bool> filter)
    {
        worker2.Clear();
        foreach (var node in nodes)
        {
            if (!filter(node))
            {
                worker2.Add(node);
            }
        }
        foreach (var node in worker2)
        {
            RemoveNode(node);
        }
    }

    public void Add(T value)
    {
        var n = new Node(value);
        nodes.Add(n);
        lookup[value] = n; 
    }

    public Node GetNode(T key)
    {
        Node result = null;
        lookup.TryGetValue(key, out result);
        return result;
    }

    public void RemoveNode(Node node)
    {
        nodes.Remove(node);
        lookup.Remove(node.value);
        foreach (var e in node.edges)
        {
            edges.Remove(e);
        }
        node.DisconnectAll();
        dirty = true;
    }

    public void ForEach(System.Action<T> func)
    {
        foreach (var n in nodes)
        {
            func(n.value);
        }
    }

    public void Connect(T a, T b)
    {
        Node na, nb;
        if (lookup.TryGetValue(a, out na) && lookup.TryGetValue(b, out nb))
        {
            Connect(na, nb);
        }
    }

    public void Connect(Node a, Node b)
    {
        if (!a.IsConnected(b))
        {
            var edge = a.Connect(b);
            if (edge != null)
            {
                edges.Add(edge);
                dirty = true;
            }
        }
    }

    public void Connect(System.Action<T,List<T>> filter)
    {
        foreach (var n in nodes)
        {
            worker.Clear();
            filter(n.value, worker);
            if (worker.Count > 0)
            {
                foreach (var c in worker)
                {
                    Node nc;
                    if (lookup.TryGetValue(c, out nc))
                    {
                        Connect(n, nc);
                    }
                }
            }
        }
    }
}
