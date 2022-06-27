namespace HokmGame.Core.CardGame
{
    [System.Serializable]
    public class Card
    {
        public Card(Suit suit, Rank rank)
        {
            Suit = suit;
            Rank = rank;
        }

        public Suit Suit { get; private set; }
        public Rank Rank { get; private set; }

        public override string ToString()
        {
            return string.Format("{0} of {1}", RankInfo.GetRankName(Rank), SuitInfo.GetSuitName(Suit));
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            Card other = obj as Card;
            if (other == null)
                return false;

            return Suit == other.Suit && Rank == other.Rank;
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        public static Card FromString(string cardString)
        {
            if (cardString == null)
                return null;

            string[] cardParts = cardString.Split("of".ToCharArray());
            if (cardParts.Length != 2)
                return null;

            Suit? suit = SuitInfo.FromString(cardParts[1]);
            Rank? rank = RankInfo.FromString(cardParts[0]);

            if (!rank.HasValue || !suit.HasValue)
                return null;

            return new Card(suit.Value, rank.Value);
        }
    }
}

