﻿using System;
using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DisposableFixer.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace DisposableFixer.CodeFix
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(UndisposedMemberCodeFixProvider)), Shared]
    public class UndisposedPropertyCodeFixProvider : UndisposedMemberCodeFixProvider
    {
        public override ImmutableArray<string> FixableDiagnosticIds { get; } =
            ImmutableArray.Create(
                Id.ForAssignment.FromMethodInvocation.ToProperty.OfSameType,
                Id.ForAssignment.FromObjectCreation.ToProperty.OfSameType
            );

        public override Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var id = context.Diagnostics.First().Id;
            if (id == Id.ForAssignment.FromObjectCreation.ToProperty.OfSameType
                || id == Id.ForAssignment.FromMethodInvocation.ToProperty.OfSameType)
            {
                context.RegisterCodeFix(
                    CodeAction.Create(
                        ActionTitle.DisposePropertyInDisposeMethod, 
                        c => CreateDisposeCallInParameterlessDisposeMethod(context, c),
                        Guid.NewGuid().ToString()),
                    context.Diagnostics);
            }

            return Task.FromResult(1);
        }

        protected override TypeSyntax GetTypeOfMemberDeclarationOrDefault(ClassDeclarationSyntax @class, string memberName)
        {
            return @class
                .DescendantNodes<PropertyDeclarationSyntax>()
                .FirstOrDefault(pds => pds.Identifier.Text == memberName)
                ?.Type;
        }
    }
}