using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HokmGame.Core.CardGame;
using HokmGame.Core.Hokm;
using HokmGame.Core.Hokm.Info;
using HokmGame.Core.Hokm.Score;
using HokmGame.Util;
using UnityEngine;

namespace HokmGame.Game
{
    public class AIPlayer : IPlayer
    {
        private readonly CardManager cardManager;

        public AIPlayer(string name, CardManager cardManager)
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
            var chosenCard = await FindACardAsync(trickNumber, playedByOthers, trumpSuit, cardsPlayedSoFar);
            Cards.Remove(chosenCard);
            return chosenCard;
        }

        private Task<Card> FindACardAsync(int trickNumber, IEnumerable<Card> playedByOthers, Suit trumpSuit, IEnumerable<Card> cardsPlayedSoFar)
        {
            //cards that other players have played in this trick
            var cardsInTrick = playedByOthers.ToList();
            //cards that player holds
            var cardsInHand = Cards.ToList();
            //cards that are out(been played before in current game)
            var cardsOut = cardsPlayedSoFar.ToList();

            //we are the first player of the round
            if (cardsInTrick.Count == 0)
            {
                //choose the highest card
                return Task.FromResult(Cards.OrderByDescending(x => x.Rank).OrderBy(x => x.Rank == Rank.Ace ? 0 : 1).First());
            }
            else if (cardsInTrick.Count == 3)
            {
                //we are the last to play in this trick
                //check if teammate is winning the trick
                var teammateCard = cardsInTrick[1];
                var winningCard = cardsInTrick[DecideWinnerCard(cardsInTrick, trumpSuit)];
                if (winningCard == teammateCard)
                {
                    //choose lowest card from the suit
                    var cards = from card in Cards select card;
                    //choose lowest card from same suit
                    var lowestCard = cards.Where(x => x.Suit == cardsInTrick[0].Suit).OrderBy(x => x.Rank).OrderBy(x => x.Rank == Rank.Ace ? 1 : 0).FirstOrDefault();
                    if (lowestCard != null)
                        return Task.FromResult(lowestCard);

                    //choose lowest non trump card
                    lowestCard = cards.Where(x => x.Suit != trumpSuit).OrderBy(x => x.Rank).OrderBy(x => x.Rank == Rank.Ace ? 1 : 0).FirstOrDefault();
                    if (lowestCard != null)
                        return Task.FromResult(lowestCard);

                    //choose lowest trump card
                    lowestCard = cards.OrderBy(x => x.Rank).OrderBy(x => x.Rank == Rank.Ace ? 1 : 0).FirstOrDefault();
                    return Task.FromResult(lowestCard);
                }
                else
                {
                    //choose lowest card from the suit to win the trick
                    var cards = from card in Cards select card;
                    //choose lowest card from same suit
                    var lowestCard = cards.Where(x => x.Suit == cardsInTrick[0].Suit).FirstOrDefault();

                    //check if has card from same suit
                    if (lowestCard != null)
                    {
                        lowestCard = cards.Where(x => x.Suit == cardsInTrick[0].Suit)
                            .Where(x => CanCardWinTrick(cardsInTrick, x, trumpSuit))
                            .OrderBy(x => x.Rank)
                            .OrderBy(x => x.Rank == Rank.Ace ? 1 : 0).FirstOrDefault();
                        if (lowestCard != null)
                            return Task.FromResult(lowestCard);

                        //can not win
                        lowestCard = cards.Where(x => x.Suit == cardsInTrick[0].Suit)
                           .OrderBy(x => x.Rank)
                            .OrderBy(x => x.Rank == Rank.Ace ? 1 : 0).FirstOrDefault();
                        return Task.FromResult(lowestCard);
                    }
                    else
                    {
                        //choose best trump card to win the trick
                        lowestCard = cards.Where(x => x.Suit == trumpSuit)
                            .Where(x => CanCardWinTrick(cardsInTrick, x, trumpSuit))
                            .OrderBy(x => x.Rank)
                            .OrderBy(x => x.Rank == Rank.Ace ? 1 : 0).FirstOrDefault();
                        if (lowestCard != null)
                            return Task.FromResult(lowestCard);

                        //choose lowest card to lose the trick
                        lowestCard = cards.OrderBy(x => x.Rank)
                        .OrderBy(x => x.Suit == trumpSuit ? 1 : 0)
                        .OrderBy(x => x.Rank == Rank.Ace ? 1 : 0)
                        .FirstOrDefault();
                        return Task.FromResult(lowestCard);
                    }
                }
            }
            else
            {
                //randomly deal cards to other remaining players
                int remainingPlayersCount = 3 - cardsInTrick.Count;

                var fullDeck = new Deck().Shuffle().Deal(52);
                var remainingCards = fullDeck.Where(x => !cardsInHand.Contains(x)
                && !cardsOut.Contains(x) && !cardsInTrick.Contains(x));
                int count = remainingCards.Count();

                //get cards in player's hand that is playable
                var playableCards = cardsInHand.Where(x => x.Suit == cardsInTrick[0].Suit);
                if (playableCards.Count() <= 0)
                {
                    playableCards = cardsInHand.Take(cardsInHand.Count());
                }

                //to keep score of cards in hand
                Dictionary<Card, int> playableCardsScore = new Dictionary<Card, int>();
                foreach (var x in playableCards)
                {
                    playableCardsScore.Add(x, 0);
                }
                //randomly deal cards to other remaining players n times
                for (int i = 0; i < IterationOverCards; i++)
                {
                    var cards = remainingCards.Take(count).Shuffle();
                    //deal remaning cards to remaining players
                    if (remainingPlayersCount == 1)
                    {
                        foreach (var x in cards)
                        {
                            foreach (var playableCard in playableCards)
                            {
                                cardsInTrick.Add(playableCard);
                                cardsInTrick.Add(x);
                                if (CanTeamWintrick(cardsInTrick, playableCard, cardsInTrick[0], trumpSuit))
                                {
                                    playableCardsScore[playableCard]++;
                                }
                                cardsInTrick.RemoveAt(cardsInTrick.Count - 1);
                                cardsInTrick.RemoveAt(cardsInTrick.Count - 1);
                            }
                        }
                    }
                    else
                    {
                        for (int j = 0; j < cards.Count() - 1; j += 2)
                            foreach (var playableCard in playableCards)
                            {
                                cardsInTrick.Add(playableCard);
                                cardsInTrick.Add(cards.ElementAt(j));
                                cardsInTrick.Add(cards.ElementAt(j + 1));
                                if (CanTeamWintrick(cardsInTrick, playableCard, cardsInTrick[3], trumpSuit))
                                {
                                    playableCardsScore[playableCard]++;
                                }
                                cardsInTrick.RemoveAt(cardsInTrick.Count - 1);
                                cardsInTrick.RemoveAt(cardsInTrick.Count - 1);
                                cardsInTrick.RemoveAt(cardsInTrick.Count - 1);
                            }
                    }
                }
                var card = playableCardsScore.OrderBy(x => x.Key.Rank).OrderByDescending(x => x.Value).First().Key;
                return Task.FromResult(card);
            }
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
    }
}
