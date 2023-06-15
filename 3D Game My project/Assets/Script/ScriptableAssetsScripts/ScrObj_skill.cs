using Cinemachine;
using JetBrains.Annotations;
using System;
using System.Runtime.Serialization;
using UnityEditor;
using UnityEngine;
using UnityEngine.VFX;

[CreateAssetMenu(fileName = "SpellInfo", menuName = "Chars/SpellInfo")]
public class ScrObj_skill : ScriptableObject
{
    public enum InputType { primary, secondary }
    [Tooltip("Jaki przycisk aktywuje skilla?")] public InputType _inputType;

    public enum CastingType { Instant, Hold, Castable }
    [Tooltip("Który rodzaj castowania aktywuje SkillTarget ?")] public CastingType _castingType;    

    public enum ResourceType { health, mana, stamina }
    [Tooltip("Jaki Resource zużywa do Skilla?")] public ResourceType _resourceType;

    public enum EffectType { None, Hit, DamageOverTime, Heal, HealOverTime, Summon }

    public enum TargetType { None, DynamicCone, Projectile, AreaOfEffectMouse, Self, Chain, Pierce, Boom, Melee }

    [Space]
    [Header("SkillRange - wymagany")]
    [Tooltip("Bazowy Maxymalny Radius skilla. \n Wykorzystany w SkillForge jako OverlapSphereNonAlloc.Range dla wszystkich targetTypów skilla \n oraz do FielOfView SpellRangeChecha")] public float _skillMaxRadius;

    [Header("SkillTime")]
    [Tooltip("Ile czasu potrzeba do Deploy Cast")] public float _timeCast;
    [Tooltip("Bazowy Cooldown skilla")] public float _baseCooldown;

    [Header("SkillDamage/Cost")]
    [Tooltip("Bazowy Resource cost skilla")] public float _baseResourceCost;
    [Tooltip("Multiplier do Levela i BonusCharStats do obliczeń [0.1f] -> na każdy lev i na każdy BonusDMG dodaje 0.1f BAZOWYCH DMG")] public float _multiplier;

    /* [Flags] public enum New_EnumEffectType
    { 
        none = 0,
        hit = 2,
        damageOverTime = 4,
        heal = 8,
        healOverTime = 16,
        summon = 32
    }*/

    [Serializable]
    public class EffectTypes
    {
        [Tooltip("Jaki SkillEffect jeset użyty na Targetach?")] public EffectType _effectType;

        [Header("SkillDamage")]
        [Tooltip("Bazowy Damage skilla")] public float _baseDamage;
    }

    [Serializable]
    public class TargetTypes
    {
        [Tooltip("Na jakie targety działa SkillEffect?")] public TargetType _targetType;
        [Tooltip("Jaki SkillEffect jeset użyty na Targetach?")] public EffectTypes[] _effectTypes;

        [Header("SkillRange")]
        [Tooltip("Bazowy Minimalny Radius skilla")] public float _minRadius;
        [Tooltip("Bazowy Minimalny Kąt skilla")] public float _minAngle;
        [Tooltip("Bazowy Maxymalny Radius skilla")] public float _maxRadius;
        [Tooltip("Bazowy Maxymalny Kąt skilla")] public float _maxAngle;

        [Header("SkillTime")]
        [Tooltip("Ile czasu potrzeba do Max/Min Radius")] public float _timeMaxRadius;
        [Tooltip("Ile czasu potrzeba do Max/Min Angle")] public float _timeMaxAngle;

        [Header("SkillObstacles")]
        [Tooltip("Layer Mask z przeszkodami przez które nie da się atakować(z klasy skill)")] public LayerMask _obstaclesMask; //Obstacles dla skilla
    }

    [Tooltip("Na jakie targety działa SkillEffect?")] public TargetTypes[] _targetTypes;
    [Space]
    [Tooltip("Maxymalna ilość TargetTypów dla skilla")] public int maxTargetTypes;
    [Tooltip("Maxymalna ilość EffectTypów dla pojedynczego TargetType")] public int maxEffectTypes;

    [Space]
    [Header("SkillAudio")]
    [CanBeNull, Tooltip("AudioClip skilla -> Caster")] public AudioClip _casterAudioClip;
    [CanBeNull, Tooltip("AudioClip skilla OnFinished -> Caster")] public AudioClip _onFinishCastingAudioClip;
    [Space]
    [CanBeNull, Tooltip("AudioClip skilla -> Target")] public AudioClip _onTargetHitAudioClip;
    [Space]
    [CanBeNull, Tooltip("AudioVolume skilla -> Caster")] public float _casterAudioVolume;
    [CanBeNull, Tooltip("AudioVolume skilla -> Target")] public float _onTargetHitAudioVolume;
    [CanBeNull, Tooltip("AudioDelay pomiędzy Audio Clipami (prevent Audio Spamming) -> Target")] public float _onTargetHitAudioDelay;
    [Space]
    [CanBeNull, Tooltip("Prefab z VisualEffectem pojawiający się na przeciwniku -> Target ")] public GameObject _onTargetHitVisualEffectPrefab;
    //Można tak zrobić że jak trafi przeciwnika to pojawia na nim (Instantiate) empty object(Prefab z Bazy) który ma na sobie VisualEffect ale może pojawiać się tylko raz
    //Można mu dać że ma zniknąć jeśli nie jest na liście targetów castującego 

    [Header("Animator Values")]
    [CanBeNull, Tooltip("Jaki Float Animatora odpowiada za animację? - Nazwa")] public string _animatorFloatName;
    [CanBeNull, Tooltip("Jaki Trigger Animatora odpowiada za animację?  - Nazwa")] public string _animatorTriggerName;
    [CanBeNull, Tooltip("Jaki Bool Animatora odpowiada za animację? - Nazwa ")] public string _animatorBoolName;
    [CanBeNull, Tooltip("Jaki Trigger Animatora przy finish casta?  - Nazwa")] public string _animatorTriggerOnFinishedCastingName;
    [Space]
    [CanBeNull, Tooltip("Jaki Float Animatora zastopować przy isCasting? - Nazwa")] public string _stopOnCastFloatNameAnimator;
    [CanBeNull, Tooltip("Jaki Trigger Animatora zastopować przy isCasting?  - Nazwa")] public string _stopOnCastTriggerNameAnimator;
    [CanBeNull, Tooltip("Jaki Bool Animatora zastopować przy isCasting? - Nazwa ")] public string _stopOnCastBoolNameAnimator;

    /* [CanBeNull, Tooltip("Na którym combo colider działa?")] public float skill_onCombo;

     [Header("SkillVariable VFX")]
     [Tooltip("VFX - Ilość particali")] public float skill_ParticlesPerSec;
     [Tooltip("VFX - Minimalny Lifetime particali")] public float skill_LifetimeMin;
     [Tooltip("VFX - Maxymalny Lifetime particali")] public float skill_LifetimeMax;
     [Tooltip("VFX - Kierunek i prędkość (wektor) rozchodzenia się particali")] public float skill_MovingTowardsFactor;*/      
}
