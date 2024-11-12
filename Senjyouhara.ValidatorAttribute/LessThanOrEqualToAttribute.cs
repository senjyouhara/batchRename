using System;

namespace Senjyouhara.ValidatorAttribute;


/**
 * 小于等于
 */
[AttributeUsage(AttributeTargets.Property)]
public class LessThanOrEqualToAttribute:System.Attribute
{
    public string ErrorMessage { get; set; }
    
    public int Length { get; private set; }

    public LessThanOrEqualToAttribute(int length)
    {
        Length = length;
    }
}