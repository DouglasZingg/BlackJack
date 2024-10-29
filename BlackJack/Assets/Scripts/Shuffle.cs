using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

public class Shuffle : MonoBehaviour
{
    public List<CardInfo> deck = new List<CardInfo>();

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
    }

    public void ShuffleDeck(List<CardInfo> temp)
    {
        for (int i = temp.Count - 1; i >= 0; --i)
        {
            int j = UnityEngine.Random.Range(0, i + 1);

            CardInfo tmp = temp[i];
            temp[i] = temp[j];
            temp[j] = tmp;
        }
    }
}
