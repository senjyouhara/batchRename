using System;

namespace Senjyouhara.ValidatorAttribute;

[AttributeUsage(AttributeTargets.Property)]
public class LengthAttribute:System.Attribute
{
    public string ErrorMessage { get; set; }
    
    public int Min { get; private set; }
    public int Max { get; private set; }

    public LengthAttribute(int min, int max)
    {
        Min = min;
        Max = max;
    }
}