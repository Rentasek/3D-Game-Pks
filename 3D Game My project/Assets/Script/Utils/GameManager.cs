using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public ScrObj_GameSettings _scrObj_GameSettings;
    public Slider _framerateSlider, _mouseSensitivitySlider, _aiDelaySlider, _gravitySlider, _gameVolumeSlider, _skillsVolumeSlider, _onTargetHitVolumeSlider;
    public TextMeshProUGUI _framerateSliderText, _mouseSensitivitySliderText, _aiDelaySliderText, _gravitySliderText, _gameVolumeSliderText, _skillsVolumeSliderText, _onTargetHitVolumeSliderText;

    #region Realtime Settings
    [Serializable]
    public class RealtimeSettings
    {
        [Tooltip("Czy gra jest Paused?")] public bool _isPaused;

        [Tooltip("Aktualny obrót myszk¹")] public float _rotateHorizontal;
    }
    [Tooltip("Realtime Settings")] public RealtimeSettings _realtimeSettings;
    #endregion

    #region Input Settings    
    [Serializable]
    public class InputSettings
    {
        [Tooltip("Czy mo¿na obracaæ myszk¹? - Player (X)")] public bool _enableMouseRotate;
        [Tooltip("Rodzaj PlayerInput - Player (P) (true - 3rd person / false - Isometric View")] public bool _playerInputEnable;
    }
    [Tooltip("Input settings")] public InputSettings _inputSettings;
    #endregion
       
    #region Character Settings    
    [Serializable]
    public class CharacterSettings
    {
        //  [Tooltip("Dostêpne postacie dla gracza")] public GameObject[] _playerChars;
        // nie mo¿na sharowac gameobj pomiedzy scenami  [Tooltip("Aktualna postaæ gracza")] public GameObject _currentPlayerChar;

        [Tooltip("Grawitacja ON/Off")] public bool _gravityEnabled = true;
        [Tooltip("Backupowa pozycja gracza")] public Vector3 _backupPlayerPosition;
        [Tooltip("Backupowa rotacja gracza")] public Quaternion _backupPlayerRotation;
    }
    [Tooltip("Character settings")] public CharacterSettings _characterSettings;
    #endregion   

    private void Awake()
    {
        SetupSliders();
    }

    private void Start()
    {
        UpdateSettings();
    }

    private void SetupSliders()
    {
        _framerateSlider.value = _scrObj_GameSettings._graphicSettings._framerate;
        _framerateSliderText.text = _scrObj_GameSettings._graphicSettings._framerate.ToString();
        _mouseSensitivitySlider.value = _scrObj_GameSettings._inputSettings._rotateSensivity;
        _mouseSensitivitySliderText.text = _scrObj_GameSettings._inputSettings._rotateSensivity.ToString();
        _aiDelaySlider.value = _scrObj_GameSettings._gameplaySettings._AIRoutineDelay;
        _aiDelaySliderText.text = _scrObj_GameSettings._gameplaySettings._AIRoutineDelay.ToString();
        _gravitySlider.value = _scrObj_GameSettings._gameplaySettings._gravity;
        _gravitySliderText.text = _scrObj_GameSettings._gameplaySettings._gravity.ToString();
        _gameVolumeSlider.value = _scrObj_GameSettings._audioSettings._gameAudioVolume;
        _gameVolumeSliderText.text = _scrObj_GameSettings._audioSettings._gameAudioVolume.ToString();
        _skillsVolumeSlider.value = _scrObj_GameSettings._audioSettings._casterAudioVolume;
        _skillsVolumeSliderText.text = _scrObj_GameSettings._audioSettings._casterAudioVolume.ToString();
        _onTargetHitVolumeSlider.value = _scrObj_GameSettings._audioSettings._onTargetHitAudioVolume;
        _onTargetHitVolumeSliderText.text = _scrObj_GameSettings._audioSettings._onTargetHitAudioVolume.ToString();
    }

    void UpdateSettings()
    {
        _framerateSlider.onValueChanged.AddListener(_framerate =>
        {
            _scrObj_GameSettings._graphicSettings._framerate = (int)_framerate;
            _framerateSliderText.text = _scrObj_GameSettings._graphicSettings._framerate.ToString();
            Application.targetFrameRate = (int)_framerate;
        });

        _mouseSensitivitySlider.onValueChanged.AddListener(_sensitivity =>
        {
            _scrObj_GameSettings._inputSettings._rotateSensivity = _sensitivity;
            _mouseSensitivitySliderText.text= _scrObj_GameSettings._inputSettings._rotateSensivity.ToString("0");            
        });

        _aiDelaySlider.onValueChanged.AddListener(_aiRoutineDelay=>
        {
            _scrObj_GameSettings._gameplaySettings._AIRoutineDelay = _aiRoutineDelay;
            _aiDelaySliderText.text = _scrObj_GameSettings._gameplaySettings._AIRoutineDelay.ToString("0.00");
        });

        _gravitySlider.onValueChanged.AddListener(_gravity =>
        {
            _scrObj_GameSettings._gameplaySettings._gravity = _gravity;
            _gravitySliderText.text = _scrObj_GameSettings._gameplaySettings._gravity.ToString("0.0");
        });

        _gameVolumeSlider.onValueChanged.AddListener(_gameVolume =>
        {
            _scrObj_GameSettings._audioSettings._gameAudioVolume = _gameVolume;
            _gameVolumeSliderText.text = _scrObj_GameSettings._audioSettings._gameAudioVolume.ToString("0.00");
        });

        _skillsVolumeSlider.onValueChanged.AddListener(_skillsVolume =>
        {
            _scrObj_GameSettings._audioSettings._casterAudioVolume = _skillsVolume;
            _skillsVolumeSliderText.text = _scrObj_GameSettings._audioSettings._casterAudioVolume.ToString("0.00");
        });
        
        _onTargetHitVolumeSlider.onValueChanged.AddListener(_onTargetHitVolume =>
        {
            _scrObj_GameSettings._audioSettings._onTargetHitAudioVolume = _onTargetHitVolume;
            _onTargetHitVolumeSliderText.text = _scrObj_GameSettings._audioSettings._onTargetHitAudioVolume.ToString("0.00");
        });

    }


}
