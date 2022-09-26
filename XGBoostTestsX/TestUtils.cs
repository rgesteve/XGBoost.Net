using System;
using System.Linq;
using NotVisualBasic.FileIO;

namespace XGBoostTestsX
{
  public static class TestUtils
  {

    public static string GetDataPath()
    {
	// var dataRootPath = Path.Combine((Environment.GetEnvironmentVariable("USERPROFILE")??""), "Documents", "Projects");
	var dataRootPath = Path.Combine("/data", "projects", "XGBoost.Net", "XGBoostTests", "test_files");
	return dataRootPath;
    }

    public static float[][] GetClassifierDataTrain()
    {
      var trainCols = 4;
      var trainRows = 891;

      var dataTrain = new float[trainRows][];
      var trainFilePath = Path.Combine(GetDataPath(), "train.csv");

      using (var parser = new CsvTextFieldParser(trainFilePath)) {
        //parser.Delimiters = new[] string() {","};

	var row = 0;

	while (!parser.EndOfData) {
	  dataTrain[row] = new float[trainCols - 1];
	  var fields = parser.ReadFields();

	  // skip label column in csv file
	  for (var col = 1; col < fields.Length; col++)
	    dataTrain[row][col - 1] = float.Parse(fields[col]);
	  row += 1;
	}
	return dataTrain;
      }
    }

    public static float[] GetClassifierLabelsTrain()
    {
      var trainRows = 891;

      var labelsTrain = new float[trainRows];
      var trainFilePath = Path.Combine(GetDataPath(), "train.csv");

      using (var parser = new CsvTextFieldParser(trainFilePath)) {
        //parser.Delimiters = new[] string() {","};

	var row = 0;

	while (!parser.EndOfData) {
	  var fields = parser.ReadFields();
	  labelsTrain[row] = float.Parse(fields[0]);
	  row += 1;
	}
	return labelsTrain;
      }
    }

    public static float[][] GetClassifierDataTest()
    {
      var testCols = 3;
      var testRows = 418;

      var dataTest = new float[testRows][];
      var testFilePath = Path.Combine(GetDataPath(), "test.csv");

      using (var parser = new CsvTextFieldParser(testFilePath)) {
        //parser.Delimiters = new[] string() {","};

	var row = 0;

        while (!parser.EndOfData) {
          dataTest[row] = new float[testCols];
          var fields = parser.ReadFields();

          for (var col = 0; col < fields.Length; col++)
            dataTest[row][col] = float.Parse(fields[col]);
          row += 1;
        }

	return dataTest;
      }
    }

    public static bool ClassifierPredsCorrect(float[] preds) {
      var predFilePath = Path.Combine(GetDataPath(), "predsclas.csv");
      using (var parser = new CsvTextFieldParser(predFilePath)) {
        var row = 0;
        var predInd = 0;

        while (!parser.EndOfData) {
          var fields = parser.ReadFields();

          for (var col = 0; col < fields.Length; col++) {
            var absDiff = Math.Abs(float.Parse(fields[col]) - preds[predInd]);
            if (absDiff > 0.01F)
              return false;
            predInd += 1;
          }
          row += 1;
        }
      }
      return true; // we haven't returned from a wrong prediction so everything is right
    }

  }
}
