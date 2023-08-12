using System.Collections;
using _Scripts;
using NUnit.Framework;
using UnityEngine.TestTools;

namespace Tests.PlayMode
{
    public class BoardTests
    {

        [UnityTest]
        public IEnumerator BoardTest1()
        {
            Assert.AreEqual(1,1); 
            yield return null;
        }
    }
}
