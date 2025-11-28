using System.Collections.Generic;
using UnityEngine;

public class AstarBehavior : MonoBehaviour
{
    public bool available = true;
    public GameObject resource;
    public DruidManager manager;
    public float speed;
    public State myGoal = State.none;
    private List<Tile> myPath = new List<Tile>();
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (myPath.Count != 0) {
            available = false;
            if (Vector3.Distance(transform.localPosition, myPath[0].tileObject.transform.localPosition + new Vector3(0, 0, -1)) < 0.000001f) {
                myPath.RemoveAt(0); 
                return;
            }
            float step = speed * Time.deltaTime;
            transform.localPosition = Vector3.MoveTowards(transform.localPosition, myPath[0].tileObject.transform.localPosition + new Vector3(0, 0, -1), step * myPath[0].tileSpeed);
            if (resource != null && Vector3.Distance(transform.localPosition, resource.transform.localPosition) < 0.000001f)
            {
                Destroy(resource);
                if (myGoal == State.Forest) manager.moreWood(5);
                if (myGoal == State.Mine) manager.moreIron(5);

            }
        }
        else
        {
            available = true;
        }
    }

    public void newPath(List<Tile> newPath)
    {
        if (myPath.Count == 0)
            myPath = newPath;
    }
}
