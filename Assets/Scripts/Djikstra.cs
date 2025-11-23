using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public class Djikstra : MonoBehaviour
{
    [SerializeField]
    public Vector3Int min;
    public Vector3Int max;
    public Vector3Int start;
    public Tilemap roads;

    enum Direction {left, right, down, up};

    void Start()
    {
        reload();
    }

    void Update()
    {
        
    }

    private List<DjikstraNode> djikstraNodes = new List<DjikstraNode>();
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void reload()
    {
        djikstraNodes.Clear();
        //we stay in 2D
        min.z = 0;
        max.z = 0;
        //int xValue = Mathf.Abs(max.x - min.x);
        //int yValue = Mathf.Abs(max.y - min.y);
        //Vector3Int[] positions = new Vector3Int[xValue * yValue];
        //TileBase[] tiles = new TileBase[xValue * yValue];
        //int count = roads.GetTilesRangeNonAlloc(min, max, positions, tiles);
        if (roads.GetTile(start) == null) return;

        TileData tileData = new TileData();
        roads.GetTile(start).GetTileData(start, roads, ref tileData);
        DjikstraNode first = new DjikstraNode();
        first.coord = start;
        first.setState(tileData.sprite.name);
        djikstraNodes.Add(first);
        createDjikstraNodes(ref first, start);
    }

    void createDjikstraNodes(ref DjikstraNode parent, Vector3Int position)
    {
        Debug.Log("! " + position);
        djikstraDirection(ref parent, Direction.left, position);
        djikstraDirection(ref parent, Direction.right, position);
        djikstraDirection(ref parent, Direction.down, position);
        djikstraDirection(ref parent, Direction.up, position);
    }

    void djikstraDirection(ref DjikstraNode parent, Direction dir, Vector3Int position)
    {
        int x = 0;
        int y = 0;
        switch (dir)
        {
            case Direction.left : 
                x = -1;
                break;
            case Direction.right : 
                x = 1;
                break;
            case Direction.down : 
                y = -1;
                break;
            case Direction.up : 
                y = 1;
                break;
            default:
                break;
        }
        Vector3Int newPosition = position;
        int amplitude = x != 0 ? max.x - min.x : max.y - min.y;
        for (int i = 1; i <= amplitude; i ++)
        {
            //check if exists
            newPosition.x = position.x + i * x;
            newPosition.y = position.y + i * y;
            if (roads.GetTile(newPosition) == null) break;
            //check if pof || road of > 2 neighbors || if 2 is it the max ?
            TileData tileData = new TileData();
            roads.GetTile(newPosition).GetTileData(newPosition, roads, ref tileData);
            int neighbors = neighborCount(newPosition);
            newPosition.x = position.x + (i + 1) * x;
            newPosition.y = position.y + (i + 1) * y;
            bool isMax = roads.GetTile(newPosition) == null;
            if (!roadTile(tileData.sprite.name) || neighbors > 2 || (neighbors == 2 && isMax))
            {
                InterestPoint pof = new InterestPoint();
                bool exists = false;
                newPosition.x = position.x + i * x;
                newPosition.y = position.y + i * y;
                foreach (DjikstraNode node in djikstraNodes)
                {
                    if (node.coord == newPosition)
                    {
                        exists = true;
                        pof.node = node;
                        break;
                    }
                }
                if (!exists)
                {
                    pof.node = new DjikstraNode();
                    pof.node.coord = newPosition;
                    pof.node.setState(tileData.sprite.name);
                    createDjikstraNodes(ref pof.node, newPosition);
                }
                pof.distance = (int)Vector3Int.Distance(parent.coord, newPosition);
                parent.neighbours.Add(pof);
                Debug.Log(newPosition);
            }

        }
    }

    int neighborCount(Vector3Int position)
    {
        int count = 0;
        Vector3Int newPosition;
        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                if (Mathf.Abs(i) == Mathf.Abs(j)) continue;
                newPosition = new Vector3Int(position.x + i, position.y + j, 0);
                if (roads.GetTile(newPosition) != null) count++;
            }
        } 
        return count;
    }

    bool roadTile(string spriteName)
    {
        return spriteName.Contains("Road");
    }
}
