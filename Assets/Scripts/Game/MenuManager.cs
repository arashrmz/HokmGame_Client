using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace HokmGame.Game
{
    public class MenuManager : MonoBehaviour
    {
        public void PlayOffline()
        {
            SceneManager.LoadScene(1);
        }

        public void PlayOnline()
        {
            SceneManager.LoadScene(2);
        }
    }
}
