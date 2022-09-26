using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Text.Json;
using System.Runtime.InteropServices;

namespace XGBoost.lib
{
  public static class XGBoostV
  {
  	public static System.Version Version()
	{
	  int major, minor, patch;
          XGBOOST_NATIVE_METHODS.XGBoostVersion(out major, out minor, out patch);
	  return new System.Version(major, minor, patch);
	}
  }
  
  public class Booster : IDisposable
  {
    private bool disposed;
    private readonly IntPtr handle;
    private const int normalPrediction = 0;  // optionMask value for XGBoosterPredict
    private int numClass = 1;

    public IntPtr Handle => handle;

    public Booster(IDictionary<string, object> parameters, DMatrix train)
    {
      var dmats = new [] { train.Handle };
      var len = unchecked((ulong)dmats.Length);
      var output = XGBOOST_NATIVE_METHODS.XGBoosterCreate(dmats, len, out handle);
      if (output == -1) throw new DllFailException(XGBOOST_NATIVE_METHODS.XGBGetLastError());
      
      SetParameters(parameters);
    }

    public Booster(DMatrix train)
    {
        var dmats = new[] { train.Handle };
        var len = unchecked((ulong)dmats.Length);
        var output = XGBOOST_NATIVE_METHODS.XGBoosterCreate(dmats, len, out handle);
        if (output == -1) throw new DllFailException(XGBOOST_NATIVE_METHODS.XGBGetLastError());
    }

    public Booster(string fileName, int silent = 1)
    {
        IntPtr tempPtr;
        var newBooster = XGBOOST_NATIVE_METHODS.XGBoosterCreate(null, 0,out tempPtr); 
        var output = XGBOOST_NATIVE_METHODS.XGBoosterLoadModel(tempPtr, fileName);
        if (output == -1) throw new DllFailException(XGBOOST_NATIVE_METHODS.XGBGetLastError());
        handle = tempPtr;
    }

    public void Update(DMatrix train, int iter)
    {
      var output = XGBOOST_NATIVE_METHODS.XGBoosterUpdateOneIter(Handle, iter, train.Handle);
      if (output == -1) throw new DllFailException(XGBOOST_NATIVE_METHODS.XGBGetLastError());
    }

    public float[] Predict(DMatrix test)
    {
      ulong predsLen;
      IntPtr predsPtr;
      var ss = DumpModelEx("", with_stats: 0, format : "json");
      Console.WriteLine($"****  The booster has {ss.Length} trees.");
      Regex boosterPrefix = new Regex(@"^booster\[\d+\]");
      for (int i = 0; i < ss.Length; i++){
        Console.WriteLine($"****  The booster's tree {i} is '{ss[i]}', which matches: {boosterPrefix.IsMatch(ss[i])}.");
      }
      #if false
      Console.WriteLine("****  Jus ready to call XGBoosterPredict");
      var output = XGBOOST_NATIVE_METHODS.XGBoosterPredict(
          handle, test.Handle, normalPrediction, 0, out predsLen, out predsPtr);
      Console.WriteLine($"****  Return with an output value of {output}.");
      #endif
 #if false
      if (output == -1) throw new DllFailException(XGBOOST_NATIVE_METHODS.XGBGetLastError());
//#else
      if (output == -1) {
      	 var msg = XGBOOST_NATIVE_METHODS.XGBGetLastError();
	 Console.WriteLine($"Got an error message reading '{msg}', now throwing exception");
	 throw new DllFailException(msg);
      }
#endif
      //return GetPredictionsArray(predsPtr, predsLen);
      return null;
    }

    public float[] GetPredictionsArray(IntPtr predsPtr, ulong predsLen)
    {
      var length = unchecked((int)predsLen);
      var preds = new float[length];
      for (var i = 0; i < length; i++)
      {
        var floatBytes = new byte[4];
        for (var b = 0; b < 4; b++)
        {
          floatBytes[b] = Marshal.ReadByte(predsPtr, 4*i + b);
        }
        preds[i] = BitConverter.ToSingle(floatBytes, 0);
      }
      return preds;
    }

    public void SetParameters(IDictionary<string, Object> parameters)
    {
      // support internationalisation i.e. support floats with commas (e.g. 0,5F)
      var nfi = new NumberFormatInfo { NumberDecimalSeparator = "." };

      SetParameter("max_depth", ((int)parameters["max_depth"]).ToString());
      SetParameter("learning_rate", ((float)parameters["learning_rate"]).ToString(nfi));
      SetParameter("n_estimators", ((int)parameters["n_estimators"]).ToString());
      SetParameter("silent", ((bool)parameters["silent"]).ToString());
      SetParameter("objective", (string)parameters["objective"]);
      SetParameter("booster", (string)parameters["booster"]);
      SetParameter("tree_method", (string)parameters["tree_method"]);

      SetParameter("nthread", ((int)parameters["nthread"]).ToString());
      SetParameter("gamma", ((float)parameters["gamma"]).ToString(nfi));
      SetParameter("min_child_weight", ((int)parameters["min_child_weight"]).ToString());
      SetParameter("max_delta_step", ((int)parameters["max_delta_step"]).ToString());
      SetParameter("subsample", ((float)parameters["subsample"]).ToString(nfi));
      SetParameter("colsample_bytree", ((float)parameters["colsample_bytree"]).ToString(nfi));
      SetParameter("colsample_bylevel", ((float)parameters["colsample_bylevel"]).ToString(nfi));
      SetParameter("reg_alpha", ((float)parameters["reg_alpha"]).ToString(nfi));
      SetParameter("reg_lambda", ((float)parameters["reg_lambda"]).ToString(nfi));
      SetParameter("scale_pos_weight", ((float)parameters["scale_pos_weight"]).ToString(nfi));

      SetParameter("base_score", ((float)parameters["base_score"]).ToString(nfi));
      SetParameter("seed", ((int)parameters["seed"]).ToString());
      SetParameter("missing", ((float)parameters["missing"]).ToString(nfi));
      
      SetParameter("sample_type", (string)parameters["sample_type"]);
      SetParameter("normalize_type ", (string)parameters["normalize_type"]);
      SetParameter("rate_drop", ((float)parameters["rate_drop"]).ToString(nfi));
      SetParameter("one_drop", ((int)parameters["one_drop"]).ToString());
      SetParameter("skip_drop", ((float)parameters["skip_drop"]).ToString(nfi));

      if (parameters.TryGetValue("num_class",out var value))
      {
          numClass = (int)value;
          SetParameter("num_class", numClass.ToString());
      }
    }

    // doesn't support floats with commas (e.g. 0,5F)
    public void SetParametersGeneric(IDictionary<string, Object> parameters)
    {
      foreach (var param in parameters)
      {
        if (param.Value != null)
          SetParameter(param.Key, param.Value.ToString());
      }
    }

    public void PrintParameters(IDictionary<string, Object> parameters)
    {
      Console.WriteLine("max_depth: " + (int)parameters["max_depth"]);
      Console.WriteLine("learning_rate: " + (float)parameters["learning_rate"]);
      Console.WriteLine("n_estimators: " + (int)parameters["n_estimators"]);
      Console.WriteLine("silent: " + (bool)parameters["silent"]);
      Console.WriteLine("objective: " + (string)parameters["objective"]);
      Console.WriteLine("booster: " + (string)parameters["booster"]);
      Console.WriteLine("tree_method: " + (string)parameters["tree_method"]);

      Console.WriteLine("nthread: " + (int)parameters["nthread"]);
      Console.WriteLine("gamma: " + (float)parameters["gamma"]);
      Console.WriteLine("min_child_weight: " + (int)parameters["min_child_weight"]);
      Console.WriteLine("max_delta_step: " + (int)parameters["max_delta_step"]);
      Console.WriteLine("subsample: " + (float)parameters["subsample"]);
      Console.WriteLine("colsample_bytree: " + (float)parameters["colsample_bytree"]);
      Console.WriteLine("colsample_bylevel: " + (float)parameters["colsample_bylevel"]);
      Console.WriteLine("reg_alpha: " + (float)parameters["reg_alpha"]);
      Console.WriteLine("reg_lambda: " + (float)parameters["reg_lambda"]);
      Console.WriteLine("scale_pos_weight: " + (float)parameters["scale_pos_weight"]);

      Console.WriteLine("base_score: " + (float)parameters["base_score"]);
      Console.WriteLine("seed: " + (int)parameters["seed"]);
      Console.WriteLine("missing: " + (float)parameters["missing"]);

      Console.WriteLine("sample_type: " + ((float)parameters["sample_type"]));
      Console.WriteLine("normalize_type: " + ((float)parameters["normalize_type"]));
      Console.WriteLine("rate_drop: ", + ((float)parameters["rate_drop"]));
      Console.WriteLine("one_drop: ", +((int)parameters["one_drop"]));
      Console.WriteLine("skip_drop: ", +((float)parameters["skip_drop"]));
    }

    public void SetParameter(string name, string val)
    {
      int output = XGBOOST_NATIVE_METHODS.XGBoosterSetParam(handle, name, val);
      if (output == -1) throw new DllFailException(XGBOOST_NATIVE_METHODS.XGBGetLastError());
    }

    public void Save(string fileName)
    {
      XGBOOST_NATIVE_METHODS.XGBoosterSaveModel(handle, fileName);
    }

    public string[] DumpModelEx(string fmap, int with_stats, string format)
    {
        int length;
        IntPtr treePtr;
        var intptrSize = IntPtr.Size;
	#if false
        XGBOOST_NATIVE_METHODS.XGBoosterDumpModel(handle, fmap, with_stats, out length, out treePtr);
	#else
	        XGBOOST_NATIVE_METHODS.XGBoosterDumpModelEx(handle, fmap, with_stats, format, out length, out treePtr);
		#endif
        var trees = new string[length];
        int readSize = 0;
        var handle2 = GCHandle.Alloc(treePtr, GCHandleType.Pinned);
        
        //iterate through the length of the tree ensemble and pull the strings out from the returned pointer's array of pointers. prepend python's api convention of adding booster[i] to the beginning of the tree
        for (var i = 0; i < length; i++)
        {
            var ipt1 = Marshal.ReadIntPtr(Marshal.ReadIntPtr(handle2.AddrOfPinnedObject()), intptrSize * i);
            string s = Marshal.PtrToStringAnsi(ipt1);
            trees[i] = string.Format("booster[{0}]\n{1}", i, s);
            var bytesToRead = (s.Length * 2) + IntPtr.Size;
            readSize += bytesToRead;
        }
        handle2.Free();
        return trees;
    }

    public void GetModel()
    {
      var ss = DumpModelEx("", with_stats: 0, format : "json");
      Console.WriteLine($"****  The booster has {ss.Length} trees.");
      var boosterPattern = @"^booster\[\d+\]";
      List<TreeNode> ensemble = new List<TreeNode>(); // should probably return this

      for (int i = 0; i < ss.Length; i++){
        var m = Regex.Matches(ss[i], boosterPattern, RegexOptions.IgnoreCase);
	if ((m.Count >= 1) && (m[0].Groups.Count >=1)) {
	  var structString = ss[i].Substring(m[0].Groups[0].Value.Length);
	  var doc = JsonDocument.Parse(structString);
	  TreeNode t = TreeNode.Create(doc.RootElement);
	  ensemble.Add(t);
	  Console.WriteLine($"**** {i} got a tree {t.NodeId}");
	} else {
          Console.WriteLine($"****  The booster's tree {i} is '{ss[i]}'didn't match.");
	}
      }

      Console.WriteLine($"**** ensemble has {ensemble.Count} trees.");
    }

    // Dispose pattern from MSDN documentation
    public void Dispose()
    {
      Dispose(true);
      GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
      if (disposed) return;
      XGBOOST_NATIVE_METHODS.XGBoosterFree(handle);
      disposed = true;
    }
  }
}
