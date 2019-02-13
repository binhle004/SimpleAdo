using SimpleAdo.Config;
using NUnit.Framework;

[TestFixture]
public class DefaultConnectionStringTests
{
    [TearDown]
    public void TearDown()
    {
        DefaultConnectionString.SetName("Default");
    }

    [Test]
    public void ConnectionString_SetInvalidName_ThrowsException()
    {
        DefaultConnectionString.SetName("BadConnectionStringName");

        Assert.That(() => DefaultConnectionString.ConnectionString, Throws.ArgumentException);
    }

    [Test]
    public void ConnectionString_SetValidName_ReturnsNewConnectionString()
    {
        DefaultConnectionString.SetName("NewValidName");

        DefaultConnectionString.ConnectionString.WriteToConsole();

        Assert.That(DefaultConnectionString.ConnectionString, Contains.Substring("NewDataSource"));
    }

    [Test]
    public void ConnectionString_SetApplicationName_ConnectionStringContainsNewName()
    {
        DefaultConnectionString.SetApplicationName("NewApplicationName");

        Assert.That(DefaultConnectionString.ConnectionString, Contains.Substring("NewApplicationName"));
    }
}