using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Runtime.ExceptionServices;
using Unity.VisualScripting;
using System.Xml.Serialization;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public List<Card> deck;
	public List<Card> cardsInHand; // New field to store cards in the player's hand

	public TextMeshProUGUI deckSizeText;

	public Transform[] cardSlots;
	public bool[] availableCardSlots;

	public List<Card> discardPile;
	public TextMeshProUGUI discardPileSizeText;

	public int coinbal;

	public TextMeshProUGUI CoinText;

	public int votes;

	public TextMeshProUGUI VotesText;

	public int corrupt;
	public TextMeshProUGUI CorruptText;

	public Button drawCardButton;
	public int WinVotes;

	private bool GameOverCoroutineRunning = false;



    //public bool[] availableHandSlots;
	private bool allhandSlotsFull = false;

    // Start is called before the first frame update
    void Start()
    {
		cardsInHand = new List<Card>();
        CheckAllHandSlotsFull();
    }

	public void DrawCard(int cost)
	{
		CheckAllHandSlotsFull();
		if ((coinbal-cost >= 0) && deck.Count >= 1 && allhandSlotsFull == false)
		{	
			if(drawCardButton != null)
			{
				drawCardButton.interactable = false;
			}
			coinbal -= cost;
			Card randomCard = deck[Random.Range(0, deck.Count)];


			// If the drawn card is not the desired type, find an available card slot to place it
			for (int i = 0; i < availableCardSlots.Length; i++)
			{
				if (availableCardSlots[i] == true)
				{
					randomCard.gameObject.SetActive(true);
					randomCard.handIndex = i;
					randomCard.transform.position = cardSlots[i].position;
					randomCard.hasBeenPlayed = false;
					deck.Remove(randomCard);
					cardsInHand.Add(randomCard);
					availableCardSlots[i] = false;

					if (randomCard.cardtype == 3||randomCard.cardtype == 4||randomCard.cardtype == 5)
					{
						// Play the card immediately
						randomCard.playcard();
					}
					StartCoroutine(EnableDrawCardButton());
					return;
				}
			}
		}else
		{
			StartCoroutine(EnableDrawCardButton());
			return;
		}
	}

	IEnumerator EnableDrawCardButton()
    {
        yield return new WaitForSeconds(0.5f); // Wait for the card to be drawn (adjust the time as needed)
        drawCardButton.interactable = true; // Enable the draw card button
    }


	public void Shuffle()
	{
		if (discardPile.Count >= 1)
		{
			foreach (Card card in discardPile)
			{
				deck.Add(card);
			}
			discardPile.Clear();
		}
	}

	public void addcoins(int coinscount){
		coinbal += coinscount;
	}

	public void addvotes(int votescount){
		votes += votescount;
	}

	public void addcorruption(int corruption){
		corrupt += corruption;
	}

	// Add card to the hand
    public void AddCardToHand(Card card)
    {
        cardsInHand.Add(card);
    }

    // Remove card from the hand
    public void RemoveCardFromHand(Card card)
    {
        cardsInHand.Remove(card);
    }


	public int getcorruption(){
		return corrupt;
	}
	// Check if all hand slots are full
    public void CheckAllHandSlotsFull()
    {
        allhandSlotsFull = true; // Assume all hand slots are full initially

        // Iterate through availableHandSlots array
        foreach (bool slotStatus in availableCardSlots)
        {
            if (slotStatus)
            {
                allhandSlotsFull = false; // If any slot is available, set allHandSlotsFull to false
                break;
            }
        }
    }

	// Check if the player has a playable card of type 1 in their hand
    public bool HasPlayableCardType1()
    {
        foreach (Card card in cardsInHand)
        {
            // Check if the card is of type 1 and can be played
            if (card.cardtype == 1)
            {
                return true; // Found a playable card of type 1
            }
        }
        return false; // No playable card of type 1 found
    }

	public void CheckCardPlayable()
    {
        allhandSlotsFull = true; // Assume all hand slots are full initially

        // Iterate through availableHandSlots array
        foreach (bool slotStatus in availableCardSlots)
        {
            if (slotStatus)
            {
                allhandSlotsFull = false; // If any slot is available, set allHandSlotsFull to false
                break;
            }
        }
    }

	private void Update()
	{
		deckSizeText.text = deck.Count.ToString();
		discardPileSizeText.text = discardPile.Count.ToString();
		CoinText.text = "$" + coinbal.ToString();
		if(votes < 0 ){
			votes = 0;
		}
		VotesText.text = votes.ToString();

		CorruptText.text = corrupt.ToString();
		if(coinbal <100 && !HasPlayableCardType1() && !GameOverCoroutineRunning)
		{
			bool output = HasPlayableCardType1();
			//Debug.Log(output);
			StartCoroutine(GameOverDelay(5));
		}
		if(coinbal < 500 && allhandSlotsFull &&  !HasPlayableCardType1() && !GameOverCoroutineRunning)
		{
			bool output = HasPlayableCardType1();
			//Debug.Log(output);
			StartCoroutine(GameOverDelay(5));
		}
		

		if(votes >= WinVotes){
			StartCoroutine(GameOverDelay(4));
		}

	}

	IEnumerator GameOverDelay(int value)
	{
		GameOverCoroutineRunning = true;
		yield return new WaitForSeconds(1f);
		SceneManager.LoadScene(value);
	}
}
