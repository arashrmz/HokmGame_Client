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
    public class RealPlayer : IPlayer
    {
        private readonly CardManager cardManager;
        private readonly PlayerManager playerManager;
        public bool playNextMove;
        private List<Card> currentPlayedDeck;

        public RealPlayer(string name, CardManager cardManager, PlayerManager playerManager)
        {
            Name = name;
            Cards = new List<Card>();
            this.cardManager = cardManager;
            this.playerManager = playerManager;
            playerManager.realPlayer = this;
        }

        public Guid Id => Guid.NewGuid();
        public string Name { get; private set; }
        public List<Card> Cards { get; private set; }
        public TrickOutcome LastOutcome { get; private set; }
        public PlayerPosition MyPosition { get; set; }

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

        public Task NewMatchAsync(IDictionary<PlayerPosition, IPlayerInfo> playerInfos, PlayerPosition yourPosition)
        {
            MyPosition = yourPosition;
            return Task.CompletedTask;
        }

        public async Task<Card> PlayAsync(int trickNumber, IEnumerable<Card> playedByOthers, Suit trumpSuit)
        {
            Debug.LogWarning("Your turn");
            playerManager.ResetSelectedCard();
            playNextMove = false;
            playerManager.SelectCard(playedByOthers, trumpSuit);
            while (!playNextMove)
            {
                await Task.Delay(1);
            }
            var selectedCard = playerManager.selectedCard;
            return selectedCard;
        }

        internal bool CanPlayCard(Card card, IEnumerable<Card> playedByOthers, Suit trumpSuit)
        {
            var local = playedByOthers.ToArray();

            //we are the first player of the round
            if (local.Length == 0)
                return true;

            //playing a trump card
            if (card.Suit == trumpSuit)
                return true;

            //player has atleast one card of the same suit, but has not chosen it
            var sameSuit = Cards.FirstOrDefault(x => x.Suit == local[0].Suit);
            if (sameSuit != null && card.Suit != sameSuit.Suit)
                return false;

            return true;
        }

        private Task<Card> FindACardAsync(int trickNumber, IEnumerable<Card> playedByOthers, Suit trumpSuit)
        {
            var local = playedByOthers.ToArray();

            if (local.Length == 0)
            {
                //we are the first player of the round
                return Task.FromResult(Cards.OrderBy(x => Guid.NewGuid()).First());
            }

            //get the first card with the same suit as round
            var sameSuit = Cards.FirstOrDefault(x => x.Suit == local[0].Suit);
            if (sameSuit != null)
                return Task.FromResult<Card>(sameSuit);

            //get the first card with the same rank as trump suit
            var trump = Cards.FirstOrDefault(x => x.Suit == trumpSuit);
            if (trump != null)
                return Task.FromResult<Card>(trump);

            //no card found, get the first card
            return Task.FromResult(Cards.First());
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
    }
}
