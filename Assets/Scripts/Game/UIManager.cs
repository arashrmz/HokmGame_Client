using System.Collections;
using System.Collections.Generic;
using HokmGame.Core.CardGame;
using HokmGame.Core.Hokm.Events;
using UnityEngine;
using TMPro;
using HokmGame.Core.Hokm.Score;
using HokmGame.Core.Hokm;

namespace HokmGame.Game
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI trumpSuitText;
        [SerializeField] private TextMeshProUGUI tricksScoreboard;
        [SerializeField] private TextMeshProUGUI gamesScoreboard;

        public void ShowTrumpSuit(CardsDealtEventArgs e)
        {
            trumpSuitText.text = $"Trump Suit: {e.TrumpSuit}";
        }

        public void UpdateTrickScores(GameScore score, TrickOutcome outcome)
        {
            var team1Score = score.TricksWonByTeam1;
            var team2Score = score.TricksWonByTeam2;
            if (outcome.Winner == PlayerPosition.Team1Player1 || outcome.Winner == PlayerPosition.Team1Player2)
            {
                team1Score++;
            }
            else
            {
                team2Score++;
            }
            tricksScoreboard.text = $"Tricks\nTeam 1: {team1Score}\nTeam 2: {team2Score}";
        }

        public void UpdateGameScores(MatchScore score)
        {
            var team1Score = score.Team1Points;
            var team2Score = score.Team2Points;
            gamesScoreboard.text = $"Games\nTeam 1: {team1Score}\nTeam 2: {team2Score}";
        }
    }
}
