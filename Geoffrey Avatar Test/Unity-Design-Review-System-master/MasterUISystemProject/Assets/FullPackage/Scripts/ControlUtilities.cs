using UnityEngine;
using System.Collections;

public static class ControlUtilities {

    public static void Pause()
    {
        Time.timeScale = .05f;
    }

    public static void UnPause()
    {
        Time.timeScale = 1;
    }
}
