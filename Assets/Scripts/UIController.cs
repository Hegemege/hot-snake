using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public Image HotnessMarker;
    public Text HotnessText;

    public Image HotHotImage;
    public Image NotHotImage;

    public Text ScoreText;
    public Text MultiplierText;

    public float ShakingMin;
    public float ShakingMax;
    public float ShakingThreshold;
    public float NearDeathThreshold;

    private Dictionary<int, string> _hotnessTextCache;
    private Vector3 _initialMarkerPosition;

    public float HotnessNumberSmoothing;
    private float _currentHotnessLevel;

    private Vector3 _initialHotHotPosition;
    private Vector3 _initialNotHotPosition;

    public float NearDeathFlashInterval;
    private float _flashingTimer;
    private bool _flashHighlight;
    public Color FlashHotColor;
    public Color FlashColdColor;
    private Color _initialTextColor;

    private int _previousSetScore;
    private float _previousSetMultiplier;

    void Awake()
    {
        _hotnessTextCache = new Dictionary<int, string>();

        for (var i = -100; i <= 100; i++)
        {
            _hotnessTextCache.Add(i, i + "%");
        }

        _currentHotnessLevel = GameManager.Instance.HotnessLevel;
        _initialTextColor = HotnessText.color;
    }

    void Start()
    {
        _initialMarkerPosition = HotnessMarker.rectTransform.localPosition;
        _initialHotHotPosition = HotHotImage.rectTransform.localPosition;
        _initialNotHotPosition = NotHotImage.rectTransform.localPosition;

        SetScore(true);
    }

    void Update()
    {
        var dt = Time.deltaTime;

        SetScore();

        _currentHotnessLevel = Mathf.Lerp(_currentHotnessLevel, GameManager.Instance.HotnessLevel, HotnessNumberSmoothing * Time.deltaTime);
        var hotnessLevel = Mathf.RoundToInt(_currentHotnessLevel * 100f);
        hotnessLevel = Mathf.Clamp(hotnessLevel, -100, 100);
        HotnessText.text = _hotnessTextCache[hotnessLevel];

        HotnessMarker.rectTransform.localPosition = _initialMarkerPosition + new Vector3(0f, Mathf.Clamp(_currentHotnessLevel, -1f, 1f) * (1080f - 350f) / 2f, 0f);

        HotHotImage.rectTransform.localPosition = _initialHotHotPosition;
        NotHotImage.rectTransform.localPosition = _initialNotHotPosition;

        // Text shaking
        if (Mathf.Abs(_currentHotnessLevel) > ShakingThreshold)
        {
            var shakeT = Mathf.Abs(_currentHotnessLevel).Remap(ShakingThreshold, 1f, 0f, 1f);
            var shakeScale = Mathf.Lerp(ShakingMin, ShakingMax, shakeT);

            Image shakingImage = _currentHotnessLevel > 0 ? HotHotImage : NotHotImage;
            Vector3 initial = _currentHotnessLevel > 0 ? _initialHotHotPosition : _initialNotHotPosition;

            shakingImage.transform.localPosition = initial + Random.onUnitSphere * shakeScale;
            shakingImage.transform.localRotation = Quaternion.AngleAxis(Random.Range(-shakeScale, shakeScale), transform.forward);
        }

        // Number flashing
        if (Mathf.Abs(_currentHotnessLevel) > NearDeathThreshold)
        {
            _flashingTimer += dt;

            if (_flashingTimer > NearDeathFlashInterval)
            {
                _flashHighlight = !_flashHighlight;
                _flashingTimer = 0f;

                HotnessText.color = _flashHighlight ? (_currentHotnessLevel > 0 ? FlashHotColor : FlashColdColor) : _initialTextColor;
            }
        }
        else
        {
            _flashingTimer = 0f;
            _flashHighlight = false;
            HotnessText.color = _initialTextColor;
        }
    }

    private void SetScore(bool always = false)
    {
        if (always || _previousSetScore != GameManager.Instance.Score)
        {
            ScoreText.text = GameManager.Instance.Score.ToString();
        }
        if (always || Mathf.Abs(_previousSetMultiplier - GameManager.Instance.ScoreMultiplier) > 0.001f)
        {
            MultiplierText.text = "X " + GameManager.Instance.ScoreMultiplier.ToString("n1");
        }

    }
}
