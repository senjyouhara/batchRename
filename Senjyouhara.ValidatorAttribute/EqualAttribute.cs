using System;

namespace Senjyouhara.ValidatorAttribute;

[AttributeUsage(AttributeTargets.Property)]
public class EqualAttribute:System.Attribute
{
    public string ErrorMessage { get; set; }
    public string EqualString { get; private set; }

    public EqualAttribute(string equal)
    {
        EqualString = equal;
    }
    
}