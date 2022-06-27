using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HokmGame.Core.CardGame;
using HokmGame.Core.Hokm;
using HokmGame.Core.Hokm.Info;
using HokmGame.Core.Hokm.Score;
using UnityEngine;

namespace HokmGame.Game
{
    public class NetworkPlayer : IPlayer
    {
        private readonly CardManager cardManager;
        private bool playNextMove;
        private Card cardToPlay;

        public NetworkPlayer(string name, CardManager cardManager)
        {
            Name = name;
            Cards = new List<Card>();
            this.cardManager = cardManager;
        }

        public Guid Id => Guid.NewGuid();
        public string Name { get; private set; }
        public List<Card> Cards { get; private set; }
        public TrickOutcome LastOutcome { get; private set; }
        public PlayerPosition MyPosition { get; set; }
        public int IterationOverCards { get; set; } = 50;

        public Task<string> BanterAsync()
        {
            return Task.FromResult<string>("Wat??");
        }

        public Task<Suit> CallTrumpSuitAsync()
        {
            return Task.FromResult(Cards.First().Suit);
        }

        public Task<string> GameFinished(GameOutcome outcome, GameScore score)
        {
            Debug.Log("Game finished");
            return Task.FromResult<string>("Game Finished?");
        }

        public Task<string> InformTrickOutcomeAsync(TrickOutcome outcome)
        {
            LastOutcome = outcome;
            return Task.FromResult<string>("Wat??");
        }

        public Task MatchFinished(MatchScore score)
        {
            Debug.Log("Match finished");
            return Task.CompletedTask;
        }

        public Task NewGameAsync(MatchScore currentMatchScore, PlayerPosition caller)
        {
            Cards = new List<Card>();
            return Task.CompletedTask;
        }

        //inform about new match
        public Task NewMatchAsync(IDictionary<PlayerPosition, IPlayerInfo> playerInfos, PlayerPosition yourPosition)
        {
            MyPosition = yourPosition;
            return Task.CompletedTask;
        }

        public async Task<Card> PlayAsync(int trickNumber, IEnumerable<Card> playedByOthers, Suit trumpSuit, IEnumerable<Card> cardsPlayedSoFar)
        {
            playNextMove = false;
            while (!playNextMove)
            {
                await Task.Delay(1);
            }
            var selectedCard = cardToPlay;
            Cards.Remove(selectedCard);
            return selectedCard;
        }

        private bool CanTeamWintrick(List<Card> cardsInTrick, Card myCard, Card teammateCard, Suit trumpSuit)
        {
            var winningCard = cardsInTrick[DecideWinnerCard(cardsInTrick, trumpSuit)];
            if (winningCard == teammateCard || winningCard == myCard)
                return true;
            else
                return false;
        }

        private bool CanCardWinTrick(List<Card> cardsInTrick, Card card, Suit trumpSuit)
        {
            var winningCard = cardsInTrick[DecideWinnerCard(cardsInTrick, trumpSuit)];
            return winningCard == card;
        }

        public static int DecideWinnerCard(List<Card> cards, Suit trumpSuit)
        {
            var copy = cards.ToList();
            var startingSuit = copy[0].Suit;
            var (cardValue, index) = copy.Select(
                (card, index) => (GetCardValue(card, startingSuit, trumpSuit), index)).Max();
            return index;
        }

        private static int GetCardValue(Card card, Suit startingSuit, Suit trumpSuit)
        {
            var value = ((int)card.Rank + 11) % 13;
            if (card.Suit == trumpSuit)
                value += 100;
            if (card.Suit != startingSuit && card.Suit != trumpSuit)
                return 0;
            else return value;
        }

        public Task ReceiveHandAsync(IEnumerable<Card> cards)
        {
            Cards.AddRange(cards);

            if (Cards.Count != 5 && Cards.Count != 9 && Cards.Count != 13)
                throw new InvalidOperationException($"The puppet has {Cards.Count}!");

            if (Cards.Count == 13)
                cardManager.SpawnDeck(Cards, MyPosition);
            return Task.CompletedTask;
        }

        public void PlayCard(Card card)
        {
            cardToPlay = card;
            playNextMove = true;
        }
    }
}
