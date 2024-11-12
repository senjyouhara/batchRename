using System;

namespace Senjyouhara.ValidatorAttribute;

[AttributeUsage(AttributeTargets.Property)]
public class MaxLengthAttribute:System.Attribute
{
    public string ErrorMessage { get; set; }
    
    public int Length { get; private set; }

    public MaxLengthAttribute(int length)
    {
        Length = length;
    }
}