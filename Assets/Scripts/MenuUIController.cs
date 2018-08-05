using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuUIController : MonoBehaviour
{
    public Image CreditsImage;

    private bool _clicked;

    void Awake()
    {
        GameManager.Instance.gameObject.GetComponent<Rotator>().enabled = true;
    }

    public void PlayPressed()
    {
        if (_clicked) return;
        _clicked = true;
        GameManager.Instance.InactivatePooledObjects();
        GameManager.Instance.gameObject.GetComponent<Rotator>().enabled = false;
        GameManager.Instance.transform.rotation = Quaternion.identity;
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
