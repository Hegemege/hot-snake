using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuUIController : MonoBehaviour
{
    public Image CreditsImage;

    private bool _clicked;

    public void PlayPressed()
    {
        if (_clicked) return;
        _clicked = true;
        SceneManager.LoadScene("main");
    }

    public void CreditsPressed()
    {
        CreditsImage.gameObject.SetActive(true);
    }

    public void CreditsBackPressed()
    {
        CreditsImage.gameObject.SetActive(false);
    }
}
