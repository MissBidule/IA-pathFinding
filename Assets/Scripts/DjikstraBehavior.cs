using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class DjikstraBehavior : MonoBehaviour
{
    public enum DruidState{ available, work, walk, pause, newGoal};
    [SerializeField]
    public Djikstra algo;
    public float speed;
    public State myGoal = State.none;
    public DruidState druidState = DruidState.available;
    public DruidManager manager;
    public List<DjikstraNode> myPath = new List<DjikstraNode>();
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        if (druidState == DruidState.newGoal) {
            druidState = DruidState.walk;
            myPath = algo.getPath(Vector3Int.FloorToInt(transform.localPosition), myGoal);
        }
        else if (myPath.Count != 0 && (druidState == DruidState.walk || druidState == DruidState.pause)) {
            if (Vector3.Distance(transform.localPosition, (Vector3)myPath[0].coord) < 0.000001f) {
                myPath.RemoveAt(0); 
                if (myPath.Count == 0 && druidState != DruidState.pause) {
                    if (myGoal != State.Village) {
                        StartCoroutine(workCD());
                        druidState = DruidState.work;
                    }
                    else {
                        druidState = DruidState.available;
                        manager.druidBackAtHome();
                    }
                    return;
                }
            }
            float step = speed * Time.deltaTime;
            transform.localPosition = Vector3.MoveTowards(transform.localPosition, myPath[0].coord, step);
        }
    }

    public void pauseWork(float waitFor)
    {
        druidState = DruidState.pause;
        StopAllCoroutines();
        StartCoroutine(AstarCD(waitFor));
    }

    void endCD()
    {
        if (myGoal != State.Rest) myGoal = State.Rest;
        else {
            myGoal = State.Village;
        }
        druidState = DruidState.newGoal;
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

        endCD();
    }

    IEnumerator AstarCD(float waitFor)
    {
        yield return new WaitForSeconds(waitFor);

        endCD();
    }
}