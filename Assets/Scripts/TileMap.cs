using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TileMap : MonoBehaviour
{
    public TileType[] tileTypes;
    public GameObject selectedUnit;

    int[,] tiles;
    Node[,] graph;

    int sizeX = 10;
    int sizeY = 10;

    public class Node
    {
        // List of neighbor nodes
        public List<Node> edges;
        public int x = 0;
        public int y = 0;

        public int gCost;
        public int hCost;
        public int fCost;

        public Node() : this(0, 0){}
        public Node(int x, int y)
        {
            this.x = x;
            this.y = y;
            edges = new List<Node>();
        }

        public void CalFCost() {
            fCost = gCost + hCost;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        selectedUnit.GetComponent<Unit>().tileX = (int)selectedUnit.transform.position.x;
        selectedUnit.GetComponent<Unit>().tileY = (int)selectedUnit.transform.position.y;
        generateMapData();
        GenerateGraph();
        GenerateMap();
    }

    void generateMapData()
    {
        tiles = new int[sizeX, sizeY];
        for (int x = 0; x < sizeX; x++)
        {
            for (int y = 0; y < sizeY; y++)
            {
                tiles[x, y] = 0;
            }
        }

        // Create a U-shaped wall
        tiles[4, 4] = 1;
        tiles[5, 4] = 1;
        tiles[6, 4] = 1;
        tiles[7, 4] = 1;
        tiles[8, 4] = 1;

        tiles[4, 3] = 1;
        tiles[8, 3] = 1;

    }

    void GenerateGraph()
    {
        graph = new Node[sizeX, sizeY];

        // Initialize our graph
        for (int x = 0; x < sizeX; x++)
        {
            for (int y = 0; y < sizeY; y++)
            {
                Node node = new Node(x, y);
                graph[x, y] = node;
                node.gCost = int.MaxValue;
                node.CalFCost();
            }
        }

        // Find neighbor node for each tile
        for (int x = 0; x < sizeX; x++)
        {
            for (int y = 0; y < sizeY; y++)
            {
                /*// 4 Way Movement
                if (x > 0)
                    graph[x, y].edges.Add(graph[x - 1, y]);
                if (x < sizeX - 1)
                    graph[x, y].edges.Add(graph[x + 1, y]);
                if (y > 0)
                    graph[x, y].edges.Add(graph[x, y - 1]);
                if (y < sizeY - 1)
                    graph[x, y].edges.Add(graph[x, y + 1]);
                */
                // 8 Way Movement
                if (x > 0)
                {
                    graph[x, y].edges.Add(graph[x - 1, y]);
                    if (y > 0)
                        graph[x, y].edges.Add(graph[x - 1, y - 1]);
                    if (y < sizeY - 1)
                        graph[x, y].edges.Add(graph[x - 1, y + 1]);
                }

                if (x < sizeX - 1)
                {
                    graph[x, y].edges.Add(graph[x + 1, y]);
                    if (y > 0)
                        graph[x, y].edges.Add(graph[x + 1, y - 1]);
                    if (y < sizeY - 1)
                        graph[x, y].edges.Add(graph[x + 1, y + 1]);
                }

                if (y > 0)
                    graph[x, y].edges.Add(graph[x, y - 1]);
                if (y < sizeY - 1)
                    graph[x, y].edges.Add(graph[x, y + 1]);

            }
        }
    }

    void GenerateMap()
    {
        for (int x = 0; x < sizeX; x++)
        {
            for (int y = 0; y < sizeY; y++)
            {
                TileType tileT = tileTypes[tiles[x, y]];
                GameObject ob = (GameObject)Instantiate(tileT.visualPrefab, new Vector2(x, y), Quaternion.identity);

                ClickOnTile click = ob.GetComponent<ClickOnTile>();
                click.tileX = x;
                click.tileY = y;
                click.map = this;
            }
        }
    }

    public Vector2 TileCoordToWorldCoord(int x, int y)
    {
        return new Vector2(x, y);
    }

    // Moving using breadth first search or depth first search
    public void moveBFS(int x, int y) {
        //Queue<Node> queue = new Queue<Node>();
        Stack<Node> st = new Stack<Node>();
        Dictionary<Node, Node> prev = new Dictionary<Node, Node>();
        HashSet<Node> explored = new HashSet<Node>();

        Node start = graph[selectedUnit.GetComponent<Unit>().tileX,
                            selectedUnit.GetComponent<Unit>().tileY];
        Node target = graph[x, y];
        prev[target] = null;
        st.Push(start);
        while (st.Count != 0) {
            Node curr = st.Pop();
            if (curr == target) {
                Debug.Log("Found Path!");
                break;
            }
            foreach(Node v in curr.edges) {
                if (!explored.Contains(v) && tileTypes[tiles[v.x,v.y]].isWalkable) {
                    explored.Add(v);
                    prev[v] = curr;
                    st.Push(v);
                    
                }
            }
        }
        prev[start] = null;


        if (prev[target] == null) {
            Debug.Log("No Path!");
            return;
        }

        List<Node> currentPath = new List<Node>();
        Node current = target;
        while (current != null)
        {
            currentPath.Add(current);
            current = prev[current];
        }
        currentPath.Reverse();
        selectedUnit.GetComponent<Unit>().currentPath = currentPath;
    }

    // Moving using Dijkstra's algorithm
    public void moveUnit(int x, int y)
    {
        Dictionary<Node, float> dist =  new Dictionary<Node, float>();
        Dictionary<Node, Node> prev = new Dictionary<Node, Node>();
        List<Node> unvisited = new List<Node>();
        Node start = graph[selectedUnit.GetComponent<Unit>().tileX,
                        selectedUnit.GetComponent<Unit>().tileY];
        Node target = graph[x, y];
        dist[start] = 0;
        prev[start] = null;
        foreach (Node v in graph) {
            if (v != start) {
                dist[v] = Mathf.Infinity;
                prev[v] = null;
            }
            unvisited.Add(v);
        }

        while (unvisited.Count != 0) {
            Node curr = null;
            foreach (Node possible in unvisited) {
                if (curr == null || dist[possible] < dist[curr]) curr = possible;
            }
            if (curr == target) {
                Debug.Log("Path found!");
                break;
            }

            unvisited.Remove(curr);

            foreach (Node n in curr.edges) {
                float alt = dist[curr] + CostToEnterTile(curr.x, curr.y, n.x, n.y);
                if (alt < dist[n]) {
                    dist[n] = alt;
                    prev[n] = curr;
                }
            }
        }

        if (prev[target] == null) {
            Debug.Log("No Path found");
            return;
        }

        List<Node> currentPath = new List<Node>();
        Node current = target;
        while (current != null) {
            currentPath.Add(current);
            current = prev[current];
        }
        currentPath.Reverse();
        selectedUnit.GetComponent<Unit>().currentPath = currentPath;
    }

    float CostToEnterTile(int sourceX, int sourceY, int targetX, int targetY){
        float tt = tileTypes[tiles[targetX, targetY]].cost;
        
        if (sourceX != targetX && sourceY != targetY) {
            tt += 0.5f;
        }
        return tt;
    }
    // Moving using A* algorrithm
    public IEnumerator moveA(int x, int y)
    {
        Dictionary<Node, Node> prev = new Dictionary<Node, Node>();
        List<Node> open = new List<Node>();
        HashSet<Node> explored = new HashSet<Node>();

        Node start = graph[selectedUnit.GetComponent<Unit>().tileX,
                        selectedUnit.GetComponent<Unit>().tileY];
        Node target = graph[x, y];

        open.Add(start);
        prev[target] = null;
        start.gCost = 0;
        start.hCost = CalculateDistance(start, target);
        start.CalFCost();

        while (open.Count != 0) {
            Node curr = null;
            foreach (Node v in open) {
                if (curr == null || v.fCost < curr.fCost) curr = v;
            }

            if (curr == target) {
                Debug.Log("Found Path");
                break;
            }

            open.Remove(curr);
            explored.Add(curr);

            foreach (Node n in curr.edges) {
                if (!explored.Contains(n) && tileTypes[tiles[n.x, n.y]].isWalkable) {
                    int tentativeGCost = curr.gCost + CalculateDistance(curr, n);
                    if (tentativeGCost < n.gCost) {
                        prev[n] = curr;
                        n.gCost = tentativeGCost;
                        n.hCost = CalculateDistance(curr, target);
                        n.CalFCost();
                        if (!open.Contains(n)) {
                            open.Add(n);
                            Debug.DrawLine(new Vector2(curr.x, curr.y), new Vector2(n.x, n.y), Color.blue, 10);
                            yield return new WaitForSeconds(0.5f);
                        }
                    }
                }
            }
        }
        prev[start] = null;
        //GOT OUT OF MAINLOOP
        // Check if didn't find target
        if (prev[target] == null) {
            Debug.Log("Did not find path!");
            RefreshGCost();
            yield break;
        }

        List<Node> currentPath = new List<Node>();
        Node current = target;
        while (current != null)
        {
            currentPath.Add(current);
            current = prev[current];
        }
        currentPath.Reverse();
        selectedUnit.GetComponent<Unit>().currentPath = currentPath;
        RefreshGCost();
    }

    void RefreshGCost() {
        foreach (Node n in graph) {
            n.gCost = int.MaxValue;
            n.CalFCost();
        }
    }

    int CalculateDistance(Node start, Node end) {
        int xDistance = Mathf.Abs(start.x - end.x);
        int yDistance = Mathf.Abs(start.y - end.y);
        int remainder = Mathf.Abs(xDistance - yDistance);
        return 14 * Mathf.Min(xDistance, yDistance) + 10 * remainder;
    }

}
