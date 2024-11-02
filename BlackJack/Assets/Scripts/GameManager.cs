using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public List<GameObject> deck = new List<GameObject>();
    public List<GameObject> backUpDeck = new List<GameObject>();
    public GameObject shuffledDeck;
    public GameObject unShuffledDeck;

    // Start is called before the first frame update
    void Start()
    {
        SaveUnShuffledDeck(deck, backUpDeck);
        ShuffleDeck(deck);
    }

    // Update is called once per frame
    void Update()
    {
        //if(Input.GetKeyDown("space"))
        //{
        //    ShuffleDeck(deck);
        //}
        //if (Input.GetKeyDown("p"))      //Deal to player
        //{
        //    DrawCardPlayer(deck, player);
        //}
        //if (Input.GetKeyDown("d"))      //deal to dealer
        //{
        //    DrawCardDealer(deck, dealer);
        //}
        if (Input.GetKeyDown("r"))      //Reset Game
        {
            RestartGame(deck);
        }
    }

    public void RestartGame(List<GameObject> deck)
    {
        ReplaceDeck(deck, backUpDeck);

        //for (int i = player.transform.childCount - 1; i >= 0; i--)
        //{
        //    player.transform.GetChild(i).transform.parent = deck[i].transform;
        //    player.transform.GetChild(i).transform.position = deck[i].transform.position;
        //}

        //for (int i = dealer.transform.childCount - 1; i >= 0; i--)
        //{
        //    dealer.transform.GetChild(i).transform.parent = deck[i].transform;
        //    dealer.transform.GetChild(i).transform.position = deck[i].transform.position;
        //}
    }

    public void SaveUnShuffledDeck(List<GameObject> deck, List<GameObject> backUpDeck)
    {
        for (int count = deck.Count - 1; count >= 0; --count)
        {
            backUpDeck[count] = deck[count];
        }
    }

    public void ReplaceDeck(List<GameObject> deck, List<GameObject> backUpDeck)
    {
        for (int count = backUpDeck.Count - 1; count >= 0; --count)
        {
            deck[count] = backUpDeck[count];
        }

        for (int i = 0; i < 10; i++)
        {
            
        }
    }

    public void ShuffleDeck(List<GameObject> tempdeck)
    {
        for (int count = tempdeck.Count - 1; count >= 0; --count)
        {
            int tempIndex = UnityEngine.Random.Range(0, count + 1);

            GameObject tmp = tempdeck[count];
            tempdeck[count] = tempdeck[tempIndex];
            tempdeck[tempIndex] = tmp;
            tempdeck[count].transform.parent = shuffledDeck.transform;
        }
    }

    //public void DrawCardPlayer(List<CardInfo> deck, GameObject player)
    //{
    //    deck[index].transform.parent = player.transform;
    //    deck[index].transform.position = player.transform.position;
    //    ++index;
    //}

    //public void DrawCardDealer(List<CardInfo> deck, GameObject dealer)
    //{
    //    deck[index].transform.parent = dealer.transform;
    //    deck[index].transform.position = dealer.transform.position;
    //    ++index;
    //}
}
