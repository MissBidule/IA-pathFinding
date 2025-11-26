using System.Collections.Generic;
using UnityEngine;

public class AstarBehavior : MonoBehaviour
{
    public bool available = true;
    public GameObject resource;
    public DruidManager manager;
    public float speed;
    public State myGoal = State.none;
    private List<Vector3Int> myPath = new List<Vector3Int>();
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (myPath.Count != 0) {
            if (Vector3.Distance(transform.localPosition, myPath[0]) < 0.000001f) {
                myPath.RemoveAt(0); 
                return;
            }
            float step = speed * Time.deltaTime;
            transform.localPosition = Vector3.MoveTowards(transform.localPosition, myPath[0], step);
            if (resource != null && Vector3.Distance(transform.localPosition, resource.transform.localPosition) < 0.000001f)
            {
                if (myGoal == State.Forest) manager.moreWood(5);
                if (myGoal == State.Mine) manager.moreIron(5);
            }
        }
        else
        {
            available = true;
        }
    }

    public void newPath(List<Vector3Int> newPath)
    {
        myPath = newPath;
    }
}
