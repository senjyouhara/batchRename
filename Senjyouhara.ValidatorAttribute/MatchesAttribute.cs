using System;

namespace Senjyouhara.ValidatorAttribute;

[AttributeUsage(AttributeTargets.Property)]
public class MatchesAttribute:System.Attribute
{
    public string ErrorMessage { get; set; }
    public string Pattern { get; private set; }

    public MatchesAttribute(string pattern)
    {
        Pattern = pattern;
    }
    
}