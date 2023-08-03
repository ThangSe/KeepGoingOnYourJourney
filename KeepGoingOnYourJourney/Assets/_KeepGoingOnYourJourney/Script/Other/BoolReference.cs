using System;

[Serializable]
public class BoolReference
{
    public BoolVariableSO variable;
    public bool Value
    {
        get
        {
            return variable.value;
        }
    }
}
