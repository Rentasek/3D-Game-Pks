using System.Reflection;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(NamedArrayAttribute))]public class NamedArrayDrawer : PropertyDrawer
{
    public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
    {
        try
        {
            int pos = int.Parse(property.propertyPath.Split('[', ']')[1]);
            EditorGUI.ObjectField(rect, property, new GUIContent(((NamedArrayAttribute)attribute).names[pos]));
        }
        catch 
        {
            EditorGUI.ObjectField(rect, property, label);
        }
    }
}

[CustomPropertyDrawer(typeof(EnumNamedArrayAttribute))]
public class DrawwerEnumNamedArray : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EnumNamedArrayAttribute enumNames = attribute as EnumNamedArrayAttribute;  //Tworzenie lokalnej (zmiennej / instancji klasy) enumNames i przypisywanie attribute z EnumNamedArrayAttribute
       
        
        //propertyPath returns something like component_hp_max.Array.data[4]        
        //so get the index from there
        int index = System.Convert.ToInt32(property.propertyPath.Substring(property.propertyPath.IndexOf("[")).Replace("[", "").Replace("]", "")); //Pobieranie Indexu z enuma
        
        //change the label
        label.text = enumNames.names[index]; //tutaj następuje już zmiana (Element 0, 1, 2, itd) na nazwę enuma o indexie[index]
        
        //draw field
        EditorGUI.PropertyField(position, property, label, true); //Rysowanie samego property Fielda
    }
}


