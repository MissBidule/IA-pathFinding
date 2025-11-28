using UnityEngine;
using UnityEngine.Tilemaps;

public class buildMode : MonoBehaviour
{
    [SerializeField]
    public DruidManager druidManager;
    public Tilemap roads;
    public Tilemap decor;
    public TileBase pathTile;
    public TileBase ForestTile;
    public TileBase MineTile;
    public TileBase VillageTile;
    public TileBase restTile;
    public State state;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (!druidManager.buildMode) return;
        if (Input.GetMouseButton(0))
        {
            Vector3 mouseWorldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                
            var tpos = roads.WorldToCell(mouseWorldPoint);

            // Try to get a tile from cell position
            var roadTile = roads.GetTile(tpos);
            var decorTile = decor.GetTile(tpos);

            if (state == State.Destroy && decorTile && roadTile) roads.SetTile(tpos, null);
            else if(decorTile && !roadTile)
            {
                switch (state)
                {
                    case State.none :
                        roads.SetTile(tpos, pathTile);
                        break;
                    case State.Forest :
                        roads.SetTile(tpos, ForestTile);
                        break;
                    case State.Mine :
                        roads.SetTile(tpos, MineTile);
                        break;
                    case State.Village :
                        roads.SetTile(tpos, VillageTile);
                        break;
                    case State.Rest :
                        roads.SetTile(tpos, restTile);
                        break;
                    default :
                        break;
                }
            }
        }
    }

    public void switchToPath()
    {
        state = State.none;
    }

    public void switchToForest()
    {
        state = State.Forest;
    }

    public void switchToMine()
    {
        state = State.Mine;
    }

    public void switchToRest()
    {
        state = State.Rest;
    }

    public void switchToVillage()
    {
        state = State.Village;
    }

    public void SwitchToDestroy()
    {
        state = State.Destroy;
    }
}
