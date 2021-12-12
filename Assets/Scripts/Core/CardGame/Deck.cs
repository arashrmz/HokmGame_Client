using System;
using System.Collections.Generic;
using System.Linq;

namespace HokmGame.Core.CardGame
{
    public class Deck
    {
        protected Stack<Card> cards;
        private Func<IEnumerable<Card>, IEnumerable<Card>> _shuffler;


        public Deck(Func<IEnumerable<Card>, IEnumerable<Card>> shuffler = null)
        {
            _shuffler = shuffler ?? DefaultShuffler;
            cards = new Stack<Card>();

            for (int i = 0; i < 4; i++)
            {
                var suit = (Suit)i;
                for (int j = 0; j < 13; j++)
                {
                    var rank = (Rank)j;
                    cards.Push(new Card(suit, rank));
                }
            }
        }

        public Card Peek()
        {
            return cards.Peek();
        }

        public Deck Shuffle()
        {
            cards = new Stack<Card>(_shuffler(cards));
            return this;
        }

        private IEnumerable<Card> DefaultShuffler(IEnumerable<Card> cards)
        {
            return cards.OrderBy(c => Guid.NewGuid());
        }

        public IEnumerable<Card> Deal(int count)
        {
            if (count > cards.Count)
                throw new IndexOutOfRangeException("Cannot deal more cards than are in the deck");

            var result = cards.Take(count);
            cards = new Stack<Card>(cards.Skip(count));
            return result;
        }
    }
}
