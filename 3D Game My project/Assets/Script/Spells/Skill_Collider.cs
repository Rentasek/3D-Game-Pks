using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class Skill_Collider : MonoBehaviour
{    
    [Tooltip("Live_charStats Castera"), SerializeField] public CharacterStatus live_charStats;    
    [Tooltip("Skill powi¹zany z Colliderem"), SerializeField] public Skill skill;

    [Space]
    [Header("Testing")]
    [Tooltip("targetList")] Collider[] _targets;

    private void OnEnable()
    {
        Invoke(nameof(DisableColliderObject), skill.scrObj_Skill._timeCast);
    }

    public void DisableColliderObject() 
    {
        gameObject.SetActive(false);
        //Debug.Log("Activate Collider!!!");
    }

    private void OnTriggerEnter(Collider other)
    {
        if(skill._enemiesArray.Contains(other.tag))
        {
            //Debug.Log("Contains target tag!!!");
            for (int targetTypeIndex = 0; targetTypeIndex < skill.targetDynamicValues.Length; targetTypeIndex++)
            {
                if (skill.targetDynamicValues[targetTypeIndex]._targetColliders.IndexOf(other) < 0) //sprawdza czy nie ma na liœcie. Je¿eli IndexOf < 0 czyli nie ma obiektów z tym indexem
                {
                    skill.targetDynamicValues[targetTypeIndex]._targetColliders.Add(other); //przypisuje do listy colliders jeœli ma taga z listy enemies  
                                                                                            // Debug.Log("Hit Target !!!");
                    for (int effectTypeIndex = 0; effectTypeIndex < skill.scrObj_Skill._targetTypes[targetTypeIndex]._effectTypes.Length; effectTypeIndex++)
                    {
                        switch (skill.scrObj_Skill._targetTypes[targetTypeIndex]._effectTypes[effectTypeIndex]._effectType)
                        {
                            case ScrObj_skill.EffectType.None:
                                break;

                            case ScrObj_skill.EffectType.Hit:
                                 SkillForge.EffectType.Skill_Hit(skill.scrObj_Skill, skill, live_charStats, targetTypeIndex, effectTypeIndex);
                                break;

                            case ScrObj_skill.EffectType.DamageOverTime:
                                SkillForge.EffectType.Skill_DamageOverTime(skill.scrObj_Skill, skill, live_charStats, targetTypeIndex, effectTypeIndex);
                                break;

                            case ScrObj_skill.EffectType.Heal:
                                SkillForge.EffectType.Skill_Heal(skill.scrObj_Skill, skill, live_charStats, targetTypeIndex, effectTypeIndex);
                                break;

                            case ScrObj_skill.EffectType.HealOverTime:
                                SkillForge.EffectType.Skill_HealOverTime(skill.scrObj_Skill, skill, live_charStats, targetTypeIndex, effectTypeIndex);
                                break;

                            case ScrObj_skill.EffectType.Summon:
                                break;
                        }
                    }
                }
            }
        }
    }    
}
