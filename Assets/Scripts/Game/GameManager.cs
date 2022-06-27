using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using HokmGame.Core.CardGame;
using HokmGame.Core.Hokm;
using HokmGame.Core.Hokm.Events;
using HokmGame.Game;
using HokmGame.Util;
using UnityEngine;
using EventType = HokmGame.Core.Hokm.Events.EventType;
using System.Linq;

namespace HokmGame.Game
{
    public class GameManager : MonoBehaviour
    {
        public int bestOf = 7;
        private Match currentMatch;
        private UIManager uiManager;
        private RealPlayer player;
        private CardManager cardManager;

        [Header("Time")]
        [SerializeField] private float timeBetweenMove = 500f;
        [SerializeField] private float timeBetweenGame = 2000f;

        [Header("Player")]
        [SerializeField] private PlayerManager playerManager;

        async Task Start()
        {
            uiManager = GetComponent<UIManager>();
            cardManager = GetComponent<CardManager>();

            try
            {
                currentMatch = CreateMatch();
                player = (RealPlayer)currentMatch.Team1.Player1;
                await currentMatch.StartAsync();
                currentMatch.MatchEvent += OnMatchEvent;
                while (!currentMatch.Score.IsCompleted)
                {
                    await currentMatch.CreateAndRunGameAsync(CancellationToken.None,
                        TimeSpan.FromMilliseconds(timeBetweenGame),
                        TimeSpan.FromMilliseconds(timeBetweenMove));
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError(e);
                throw;
            }

        }

        //creates and returns a new match
        Match CreateMatch()
        {
            var team1 = CreateTeam(true, "Team 1");
            var team2 = CreateTeam(false, "Team 2");
            return new Match(team1, team2, bestOf);
        }

        //creates a team with two players
        Team CreateTeam(bool isRealPlayer, string name)
        {
            var team = new Team();
            team.Name = name;
            if (isRealPlayer)
                team.Player1 = new RealPlayer("player", GetComponent<CardManager>(), playerManager);
            else
                team.Player1 = new AIPlayer("player", GetComponent<CardManager>());
            team.Player2 = new AIPlayer("player", GetComponent<CardManager>());
            return team;
        }


        private void OnMatchEvent(object sender, MatchEventArgs e)
        {
            switch (e.EventType)
            {
                case EventType.GameStarted:
                    break;
                case EventType.GameFinished:
                    OnGameFinished(e);
                    break;
                case EventType.CardsDealt:
                    var cardsDealtEventArgs = e.OriginalEventArgs as CardsDealtEventArgs;
                    uiManager.ShowTrumpSuit(cardsDealtEventArgs);
                    break;
                case EventType.CardPlayed:
                    OnCardPlayed(e);
                    break;
                case EventType.TrickStarted:
                    cardManager.ClearCenterCards();
                    break;
                case EventType.TrickFinished:
                    OnTrickFinished(e);
                    break;
                default:
                    break;
            }
        }

        private void OnCardPlayed(MatchEventArgs e)
        {
            var cardPlayedEventArgs = e.OriginalEventArgs as CardPlayedEventArgs;
            var cards = cardPlayedEventArgs.Cards.ToList();
            cardManager.SpawnCenterCard(cards.Last(), cardPlayedEventArgs.PlayerPlayingTheCard);
            cardManager.RemoveCardFromDeck(cards.Last(), cardPlayedEventArgs.PlayerPlayingTheCard);
        }

        private void OnGameFinished(MatchEventArgs e)
        {
            uiManager.UpdateGameScores(currentMatch.Score);
            cardManager.RemoveDecks();
        }

        private void OnTrickFinished(MatchEventArgs e)
        {
            var trickFinishedEventArgs = e.OriginalEventArgs as TrickFinishedEventArgs;
            var currentScore = currentMatch.ToInfo().CurrentGame.Score;
            uiManager.UpdateTrickScores(currentScore, trickFinishedEventArgs.Outcome);
        }
    }
}
