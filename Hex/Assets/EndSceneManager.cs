using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndSceneManager : MonoBehaviour
{
    public int returntoMM;

    public void ReturnToMainMenu()
        {
            SceneManager.LoadScene(returntoMM);
        }
}
