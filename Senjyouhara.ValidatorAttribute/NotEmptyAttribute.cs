using System;

namespace Senjyouhara.ValidatorAttribute;

[AttributeUsage(AttributeTargets.Property)]
public class NotEmptyAttribute:System.Attribute
{
    public string ErrorMessage { get; set; }
}