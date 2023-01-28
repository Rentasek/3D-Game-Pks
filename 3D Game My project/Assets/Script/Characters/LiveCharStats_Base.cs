using System.Collections;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;



/// <summary>
/// Statyczna klasa powiązana z komponentem Character Status
/// </summary>

public static class LiveCharStats_Base
{

    //////FieldOfView//////

    /// <summary>
    /// <br>Metoda szuka przeciwnika w dynamicznym Range    
    /// <br><i>Metoda pobiera CharacterStatus(live_charStats) i korzystając z watości z live_charStats przypisuje live_charStats.fov_aquiredTargetGameObject</i></br>>
    /// </summary>
    /// <param name="live_charStats">Attached CharacterStatus object / Przypięty obiekt CharacterStatus</param>
    /// <returns></returns>
    public static IEnumerator FOVRoutine(CharacterStatus live_charStats)
    {
        if (live_charStats.fov_isSearchingForTarget)
        {
            yield break; // żeby nie nadpisywał coroutine co klatke
        }
        else
        {
            live_charStats.fov_isSearchingForTarget = true;

            yield return new WaitForSeconds(live_charStats.fov_coneRoutineDelay);

            FieldOfViewTarget(live_charStats);

            live_charStats.fov_isSearchingForTarget = false;
        }
    }

    public static void FieldOfViewTarget(CharacterStatus live_charStats)
    {
        CheckForTargetInDynamicSightRange(live_charStats);
        if (live_charStats.navMeAge_targetInDynamicSightRange)
        {
            CheckForTargetInSpellRange(live_charStats);
            CheckForTargetInAttackRange(live_charStats);
        }
    }

    public static void CheckForTargetInDynamicSightRange(CharacterStatus live_charStats)
    {
        /*foreach (Collider collider in Physics.OverlapSphere(live_charStats.gameObject.transform.position, live_charStats.fov_CurrentDynamicSightRadius)) //dla każdego collidera w zasięgu wzroku
        {
            live_charStats.navMeAge_TestALLtargetsInDynamicSightRange = Physics.OverlapSphere(live_charStats.gameObject.transform.position, live_charStats.fov_CurrentDynamicSightRadius);
            if (live_charStats.currentEnemiesArray.Contains(collider.tag))    //jeśli ma tag zawarty w arrayu enemiesArray
            {
                live_charStats.navMeAge_targetInDynamicSightRange = true;    //target jest w sight range 

                Vector3 directionToTarget = (collider.transform.position - live_charStats.gameObject.transform.position).normalized; //0-1(normalized) różnica pomiędzy targetem a characterem

                if (Vector3.Angle(live_charStats.gameObject.transform.forward, directionToTarget) < live_charStats.fov_CurrentDynamicSightAngle / 2) //sprawdzanie angle wektora forward charactera i direction to target
                                                                                                                                                     //target może być na + albo - od charactera dlatego w każdą stronę angle / 2
                {
                    if (!Physics.Raycast(live_charStats.gameObject.transform.position, directionToTarget, live_charStats.fov_CurrentDynamicSightRadius, live_charStats.fov_obstaclesLayerMask))
                    {
                        //jeśli raycast do targetu nie jest zasłonięty przez jakiekolwiek obstacles!!
                        live_charStats.fov_aquiredTargetGameObject = collider.gameObject;           //ustawia znaleziony colliderem game objecta jako target
                        live_charStats.navMeAge_targetAquired = true;
                        break;          //najważniejszy jest brake, nie da się jednocześnie porównać listy colliderów z Overlapa z listą tagów z enemiesArray
                                        //foreach sprawdza wszystkie collidery jeśli trafi => target bool = true, ale że sprawdza wszystkie collidery
                                        //to trafia i pudłuje cały czas, dlatego trzeba foreacha zatrzymać breakiem kiedy trafi !!!
                    }
                    else
                    {
                        //live_charStats.fov_aquiredTargetGameObject = null;           //ustawia nie znaleziony colliderem game objecta jako null
                        live_charStats.navMeAge_targetAquired = false;
                    }
                }
                else
                {
                    //live_charStats.fov_aquiredTargetGameObject = null;           //ustawia nie znaleziony colliderem game objecta jako null
                    live_charStats.navMeAge_targetAquired = false;
                }
            }
            else
            {
                live_charStats.navMeAge_targetInDynamicSightRange = false;                     //target nie jest w sight range
                live_charStats.fov_aquiredTargetGameObject = null;           //ustawia nie znaleziony colliderem game objecta jako null
                live_charStats.navMeAge_targetAquired = false;
            }
        }*/        
        for (int i = 0; i < Physics.OverlapSphere(live_charStats.gameObject.transform.position, live_charStats.fov_CurrentDynamicSightRadius).Length; i++)
        {
            for (int j = 0; j < live_charStats.currentEnemiesArray.Length; j++)
            {                
                if (Physics.OverlapSphere(live_charStats.gameObject.transform.position, live_charStats.fov_CurrentDynamicSightRadius)[i].CompareTag(live_charStats.currentEnemiesArray[j]))
                {                    
                    live_charStats.navMeAge_targetInDynamicSightRange = true;
                    
                    Vector3 directionTotarget = (Physics.OverlapSphere(live_charStats.gameObject.transform.position, live_charStats.fov_CurrentDynamicSightRadius)[i].transform.position - live_charStats.gameObject.transform.position).normalized;
                    
                    if(Vector3.Angle(live_charStats.gameObject.transform.forward, directionTotarget) < live_charStats.fov_CurrentDynamicSightAngle / 2)
                    //sprawdzanie angle wektora forward charactera i direction to target
                    //target może być na + albo - od charactera dlatego w każdą stronę angle / 2  
                    {
                        live_charStats.navMeAge_TestALLtargetsInDynamicSightRange = Physics.OverlapSphere(live_charStats.gameObject.transform.position, live_charStats.fov_CurrentDynamicSightRadius);
                        live_charStats.fov_aquiredTargetGameObject = Physics.OverlapSphere(live_charStats.gameObject.transform.position, live_charStats.fov_CurrentDynamicSightRadius)[0].gameObject;
                        /*if (!Physics.Raycast(live_charStats.gameObject.transform.position, directionTotarget, live_charStats.fov_CurrentDynamicSightRadius, live_charStats.fov_obstaclesLayerMask)) //dodatkowo sprawdza Raycastem czy nie ma przeszkody pomiędzy playerem a targetem //  
                        {
                            
                        }*/
                    }
                }
            }
        }

        //Dynamiczny Sight Range
        if (live_charStats.navMeAge_targetInDynamicSightRange)
        {
            live_charStats.fov_CurrentDynamicSightRadius = Mathf.SmoothDamp(live_charStats.fov_CurrentDynamicSightRadius, live_charStats.fov_MinSightRadius, ref live_charStats.fov_CurrentVectorDynamicSightRadius, live_charStats.fov_TimeDynamicSightRadius);
            //dynamiczny sight range zmniejsza się żeby player nie lockował się na stałe na targecie

            live_charStats.fov_CurrentDynamicSightAngle = Mathf.SmoothDamp(live_charStats.fov_CurrentDynamicSightAngle, live_charStats.fov_MaxSightAngle, ref live_charStats.fov_CurrentVectorDynamicSightAngle, live_charStats.fov_TimeDynamicSightAngle);
            //dynamiczny sight Angle jeśli jest w range zwiększa kąt jeśli jest poza range zmniejsza kąt
        }
        else
        {
            live_charStats.fov_CurrentDynamicSightRadius = Mathf.SmoothDamp(live_charStats.fov_CurrentDynamicSightRadius, live_charStats.fov_MaxSightRadius, ref live_charStats.fov_CurrentVectorDynamicSightRadius, live_charStats.fov_TimeDynamicSightRadius);
            //dynamiczny sight range -> wraca do maxymalnego sight range

            live_charStats.fov_CurrentDynamicSightAngle = Mathf.SmoothDamp(live_charStats.fov_CurrentDynamicSightAngle, live_charStats.fov_MinSightAngle, ref live_charStats.fov_CurrentVectorDynamicSightAngle, live_charStats.fov_TimeDynamicSightAngle);
            //dynamiczny sight Angle, poza sight range wraca do minimalnego Sight Angle
        }
    }

    private static void CheckForTargetInAttackRange(CharacterStatus live_charStats)
    {
        foreach (Collider collider in Physics.OverlapSphere(live_charStats.gameObject.transform.position, live_charStats.navMeAge_attackRange)) //dla każdego collidera w zasięgu wzroku
        {
            if (live_charStats.currentEnemiesArray.Contains(collider.tag))                                         //jeśli ma tag zawarty w arrayu enemiesArray
            {
                live_charStats.navMeAge_targetInAttackRange = true;                          //ustawia znaleziony colliderem game objecta jako target

                if (live_charStats.fov_aquiredTargetGameObject == null) live_charStats.fov_aquiredTargetGameObject = collider.gameObject; //debugowanie gdyby nie złapał targeta w attack range
                break;
            }
            else live_charStats.navMeAge_targetInAttackRange = false;
        }
        //Jeśli jest w zasięgu ataku, triggeruje booleana inputAttacking w charStats, które posyła go dalej => CharacterMovement
        live_charStats.inputAttacking = live_charStats.navMeAge_targetInAttackRange;
    }

    private static void CheckForTargetInSpellRange(CharacterStatus live_charStats)
    {
        foreach (Collider collider in Physics.OverlapSphere(live_charStats.gameObject.transform.position, live_charStats.spell_MaxRadius * live_charStats.spell_AISpellRangeFromMax)) //dla każdego collidera w zasięgu spella
        {
            if (live_charStats.currentEnemiesArray.Contains(collider.tag))                                         //jeśli ma tag zawarty w arrayu enemiesArray
            {
                live_charStats.spell_targetInSpellRange = true;

                if (live_charStats.fov_aquiredTargetGameObject == null) live_charStats.fov_aquiredTargetGameObject = collider.gameObject; //debugowanie gdyby nie złapał targeta w attack range
                break;
            }
            else live_charStats.spell_targetInSpellRange = false;
        }

    }
}