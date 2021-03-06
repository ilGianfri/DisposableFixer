﻿using System;
using NUnit.Framework;

namespace DisposableFixer.Test.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class Category3rdPartyAttribute : CategoryAttribute
    {
    }

    [AttributeUsage(AttributeTargets.Class|AttributeTargets.Method)]
    public class CategoryReactiveExtensionAttribute : Category3rdPartyAttribute
    {
    }

    public abstract class UsingSyntaxAttribute : CategoryAttribute
    {

    }


    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class UsingStatementAttribute : UsingSyntaxAttribute
    {
        
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class UsingDeclarationStatementAttribute : UsingSyntaxAttribute
    {

    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class HiddenDisposableAttribute : Attribute
    {

    }
}