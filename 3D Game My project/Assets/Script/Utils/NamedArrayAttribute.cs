using UnityEngine;
using UnityEngine.ProBuilder.MeshOperations;

public class NamedArrayAttribute : PropertyAttribute
{
    public readonly string[] names;
    public NamedArrayAttribute(string[] names)
    {
        this.names = names;
    }
}

public class EnumNamedArrayAttribute : PropertyAttribute
{    
    public string[] names;
    
    public EnumNamedArrayAttribute(System.Type names_enum_type)     
    {
        this.names = System.Enum.GetNames(names_enum_type);    //Tutaj pobiera name do fieldów z Enuma wskazanego przy wpisywaniu zmiennej [EnumNamedArray(typeof(names_enum_type))]
    }
}


