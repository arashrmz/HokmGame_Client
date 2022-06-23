using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HokmGame.Core.Hokm.Score
{
    public class MatchScore
    {
        //minimum number of tricks to win a game
        private int _limit;

        //total number of tricks
        public int BestOf { get; set; }
        public Dictionary<PlayerPosition, int> TricksWon { get; }
        public int Team1Points { get; private set; }
        public int Team2Points { get; private set; }

        public MatchScore(int bestOf = 7)
        {
            BestOf = bestOf;
            if (bestOf % 2 == 0)
            {
                throw new ArgumentException("Best of must be even");
            }
            _limit = (bestOf + 1) / 2;
            TricksWon = PlayerPositions.All.ToDictionary(p => p, s => 0);
        }

        public bool IsCompleted => Team1Points >= _limit || Team2Points >= _limit;

        public void RegisterGameWin(GameScore score, PlayerPosition caller)
        {
            var point = 1;

            //if the losing team didnt win any trick in the last game, winner gets extra point
            if (score.TricksWonByTeam1 == 0 || score.TricksWonByTeam2 == 0)
            {
                point++;
            }

            var team1Won = score.TricksWonByTeam1 > score.TricksWonByTeam2;
            var team1WasCaller = PlayerPositions.IsTeam1(caller);

            //if winner won 7-0 && called trump card, winner gets another extra point
            if (point > 1 && (team1Won ^ team1WasCaller))
                point++;

            if (team1Won)
                Team1Points += point;
            else
                Team2Points += point;
        }

    }
}