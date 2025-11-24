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
        if (roads.GetTile(start) == null) return;

        TileData tileData = new TileData();
        roads.GetTile(start).GetTileData(start, roads, ref tileData);
        DjikstraNode first = new DjikstraNode();
        first.coord = start;
        first.coord.z = -4;
        first.setState(tileData.sprite.name);
        djikstraNodes.Add(first);
        createDjikstraNodes(ref first, start);
    }

    void createDjikstraNodes(ref DjikstraNode parent, Vector3Int position)
    {
        djikstraDirection(ref parent, Direction.left, position);
        djikstraDirection(ref parent, Direction.right, position);
        djikstraDirection(ref parent, Direction.down, position);
        djikstraDirection(ref parent, Direction.up, position);
    }

    void djikstraDirection(ref DjikstraNode parent, Direction dir, Vector3Int position)
    {
        int x = 0;
        int y = 0;
        position.z = 0;
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
        Vector3Int nextPosition = position;
        int amplitude = x != 0 ? max.x - min.x : max.y - min.y;
        for (int i = 1; i <= amplitude; i ++)
        {
            //check if exists
            nextPosition.x = position.x + i * x;
            nextPosition.y = position.y + i * y;
            if (roads.GetTile(nextPosition) == null) break;
            //check if pof || road of > 2 neighbors || if 2 is it the max ?
            TileData tileData = new TileData();
            roads.GetTile(nextPosition).GetTileData(nextPosition, roads, ref tileData);
            int neighbors = neighborCount(nextPosition);
            Vector3Int nextPosition2 = nextPosition;
            nextPosition2.x += 1 * x;
            nextPosition2.y += 1 * y;
            bool isMax = roads.GetTile(nextPosition2) == null;
            if (!roadTile(tileData.sprite.name) || neighbors > 2 || (neighbors == 2 && isMax))
            {
                nextPosition.z = -4;
                InterestPoint pof = new InterestPoint();
                bool exists = false;
                foreach (DjikstraNode node in djikstraNodes)
                {
                    if (node.coord == nextPosition)
                    {
                        exists = true;
                        pof.node = node;
                        break;
                    }
                }
                if (!exists)
                {
                    pof.node = new DjikstraNode();
                    pof.node.coord = nextPosition;
                    pof.node.setState(tileData.sprite.name);
                    djikstraNodes.Add(pof.node);
                    createDjikstraNodes(ref pof.node, nextPosition);
                }
                pof.distance = (int)Vector3Int.Distance(parent.coord, nextPosition);
                parent.neighbours.Add(pof);
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

    public List<DjikstraNode> getPath(Vector3Int start, State objective)
    {
        List<DjikstraNode> returnList = new List<DjikstraNode>();
        DjikstraNode first = new DjikstraNode();
        bool exists = false;
        foreach (DjikstraNode node in djikstraNodes)
        {
            if (node.coord == start)
            {
                exists = true;
                first = node;
                break;
            }
        }
        if (!exists || objective == State.none || first.state == objective) return returnList;

        List<PointAdded> pathFound = new List<PointAdded>();
        List<PointAdded> visited = new List<PointAdded>();

        PointAdded firstPoint = new PointAdded();
        firstPoint.end = first;
        firstPoint.start = first;
        firstPoint.distance = 0;
        visited.Add(firstPoint);

        while (pathFound.Count == 0 || pathFound[pathFound.Count-1].end.state != objective)
        {
            PointAdded nextPoint = new PointAdded();
            nextPoint.distance = int.MaxValue;
            for (int i = 0; i < visited.Count; i++)
            {
                foreach (InterestPoint neighbour in visited[i].end.neighbours)
                {
                    bool alreadyVisited = false;
                    foreach (PointAdded visitedPoint in visited)
                    {
                        if (visitedPoint.end.coord == neighbour.node.coord)
                        {
                            alreadyVisited = true;
                            break;
                        }
                    }
                    if (alreadyVisited) continue;
                    if (visited[i].distance + neighbour.distance < nextPoint.distance)
                    {
                        nextPoint.end = neighbour.node;
                        nextPoint.start = visited[i].end;
                        nextPoint.distance = visited[i].distance + neighbour.distance;
                    }
                }
            }
            if (nextPoint.distance == int.MaxValue) return returnList;
            pathFound.Add(nextPoint);
            visited.Add(nextPoint);
        }
        
        Vector3Int goal = pathFound[pathFound.Count-1].start.coord;
        returnList.Add(pathFound[pathFound.Count-1].end);
        returnList.Insert(0, pathFound[pathFound.Count-1].start);
        while (returnList[0].coord != start)
        {
            foreach (PointAdded visitedPoint in pathFound)
            {
                if (visitedPoint.end.coord == goal)
                {
                    returnList.Insert(0, visitedPoint.start);
                    goal = visitedPoint.start.coord;
                    break;
                }
            }
        }

        return returnList;
    }
}

public class PointAdded
{
    public DjikstraNode end;
    public DjikstraNode start;
    public int distance;
}