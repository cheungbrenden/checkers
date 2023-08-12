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
        
        var x = BoardManager.Instance.Tiles;
        // Assert.AreEqual(BoardManager.Instance.Tiles, new Tile[8,8]);
    }
    
}
