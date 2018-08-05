using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ScoreUIController : MonoBehaviour
{
    public Text ScoreText;

    public GameObject HotImage;
    public GameObject NotImage;

    private bool _clicked;

    void Awake()
    {
        ScoreText.text = GameManager.Instance.Score.ToString();

        HotImage.SetActive(false);
        NotImage.SetActive(false);

        if (GameManager.Instance.DeathWasHot)
        {
            HotImage.SetActive(true);
        }
        else
        {
            NotImage.SetActive(true);
        }
    }

    public void OkClicked()
    {
        if (_clicked) return;
        _clicked = true;
        SceneManager.LoadScene("menu");
    }
}
