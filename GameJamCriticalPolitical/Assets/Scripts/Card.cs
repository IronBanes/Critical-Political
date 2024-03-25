using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class Card : MonoBehaviour
{
    public bool hasBeenPlayed;
    public int handIndex;

	public Transform playcardslot;

    public GameManager gm;

    private Animator anim;

    private Animator camAnim;
	
	public int cardtype;

	public int coinscount;
	public int votecount;
	public int corruption;

    

    // Start is called before the first frame update
    void Start()
    {
        gm = FindObjectOfType<GameManager>();
    }

    // Update is called once per frame
    private void OnMouseDown()
	{
		if (!hasBeenPlayed)
		{
			
			playcard();

			
		}
	}

	public void playcard(){
		
		//add coins 
		if(cardtype == 1){
			gm.addcoins(coinscount);
			gm.addvotes(votecount);
			gm.addcorruption(corruption);
		}
		//nature reserve animal hit piece 
		else if(cardtype == 2){
			if ((gm.coinbal + coinscount) >= 0){
				gm.addvotes(votecount);
				gm.addcoins(coinscount);
				gm.addcorruption(corruption);
			}else{
				return;
			}
		}
		//environment votes
		else if(cardtype == 3 ){//keep
			gm.addvotes(votecount);
		}
		//investigation
		else if(cardtype == 4 ){//keep
			int corruptpoints = gm.getcorruption();
			if (corruptpoints >=0){
				gm.addvotes(corruptpoints*votecount);
				gm.corrupt /= 2;
			}else{
				gm.corrupt = 0;
			}
			
		}
		
		//money man
		else if(cardtype == 5){
			gm.coinbal /= 2;
		}
		hasBeenPlayed = true;
		
		gm.availableCardSlots[handIndex] = true;
		transform.position = playcardslot.position;
		//transform.localScale *= 0.7f;//+= Vector3.up * 3f;
		Invoke("MoveToDiscardPile", 2f);
		gm.CheckAllHandSlotsFull();
	}

	void MoveToDiscardPile()
	{
		gm.cardsInHand.Remove(this);
		gm.discardPile.Add(this);
		gameObject.SetActive(false);
		//transform.localScale /= 0.7f;
	}
}
