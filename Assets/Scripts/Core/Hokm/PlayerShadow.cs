using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HokmGame.Core.CardGame;

namespace HokmGame.Core.Hokm
{
    public class PlayerShadow
    {
        internal HashSet<Card> _handCards;
        private HashSet<Card> _playedCards;

        public PlayerShadow()
        {
            _handCards = new HashSet<Card>();
            _playedCards = new HashSet<Card>();
        }

        public void ReceiveHand(IEnumerable<Card> cards)
        {
            foreach (var card in cards)
            {
                if (_handCards.Contains(card))
                    throw new InvalidOperationException($"Card {card} is already in hand");
                _handCards.Add(card);
            }

            if (_handCards.Count != 5 && _handCards.Count != 9 && _handCards.Count != 13)
                throw new InvalidOperationException($"The shadow has {_handCards.Count}");
        }

        public ValidationResult ValidateAndPlay(Card card, Suit playedSuit)
        {
            if (!_handCards.Contains(card))
                throw new InvalidOperationException($"Card {card} is not in hand");

            if (_playedCards.Contains(card))
                throw new InvalidOperationException($"Card {card} is already played");

            if (playedSuit != card.Suit && _handCards.Any(c => c.Suit == playedSuit))
                return ValidationResult.ErrorResult($"You must play a {playedSuit} card, you have them");

            _playedCards.Add(card);
            _handCards.Remove(card);

            return ValidationResult.Valid;
        }
    }


}