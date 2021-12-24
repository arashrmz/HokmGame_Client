using System;
using System.Collections;
using System.Collections.Generic;
using HokmGame.Core.CardGame;
using HokmGame.Core.Hokm;
using UnityEngine;

namespace HokmGame.Game
{
    public class CardManager : MonoBehaviour
    {
        [Header("Card Manager")]
        public GameCardData gameCardData;
        public Transform[] cardSpawnPositions;
        public GameObject cardPrefab;
        public GameObject cardBackPrefab;
        public float cardDistanceOffset = 0.1f;
        private Dictionary<PlayerPosition, List<GameObject>> _hands = new Dictionary<PlayerPosition, List<GameObject>>();

        public Transform[] centerCardSpawnPositions;
        List<GameObject> centerCards = new List<GameObject>();

        public void SpawnDeck(IEnumerable<Card> cards, PlayerPosition playerPosition)
        {
            _hands[playerPosition] = new List<GameObject>();
            var position = cardSpawnPositions[(int)playerPosition].position;
            int i = 0;
            foreach (var card in cards)
            {
                var cardObject = Instantiate(cardPrefab, position + new Vector3(cardDistanceOffset * i, 0f, -i), Quaternion.identity, cardSpawnPositions[(int)playerPosition]);
                cardObject.GetComponent<SpriteRenderer>().sprite = gameCardData.GetCardSprite(card);
                _hands[playerPosition].Add(cardObject);
                cardObject.AddComponent<CardData>();
                cardObject.GetComponent<CardData>().SetCard(card, playerPosition);
                i++;
            }

            if (playerPosition == PlayerPosition.Team2Player1)
            {
                cardSpawnPositions[(int)playerPosition].Rotate(0f, 0f, 90f);
            }
            else if (playerPosition == PlayerPosition.Team2Player2)
            {
                cardSpawnPositions[(int)playerPosition].Rotate(0f, 0f, -90f);
            }
        }

        public void RemoveDecks()
        {
            foreach (var k in _hands.Keys)
            {
                foreach (var v in _hands[k])
                {
                    Destroy(v);
                }
            }
        }

        public void SpawnCenterCard(Card card, PlayerPosition playerPosition)
        {
            var position = centerCardSpawnPositions[(int)playerPosition].position;
            var cardObject = Instantiate(cardPrefab, position, Quaternion.identity);
            cardObject.GetComponent<SpriteRenderer>().sprite = gameCardData.GetCardSprite(card);
            centerCards.Add(cardObject);
        }

        internal void ClearCenterCards()
        {
            foreach (var card in centerCards)
            {
                Destroy(card);
            }
            centerCards.Clear();
        }

        //remove the played card from player's hand
        internal void RemoveCardFromDeck(Card card, PlayerPosition playerPlayingTheCard)
        {
            var cardObject = _hands[playerPlayingTheCard].Find(x => x.GetComponent<CardData>().Card == card);
            _hands[playerPlayingTheCard].Remove(cardObject);
            Destroy(cardObject);
        }
    }
}
