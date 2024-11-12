using System;

namespace Senjyouhara.ValidatorAttribute;

[AttributeUsage(AttributeTargets.Property)]
public class BetweenAttribute:System.Attribute
{
    public string ErrorMessage { get; set; }
    
    public int Min { get; private set; }
    public int Max { get; private set; }

    public BetweenAttribute(int min, int max)
    {
        Min = min;
        Max = max;
    }
}