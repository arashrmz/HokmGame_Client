namespace HokmGame.Core.CardGame
{
    public enum Rank
    {
        Ace = 1,
        Two = 2,
        Three = 3,
        Four = 4,
        Five = 5,
        Six = 6,
        Seven = 7,
        Eight = 8,
        Nine = 9,
        Ten = 10,
        Jack = 11,
        Queen = 12,
        King = 13
    }

    public static class RankInfo
    {
        private static readonly string[] _rankNames = new string[]
        {
            "A",
            "2",
            "3",
            "4",
            "5",
            "6",
            "7",
            "8",
            "9",
            "10",
            "J",
            "Q",
            "K"
        };

        public static string GetRankName(Rank rank)
        {
            return _rankNames[(int)rank - 1];
        }

        public static Rank? FromString(string rankName)
        {
            for (int i = 0; i < _rankNames.Length; i++)
            {
                if (_rankNames[i] == rankName)
                {
                    return (Rank)(i + 1);
                }
            }

            return null;
        }
    }
}