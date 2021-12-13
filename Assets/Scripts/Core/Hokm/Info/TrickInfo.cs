using System.Collections.Generic;
using HokmGame.Core.CardGame;

namespace HokmGame.Core.Hokm.Info
{
    public class TrickInfo
    {
        public PlayerPosition Starter { get; set; }
        public IEnumerable<Card> CardsPlayed { get; set; }
        public PlayerPosition? CurrentWinningPosition { get; set; }
        public int TrickNumber { get; set; }
    }
}