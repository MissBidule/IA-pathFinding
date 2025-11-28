using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor.ProjectWindowCallback;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Astar : MonoBehaviour
{
    [SerializeField]
    public Tilemap roads;
    public List<Sprite> woodSprites = new List<Sprite>();
    public List<Sprite> ironSprites = new List<Sprite>();
    public List<Sprite> resourceSprites = new List<Sprite>();
    public DruidManager druidManager;
    public GameObject tileSpawner;
    public GameObject tileModel;
    public GameObject druid;
    public GameObject resource;
    private List<Tile> currentTiles = new List<Tile>();
    private int gridSize = 10;
    private float astarTimer = 5;
    private GameObject currentDruid;
    private GameObject currentResource;
    private bool playing = false;
    private State state;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (druidManager.buildMode) return;
        if (Input.GetMouseButtonDown(0) && !playing)
        {
            Vector3 mouseWorldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                
            var tpos = roads.WorldToCell(mouseWorldPoint);

            // Try to get a tile from cell position
            var tile = roads.GetTile(tpos);

            if(tile)
            {
                TileData tileData = new TileData();
                tile.GetTileData(tpos, roads, ref tileData);

                if (!tileData.sprite.name.Contains("vfmr")) return;
                switch (tileData.sprite.name[tileData.sprite.name.Length-1])
                {
                    case '0' :
                        state = State.Village;
                        break;
                    case '1' :
                        state = State.Forest;
                        break;
                    case '2' :
                        state = State.Mine;
                        break;
                    case '3' :
                        state = State.Rest;
                        break;
                    default :
                        state = State.none;
                        break;
                }

                if (state == State.Forest || state == State.Mine) {
                    tpos.z = -4;
                    if (druidManager.isDruidAvailable(tpos, astarTimer)) initAstarGrid();
                }
            }
        }
        else if (playing && currentResource == null)
        {
            initResource();
        }
        else if (Input.GetMouseButtonDown(0) && currentDruid.GetComponent<AstarBehavior>().available)
        {
            Vector3 mouseWorldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3 mouseLocalPoint = tileSpawner.transform.InverseTransformPoint(mouseWorldPoint);
            mouseLocalPoint.z = -1;
            currentDruid.GetComponent<AstarBehavior>().newPath(calculateAstar(Vector3Int.RoundToInt(mouseLocalPoint)));
        }
    }

    void initAstarGrid()
    {
        druidManager.pauseButton(true);
        currentTiles.Clear();

        GetComponent<SpriteRenderer>().sprite = state == State.Forest ? woodSprites[0] : ironSprites[0];
        GetComponent<SpriteRenderer>().enabled = true;

        for (int i = 0; i  < gridSize; i ++)
        {
            for (int j = 0; j  < gridSize; j ++)
            {
                GameObject newSprite = Instantiate(tileModel, tileSpawner.transform);
                newSprite.transform.localPosition = new Vector3(j, i, -1);
                Tile newTile = new Tile();
                float randType = Random.Range(0.0f, 1.0f);
                if (randType < 0.3333f)
                {
                    newSprite.GetComponent<SpriteRenderer>().sprite = state == State.Forest ? woodSprites[1] : ironSprites[1];
                    newTile.tileSpeed = 0.25f;
                }
                else
                {
                    newSprite.GetComponent<SpriteRenderer>().sprite = state == State.Forest ? woodSprites[0] : ironSprites[0];
                    newTile.tileSpeed = 1;
                }
                newSprite.name = "Tile" + (j + i * gridSize).ToString();
                newSprite.SetActive(true);
                newTile.tileObject = newSprite;
                currentTiles.Add(newTile);
            }
        }

        for (int i = 0; i < gridSize; i++)
        {
            int randTile = Random.Range(0, currentTiles.Count);
            currentTiles[randTile].tileObject.GetComponent<SpriteRenderer>().sprite = state == State.Forest ? woodSprites[2] : ironSprites[2];
            currentTiles[randTile].tileSpeed = 0;
        }

        int randSpawn = Random.Range(0, currentTiles.Count);
        while (currentTiles[randSpawn].tileSpeed == 0) randSpawn = Random.Range(0, currentTiles.Count);
        currentDruid = Instantiate(druid, tileSpawner.transform);
        currentDruid.transform.localScale = druid.transform.localScale;
        currentDruid.transform.localPosition = currentTiles[randSpawn].tileObject.transform.localPosition + new Vector3(0, 0, -1);
        currentDruid.GetComponent<AstarBehavior>().myGoal = state;
        currentDruid.SetActive(true);

        initResource();

        playing = true;

        StartCoroutine(AstarCD());
    }

    void initResource()
    {
        int randResource = Random.Range(0, currentTiles.Count);
        while (currentTiles[randResource].tileSpeed == 0) randResource = Random.Range(0, currentTiles.Count);
        currentResource = Instantiate(resource, tileSpawner.transform);
        currentResource.transform.localScale = resource.transform.localScale;
        currentResource.transform.localPosition = currentTiles[randResource].tileObject.transform.localPosition + new Vector3(0, 0, -1);
        currentResource.GetComponent<SpriteRenderer>().sprite = state == State.Forest ? resourceSprites[0] : resourceSprites[1];
        currentDruid.GetComponent<AstarBehavior>().resource = currentResource;
        currentResource.SetActive(true);
    }

    void closeAstarGrid()
    {
        playing = false;
        for (int i = 0; i < currentTiles.Count; i++)
        {
            Destroy(currentTiles[i].tileObject);
        }
        Destroy(currentDruid);
        Destroy(currentResource);
        GetComponent<SpriteRenderer>().enabled = false;
        druidManager.pauseButton(false);
    }

    IEnumerator AstarCD()
    {
        yield return new WaitForSeconds(astarTimer);
        closeAstarGrid();
    }

    List<Tile> calculateAstar(Vector3Int objective)
    {
        List<Tile> returnList = new List<Tile>();
        int x = (int)currentDruid.transform.localPosition.x;
        int y = (int)currentDruid.transform.localPosition.y;
        if (x + y * gridSize >= currentTiles.Count || x + y * gridSize < 0) return returnList;
        Tile first = currentTiles[x + y * gridSize];
        if (objective == Vector3Int.FloorToInt(first.tileObject.transform.localPosition) || first.tileSpeed == 0) {
            return returnList;
        }

        List<PointIn> closedList = new List<PointIn>();
        List<PointIn> openedList = new List<PointIn>();

        PointIn firstPoint = new PointIn();
        firstPoint.end = first;
        firstPoint.start = first;
        firstPoint.distance = Vector3Int.Distance(objective, Vector3Int.FloorToInt(firstPoint.end.tileObject.transform.localPosition)) + 1 * (1 - firstPoint.end.tileSpeed);
        openedList.Add(firstPoint);

        while (openedList.Count != 0)
        {
            float minDist = Mathf.Infinity;
            int index = 0;
            for (int i = 0; i < openedList.Count; i++)
            {
                if (openedList[i].distance < minDist)
                {
                    minDist = openedList[i].distance;
                    index = i;
                }
            }

            if (minDist == Mathf.Infinity) return returnList;

            PointIn pointSelected = openedList[index];
            closedList.Add(pointSelected);
            openedList.RemoveAt(index);

            if (pointSelected.end.tileObject.transform.localPosition != objective) {
                for (int i = -1; i <= 1; i ++)
                {
                    for (int j = -1; j <= 1; j ++)
                    {
                        if (Mathf.Abs(i) == Mathf.Abs(j)) continue;

                        bool alreadyVisited = false;
                        int tempX = (int)pointSelected.end.tileObject.transform.localPosition.x + j;
                        int tempY = (int)pointSelected.end.tileObject.transform.localPosition.y + i;
                        if (tempX < 0 || tempY < 0 || tempX >= gridSize || tempY >= gridSize) continue;
                        if (currentTiles[tempX + tempY * gridSize].tileSpeed == 0) continue;
                        foreach (PointIn closedPoint in closedList)
                        {
                            if (closedPoint.end.tileObject.transform.localPosition == currentTiles[tempX + tempY * gridSize].tileObject.transform.localPosition)
                            {
                                alreadyVisited = true;
                                break;
                            }
                            
                        }
                        if (alreadyVisited) continue;
                        
                        PointIn neighbor = new PointIn();
                        neighbor.start = pointSelected.end;
                        neighbor.end = currentTiles[tempX + tempY * gridSize];
                        neighbor.distance = Vector3Int.Distance(objective, Vector3Int.FloorToInt(neighbor.end.tileObject.transform.localPosition)) + 1 * (1-neighbor.end.tileSpeed);
                        openedList.Add(neighbor);
                    }
                }
            }
            else
            {
                openedList.Clear();
            }
        }
        
        Vector3 goal = closedList[closedList.Count-1].start.tileObject.transform.localPosition;
        returnList.Add(closedList[closedList.Count-1].end);
        returnList.Insert(0, closedList[closedList.Count-1].start);
        while (new Vector3(goal.x, goal.y, -2) != currentDruid.transform.localPosition)
        {
            foreach (PointIn closedPoint in closedList)
            {
                if (closedPoint.end.tileObject.transform.localPosition == goal)
                {
                    returnList.Insert(0, closedPoint.start);
                    goal = closedPoint.start.tileObject.transform.localPosition;
                    break;
                }
            }
        }

        return returnList;
    }
}

public class Tile
{
    public GameObject tileObject;
    public float tileSpeed = 1;
}

public class PointIn
{
    public Tile end;
    public Tile start;
    public float distance;
}
