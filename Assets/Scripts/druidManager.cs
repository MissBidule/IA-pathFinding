using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum State {none, Village, Forest, Mine, Rest};

public class DruidManager : MonoBehaviour
{
    [SerializeField]
    public GameObject druid;
    public GameObject druidSpawner;
    public int druidNb;
    public TextMeshProUGUI availables;
    public Button woodButton;
    public Button ironButton;
    public TextMeshProUGUI woodQtyTxt;
    public TextMeshProUGUI ironQtyTxt;

    private List<GameObject> druids = new List<GameObject>();
    private int druisdAvailable;
    private int woodQty = 0;
    private int ironQty = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        druisdAvailable = druidNb;
        availables.text = druisdAvailable + " / " + druidNb;
        woodQtyTxt.text = woodQty.ToString();
        ironQtyTxt.text = ironQty.ToString();
        for (int i = 0; i < druidNb; i++)
        {
            GameObject newDruid = Instantiate(druid, druidSpawner.transform);
            newDruid.transform.localScale = druid.transform.localScale;
            newDruid.transform.localPosition = druid.transform.localPosition;
            newDruid.SetActive(true);
            druids.Add(newDruid);
        }
    }

    // Update is called once per frame
    void Update()
    {
    }

    void FixedUpdate()
    {
        woodQtyTxt.text = woodQty.ToString();
        ironQtyTxt.text = ironQty.ToString();
    }

    public void sendDruidToWork(bool wood)
    {
        State goal = wood ? State.Forest : State.Mine;
        foreach (GameObject druid in druids)
        {
            if (druid.GetComponent<DjikstraBehavior>().available)
            {
                druid.GetComponent<DjikstraBehavior>().myGoal = goal;
                druid.GetComponent<DjikstraBehavior>().newGoal = true;
                if (--druisdAvailable == 0)
                {
                    woodButton.interactable = false;
                    ironButton.interactable = false;
                }
                availables.text = druisdAvailable + " / " + druidNb;
                break;
            }
        }
    }

    public void druidBackAtHome()
    {
        if (druisdAvailable++ == 0)
        {
            woodButton.interactable = true;
            ironButton.interactable = true;
        }
        availables.text = druisdAvailable + " / " + druidNb;
    }

    public void moreWood()
    {
        woodQty++;
    }

    public void moreIron()
    {
        ironQty++;
    }

    public void pauseButton(bool pause)
    {
        woodButton.gameObject.SetActive(pause);
        ironButton.gameObject.SetActive(pause);
    }
}
