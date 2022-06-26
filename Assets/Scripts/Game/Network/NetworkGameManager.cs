using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.AspNetCore.SignalR.Client;
using System.Threading.Tasks;

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

        void Start()
        {
            //connect to server
            connection = new HubConnectionBuilder()
                        .WithUrl("https://localhost:7161/chathub")
                        .Build();

            connection.Closed += async (error) =>
            {
                await Task.Delay(Random.Range(0, 5) * 1000);
                await connection.StartAsync();
            };

            //register event handlers
            connection.On<string>("Register", (message) =>
            {
                //register was successful
                if (message == "Success")
                {
                    //save username and password in player prefs
                    Debug.Log("Successfully registered");
                    PlayerPrefs.SetString("username", username);
                    PlayerPrefs.SetString("password", password);
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
            connection.On<string>("GameReady", (message) =>
            {
                numberOfRoom = int.Parse(message);
                //player 1 should initilize game here
                if (playerNum == 1)
                {
                    InitGame();
                }
            });

            Connect();
        }

        private async void Connect()
        {
            try
            {
                await connection.StartAsync();

                await Register();
            }
            catch (System.Exception ex)
            {
                Debug.Log(ex.Message);
            }
        }

        private async Task Register()
        {
            try
            {
                await connection.InvokeAsync("Register", "HokmPlayer", "MyPassword");
            }
            catch (System.Exception ex)
            {
                Debug.Log(ex.Message);
            }
        }

        private async Task Login()
        {
            try
            {
                await connection.InvokeAsync("Login", username, password);
            }
            catch (System.Exception ex)
            {
                Debug.Log(ex.Message);
            }
        }

        private void InitGame()
        { }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
