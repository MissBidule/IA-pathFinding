using UnityEngine;
using System.Collections.Generic;

public class DjikstraBehavior : MonoBehaviour
{
    [SerializeField]
    public Djikstra algo;
    public float speed;
    public State myGoal = State.none;
    public bool newGoal = false;
    public bool available = true;
    private List<DjikstraNode> myPath = new List<DjikstraNode>();
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        if (newGoal && available) {
            available = false;
            newGoal = false;
            myPath = algo.getPath(Vector3Int.FloorToInt(transform.localPosition), myGoal);
        }
        else if (myPath.Count != 0) {
            if (Vector3.Distance(transform.localPosition, myPath[0].coord) < 0.01f) {
                myPath.RemoveAt(0); 
                return;
            }
            float step = speed * Time.deltaTime;
            transform.localPosition = Vector3.MoveTowards(transform.localPosition, myPath[0].coord, step);
        }
        else
        {
            available = true;
            myGoal = State.none;
        }
    }
}