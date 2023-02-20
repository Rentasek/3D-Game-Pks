using UnityEngine;
using UnityEngine.ProBuilder.MeshOperations;

public class PropertyAttribute_NamedArrayAttribute : PropertyAttribute
{
    public readonly string[] names;
    public PropertyAttribute_NamedArrayAttribute(string[] names)
    {
        this.names = names;
    }
}

public class PropertyAttribute_EnumNamedArrayAttribute : PropertyAttribute
{    
    public string[] names;
    
    public PropertyAttribute_EnumNamedArrayAttribute(System.Type names_enum_type)     
    {
        this.names = System.Enum.GetNames(names_enum_type);    //Tutaj pobiera name do fieldów z Enuma wskazanego przy wpisywaniu zmiennej [EnumNamedArray(typeof(names_enum_type))]
    }
}

public class PropertyAttribute_EnumNamedNestedArrayAttribute : PropertyAttribute
{
    public string[] names;

    public PropertyAttribute_EnumNamedNestedArrayAttribute(System.Type names_enum_type)
    {
        this.names = System.Enum.GetNames(names_enum_type);    //Tutaj pobiera name do fieldów z Enuma wskazanego przy wpisywaniu zmiennej [EnumNamedArray(typeof(names_enum_type))]
    }
}
