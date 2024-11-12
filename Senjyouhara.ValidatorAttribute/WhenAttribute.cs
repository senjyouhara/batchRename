using System;

namespace Senjyouhara.ValidatorAttribute;

[AttributeUsage(AttributeTargets.Property)]
public class WhenAttribute:System.Attribute
{
    public string PredicateFn;

    public WhenAttribute(string predicate)
    {
        PredicateFn = predicate;
    }
    
}