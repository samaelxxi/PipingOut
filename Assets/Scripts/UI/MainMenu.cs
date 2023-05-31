using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;

public class MainMenu : MonoBehaviour
{
    [SerializeField] Transform _levelsPanel;
    [SerializeField] TMPro.TMP_Text _startText;
    [SerializeField] TMPro.TMP_Text _openText;
    [SerializeField] Game _game;

    [SerializeField] GameObject _mainMenu;
    [SerializeField] GameObject _levelsMenu;
    [SerializeField] GameObject _cheatsMenu;
    [SerializeField] Image _overlay;

    [SerializeField] AudioSource _clickSound;

    void Start()
    {
        if (PlayerData.NextLevel == 1)
            _startText.text = "Start";
        else
            _startText.text = "Continue";

        _mainMenu.SetActive(true);
        _levelsMenu.SetActive(false);
        SetupLevelButtons();
        Game.Instance.MutePlaneMusic(1);
    }

    public void SetupLevelButtons()
    {
        for (int i = 0; i < _levelsPanel.childCount; i++)
        {
            int level = i + 1;
            var button = _levelsPanel.GetChild(i).GetComponent<Button>();
            button.GetComponentInChildren<TMPro.TMP_Text>().text = level.ToString();
            if (level > PlayerData.NextLevel)
                button.interactable = false;
            else
                button.interactable = true;
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => StartLevel(level));
            button.onClick.AddListener(() => _clickSound.Play());
        }
    }

    void StartLevel(int levelIndex)
    {
        _overlay.DOFade(1, 2).OnComplete(() => Game.Instance.LoadLevel(levelIndex));
    }

    public void OpenLevelsMenu()
    {
        _clickSound.Play();
        _mainMenu.SetActive(false);
        _levelsMenu.SetActive(true);
    }

    public void CloseLevelsMenu()
    {
        _clickSound.Play();
        _mainMenu.SetActive(true);
        _levelsMenu.SetActive(false);
    }

    public void StartGame()
    {
        _clickSound.Play();
        StartLevel(PlayerData.NextLevel);
    }

    public void OpenAllLevels()
    {
        _clickSound.Play();
        if (!PlayerData.CompletedTutorial)
        {
            _openText.text = "Cmon, complete at least first level";
            return;
        }
        PlayerData.OpenAllLevels();
        SetupLevelButtons();
    }

    public void ResetLevels()
    {
        _clickSound.Play();
        PlayerData.ResetLevels();
        SetupLevelButtons();
    }

    public void ExitGame()
    {
        _clickSound.Play();
        Application.Quit();
    }
}
