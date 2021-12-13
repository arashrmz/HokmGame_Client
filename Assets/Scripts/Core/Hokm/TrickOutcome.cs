using System.Collections.Generic;
using HokmGame.Core.CardGame;

namespace HokmGame.Core.Hokm
{
    public class TrickOutcome
    {
        public TrumpUsage TrumpUsage { get; set; }
        public PlayerPosition Winner { get; set; }
        public IEnumerable<Card> CardsPlayed { get; set; }
    }

    public class TrickReportForPlayer
    {
        public bool YourTeamWon { get; set; }
    }
}