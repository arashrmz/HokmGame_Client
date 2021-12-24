using System.Collections;
using System.Collections.Generic;
using HokmGame.Core.CardGame;
using HokmGame.Core.Hokm;
using UnityEngine;

namespace HokmGame.Game
{
    public class CardData : MonoBehaviour
    {
        public Card Card { get; set; }
        public PlayerPosition playerPosition { get; set; }

        public void SetCard(Card card, PlayerPosition playerPosition)
        {
            this.Card = card;
            this.playerPosition = playerPosition;
        }
    }
}
