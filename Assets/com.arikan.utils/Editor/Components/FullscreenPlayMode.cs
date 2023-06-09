using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public class FullscreenPlayMode : MonoBehaviour
{
    //The size of the toolbar above the game view, excluding the OS border.
    private static int tabHeight = 22;

    private static Dictionary<string, Vector2> settings = new Dictionary<string, Vector2> {
      {"mnml", new Vector2(1920,0)} // sharing your code? offsets go in here!
   };

    static FullscreenPlayMode()
    {
        if (settings.ContainsKey(System.Environment.UserName))
        {
            EditorApplication.playModeStateChanged -= CheckPlayModeState;
            EditorApplication.playModeStateChanged += CheckPlayModeState;
        }
    }

    static void CheckPlayModeState(PlayModeStateChange mode)
    {
        // looks strange, but works much better!
        if (EditorApplication.isPlaying)
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode)
            {
                FullScreenGameWindow();
            }
            else
            {
                CloseGameWindow();
            }
        }
    }

    static EditorWindow GetMainGameView()
    {
        EditorApplication.ExecuteMenuItem("Window/Game");
        System.Type T = System.Type.GetType("UnityEditor.GameView,UnityEditor");
        System.Reflection.MethodInfo GetMainGameView = T.GetMethod("GetMainGameView", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
        System.Object Res = GetMainGameView.Invoke(null, null);
        return (EditorWindow)Res;
    }

    static Rect orig;
    static Vector2 min;
    static Vector2 max;

    static void FullScreenGameWindow()
    {
        EditorWindow gameView = GetMainGameView();

        Rect newPos = new Rect(0, 0 - tabHeight, Screen.currentResolution.width, Screen.currentResolution.height + tabHeight);
        newPos.position = newPos.position + settings[System.Environment.UserName];
        orig = gameView.position;
        min = gameView.minSize;
        max = gameView.maxSize;

        gameView.position = newPos;
        gameView.minSize = new Vector2(Screen.currentResolution.width, Screen.currentResolution.height + tabHeight);
        gameView.maxSize = gameView.minSize;
        gameView.position = newPos;

    }

    static void CloseGameWindow()
    {
        EditorWindow gameView = GetMainGameView();

        gameView.position = orig;
        gameView.minSize = min;
        gameView.maxSize = max;
        gameView.position = orig;
    }
}