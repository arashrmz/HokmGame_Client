using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HokmGame.Core.CardGame;
using System;

namespace HokmGame.Game
{
    //holds the cards sprites
    [CreateAssetMenu(fileName = "GameCardData", menuName = "Hokm/GameCardData", order = 0)]
    public class GameCardData : ScriptableObject
    {
        [SerializeField]
        private CardsInfo[] cardsInfos;

        public Sprite GetCardSprite(Card card)
        {
            return cardsInfos[(int)card.Suit].sprites[(int)card.Rank - 1];
        }
    }

    [Serializable]
    public class CardsInfo
    {
        public Suit suit;
        public Sprite[] sprites;
    }
}
