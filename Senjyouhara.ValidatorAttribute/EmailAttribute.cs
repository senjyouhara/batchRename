using System;

namespace Senjyouhara.ValidatorAttribute;

[AttributeUsage(AttributeTargets.Property)]
public class EmailAttribute:System.Attribute
{
    public string ErrorMessage { get; set; }
}