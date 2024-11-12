using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Senjyouhara.ValidatorGenerator;

public abstract class TypeWithAttributeGenerator : IIncrementalGenerator
{
    internal abstract string AttributeName { get; }
    internal abstract string AttributeNamespace { get; }

    // 注：由于我写的所有`Attribute`都是用的同一个命名空间，
    // 所以可以通过组合`AttributeNamespace`和`AttributeName`便可以得到完整名称。
    // `AttributeNamespace`为"Attributes."
    private string AttributeFullName => AttributeNamespace + AttributeName;

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var generatorAttributes = context.SyntaxProvider.ForAttributeWithMetadataName(
            AttributeFullName,
            delegate { return true; },
            (gasc, _) => gasc
        ).Collect();

        context.RegisterSourceOutput(generatorAttributes, (spc, data) =>
        {
            // 注：此处我指定了一个特殊的`Attribute`，如果使用了它就禁用所有源生成器。
            // 如：[assembly: DisableSourceGenerator]
            // if (compilation.Assembly.GetAttributes().Any(attrData => attrData.AttributeClass?.ToDisplayString() == DisableSourceGeneratorAttribute))
            // return;

            foreach (var ga in data)
            {
                if (ga.TargetSymbol is not INamedTypeSymbol symbol)
                    continue;

                var valueTuples = (from member in symbol.GetMembers()
                        where member is IPropertySymbol
                        let attributes = from attr in member.GetAttributes()
                            where attr.AttributeClass.ToDisplayString().StartsWith(AttributeNamespace)
                            select attr
                        select (memberFullName: member.ToDisplayString(),
                                sortName: member.Name,
                                memberType: member,
                                attributes
                            )
                    ).ToList();

                if (GeneratorByMemberType(symbol, valueTuples) is { } source)

                    spc.AddSource(
                        // 不能重名
                        $"{symbol.ToDisplayString()}_{AttributeFullName}.g.cs",
                        source);
            }
        });
    }

    internal abstract string? GeneratorByMemberType(INamedTypeSymbol ClassType,
        List<(string memberFullName, string sortName, ISymbol member, IEnumerable<AttributeData> attributes)>
            valueTuples);
}