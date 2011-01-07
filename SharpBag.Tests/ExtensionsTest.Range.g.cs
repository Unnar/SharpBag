// <copyright file="ExtensionsTest.Range.g.cs" company="SuprDewd">Copyright � SuprDewd 2010</copyright>
// <auto-generated>
// This file contains automatically generated unit tests.
// Do NOT modify this file manually.
//
// When Pex is invoked again,
// it might remove or update any previously generated unit tests.
//
// If the contents of this file becomes outdated, e.g. if it does not
// compile anymore, you may delete this file and invoke Pex again.
// </auto-generated>
using System;
using System.Collections.Generic;
using Microsoft.Pex.Engine.Exceptions;
using Microsoft.Pex.Framework.Generated;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SharpBag
{
    public partial class ExtensionsTest
    {
        [TestMethod]
        [PexGeneratedBy(typeof(ExtensionsTest))]
        [PexRaisedContractException(PexExceptionState.Expected)]
        public void RangeThrowsContractException560()
        {
            try
            {
                IEnumerable<int> iEnumerable;
                iEnumerable = this.Range<int>((IEnumerable<int>)null, 0, 0);
                throw
                  new AssertFailedException("expected an exception of type ContractException");
            }
            catch (Exception ex)
            {
                if (!PexContract.IsContractException(ex))
                    throw ex;
            }
        }

        [TestMethod]
        [PexGeneratedBy(typeof(ExtensionsTest))]
        [PexRaisedContractException(PexExceptionState.Expected)]
        public void RangeThrowsContractException958()
        {
            try
            {
                IEnumerable<int> iEnumerable;
                int[] ints = new int[0];
                iEnumerable = this.Range<int>((IEnumerable<int>)ints, 0, 0);
                throw
                  new AssertFailedException("expected an exception of type ContractException");
            }
            catch (Exception ex)
            {
                if (!PexContract.IsContractException(ex))
                    throw ex;
            }
        }
[TestMethod]
[PexGeneratedBy(typeof(ExtensionsTest))]
[PexRaisedContractException(PexExceptionState.Expected)]
public void RangeThrowsContractException480()
{
    try
    {
      IEnumerable<int> iEnumerable;
      int[] ints = new int[1];
      iEnumerable = this.Range<int>((IEnumerable<int>)ints, 0, 1);
      throw 
        new AssertFailedException("expected an exception of type ContractException");
    }
    catch(Exception ex)
    {
      if (!PexContract.IsContractException(ex))
        throw ex;
    }
}
[TestMethod]
[PexGeneratedBy(typeof(ExtensionsTest))]
public void Range377()
{
    IEnumerable<int> iEnumerable;
    int[] ints = new int[1];
    iEnumerable = this.Range<int>((IEnumerable<int>)ints, 0, 0);
    Assert.IsNotNull((object)iEnumerable);
}
[TestMethod]
[PexGeneratedBy(typeof(ExtensionsTest))]
[PexRaisedContractException(PexExceptionState.Expected)]
public void RangeThrowsContractException95()
{
    try
    {
      IEnumerable<int> iEnumerable;
      int[] ints = new int[0];
      iEnumerable = this.Range<int>((IEnumerable<int>)ints, -2147483647, 0);
      throw 
        new AssertFailedException("expected an exception of type ContractException");
    }
    catch(Exception ex)
    {
      if (!PexContract.IsContractException(ex))
        throw ex;
    }
}
    }
}