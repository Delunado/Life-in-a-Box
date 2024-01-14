using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSUtils : MonoBehaviour
{
    private void Update()
    {
        // This is for testing FPS
        if (Input.GetKeyDown(KeyCode.F1))
        {
            Application.targetFrameRate = 10;
        }
        else if (Input.GetKeyDown(KeyCode.F2))
        {
            Application.targetFrameRate = 20;
        }
        else if (Input.GetKeyDown(KeyCode.F3))
        {
            Application.targetFrameRate = 30;
        }
        else if (Input.GetKeyDown(KeyCode.F4))
        {
            Application.targetFrameRate = 40;
        }
        else if (Input.GetKeyDown(KeyCode.F5))
        {
            Application.targetFrameRate = 50;
        }
        else if (Input.GetKeyDown(KeyCode.F6))
        {
            Application.targetFrameRate = 60;
        }
        else if (Input.GetKeyDown(KeyCode.F7))
        {
            Application.targetFrameRate = 70;
        }
        else if (Input.GetKeyDown(KeyCode.F8))
        {
            Application.targetFrameRate = 80;
        }
        else if (Input.GetKeyDown(KeyCode.F9))
        {
            Application.targetFrameRate = 90;
        }
        else if (Input.GetKeyDown(KeyCode.F10))
        {
            Application.targetFrameRate = 100;
        }
        else if (Input.GetKeyDown(KeyCode.F11))
        {
            Application.targetFrameRate = 110;
        }
        else if (Input.GetKeyDown(KeyCode.F12))
        {
            Application.targetFrameRate = 120;
        }
    }
}