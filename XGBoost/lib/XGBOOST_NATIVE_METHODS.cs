﻿using System;
using System.Runtime.InteropServices;

namespace XGBoost.lib
{
  public class XGBOOST_NATIVE_METHODS
  {
//    private const string dllLocation = "libxgboost.dll";
      private const string dllLocation = "/data/projects/xgboost/build_docker/install/lib/libxgboost.so";
    
    static XGBOOST_NATIVE_METHODS() {
    #if false
    DllLoader.LoadXGBoostDll();
    #endif
    }

    [DllImport(dllLocation)]
    public static extern void XGBoostVersion(out int major, out int minor, out int patch);

    [DllImport(dllLocation)]
    public static extern string XGBGetLastError();

    [DllImport(dllLocation)]
    public static extern int XGDMatrixCreateFromMat(float[] data, ulong nrow, ulong ncol, 
                                                    float missing, out IntPtr handle);

    [DllImport(dllLocation)]
    public static extern int XGDMatrixFree(IntPtr handle);

    [DllImport(dllLocation)]
    public static extern int XGDMatrixGetFloatInfo(IntPtr handle, string field, 
                                                   out ulong len, out IntPtr result);

    [DllImport(dllLocation)]
    public static extern int XGDMatrixSetFloatInfo(IntPtr handle, string field,
                                                   float[] array, ulong len);

    [DllImport(dllLocation)]
    public static extern int XGBoosterCreate(IntPtr[] dmats, 
                                             ulong len, out IntPtr handle);

    [DllImport(dllLocation)]
    public static extern int XGBoosterFree(IntPtr handle);

    [DllImport(dllLocation)]
    public static extern int XGBoosterSetParam(IntPtr handle, string name, string val);

    [DllImport(dllLocation)]
    public static extern int XGBoosterUpdateOneIter(IntPtr bHandle, int iter, 
                                                    IntPtr dHandle);

    [DllImport(dllLocation)]
    public static extern int XGBoosterPredict(IntPtr bHandle, IntPtr dHandle, 
                                              int optionMask, int ntreeLimit, 
                                              out ulong predsLen, out IntPtr predsPtr);

    [DllImport(dllLocation)]
    public static extern int XGBoosterSaveModel(IntPtr bHandle, string fileName);

    [DllImport(dllLocation)]
    public static extern int XGBoosterLoadModel(IntPtr bHandle, string fileName);

    [DllImport(dllLocation)]
    public static extern int XGDMatrixCreateFromFile(string fname, int silent, out IntPtr DMtrxHandle);

    [DllImport(dllLocation)]
    public static extern int XGBoosterDumpModel(IntPtr handle, string fmap, int with_stats, out int out_len, out IntPtr dumpStr);

    [DllImport(dllLocation)]
    public static extern int XGBoosterDumpModelEx(IntPtr handle, string fmap, int with_stats, string format, out int out_len, out IntPtr dumpStr);
  }

}
