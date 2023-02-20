﻿using Cinemachine;
using JetBrains.Annotations;
using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.VFX;

[CreateAssetMenu(fileName = "SpellInfo", menuName = "Chars/SpellInfo")]
public class ScrObj_skill : ScriptableObject
{
    public enum Skill_InputType { primary, secondary }
    [Tooltip("Jaki przycisk aktywuje skilla?")] public Skill_InputType skill_InputType;

    public enum Skill_CastingType { Castable, Instant, Hold }
    [Tooltip("Który rodzaj castowania aktywuje SkillEfect?"), HideInInspector] public Skill_CastingType skill_CastingType;

    public enum Skill_TargetType { melee, cone, projectile, aoeMouse, self, summon }
    [Tooltip("Na jakie targety działa SkillEffect?")] public Skill_TargetType skill_TargetType;

    public enum Skill_ResourceType { health, mana, stamina }
    [Tooltip("Jaki Resource zużywa do Skilla?")] public Skill_ResourceType skill_ResourceType;

    public enum Skill_EffectTypeArray { none, hit, boom, pierce, chain, damageOverTime, healOverTime, heal, summon }
    [Tooltip("Jaki SkillEffect jeset użyty na Targetach?") ,PropertyAttribute_EnumNamedArray(typeof(Skill_CastingType))] public Skill_EffectTypeArray[] skill_EffectTypeArray = new Skill_EffectTypeArray[3]; //Max 3 elementy

    [Space]
    [Header("SkillRange")]
    [Tooltip("Bazowy Minimalny Radius skilla")] public float skill_MinRadius;
    [Tooltip("Bazowy Minimalny Kąt skilla")] public float skill_MinAngle;
    [Tooltip("Bazowy Maxymalny Radius skilla")] public float skill_MaxRadius;
    [Tooltip("Bazowy Maxymalny Kąt skilla")] public float skill_MaxAngle;

    [Header("SkillTime")]
    [Tooltip("Ile czasu potrzeba do Max/Min Radius")] public float skill_TimeMaxRadius;
    [Tooltip("Ile czasu potrzeba do Max/Min Angle")] public float skill_TimeMaxAngle;
    [Tooltip("Ile czasu potrzeba do Deploy Cast")] public float skill_TimeCast;
    [Tooltip("Bazowy Cooldown skilla")] public float skill_BaseCooldown;

    [Header("SkillDamage/Cost")]
    [Tooltip("Bazowy Resource cost skilla")] public float skill_BaseResourceCost;
    [Tooltip("Bazowy Damage skilla")] public float skill_BaseDamage;    
    [Tooltip("Multiplier do Levela i BonusCharStats do obliczeń [0.1f] -> na każdy lev i na każdy BonusDMG dodaje 0.1f BAZOWYCH DMG")] public float skill_Multiplier;

    [Header("SkillObstacles")]
    [Tooltip("Layer Mask z przeszkodami przez które nie da się atakować(z klasy skill)")] public LayerMask skill_ObstaclesMask; //Obstacles dla skilla

    [Header("SkillAudio")]
    [CanBeNull, Tooltip("AudioClip skilla OneShotOverlap -> Caster")] public AudioClip skill_OneShotOverlapAudioClip;
    [CanBeNull, Tooltip("AudioClip skilla OneShot non-Overlap -> Caster")] public AudioClip skill_OneShotNonOverlapAudioClip;   
    [CanBeNull, Tooltip("AudioClip skilla TimeCastOverlap -> Caster")] public AudioClip skill_TimeCastOverlapAudioClip;
    [CanBeNull, Tooltip("AudioClip skilla TimeCast Non-Overlap -> Caster")] public AudioClip skill_TimeCastNonOverlapAudioClip;
    [CanBeNull, Tooltip("AudioClip skilla OnFinished -> Caster")] public AudioClip skill_OnFinishCastingAudioClip;
    [CanBeNull, Tooltip("Czas Loopa skilla Hold -> Caster")] public float skill_HoldAudioLoopTime;
    [Space]
    [CanBeNull, Tooltip("AudioClip skilla -> Target")] public AudioClip skill_OnTargetHitAudioClip;
    [Space]
    [CanBeNull, Tooltip("AudioVolume skilla -> Caster")] public float skill_CastingAudioVolume;
    [CanBeNull, Tooltip("AudioVolume skilla -> Target")] public float skill_OnTargetHitAudioVolume;
    [Space]
    [CanBeNull, Tooltip("Prefab z VisualEffectem pojawiający się na przeciwniku -> Target ")] public GameObject skill_OnTargetHitVisualEffectPrefab;
    //Można tak zrobić że jak trafi przeciwnika to pojawia na nim (Instantiate) empty object(Prefab z Bazy) który ma na sobie VisualEffect ale może pojawiać się tylko raz
    //Można mu dać że ma zniknąć jeśli nie jest na liście targetów castującego    

    
    
    [Header("Animator Values")]
    [CanBeNull, Tooltip("Jaki Float Animatora odpowiada za animację? - Nazwa")] public string skill_AnimatorFloatName;
    [CanBeNull, Tooltip("Jaki Trigger Animatora odpowiada za animację?  - Nazwa")] public string skill_AnimatorTriggerName;
    [CanBeNull, Tooltip("Jaki Bool Animatora odpowiada za animację? - Nazwa ")] public string skill_AnimatorBoolName;
    [CanBeNull, Tooltip("Jaki Trigger Animatora przy finish casta?  - Nazwa")] public string skill_AnimatorTriggerOnFinishedCastingName;
    [Space]
    [CanBeNull, Tooltip("Jaki Float Animatora zastopować przy isCasting? - Nazwa")] public string skill_StopOnCastFloatNameAnimator;
    [CanBeNull, Tooltip("Jaki Trigger Animatora zastopować przy isCasting?  - Nazwa")] public string skill_StopOnCastTriggerNameAnimator;
    [CanBeNull, Tooltip("Jaki Bool Animatora zastopować przy isCasting? - Nazwa ")] public string skill_StopOnCastBoolNameAnimator;

    /* [CanBeNull, Tooltip("Na którym combo colider działa?")] public float skill_onCombo;

     [Header("SkillVariable VFX")]
     [Tooltip("VFX - Ilość particali")] public float skill_ParticlesPerSec;
     [Tooltip("VFX - Minimalny Lifetime particali")] public float skill_LifetimeMin;
     [Tooltip("VFX - Maxymalny Lifetime particali")] public float skill_LifetimeMax;
     [Tooltip("VFX - Kierunek i prędkość (wektor) rozchodzenia się particali")] public float skill_MovingTowardsFactor;*/

    #region TestingCustomPropertyArray
    [Serializable]
    public class CosixClass
    {
        [Header("SkillRange")]
        [Tooltip("Bazowy Minimalny Radius skilla")] public float flaot_1;
        [Tooltip("Bazowy Minimalny Kąt skilla")] public float flaot_2;
        [Tooltip("Bazowy Maxymalny Radius skilla")] public float flaot_3;
        [Tooltip("Bazowy Maxymalny Kąt skilla")] public float flaot_4;
    }
    [PropertyAttribute_EnumNamedNestedArray(typeof(Skill_CastingType))] public CosixClass[] cosixClasses = new CosixClass[3];
    //public CosixClass cosixClasses;
    #endregion
}
