using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

public class Shuffle : MonoBehaviour
{
    public List<CardInfo> deck = new List<CardInfo>();
    public GameObject newDeck;
    public GameObject player;
    public GameObject dealer;
    private int index = 0;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown("space"))
        {
            ShuffleDeck(deck);
        }
        if (Input.GetKeyDown("p"))
        {
            DrawCard(deck, player);
        }
        if (Input.GetKeyDown("d"))
        {
            DrawCard(deck, dealer);
        }
    }

    public void ShuffleDeck(List<CardInfo> tempdeck)
    {
        for (int count = tempdeck.Count - 1; count >= 0; --count)
        {
            int tempIndex = UnityEngine.Random.Range(0, count + 1);

            CardInfo tmp = tempdeck[count];
            tempdeck[count] = tempdeck[tempIndex];
            tempdeck[tempIndex] = tmp;
            tempdeck[count].transform.parent = newDeck.transform;
        }
    }

    public void DrawCard(List<CardInfo> tempdeck, GameObject gameObject)
    {
        tempdeck[index].transform.parent = gameObject.transform;
        ++index;
    }
}
