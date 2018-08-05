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

        var sfx = GameManager.Instance.AudioEffectManager.MenuClickPool.GetPooledObject();
        sfx.SetActive(true);

        GameManager.Instance.InactivatePooledObjects();
        GameManager.Instance.gameObject.GetComponent<Rotator>().enabled = false;
        GameManager.Instance.transform.rotation = Quaternion.identity;
        SceneManager.LoadScene("main");
    }

    public void CreditsPressed()
    {
        var sfx = GameManager.Instance.AudioEffectManager.MenuClickPool.GetPooledObject();
        sfx.SetActive(true);
        CreditsImage.gameObject.SetActive(true);
    }

    public void CreditsBackPressed()
    {
        var sfx = GameManager.Instance.AudioEffectManager.MenuClickPool.GetPooledObject();
        sfx.SetActive(true);
        CreditsImage.gameObject.SetActive(false);
    }
}
