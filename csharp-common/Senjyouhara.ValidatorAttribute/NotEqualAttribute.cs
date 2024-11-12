using System;

namespace Senjyouhara.ValidatorAttribute;

[AttributeUsage(AttributeTargets.Property)]
public class NotEqualAttribute:System.Attribute
{
    public string ErrorMessage { get; set; }
    public string EqualString { get; private set; }

    public NotEqualAttribute(string notEqual)
    {
        EqualString = notEqual;
    }
    
}