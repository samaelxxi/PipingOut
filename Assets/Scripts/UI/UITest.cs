using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UITest : MonoBehaviour
{
    [SerializeField] LevelUI _ui;

    // Start is called before the first frame update
    void Start()
    {
        Invoke(nameof(GoOn), 1);
    }

    void GoOn()
    {
        _ui.ShowDialog(new List<string>() { "Hey, baby, what's up?", "I'm fine, thanks, and you?", "I'm fine too, thanks for asking", "You're welcome, baby"});
    }
}
