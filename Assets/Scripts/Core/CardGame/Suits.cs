namespace HokmGame.Core.CardGame
{
    [System.Serializable]
    public enum Suit
    {
        Club,
        Diamond,
        Heart,
        Spade
    }

    public static class SuitInfo
    {
        private static readonly string[] _suitNames = new string[]
        {
            "Club",
            "Diamond",
            "Heart",
            "Spade"
        };

        public static string GetSuitName(Suit suit)
        {
            return _suitNames[(int)suit];
        }

        public static Suit? FromString(string name)
        {
            return name switch
            {
                "Club" => Suit.Club,
                "Diamond" => Suit.Diamond,
                "Heart" => Suit.Heart,
                "Spade" => Suit.Spade,
                _ => null
            };
        }
    }
}