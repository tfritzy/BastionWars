using System.Numerics;
using KeepLordWarriors;

namespace Tests;

[TestClass]
public class WordTests
{
    [TestMethod]
    public void Word_IncrementsProgress()
    {
        Word word = new("test", new V2Int(0, 0));
        Assert.AreEqual(0, word.TypedIndex);
        word.HandleKeystroke('t');
        Assert.AreEqual(1, word.TypedIndex);
        word.HandleKeystroke('e');
        Assert.AreEqual(2, word.TypedIndex);
        word.HandleKeystroke('e');
        Assert.AreEqual(0, word.TypedIndex);
        word.HandleKeystroke('t');
        Assert.AreEqual(1, word.TypedIndex);
        word.HandleKeystroke('e');
        Assert.AreEqual(2, word.TypedIndex);
        word.HandleKeystroke('s');
        Assert.AreEqual(3, word.TypedIndex);
        word.HandleKeystroke('t');
        Assert.AreEqual(4, word.TypedIndex);
    }
}