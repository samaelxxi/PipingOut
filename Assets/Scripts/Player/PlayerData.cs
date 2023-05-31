using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public static class PlayerData
{
    static int _completedLevels = -1;

    public static int CompletedLevels
    {
        get
        {
            if (_completedLevels == -1)
                _completedLevels = PlayerPrefs.GetInt("CurrentLevel", 0);
            return _completedLevels;
        }
    }

    public static bool CompletedTutorial
    {
        get
        {
            return PlayerPrefs.GetInt("CompletedTutorial", 0) == 1;
        }
        set
        {
            PlayerPrefs.SetInt("CompletedTutorial", value ? 1 : 0);
        }
    }

    public static int NextLevel => (CompletedLevels + 1) > Globals.TOTAL_LEVELS ? 
                                                            Globals.TOTAL_LEVELS : CompletedLevels + 1;

    public static void IncreaseLevel()
    {
        if (_completedLevels < Globals.TOTAL_LEVELS)
            _completedLevels++;
        PlayerPrefs.SetInt("CurrentLevel", _completedLevels);
    }

    public static void CompleteLevel(int level)
    {
        if (!CompletedTutorial)
            CompletedTutorial = true;
        if (level > _completedLevels)
        {
            _completedLevels = level;
            PlayerPrefs.SetInt("CurrentLevel", _completedLevels);
        }
    }

    public static void SetCurrentLevel(int level)  // cheat
    {
        _completedLevels = level;
        PlayerPrefs.SetInt("CurrentLevel", _completedLevels);
    }

    public static void OpenAllLevels()  // cheat
    {
        _completedLevels = 100;
        PlayerPrefs.SetInt("CurrentLevel", _completedLevels);
    }

    public static void ResetLevels()  // cheat
    {
        _completedLevels = 0;
        PlayerPrefs.SetInt("CurrentLevel", _completedLevels);
    }
}
