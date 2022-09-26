using System.IO;
using XGBoost.lib;
using XGBoost;

namespace XGBoostTestsX;

public class XGBClassifierTests
{
    [Fact]
    public void TestVersion()
    {
       var ver = XGBoostV.Version();
       Assert.True(ver.Major >= 2);
    }
    
    [Fact]
    public void SanityTest()
    {
    
      Assert.True(Directory.Exists(TestUtils.GetDataPath()));
      var trainData = TestUtils.GetClassifierDataTrain();
      Assert.Equal(891, trainData.Length);
      var trainLabels = TestUtils.GetClassifierLabelsTrain();
      Assert.Equal(891, trainLabels.Length);

      var testData = TestUtils.GetClassifierDataTest();
      Assert.Equal(418, testData.Length);
    }

    [Fact]
    public void DumpModel()
    {
      var dataTrain = TestUtils.GetClassifierDataTrain();
      var labelsTrain = TestUtils.GetClassifierLabelsTrain();
      var dataTest = TestUtils.GetClassifierDataTest();

      Console.WriteLine("****** Initializing classifier");

      var xgbc = new XGBClassifier();
      Console.WriteLine("****** Running fit");
      xgbc.Fit(dataTrain, labelsTrain);
      Console.WriteLine("****** Dumping model");
      xgbc.GetModel();
      //Assert.True(TestUtils.ClassifierPredsCorrect(preds));
    }
}