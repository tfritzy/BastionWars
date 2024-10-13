using System.Numerics;
using KeepLordWarriors;

namespace Tests;

[TestClass]
public class FieldTests
{
    [TestMethod]
    public void Field_IncrementsProgress()
    {
        Field field = new(new Vector2Int(0, 0));
        Assert.AreEqual(0, field.TypedIndex);
        field.HandleKeystroke(field.Text[0]);
        Assert.AreEqual(1, field.TypedIndex);
        field.HandleKeystroke('.');
        Assert.AreEqual(1, field.TypedIndex);
        field.HandleKeystroke(field.Text[1]);
        Assert.AreEqual(2, field.TypedIndex);
    }
}