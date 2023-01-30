using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class NewTestScript
{
    // A Test behaves as an ordinary method
    [Test]
    public void Test1() {
        // Use the Assert class to test conditions
        Debug.Log("First test!");
        Assert.That(5 * 20, Is.EqualTo(100)); // should pass

    }

    [Test]
    public void Test2() {
        // Use the Assert class to test conditions
        Debug.Log("Second test!");
        Assert.That(4, Is.InRange(5,25)); // should fail

    }

    // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
    // `yield return null;` to skip a frame.
    [UnityTest]
    public IEnumerator NewTestScriptWithEnumeratorPasses()
    {
        // Use the Assert class to test conditions.
        // Use yield to skip a frame.
        yield return null;
    }
}
