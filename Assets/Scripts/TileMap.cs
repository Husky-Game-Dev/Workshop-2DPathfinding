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

        public Node() : this(0, 0){}
        public Node(int x, int y)
        {
            this.x = x;
            this.y = y;
            edges = new List<Node>();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        selectedUnit.GetComponent<Unit>().tileX = (int)selectedUnit.transform.position.x;
        selectedUnit.GetComponent<Unit>().tileX = (int)selectedUnit.transform.position.x;
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
            }
        }

        // Find neighbor node for each tile
        for (int x = 0; x < sizeX; x++)
        {
            for (int y = 0; y < sizeY; y++)
            {
                // 4 Way Movement
                if (x > 0)
                    graph[x, y].edges.Add(graph[x - 1, y]);
                if (x < sizeX - 1)
                    graph[x, y].edges.Add(graph[x + 1, y]);
                if (y > 0)
                    graph[x, y].edges.Add(graph[x, y - 1]);
                if (y < sizeY - 1)
                    graph[x, y].edges.Add(graph[x, y + 1]);

                /*// 8 Way Movement
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
                    graph[x, y].edges.Add(graph[x, y + 1]);*/

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
        // TODO
    }

    // Moving using Dijkstra's algorithm
    public void moveUnit(int x, int y)
    {
        // TODO
    }
    // Moving using A* algorrithm
    public void moveA(int x, int y)
    {
        // TODO
    }
}
