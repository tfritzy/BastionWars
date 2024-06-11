using KeepLordWarriors;

namespace Tests;

[TestClass]
public class BastionTests
{
    [TestMethod]
    public void Bastion_BasicStuff()
    {
        Bastion bastion = new(SoldierType.Mage);
        Assert.AreEqual(0, bastion.MageCount);
        Assert.AreEqual(0, bastion.ArcherCount);
        Assert.AreEqual(0, bastion.WarriorCount);
        Assert.AreEqual(SoldierType.Mage, bastion.SoldierType);
        Assert.AreEqual(1u, bastion.Id);
        Bastion bastion2 = new(SoldierType.Archer, 3);
        Assert.AreEqual(2u, bastion2.Id);
        Assert.AreEqual(3, bastion2.Alliance);
    }

    [TestMethod]
    public void Bastion_IncreasesPopulationAppropriately()
    {
        foreach (var type in Enum.GetValues<SoldierType>())
        {
            Bastion bastion = new(type);
            Assert.AreEqual(0, bastion.GetCount(type));
            bastion.Produce();
            Assert.AreEqual(1, bastion.GetCount(type));
        }
    }
}