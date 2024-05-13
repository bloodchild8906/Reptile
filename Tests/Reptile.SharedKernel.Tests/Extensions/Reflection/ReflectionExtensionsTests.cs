using System.Linq.Expressions;
using FluentAssertions;
using Reptile.SharedKernel.Extensions.Reflection;

namespace Reptile.SharedKernel.Tests.Extensions.Reflection;

[TestFixture]
public class ReflectionExtensionsTests
{
    [Test(Description = "Get property info from expression")]
    [Category("GetProperty")]
    public void GetProperty_Returns_PropertyInfo()
    {
        // Arrange
        var obj = new TestClass { Property = "Value" };
        var expression = (Expression<Func<TestClass, string>>)(x => x.Property);

        // Act
        var propertyInfo = obj.GetProperty(expression);

        // Assert
        propertyInfo.Should().NotBeNull();
        propertyInfo.Name.Should().Be("Property");
    }

    /* Implement this function */
    [Test(Description = "Get attribute from property")]
    [Category("GetAttribute")]
    public void GetAttribute_Returns_Attribute()
    {
        // Arrange
        var obj = new TestClass { Property = "Value" };

        var expression = (Expression<Func<TestClass, string>>)(x => x.Property);
        var propertyInfo = obj.GetProperty(expression);

        // Act
        var attribute = propertyInfo.GetAttribute<ObsoleteAttribute>();

        // Assert
        attribute.Should().BeNull("Because ObsoleteAttribute is not Applied on Property of TestClass.");
    }

    [Test(Description = "Check if property type is in System namespace")]
    [Category("IsSystem")]
    public void IsSystem_Returns_True_For_System_Type()
    {
        // Arrange
        var obj = new TestClass { Property = "Value" };
        var expression = (Expression<Func<TestClass, string>>)(x => x.Property);
        var propertyInfo = obj.GetProperty(expression);

        // Act
        var isSystemType = propertyInfo.IsSystem();

        // Assert
        isSystemType.Should().BeTrue("Because Property of TestClass is System.String which is in System namespace.");
    }


    [Test(Description = "Check if public instance properties are equal")]
    [Category("PublicInstancePropertiesEqual")]
    public void PublicInstancePropertiesEqual_Returns_True_For_Equal_Objects()
    {
        // Arrange
        var obj1 = new TestClass { Property = "Value" };
        var obj2 = new TestClass { Property = "Value" };

        // Act
        var areEqual = obj1.PublicInstancePropertiesEqual(obj2);

        // Assert
        areEqual.Should().BeTrue();
    }

    [Test(Description = "Check if public instance properties are equal")]
    [Category("PublicInstancePropertiesEqual")]
    public void PublicInstancePropertiesEqual_Returns_False_For_Unequal_Objects()
    {
        // Arrange
        var obj1 = new TestClass { Property = "Value" };
        var obj2 = new TestClass { Property = "DifferentValue" };

        // Act
        var areEqual = obj1.PublicInstancePropertiesEqual(obj2);

        // Assert
        areEqual.Should().BeFalse();
    }
}