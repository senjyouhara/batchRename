using System;

namespace Senjyouhara.ValidatorAttribute;

/**
 * 大于等于
 */
[AttributeUsage(AttributeTargets.Property)]
public class GreaterThanOrEqualToAttribute:System.Attribute
{
    public string ErrorMessage { get; set; }
    
    public int Length { get; private set; }

    public GreaterThanOrEqualToAttribute(int length)
    {
        Length = length;
    }
}