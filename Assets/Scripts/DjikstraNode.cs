using UnityEngine;
using System.Collections.Generic;

public class DjikstraNode
{
    public State state = State.none;
    public Vector3Int coord;
    public List<InterestPoint> neighbours = new List<InterestPoint>();

    public void setState(string spriteName)
    {
        if (!spriteName.Contains("vfmr")) return;
        switch (spriteName[spriteName.Length-1])
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
    }
}

public class InterestPoint
{
    public DjikstraNode node;
    public int distance;
}