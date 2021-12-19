using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using HokmGame.Core.CardGame;
using HokmGame.Core.Hokm;
using HokmGame.Core.Hokm.Info;
using HokmGame.Core.Hokm.Score;
using UnityEngine;

namespace HokmGame.Game
{
    public class RealPlayer : MonoBehaviour, IPlayer
    {
        public Guid Id => throw new NotImplementedException();

        public string Name => throw new NotImplementedException();

        public Task<string> BanterAsync()
        {
            throw new NotImplementedException();
        }

        public Task<Suit> CallTrumpSuitAsync()
        {
            throw new NotImplementedException();
        }

        public Task<string> GameFinished(GameOutcome outcome, GameScore score)
        {
            throw new NotImplementedException();
        }

        public Task<string> InformTrickOutcomeAsync(TrickOutcome outcome)
        {
            throw new NotImplementedException();
        }

        public Task MatchFinished(MatchScore score)
        {
            throw new NotImplementedException();
        }

        public Task NewGameAsync(MatchScore currentMatchScore, PlayerPosition caller)
        {
            throw new NotImplementedException();
        }

        public Task NewMatchAsync(IDictionary<PlayerPosition, IPlayerInfo> playerInfos, PlayerPosition yourPosition)
        {
            throw new NotImplementedException();
        }

        public Task<Card> PlayAsync(int trickNumber, IEnumerable<Card> playedByOthers, Suit trumpSuit)
        {
            throw new NotImplementedException();
        }

        public Task ReceiveHandAsync(IEnumerable<Card> cards)
        {
            throw new NotImplementedException();
        }
    }
}
