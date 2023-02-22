using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Skill_Projectle : MonoBehaviour
{
    [Tooltip("Skill wywo³uj¹cy ten projectile"), CanBeNull] public Skill skill;

    /// <summary>
    /// Wywo³uje CastingType Instant
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {
        if (skill!=null)
        {
            if(skill._enemiesArray.Contains(other.tag)) 
            {
                if (skill.skill_targetColliders.IndexOf(other) < 0) //sprawdza czy nie ma na liœcie. Je¿eli IndexOf < 0 czyli nie ma obiektów z tym indexem
                {
                    skill.skill_targetColliders.Add(other); //przypisuje do listy colliders jeœli ma taga z listy enemies
                    skill.skill_IsCastingInstant = true;
                }
                else
                {
                    skill.skill_targetColliders.Remove(other);
                    if (skill.skill_targetColliders.Count <= 0) skill.skill_IsCastingInstant = false;  //jeœli nie ma ¿adnych targetów w Collision/Trigger
                }
            }
            else
            {
                skill.skill_targetColliders.Remove(other);
                skill.skill_IsCastingInstant = false;
            }
        }
    }

    /// <summary>
    /// Wywo³uje CastingType Hold
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerStay(Collider other)
    {
        if (skill != null)
        {
            if (skill._enemiesArray.Contains(other.tag))
            {
                skill.skill_IsCastingInstant = true;
            }



            skill.skill_IsCastingHold = true;
        }
    }

    /// <summary>
    /// Wywo³uje CastingType Finished Casting
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerExit(Collider other)
    {
        if (skill != null)
        {
            if (skill._enemiesArray.Contains(other.tag))
            {
                skill.skill_IsCastingInstant = true;
            }



            skill.skill_IsCastingFinishedCastable = true;
        }
    }
}

