using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;

namespace Lobby
{
    public class MenuManager : MonoBehaviour
    {
        public void ExitButton() => Application.Quit();
        public void EnterArenaButton() => SceneManager.LoadSceneAsync(0);
        public void SettingsButton()
        {
            //TODO: Settings, volumen, etc
        }
    }
}
