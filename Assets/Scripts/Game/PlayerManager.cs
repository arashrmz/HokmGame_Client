using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using HokmGame.Core.CardGame;
using UnityEngine;

namespace HokmGame.Game
{
    public class PlayerManager : MonoBehaviour
    {
        public Card selectedCard;
        public bool myTurn = false;
        private List<Card> currentPlayedCards;
        private Suit currentTrumpSuit;
        public RealPlayer realPlayer;

        void Start()
        {

        }


        void Update()
        {
            if (myTurn & selectedCard == null)
            {
                selectedCard = ChooseCard();
            }
        }

        public void ResetSelectedCard()
        {
            selectedCard = null;
            myTurn = true;
        }

        public void StopSearchingForCard()
        {
            selectedCard = null;
            myTurn = false;
        }

        public Card ChooseCard()
        {
            if (Input.GetMouseButtonDown(0))
            {
                Vector2 cubeRay = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                RaycastHit2D hit = Physics2D.Raycast(cubeRay, Vector2.zero);

                if (hit)
                {
                    if (hit.collider != null)
                    {
                        var cardData = hit.collider.GetComponent<CardData>();
                        if (cardData != null)
                        {
                            if (CanPlayCard(cardData.Card, currentPlayedCards, currentTrumpSuit))
                            {
                                realPlayer.playNextMove = true;
                                return cardData.Card;
                            }

                        }

                    }
                }
            }
            return null;
        }

        internal bool CanPlayCard(Card card, IEnumerable<Card> playedByOthers, Suit trumpSuit)
        {
            var local = playedByOthers.ToArray();

            //we are the first player of the round
            if (local.Length == 0)
                return true;

            //player has atleast one card of the same suit, but has not chosen it
            var sameSuit = realPlayer.Cards.FirstOrDefault(x => x.Suit == local[0].Suit);
            if (sameSuit != null && card.Suit != sameSuit.Suit)
                return false;

            //playing a trump card
            if (card.Suit == trumpSuit)
                return true;

            return true;
        }

        internal void SelectCard(IEnumerable<Card> playedByOthers, Suit trumpSuit)
        {
            currentPlayedCards = playedByOthers.ToList();
            currentTrumpSuit = trumpSuit;
        }
    }
}
