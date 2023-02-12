using Cinemachine;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.VFX;

[CreateAssetMenu(fileName = "SpellInfo", menuName = "Chars/SpellInfo")]
public class ScrObj_skill : ScriptableObject
{
    public enum Skill_InputType
    {
        primary, secondary
    }
    [SerializeField] public Skill_InputType skill_InputType;

    public enum Skill_CastingType
    {
        castable, instant, hold
    }
    [SerializeField] public Skill_CastingType skill_CastingType;

    public enum Skill_RangeType
    {
        melee, cone, projectile, aoeMouse, self, summon
    }
    [SerializeField] public Skill_RangeType skill_RangeType;

    public enum Skill_EffectType
    {
        hit, boom, pierce, chain, damageOverTime, healOverTime, heal, summon
    }
    [SerializeField] public Skill_EffectType skill_EffectType;

    public enum Skill_ResourceType
    {
        health, mana, stamina
    }
    [SerializeField] public Skill_ResourceType skill_ResourceType;


    [Space]
    [Header("SkillRange")]
    [Tooltip("Bazowy Minimalny Radius skilla"), SerializeField] public float skill_MinRadius;
    [Tooltip("Bazowy Minimalny Kąt skilla"), SerializeField] public float skill_MinAngle;
    [Tooltip("Bazowy Maxymalny Radius skilla"), SerializeField] public float skill_MaxRadius;
    [Tooltip("Bazowy Maxymalny Kąt skilla"), SerializeField] public float skill_MaxAngle;

    [Header("SkillTime")]
    [Tooltip("Ile czasu potrzeba do Max/Min Radius"), SerializeField] public float skill_TimeMaxRadius;
    [Tooltip("Ile czasu potrzeba do Max/Min Angle"), SerializeField] public float skill_TimeMaxAngle;
    [Tooltip("Ile czasu potrzeba do Deploy Cast"), SerializeField] public float skill_TimeCast;
    
    [Header("SkillDamage/Cost")]
    [Tooltip("Bazowy Resource cost skilla"), SerializeField] public float skill_BaseResourceCost;
    [Tooltip("Bazowy Damage skilla"), SerializeField] public float skill_BaseDamage;
    [Tooltip("Bazowy Cooldown skilla"), SerializeField] public float skill_BaseCooldown;
    [Tooltip("Multiplier do Levela i BonusCharStats do obliczeń [0.1f] -> na każdy lev i na każdy BonusDMG dodaje 0.1f BAZOWYCH DMG"), SerializeField] public float skill_Multiplier;

    [Header("SkillObstacles")]
    [Tooltip("Layer Mask z przeszkodami przez które nie da się atakować(z klasy skill)"), SerializeField] public LayerMask skill_ObstaclesMask; //Obstacles dla skilla
    
    [Header("SkillAudio")]    
    [CanBeNull, Tooltip("AudioClip skilla -> Caster"), SerializeField] public AudioClip skill_CastingAudioClip;    
    [CanBeNull, Tooltip("AudioClip skilla -> Target"), SerializeField] public AudioClip skill_OnHitAudioClip;
    [CanBeNull, Tooltip("VFX skilla -> Target"), SerializeField] public VisualEffect skill_OnHitVisualEffect;
    [CanBeNull, Tooltip("AudioVolume skilla -> Caster"), SerializeField] public float skill_CastingAudioVolume;
    [CanBeNull, Tooltip("AudioVolume skilla -> Target"), SerializeField] public float skill_OnHitAudioVolume;    

    [Header("Animator Values")]
    [CanBeNull, Tooltip("Jaki Float Animatora odpowiada za animację? - Nazwa"), SerializeField] public string skill_AnimatorFloat;
    [CanBeNull, Tooltip("Jaki Trigger Animatora odpowiada za animację?  - Nazwa"), SerializeField] public string skill_AnimatorTrigger;
    [CanBeNull, Tooltip("Jaki Bool Animatora odpowiada za animację? - Nazwa "), SerializeField] public string skill_AnimatorBool;

    [CanBeNull, Tooltip("Na którym combo colider działa?"), SerializeField] public float skill_onCombo;

    [Header("SkillVariable VFX - niepodpięte")]
    [Tooltip("VFX - Ilość particali"), SerializeField] public float skill_ParticlesPerSec;
    [Tooltip("VFX - Minimalny Lifetime particali"), SerializeField] public float skill_LifetimeMin;
    [Tooltip("VFX - Maxymalny Lifetime particali"), SerializeField] public float skill_LifetimeMax;
    [Tooltip("VFX - Kierunek i prędkość (wektor) rozchodzenia się particali"), SerializeField] public float skill_MovingTowardsFactor;

}


