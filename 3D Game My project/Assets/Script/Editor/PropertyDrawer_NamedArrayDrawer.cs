using System.Reflection;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(PropertyAttribute_NamedArrayAttribute))]
public class PropertyDrawer_NamedArrayDrawer : PropertyDrawer
{
    public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
    {
        try
        {
            int pos = int.Parse(property.propertyPath.Split('[', ']')[1]);
            EditorGUI.ObjectField(rect, property, new GUIContent(((PropertyAttribute_NamedArrayAttribute)attribute).names[pos]));
        }
        catch 
        {
            EditorGUI.ObjectField(rect, property, label);
        }
    }
}

[CustomPropertyDrawer(typeof(PropertyAttribute_EnumNamedArrayAttribute))]
public class PropertyDrawer_EnumNamedArray : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        PropertyAttribute_EnumNamedArrayAttribute enumNames = attribute as PropertyAttribute_EnumNamedArrayAttribute;  //Tworzenie lokalnej (zmiennej / instancji klasy) enumNames i przypisywanie attribute z EnumNamedArrayAttribute
       
        
        //propertyPath returns something like component_hp_max.Array.data[4]        
        //so get the index from there
        int index = System.Convert.ToInt32(property.propertyPath.Substring(property.propertyPath.IndexOf("[")).Replace("[", "").Replace("]", "")); //Pobieranie Indexu z enuma
        
        //change the label
        label.text = enumNames.names[index]; //tutaj następuje już zmiana (Element 0, 1, 2, itd) na nazwę enuma o indexie[index]
        
        //draw field
        EditorGUI.PropertyField(position, property, label, true); //Rysowanie samego property Fielda
    }
}


