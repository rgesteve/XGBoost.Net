using System.IO;

namespace XGBoostTestsX;

public class UnitTest1
{
    [Fact]
    public void Test1()
    {
      Assert.True(Directory.Exists(TestUtils.GetDataPath()));
      var trainData = TestUtils.GetClassifierDataTrain();
      Assert.Equal(891, trainData.Length);
      var trainLabels = TestUtils.GetClassifierLabelsTrain();
      Assert.Equal(891, trainLabels.Length);
    }
}