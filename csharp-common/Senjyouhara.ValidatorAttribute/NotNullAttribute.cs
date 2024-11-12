using System;

namespace Senjyouhara.ValidatorAttribute;

[AttributeUsage(AttributeTargets.Property)]
public class NotNullAttribute:System.Attribute
{
    public string ErrorMessage { get; set; }
}