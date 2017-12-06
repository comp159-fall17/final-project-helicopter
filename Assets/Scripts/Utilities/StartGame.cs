using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartGame : MonoBehaviour {
    //switch to game scenes
    public void BeginGame() {
        SceneManager.LoadScene(1);
    }
}
