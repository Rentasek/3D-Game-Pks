using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.PackageManager.UI;
using UnityEngine;
/// <summary>
/// Prosta testowa statyczna klasa :)
/// </summary>
public static class StaticTestingClass
{
    private static CharacterStatus local_live_charStats;

    //public SimpleDebugTestingClass(GameObject player) { this.live_charStats = player.GetComponent<CharacterStatus>(); }  

    /// <summary>
    /// Static Method for Character Status
    /// </summary>
    /// <param name="characterStatus"></param>
    public static void SimpleDebugTestingCharStats(CharacterStatus characterStatus, bool enableAttack)
    {
        local_live_charStats = characterStatus;
        if (enableAttack) { Attacking(); }
        

    }

    private static void Attacking()
    {
        if (local_live_charStats.currentAnimator != null)
        {
            local_live_charStats.currentAnimator.SetTrigger("MeeleAttack");
        }
    }

    public static void Attacking2(CharacterStatus characterStatus)
    {
        if (characterStatus.currentAnimator != null)
        {
            characterStatus.currentAnimator.SetTrigger("MeeleAttack");
        }
    }

    /// <summary>
    /// Simple method adding CurrentHP to Character / Tak odnosimy się bezpośrednio do vara w zdefinowanym object
    /// </summary>
    /// <param name="live_charStats">Attached CharacterStatus object / Przypięty obiekt CharacterStatus</param>
    /// <param name="addFloatValue">Add value / Dodawana wartość</param>
    public static void AddCurrentHP(CharacterStatus live_charStats, float addFloatValue)
    {        
        live_charStats.currentHP += addFloatValue;
    }


    /// <summary>
    /// <br>Simple method adding Float to Character / Tutaj metoda ma format zwracany <b> [return] i musi zostać przypisana </b> do vara objectu</br>
    /// <br><i>Metoda pobiera całą wartość np currentHP -> modyfikuje ją [addFloatValue] -> i później musi być zwrócona [return] jako zdefiniowany var np.[float]</i></br>
    /// </summary>
    /// <param name="floatValueToAdd">Wybierz float do dodania / Choose float to add</param>
    /// <param name="addFloatValue">Dodaj wartość / Add value</param>
    public static float AddCurrentFloat(float floatValueToAdd, float addFloatValue)
    {
        floatValueToAdd += addFloatValue;
        return floatValueToAdd;
    }

    /// <summary>
    /// <br>Simple method adding Float to Character / Tutaj metoda ma format zwracany <b> [return] i musi zostać przypisana </b> do vara objectu</br>
    /// <br><i>Metoda pobiera całą wartość np currentHP -> modyfikuje ją [addFloatValue] *  [addFloatValueMultiplier] -> </i></br>>
    /// <br><i>-> i później musi być zwrócona [return] jako zdefiniowany var np.[float]</i></br>
    /// </summary>
    /// <param name="floatValueToAdd">Wybierz float do dodania / Choose float to add</param>
    /// <param name="addFloatValue">Dodaj wartość / Add value</param>
    /// <param name="addFloatMultiplier">Pomnóż Dodaną wartość /Multiply Added value</param>
    public static float AddCurrentFloat(float floatValueToAdd, float addFloatValue, float addFloatMultiplier)
    {
        floatValueToAdd += addFloatValue*addFloatMultiplier;
        return floatValueToAdd;
    }

    /*public static IEnumerator FOVRoutine(CharacterStatus live_charStats)
    {
        if (live_charStats.fov_isSearchingForTarget)
        {            
            yield break; // żeby nie nadpisywał coroutine co klatke
        }
        else
        {
            live_charStats.fov_isSearchingForTarget = true;

            FieldOfViewTarget(live_charStats);

            yield return new WaitForSeconds(live_charStats.fov_coneRoutineDelay);                       

            live_charStats.fov_isSearchingForTarget = false;
        }
    }



    /// <summary>
    /// <br>Metoda szuka przeciwnika w dynamicznym Range    
    /// <br><i>Metoda pobiera CharacterStatus(live_charStats) i korzystając z watości z live_charStats przypisuje live_charStats.fov_aquiredTargetGameObject</i></br>>
    /// </summary>
    /// <param name="live_charStats">Attached CharacterStatus object / Przypięty obiekt CharacterStatus</param>
    /// <returns></returns>
    public static void FieldOfViewTarget(CharacterStatus live_charStats)
    {
        foreach (Collider collider in Physics.OverlapSphere(live_charStats.gameObject.transform.position, live_charStats.fov_CurrentDynamicSightRadius)) //dla każdego collidera w zasięgu wzroku

        {
            if (live_charStats.currentEnemiesArray.Contains(collider.tag))                                        //jeśli ma tag zawarty w arrayu enemiesArray
            {
                live_charStats.navMeAge_targetInDynamicSightRange = true;                     //target jest w sight range                
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
                        live_charStats.fov_aquiredTargetGameObject = null;           //ustawia nie znaleziony colliderem game objecta jako null
                        live_charStats.navMeAge_targetAquired = false;
                    }

                }
                else
                {
                    live_charStats.fov_aquiredTargetGameObject = null;           //ustawia nie znaleziony colliderem game objecta jako null
                    live_charStats.navMeAge_targetAquired = false;
                }
            }
            else
            {
                live_charStats.navMeAge_targetInDynamicSightRange = false;                     //target nie jest w sight range
                live_charStats.fov_aquiredTargetGameObject = null;           //ustawia nie znaleziony colliderem game objecta jako null
                live_charStats.navMeAge_targetAquired = false;
            }
        }
    }*/

}