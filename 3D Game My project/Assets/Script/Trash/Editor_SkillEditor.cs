//using PlasticPipe.PlasticProtocol.Messages;
//using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;


//[CustomEditor(typeof(Skill))]
public class Editor_SkillEditor //: Editor
{/*
    Skill skill;

    public override void OnInspectorGUI()
    {
        using(var check = new EditorGUI.ChangeCheckScope())
        {
            base.OnInspectorGUI();
            if(check.changed)
            {
                //scrObj_Skill.UpdateScrObj;
            }
        }

        //DrawSettingsEditor(planet.shapeSettings, planet.OnShapeSettingsUpdated, ref planet.shapeSettingsFoldout, ref shapeEditor);
        //DrawSettingsEditor(planet.colourSettings, planet.OnColourSettingsUpdated, ref planet.colorSettingsFoldout, ref colourEditor);
    }

    void DrawSettingsEditor(Object settings, System.Action onSettingsUpdated, ref bool foldout, ref Editor editor)  //foldout - rozwijanie settingsów w inspectorze, musi byc ref(reference) ponieważ jest customova klasa bez mono behaviour?
    {
        if (settings != null)
        {
            foldout = EditorGUILayout.InspectorTitlebar(foldout, settings);

            using (var check = new EditorGUI.ChangeCheckScope())
            {


                if (foldout)
                {
                    CreateCachedEditor(settings, null, ref editor); //zapobiega tworzeniu nowego edytora za każdym razem
                    editor.OnInspectorGUI();

                    if (check.changed)                  //check sprawdza czy w inspectorze dokonały się zmiany i przekazuje info do OnInspectorGiu override
                    {
                        if (onSettingsUpdated != null)
                        {
                            onSettingsUpdated();
                        }

                    }
                }

            }
        }
    }

    private void OnEnable()
    {
        skill = (Skill)target;
    }
*/
}
