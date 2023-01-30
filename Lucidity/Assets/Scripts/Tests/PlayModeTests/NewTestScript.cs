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
        Debug.Log("First Play Mode test!");
        List<string> list = new List<string>() {
            "carrot",
            "octopus",
            "computer"
        };
        Assert.That(list, Has.Member("octopus")); // should pass

    }

    [Test]
    public void Test2() {
        // Use the Assert class to test conditions
        Debug.Log("Second Play Mode test!");
        int c = 4;
        Assert.That(c, Is.Not.EqualTo(3)); // should pass

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
