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
  }
}
