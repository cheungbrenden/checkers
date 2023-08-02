using System.Collections;
using System.Collections.Generic;
using _Scripts;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class someTestScript
{

    [Test]
    public void someTestScriptSimplePasses()
    {
        Assert.AreEqual(1, 1);
    }
    
}
