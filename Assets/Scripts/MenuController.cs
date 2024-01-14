using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    [SerializeField] private Button _exitButton;
    [SerializeField] private Button _resetButton;

    void Start()
    {
        _exitButton.onClick.AddListener(Exit);
        _resetButton.onClick.AddListener(Reset);
    }

    private void Exit()
    {
        Application.Quit();
    }

    private void Reset()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }
}