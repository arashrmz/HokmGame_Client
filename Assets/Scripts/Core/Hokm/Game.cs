using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HokmGame.Core.CardGame;
using HokmGame.Core.Hokm.Events;
using HokmGame.Core.Hokm.Info;
using HokmGame.Core.Hokm.Score;

namespace HokmGame.Core.Hokm
{
    // A round of hokm game
    public class Game
    {
        public PlayerPosition TrumpCaller { get; }
        private Suit _trumpSuit;
        private int _currentTrickNumber = 0;
        private PlayerPosition _currentTrickStarter;
        private Dictionary<PlayerPosition, PlayerShadow> _playerShadows; //init in ctor
        private readonly MatchScore _matchScore;
        private Func<IEnumerable<Card>, IEnumerable<Card>> _shuffler;

        //events
        public event EventHandler<TrickFinishedEventArgs> TrickFinished;
        public event EventHandler<BanterUtteredEventArgs> BanterUttered;
        public event EventHandler<GameFinishedEventArgs> GameFinished;
        public event EventHandler<CardPlayedEventArgs> CardPlayed;
        public event EventHandler<CardsDealtEventArgs> CardsDealt;
        public event EventHandler<EventArgs> TrickStarted;

        public Team Team1 { get; set; }
        public Team Team2 { get; set; }
        public int GameNumber { get; set; }
        public GameScore Score { get; } //init in ctor
        public TrickInfo CurrentTrick { get; set; }

        public Game(int gameNumber, MatchScore matchScore, Team team1, Team team2, PlayerPosition trumpCaller,
        Func<IEnumerable<Card>, IEnumerable<Card>> shuffler)
        {
            _matchScore = matchScore;
            _shuffler = shuffler;
            Team1 = team1;
            Team2 = team2;
            TrumpCaller = trumpCaller;
            _currentTrickStarter = trumpCaller;
            GameNumber = gameNumber;

            //init variables
            _playerShadows = PlayerPositions.All.ToDictionary(p => p, s => new PlayerShadow());
            Score = new GameScore();
        }

        internal static List<PlayerPosition> BuildPlayingOrder(PlayerPosition startingPosition)
        {
            return Enumerable.Range(0, 4).Select(x => (x + (int)startingPosition) % 4).Cast<PlayerPosition>().ToList();
        }

        internal IPlayer GetPlayer(PlayerPosition position)
        {
            return position switch
            {
                PlayerPosition.Team1Player1 => Team1.Player1,
                PlayerPosition.Team1Player2 => Team1.Player2,
                PlayerPosition.Team2Player1 => Team2.Player1,
                PlayerPosition.Team2Player2 => Team2.Player2,
                _ => throw new ArgumentException($"Invalid player position : {position}")
            };
        }

        protected void OnTrickFinished(TrickFinishedEventArgs args)
        {
            TrickFinished?.Invoke(this, args);

            Score.RegisterWin(args.Outcome.Winner);
            if (Score.IsGameOver)
                OnGameFinished(new GameFinishedEventArgs() { Game = this });
        }

        protected void OnGameFinished(GameFinishedEventArgs args)
        {
            GameFinished?.Invoke(this, args);
        }

        protected void OnBanterUttered(BanterUtteredEventArgs args)
        {
            BanterUttered?.Invoke(this, args);
        }

        protected void OnCardPlayed(CardPlayedEventArgs args)
        {
            CardPlayed?.Invoke(this, args);
        }

        protected void OnCardsDealt(CardsDealtEventArgs args)
        {
            CardsDealt?.Invoke(this, args);
        }

        protected void OnTrickStarted(EventArgs args)
        {
            TrickStarted?.Invoke(this, args);
        }
    }
}