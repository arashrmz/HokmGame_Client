using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.AspNetCore.SignalR.Client;
using System.Threading.Tasks;
using HokmGame.Core.Hokm;
using HokmGame.Core.Hokm.Events;
using EventType = HokmGame.Core.Hokm.Events.EventType;
using System.Threading;
using System;
using System.Linq;
using Newtonsoft.Json;
using HokmGame.Core.CardGame;
using UnityEngine.UI;
using TMPro;

namespace HokmGame.Game
{
    public class NetworkGameManager : MonoBehaviour
    {
        private static HubConnection connection;

        //username and password of the user
        [SerializeField]
        private string username;
        [SerializeField]
        private string password;

        //the current game room number
        private int numberOfRoom;
        //shows which player we are
        private int playerNum = 2;


        //GAME SETTINGS **************************
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

        public TMP_InputField usernameField;
        public TMP_InputField passwordField;

        public GameObject registerPanel;

        async Task Start()
        {
            //connect to server
            connection = new HubConnectionBuilder()
                        .WithUrl("https://localhost:7161/chathub")
                        .Build();

            connection.Closed += async (error) =>
            {
                await Task.Delay(UnityEngine.Random.Range(0, 5) * 1000);
                await connection.StartAsync();
            };

            //register event handlers
            connection.On<string>("Register", async (message) =>
            {
                //register was successful
                if (message == "Success")
                {
                    //save username and password in player prefs
                    Debug.Log("Successfully registered");
                    await Login();
                }
                else
                {
                    Debug.Log("Failed to register");
                }
            });

            //event handler for login
            connection.On<string>("Login", (message) =>
            {
                //login was successful
                if (message == "Success")
                {
                    //load menu
                    Debug.Log("Successfully logged in");
                    registerPanel.SetActive(false);
                    connection.InvokeAsync("JoinRoom");
                    //TODO: load menu here
                }
                else
                {
                    Debug.Log("Failed to login");
                }
            });

            //event handler for Join room for player 1
            connection.On<string>("JoinRoom", (message) =>
            {
                numberOfRoom = int.Parse(message);
                playerNum = 1;
            });

            //event handler for game ready
            connection.On<string>("GameReady", async (message) =>
            {
                numberOfRoom = int.Parse(message);
                //player 1 should initilize game here
                if (playerNum == 1)
                {
                    await InitGameAsync();
                }
            });

            //event handler for receiving game data
            connection.On<string>("GameData", async (message) =>
            {
                if (playerNum == 2)
                {
                    CardsDealtEventArgs args = JsonConvert.DeserializeObject<CardsDealtEventArgs>(message);
                    await currentMatch.CreateGame(args);
                    await connection.InvokeAsync("StartGame", numberOfRoom);
                }
            });

            //event handler for receiving start game signal
            connection.On<string>("StartGame", async (message) =>
            {
                await StartGame();
            });


            //event handler for receiving game data
            connection.On<string>("ReceiveCard", async (message) =>
            {
                var cardPlayedEventArgs = JsonConvert.DeserializeObject<CardPlayedEventArgs>(message);
                currentMatch.PlayCardForPlayer(cardPlayedEventArgs.PlayerPlayingTheCard, cardPlayedEventArgs.Cards.Last());
            });
            Connect();
        }

        public void Register()
        {
            username = usernameField.text;
            password = passwordField.text;
            connection.InvokeAsync("Register", username, password);
        }

        private async void Connect()
        {
            try
            {
                await connection.StartAsync();
            }
            catch (System.Exception ex)
            {
                Debug.Log(ex.Message);
            }
        }

        private async Task Login()
        {
            Debug.Log("Logging in");
            try
            {

                await connection.InvokeAsync("Login", username, password);
            }
            catch (System.Exception ex)
            {
                Debug.Log(ex.Message);
            }
        }

        private async Task StartGame()
        {
            while (!currentMatch.Score.IsCompleted)
            {
                await currentMatch.RunGameAsync(CancellationToken.None,
                    TimeSpan.FromMilliseconds(timeBetweenGame),
                    TimeSpan.FromMilliseconds(timeBetweenMove));
            }
        }

        private async Task InitGameAsync()
        {
            uiManager = GetComponent<UIManager>();
            cardManager = GetComponent<CardManager>();

            try
            {
                currentMatch = CreateMatch();
                player = (RealPlayer)currentMatch.Team1.Player1;
                await currentMatch.StartAsync();
                currentMatch.MatchEvent += OnMatchEvent;
                await currentMatch.CreateGame();
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

        //creates a team with two players for each player accordingly
        Team CreateTeam(bool team1, string name)
        {
            var team = new Team();
            team.Name = name;
            if (team1)
            {
                if (playerNum == 1)
                {
                    team.Player1 = new RealPlayer("player", GetComponent<CardManager>(), playerManager);
                    team.Player2 = new AIPlayer("player", GetComponent<CardManager>());
                }
                else
                {
                    team.Player1 = new NetworkPlayer("player", GetComponent<CardManager>());
                    team.Player2 = new NetworkPlayer("player", GetComponent<CardManager>());
                }
            }
            else
            {
                if (playerNum == 1)
                {
                    team.Player1 = new NetworkPlayer("player", GetComponent<CardManager>());
                    team.Player2 = new NetworkPlayer("player", GetComponent<CardManager>());
                }
                else
                {
                    team.Player1 = new RealPlayer("player", GetComponent<CardManager>(), playerManager);
                    team.Player2 = new AIPlayer("player", GetComponent<CardManager>());
                }
            }
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
                    if (playerNum == 1)
                    {
                        SendGameData(cardsDealtEventArgs);
                    }
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

        //send game state to player 2
        private async Task SendGameData(CardsDealtEventArgs cardsDealtEventArgs)
        {
            try
            {
                await connection.InvokeAsync("GameData", numberOfRoom, JsonConvert.SerializeObject(cardsDealtEventArgs));
            }
            catch (System.Exception ex)
            {
                Debug.Log(ex.Message);
            }
        }

        private void OnCardPlayed(MatchEventArgs e)
        {
            var cardPlayedEventArgs = e.OriginalEventArgs as CardPlayedEventArgs;
            var cards = cardPlayedEventArgs.Cards.ToList();
            cardManager.SpawnCenterCard(cards.Last(), cardPlayedEventArgs.PlayerPlayingTheCard);
            cardManager.RemoveCardFromDeck(cards.Last(), cardPlayedEventArgs.PlayerPlayingTheCard);
            if (playerNum == 1)
            {
                if (cardPlayedEventArgs.PlayerPlayingTheCard == PlayerPosition.Team1Player1 || cardPlayedEventArgs.PlayerPlayingTheCard == PlayerPosition.Team1Player2)
                {
                    SendCard(JsonConvert.SerializeObject(cardPlayedEventArgs));
                }
            }
            else
            {
                if (cardPlayedEventArgs.PlayerPlayingTheCard == PlayerPosition.Team2Player1 || cardPlayedEventArgs.PlayerPlayingTheCard == PlayerPosition.Team2Player2)
                {
                    SendCard(JsonConvert.SerializeObject(cardPlayedEventArgs));
                }
            }
        }

        //sends played card info to server
        private async Task SendCard(string card)
        {
            try
            {
                await connection.InvokeAsync("PlayCard", numberOfRoom, card);
            }
            catch (System.Exception ex)
            {
                Debug.Log(ex.Message);
            }
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
