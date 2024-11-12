using System;

namespace Senjyouhara.ValidatorAttribute;

[AttributeUsage(AttributeTargets.Property)]
public class MinLengthAttribute:System.Attribute
{
    public string ErrorMessage { get; set; }
    
    public int Length { get; private set; }

    public MinLengthAttribute(int length)
    {
        Length = length;
    }
}