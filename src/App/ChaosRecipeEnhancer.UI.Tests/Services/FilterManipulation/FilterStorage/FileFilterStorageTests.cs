//using ChaosRecipeEnhancer.UI.Services.FilterManipulation.FilterStorage;
//using Moq;
//using System.IO.Abstractions;

//namespace ChaosRecipeEnhancer.UI.Tests.Services.FilterManipulation.FilterStorage;

//public class FileFilterStorageTests
//{
//    [Fact]
//    public async Task ReadLootFilterAsync_ReturnsFilterContent_WhenFileExists()
//    {
//        // Arrange
//        var fileLocation = "testFilter.txt";
//        var expectedFilter = "Test filter content";
//        var mockFileSystem = new Mock<IFileSystem>();
//        mockFileSystem.Setup(fs => fs.File.Exists(fileLocation)).Returns(true);
//        mockFileSystem.Setup(fs => fs.File.ReadAllTextAsync(fileLocation, default))
//            .ReturnsAsync(expectedFilter);

//        var filterStorage = new FileFilterStorage(fileLocation, mockFileSystem.Object);

//        // Act
//        var result = await filterStorage.ReadLootFilterAsync();

//        // Assert
//        Assert.Equal(expectedFilter, result);
//    }

//    [Fact]
//    public async Task ReadLootFilterAsync_ReturnsNull_WhenFileDoesNotExist()
//    {
//        // Arrange
//        var fileLocation = "nonExistentFilter.txt";
//        var mockFileSystem = new Mock<IFileSystem>();
//        mockFileSystem.Setup(fs => fs.File.Exists(fileLocation)).Returns(false);

//        var filterStorage = new FileFilterStorage(fileLocation, mockFileSystem.Object);

//        // Act
//        var result = await filterStorage.ReadLootFilterAsync();

//        // Assert
//        Assert.Null(result);
//    }

//    [Fact]
//    public async Task WriteLootFilterAsync_ThrowsUnauthorizedAccessException_WhenAccessDenied()
//    {
//        // Arrange
//        var fileLocation = "testFilter.txt";
//        var filterContent = "Test filter content";
//        var mockFileSystem = new Mock<IFileSystem>();
//        mockFileSystem.Setup(fs => fs.File.WriteAllTextAsync(fileLocation, filterContent, default))
//            .ThrowsAsync(new UnauthorizedAccessException());

//        var filterStorage = new FileFilterStorage(fileLocation, mockFileSystem.Object);

//        // Act & Assert
//        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => filterStorage.WriteLootFilterAsync(filterContent));
//    }
//}