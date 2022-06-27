using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HokmGame.Core.CardGame;
using HokmGame.Core.Hokm.Events;
using HokmGame.Core.Hokm.Info;
using HokmGame.Core.Hokm.Score;

namespace HokmGame.Core.Hokm
{
    public class Match
    {
        public Team Team1 { get; set; }
        public Team Team2 { get; set; }
        public MatchScore Score { get; set; }
        public PlayerPosition CurrentTrumpCaller { get; set; }
        public int CurrentGameNumber { get; set; }
        public event EventHandler<MatchEventArgs> MatchEvent;
        private Dictionary<PlayerPosition, IPlayer> _players;
        private Game _currentGame;

        public Match(Team team1, Team team2, int bestOf = 7)
        {
            Team1 = team1;
            Team2 = team2;
            Score = new MatchScore(bestOf);
            //determine trump caller randomly
            CurrentTrumpCaller = PlayerPosition.Team1Player1;
            _players = new Dictionary<PlayerPosition, IPlayer>();
            _currentGame = null;
            _players[PlayerPosition.Team1Player1] = team1.Player1;
            _players[PlayerPosition.Team1Player2] = team1.Player2;
            _players[PlayerPosition.Team2Player1] = team2.Player1;
            _players[PlayerPosition.Team2Player2] = team2.Player2;
        }

        public async Task StartAsync()
        {
            var infos = _players.ToDictionary(x => x.Key, y => y.Value.ToPlayerInfo());
            foreach (var kv in _players)
            {
                //inform players match is starting
                await kv.Value.NewMatchAsync(infos, kv.Key);
            }
        }

        public async Task CreateGame()
        {
            if (Score.IsCompleted)
                throw new InvalidOperationException("Match is finished, game cannot start");
            CurrentGameNumber++;
            var g = new Game(CurrentGameNumber, Score, Team1, Team2, CurrentTrumpCaller);
            _currentGame = g;
            g.BanterUttered += OnBanterUttered;
            g.TrickFinished += OnTrickFinished;
            g.CardPlayed += OnCardPlayed;
            g.CardsDealt += OnCardsDealt;
            g.TrickStarted += OnTrickStarted;

            RaiseEvent(EventType.GameStarted, null);

            // tell players the game is about to start
            foreach (var player in _players.Values)
            {
                await player.NewGameAsync(Score, CurrentTrumpCaller);
            }

            var trumpSuit = await g.StartAndDealAsync(CancellationToken.None);

        }

        //create a game based on received data from server
        public async Task CreateGame(CardsDealtEventArgs args)
        {
            if (Score.IsCompleted)
                throw new InvalidOperationException("Match is finished, game cannot start");
            CurrentGameNumber++;
            var g = new Game(CurrentGameNumber, Score, Team1, Team2, CurrentTrumpCaller, args);
            _currentGame = g;
            g.BanterUttered += OnBanterUttered;
            g.TrickFinished += OnTrickFinished;
            g.CardPlayed += OnCardPlayed;
            g.CardsDealt += OnCardsDealt;
            g.TrickStarted += OnTrickStarted;

            RaiseEvent(EventType.GameStarted, null);
            RaiseEvent(EventType.CardsDealt, args);

            // tell players the game is about to start
            foreach (var player in _players.Values)
            {
                await player.NewGameAsync(Score, CurrentTrumpCaller);
            }
        }

        public async Task<Game> RunGameAsync(CancellationToken cancellationToken,
                 TimeSpan? inBetweenTrickDelay = null,
                 TimeSpan? inBetweenCardDelay = null)
        {
            while (!_currentGame.Score.IsGameOver)
            {
                await _currentGame.PlayTrickAsync(cancellationToken, inBetweenCardDelay);
                if (inBetweenTrickDelay.HasValue)
                    await Task.Delay(inBetweenTrickDelay.Value, cancellationToken);
            }

            foreach (var kv in _currentGame.Score.TricksWon)
                Score.TricksWon[kv.Key] += kv.Value;

            var team1Won = _currentGame.Score.TricksWonByTeam1 > _currentGame.Score.TricksWonByTeam2;
            Score.RegisterGameWin(_currentGame.Score, CurrentTrumpCaller);
            var team1WasCaller = PlayerPositions.IsTeam1(CurrentTrumpCaller);

            if (team1Won ^ team1WasCaller) // if not the caller team has won, move forward for TrumpCaller
                CurrentTrumpCaller = (PlayerPosition)(((int)CurrentTrumpCaller + 1) % 4);


            _currentGame.BanterUttered -= OnBanterUttered;
            _currentGame.TrickFinished -= OnTrickFinished;
            _currentGame.CardPlayed -= OnCardPlayed;
            _currentGame.CardsDealt -= OnCardsDealt;
            _currentGame.TrickStarted -= OnTrickStarted;

            RaiseEvent(EventType.GameFinished, new GameFinishedEventArgs
            {
                Game = _currentGame
            });
            _currentGame = null;
            return null;
        }



        public async Task<Game> CreateAndRunGameAsync(CancellationToken cancellationToken,
         TimeSpan? inBetweenTrickDelay = null,
         TimeSpan? inBetweenCardDelay = null)
        {
            if (Score.IsCompleted)
                throw new InvalidOperationException("Match is finished, game cannot start");
            CurrentGameNumber++;
            var g = new Game(CurrentGameNumber, Score, Team1, Team2, CurrentTrumpCaller);
            _currentGame = g;
            g.BanterUttered += OnBanterUttered;
            g.TrickFinished += OnTrickFinished;
            g.CardPlayed += OnCardPlayed;
            g.CardsDealt += OnCardsDealt;
            g.TrickStarted += OnTrickStarted;

            RaiseEvent(EventType.GameStarted, null);

            // tell players the game is about to start
            foreach (var player in _players.Values)
            {
                await player.NewGameAsync(Score, CurrentTrumpCaller);
            }

            var trumpSuit = await g.StartAndDealAsync(cancellationToken);

            if (trumpSuit.HasValue)
            {
                while (!g.Score.IsGameOver)
                {
                    await g.PlayTrickAsync(cancellationToken, inBetweenCardDelay);
                    if (inBetweenTrickDelay.HasValue)
                        await Task.Delay(inBetweenTrickDelay.Value, cancellationToken);
                }

                foreach (var kv in g.Score.TricksWon)
                    Score.TricksWon[kv.Key] += kv.Value;

                var team1Won = g.Score.TricksWonByTeam1 > g.Score.TricksWonByTeam2;
                Score.RegisterGameWin(g.Score, CurrentTrumpCaller);
                var team1WasCaller = PlayerPositions.IsTeam1(CurrentTrumpCaller);

                if (team1Won ^ team1WasCaller) // if not the caller team has won, move forward for TrumpCaller
                    CurrentTrumpCaller = (PlayerPosition)(((int)CurrentTrumpCaller + 1) % 4);
            }

            g.BanterUttered -= OnBanterUttered;
            g.TrickFinished -= OnTrickFinished;
            g.CardPlayed -= OnCardPlayed;
            g.CardsDealt -= OnCardsDealt;
            g.TrickStarted -= OnTrickStarted;

            _currentGame = null;

            RaiseEvent(EventType.GameFinished, new GameFinishedEventArgs { Game = g });

            return g;
        }

        //to run game in client
        public async Task<Game> RunNetworkGameAsync(CancellationToken cancellationToken,
         CardsDealtEventArgs cardsDealtEventArgs,
         TimeSpan? inBetweenTrickDelay = null,
         TimeSpan? inBetweenCardDelay = null
         )
        {
            if (Score.IsCompleted)
                throw new InvalidOperationException("Match is finished, game cannot start");
            CurrentGameNumber++;
            var g = new Game(CurrentGameNumber, Score, Team1, Team2, CurrentTrumpCaller, cardsDealtEventArgs);
            _currentGame = g;
            g.BanterUttered += OnBanterUttered;
            g.TrickFinished += OnTrickFinished;
            g.CardPlayed += OnCardPlayed;
            g.CardsDealt += OnCardsDealt;
            g.TrickStarted += OnTrickStarted;

            RaiseEvent(EventType.GameStarted, null);

            // tell players the game is about to start
            foreach (var player in _players.Values)
            {
                await player.NewGameAsync(Score, CurrentTrumpCaller);
            }

            var trumpSuit = await g.StartAndDealAsync(cancellationToken);

            if (trumpSuit.HasValue)
            {
                while (!g.Score.IsGameOver)
                {
                    await g.PlayTrickAsync(cancellationToken, inBetweenCardDelay);
                    if (inBetweenTrickDelay.HasValue)
                        await Task.Delay(inBetweenTrickDelay.Value, cancellationToken);
                }

                foreach (var kv in g.Score.TricksWon)
                    Score.TricksWon[kv.Key] += kv.Value;

                var team1Won = g.Score.TricksWonByTeam1 > g.Score.TricksWonByTeam2;
                Score.RegisterGameWin(g.Score, CurrentTrumpCaller);
                var team1WasCaller = PlayerPositions.IsTeam1(CurrentTrumpCaller);

                if (team1Won ^ team1WasCaller) // if not the caller team has won, move forward for TrumpCaller
                    CurrentTrumpCaller = (PlayerPosition)(((int)CurrentTrumpCaller + 1) % 4);
            }

            g.BanterUttered -= OnBanterUttered;
            g.TrickFinished -= OnTrickFinished;
            g.CardPlayed -= OnCardPlayed;
            g.CardsDealt -= OnCardsDealt;
            g.TrickStarted -= OnTrickStarted;

            _currentGame = null;

            RaiseEvent(EventType.GameFinished, new GameFinishedEventArgs { Game = g });

            return g;
        }

        public void PlayCardForPlayer(PlayerPosition pos, Card card)
        {
            var player = _players[pos];
            player.PlayCard(card);
        }

        private void OnTrickStarted(object sender, EventArgs e)
        {
            RaiseEvent(EventType.TrickStarted, e);
        }

        private void OnCardsDealt(object sender, CardsDealtEventArgs e)
        {
            RaiseEvent(EventType.CardsDealt, e);
        }

        private void OnCardPlayed(object sender, CardPlayedEventArgs e)
        {
            RaiseEvent(EventType.CardPlayed, e);
        }

        private void OnTrickFinished(object sender, TrickFinishedEventArgs e)
        {
            RaiseEvent(EventType.TrickFinished, e);
        }

        private void OnBanterUttered(object sender, BanterUtteredEventArgs e)
        {

        }

        private void RaiseEvent(EventType eventType, EventArgs args)
        {
            OnMatchEvent(new MatchEventArgs()
            {
                Info = this.ToInfo(),
                EventType = eventType,
                OriginalEventArgs = args
            });
        }

        protected void OnMatchEvent(MatchEventArgs args)
        {
            MatchEvent?.Invoke(this, args);
        }

        public MatchInfo ToInfo()
        {
            return new MatchInfo()
            {
                Score = Score,
                Team1 = Team1.ToTeamInfo(),
                Team2 = Team2.ToTeamInfo(),
                CurrentGame = _currentGame?.ToGameInfo()
            };
        }
    }
}