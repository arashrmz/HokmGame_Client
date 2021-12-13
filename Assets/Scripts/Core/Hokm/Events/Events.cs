using System;
using System.Collections.Generic;
using HokmGame.Core.CardGame;
using HokmGame.Core.Hokm.Info;

namespace HokmGame.Core.Hokm.Events
{
    public class TrickFinishedEventArgs : EventArgs
    {
        public TrickOutcome Outcome { get; set; }
    }

    public class BanterUttered : EventArgs
    {
        public IPlayerInfo PlayerInfo { get; set; }
        public string Banter { get; set; }
    }

    public class CardPlayedEventArgs : EventArgs
    {
        public IEnumerable<Card> Cards { get; set; }
        public PlayerPosition StarterPlayer { get; set; }
        public Suit TrumpSuit { get; set; }
        public int TrickNumber { get; set; }
        public int GameNumber { get; set; }
        public PlayerPosition PlayerPlayingTheCard { get; set; }
    }

    public class GameStartedEventArgs : EventArgs
    {
        public Game Game { get; set; }
    }

    public class GameFinishedEventArgs : EventArgs
    {
        public Game Game { get; set; }
    }

    public class CardsDealtEventArgs : EventArgs
    {
        public IDictionary<PlayerPosition, IEnumerable<Card>> Hands { get; set; }
        public Suit TrumpSuit { get; set; }
    }

    public class MatchEventArgs : EventArgs
    {
        public MatchInfo Info { get; set; }
        public EventType EventType { get; set; }
        public EventArgs OriginalEventArgs { get; set; }
    }
}