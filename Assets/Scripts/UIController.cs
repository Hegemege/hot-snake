using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public Image HotnessMarker;
    public Text HotnessText;

    private Dictionary<int, string> _hotnessTextCache;
    private Vector3 _initialMarkerPosition;

    void Awake()
    {
        _hotnessTextCache = new Dictionary<int, string>();

        for (var i = -100; i <= 100; i++)
        {
            _hotnessTextCache.Add(i, i + "%");
        }

        _initialMarkerPosition = HotnessMarker.rectTransform.localPosition;
    }

    void Update()
    {
        var hotnessLevel = Mathf.RoundToInt(GameManager.Instance.HotnessLevel * 100);
        hotnessLevel = Mathf.Clamp(hotnessLevel, -100, 100);
        HotnessText.text = _hotnessTextCache[hotnessLevel];

        HotnessMarker.rectTransform.localPosition = _initialMarkerPosition + new Vector3(0f, Mathf.Clamp(GameManager.Instance.HotnessLevel, -1f, 1f) * (1080f - 120f) / 2f, 0f);
    }
}
