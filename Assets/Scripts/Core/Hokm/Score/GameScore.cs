using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HokmGame.Core.Hokm.Score
{
    public class GameScore
    {
        public Dictionary<PlayerPosition, int> TricksWon { get; }

        public GameScore()
        {
            TricksWon = PlayerPositions.All.ToDictionary(p => p, s => 0);
        }

        public void RegisterWin(PlayerPosition position)
        {
            TricksWon[position]++;
        }

        public int TricksWonByTeam1 => TricksWon[PlayerPosition.Team1Player1] + TricksWon[PlayerPosition.Team1Player2];
        public int TricksWonByTeam2 => TricksWon[PlayerPosition.Team2Player1] + TricksWon[PlayerPosition.Team2Player2];
        public bool IsGameOver => TricksWonByTeam1 >= 7 || TricksWonByTeam2 >= 7;

    }
}