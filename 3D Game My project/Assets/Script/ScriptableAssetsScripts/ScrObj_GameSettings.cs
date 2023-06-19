using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "GameSettings", menuName = "Game/Settings")]
public class ScrObj_GameSettings : ScriptableObject
{
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
        [Tooltip("Sensitivity myszki")] public float _rotateSensivity;
        [Tooltip("Czy mo¿na obracaæ myszk¹? - Player (X)")] public bool _enableMouseRotate;
        [Tooltip("Rodzaj PlayerInput - Player (P) (true - 3rd person / false - Isometric View")] public bool _playerInputEnable;
    }
    [Tooltip("Input settings")] public InputSettings _inputSettings;
    #endregion

    #region Gameplay Settings
    [Serializable]
    public class GameplaySettings
    {
        [Tooltip("OpóŸnienie _AIRoutine w Sek - im wiêcej tym ³atwiej dla kompa"), Range(0f, 0.3f)] public float _AIRoutineDelay;

        [Tooltip("Grawitacja ON/Off")] public bool _gravityEnabled = true;
        [Tooltip("Aktualna si³a grawitacji")] public float _gravity;
    }
    [Tooltip("Gameplay Settings")] public GameplaySettings _gameplaySettings;
    #endregion

    #region Character Settings    
    [Serializable]
    public class CharacterSettings
    {
      //  [Tooltip("Dostêpne postacie dla gracza")] public GameObject[] _playerChars;
      // nie mo¿na sharowac gameobj pomiedzy scenami  [Tooltip("Aktualna postaæ gracza")] public GameObject _currentPlayerChar;

        
        [Tooltip("Backupowa pozycja gracza")] public Vector3 _backupPlayerPosition;
        [Tooltip("Backupowa rotacja gracza")] public Quaternion _backupPlayerRotation;
    }
    [Tooltip("Character settings")] public CharacterSettings _characterSettings;
    #endregion

    #region Graphic Settings
    [Serializable]
    public class GraphicSettings
    {
        [Tooltip("Aktualny ustawiony FPS gry")] public int _framerate;
    }
    [Tooltip("Graphic settings")] public GraphicSettings _graphicSettings;
    #endregion

    #region Audio Settings
    [Serializable]
    public class AudioSettings
    {
        [Tooltip("Aktualna g³oœnoœæ gry")] public float _gameAudioVolume;

        [CanBeNull, Tooltip("AudioVolume skilla -> Caster")] public float _casterAudioVolume;
        [CanBeNull, Tooltip("AudioVolume skilla -> Target")] public float _onTargetHitAudioVolume;
        [CanBeNull, Tooltip("AudioDelay pomiêdzy Audio Clipami (prevent Audio Spamming) -> Target")] public float _onTargetHitAudioDelay;
    }
    [Tooltip("Audio Settings")] public AudioSettings _audioSettings;
    #endregion

    #region Editor Settings
    [Serializable]
    public class EditorSettings
    {
        [Tooltip("FoV Settings")] public FoVSettings _fovSettings;
        [Space]
        [Tooltip("Skill Settings")] public SkillSettings _skillSettings;
    }
    [Tooltip("Editor Settings")] public EditorSettings _editorSettings;    

    #region FoV Settings
    [Serializable]
    public class FoVSettings
    {
        [Header("GizmosColor")]
        [Tooltip("Gruboœæ lini Gizmos"), Range(0, 10)] public float _editorLineThickness;
        [Tooltip("Kolor Gizmos - AngleLineColor (kolor HDR picker)"), ColorUsageAttribute(true, true)] public Color _editorAngleLineColor;
        [Tooltip("Kolor Gizmos - AngleColor (kolor HDR picker)"), ColorUsageAttribute(true, true)] public Color _editorAngleColor;
        [Tooltip("Kolor  Gizmos - MaxRadius"), ColorUsageAttribute(true, true)] public Color _editorMaxRadiusColor;
        [Tooltip("Kolor  Gizmos - DynamicRadius"), ColorUsageAttribute(true, true)] public Color _editorDynamicRadiusColor;
        [Tooltip("Kolor  Gizmos - Raycast"), ColorUsageAttribute(true, true)] public Color _editorRaycastColor;
    }
    #endregion

    #region Skill Settings
    [Serializable]
    public class SkillSettings
    {
        [Header("GizmosColor")]
        [Tooltip("Kolor Gizmos - AISpellRadius (kolor HDR picker)"), ColorUsageAttribute(true, true)] public Color _editorAISpellRadiusColor;
        [Tooltip("Kolor Gizmos - SkillAngle (kolor HDR picker)"), ColorUsageAttribute(true, true)] public Color _skillAngleColor;
        [Tooltip("Kolor Gizmos - SkillRaycast (kolor HDR picker)"), ColorUsageAttribute(true, true)] public Color _skillRaycastColor;
    }
    #endregion
    #endregion
}
