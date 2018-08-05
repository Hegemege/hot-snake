using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ScoreUIController : MonoBehaviour
{
    public Text ScoreText;

    private bool _clicked;

    void Awake()
    {
        ScoreText.text = GameManager.Instance.Score.ToString();
    }

    public void OkClicked()
    {
        if (_clicked) return;
        _clicked = true;
        SceneManager.LoadScene("menu");
    }
}
