using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Astar : MonoBehaviour
{
    [SerializeField]
    public Tilemap roads;
    public List<Sprite> woodSprites = new List<Sprite>();
    public List<Sprite> ironSprites = new List<Sprite>();
    public DruidManager druidManager;
    public GameObject tileSpawner;
    public GameObject tileModel;
    private List<Tile> currentTiles = new List<Tile>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
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
                State state;
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

                if (state == State.Forest || state == State.Mine) initAstarGrid(state);
            }
        }
    }

    void initAstarGrid(State state)
    {
        druidManager.pauseButton(true);
        currentTiles.Clear();

        GetComponent<SpriteRenderer>().sprite = state == State.Forest ? woodSprites[0] : ironSprites[0];
        GetComponent<SpriteRenderer>().enabled = true;

        for (int i = -10; i  < 10; i ++)
        {
            for (int j = -10; j  < 10; j ++)
            {
                GameObject newSprite = Instantiate(tileModel, tileSpawner.transform);
                newSprite.transform.localPosition = new Vector3(i, j, -1);
                Tile newTile = new Tile();
                float randType = Random.Range(0, 1);
                if (randType < 0.3333f)
                {
                    newSprite.GetComponent<SpriteRenderer>().sprite = state == State.Forest ? woodSprites[1] : ironSprites[1];
                    newTile.tileSpeed = 0.5f;
                }
                else
                {
                    newSprite.GetComponent<SpriteRenderer>().sprite = state == State.Forest ? woodSprites[0] : ironSprites[0];
                    newTile.tileSpeed = 1;
                }
                newSprite.SetActive(true);
                newTile.tileObject = newSprite;
                currentTiles.Add(newTile);
            }
        }

        for (int i = 0; i < 10; i++)
        {
            int randTile = Random.Range(0, 100);
            currentTiles[randTile].tileObject.GetComponent<SpriteRenderer>().sprite = state == State.Forest ? woodSprites[2] : ironSprites[2];
            currentTiles[randTile].tileSpeed = 0;
        }
        StartCoroutine(AstarCD());
    }

    void closeAstarGrid()
    {
        for (int i = 0; i < currentTiles.Count; i++)
        {
            Destroy(currentTiles[i].tileObject);
        }
        GetComponent<SpriteRenderer>().enabled = false;
        druidManager.pauseButton(false);
    }

    IEnumerator AstarCD()
    {
        yield return new WaitForSeconds(5);
        closeAstarGrid();
    }
}

public class Tile
{
    public GameObject tileObject;
    public float tileSpeed = 1;
}
