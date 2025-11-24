using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class DjikstraBehavior : MonoBehaviour
{
    [SerializeField]
    public Djikstra algo;
    public float speed;
    public State myGoal = State.none;
    public bool newGoal = false;
    public bool available = true;
    public bool work = false;
    public bool rest = false;
    public bool pause = false;
    public DruidManager manager;
    private List<DjikstraNode> myPath = new List<DjikstraNode>();
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        if (pause)
        {
            StopCoroutine(workCD());
            myGoal = State.Rest;
            newGoal = true;
        }
        if (newGoal) {
            available = false;
            newGoal = false;
            if (myGoal != State.Village) work = true;
            myPath = algo.getPath(Vector3Int.FloorToInt(transform.localPosition), myGoal);
        }
        else if (myPath.Count != 0) {
            if (Vector3.Distance(transform.localPosition, myPath[0].coord) < 0.0001f) {
                myPath.RemoveAt(0); 
                if (work && myPath.Count == 0) StartCoroutine(workCD());
                return;
            }
            float step = speed * Time.deltaTime;
            transform.localPosition = Vector3.MoveTowards(transform.localPosition, myPath[0].coord, step);
        }
        else if (!work && !available)
        {
            available = true;
            manager.druidBackAtHome();
        }
    }

    IEnumerator workCD()
    {
        yield return new WaitForSeconds(1);
        if (myGoal == State.Forest) manager.moreWood();
        if (myGoal == State.Mine) manager.moreIron();
        yield return new WaitForSeconds(1);
        if (myGoal == State.Forest) manager.moreWood();
        if (myGoal == State.Mine) manager.moreIron();
        yield return new WaitForSeconds(1);
        if (myGoal == State.Forest) manager.moreWood();
        if (myGoal == State.Mine) manager.moreIron();

        if (myGoal != State.Rest) myGoal = State.Rest;
        else {
            myGoal = State.Village;
            work = false;
        }
        newGoal = true;
    }
}