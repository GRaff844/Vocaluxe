﻿using System;
using System.Runtime.InteropServices;

namespace Vocaluxe.Lib.Sound.Record
{
    class CAnalyzer : IDisposable
    {
        #region Imports
        [DllImport("PitchTracker.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr Analyzer_Create(double baseToneFrequency, int minHalfTone, int maxHalfTone, uint step = 200);

        [DllImport("PitchTracker.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void Analyzer_Free(IntPtr analyzer);

        [DllImport("PitchTracker.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void Analyzer_InputFloat(IntPtr analyzer, [In] float[] data, int sampleCt);

        [DllImport("PitchTracker.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void Analyzer_InputShort(IntPtr analyzer, [In] short[] data, int sampleCt);

        [DllImport("PitchTracker.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void Analyzer_InputByte(IntPtr analyzer, [In] byte[] data, int sampleCt);

        [DllImport("PitchTracker.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void Analyzer_Process(IntPtr analyzer);

        [DllImport("PitchTracker.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern double Analyzer_GetPeak(IntPtr analyzer);

        [DllImport("PitchTracker.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern int Analyzer_GetNoteFast(IntPtr analyzer, [Out] out double maxVolume, [Out] float[] weights);

        [DllImport("PitchTracker.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern double Analyzer_FindNote(IntPtr analyzer, double minFreq, double maxFreq);

        [DllImport("PitchTracker.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern bool Analyzer_OutputFloat(IntPtr analyzer, [Out] float[] data, int sampleCt, float rate);
        #endregion

        private IntPtr _Instance;

        public CAnalyzer(double baseToneFrequency, int minHalfTone, int maxHalfTone, uint step = 200)
        {
            _Instance = Analyzer_Create(baseToneFrequency, minHalfTone, maxHalfTone, step);
        }

        ~CAnalyzer()
        {
            _Dispose(false);
        }

        public void Input(float[] data)
        {
            Analyzer_InputFloat(_Instance, data, data.Length);
        }

        public void Input(short[] data)
        {
            Analyzer_InputShort(_Instance, data, data.Length);
        }

        public void Input(byte[] data)
        {
            Analyzer_InputByte(_Instance, data, data.Length / 2);
        }

        public void Process()
        {
            Analyzer_Process(_Instance);
        }

        public double GetPeak()
        {
            return Analyzer_GetPeak(_Instance);
        }

        public double FindNote(double minFreq = 35.0, double maxFreq = 1000.0)
        {
            return Analyzer_FindNote(_Instance, minFreq, maxFreq);
        }

        public int GetNoteFast(out double maxVolume, float[] weights)
        {
            return Analyzer_GetNoteFast(_Instance, out maxVolume, weights);
        }

        public bool Output(float[] data, float rate)
        {
            return Analyzer_OutputFloat(_Instance, data, data.Length, rate);
        }

        private void _Dispose(bool disposing)
        {
            if (_Instance == IntPtr.Zero)
                throw new ObjectDisposedException(GetType().Name);
            Analyzer_Free(_Instance);
            _Instance = IntPtr.Zero;
        }

        public void Dispose()
        {
            _Dispose(true);
            GC.SuppressFinalize(this);
        }
    }

    static class CFastAnalyzer
    {
        #region Imports
        [DllImport("PitchTracker.dll", EntryPoint = "PtFast_Init", CallingConvention = CallingConvention.Cdecl)]
        public static extern void Init(double baseToneFrequency, int minHalfTone, int maxHalfTone);

        [DllImport("PitchTracker.dll", EntryPoint = "PtFast_DeInit", CallingConvention = CallingConvention.Cdecl)]
        public static extern void DeInit();

        [DllImport("PitchTracker.dll", EntryPoint = "PtFast_GetTone", CallingConvention = CallingConvention.Cdecl)]
        private static extern int PtFast_GetTone([In] short[] samples, int sampleCt, [Out] out double maxVolume, [Out] float[] weights);
        #endregion

        public static int GetTone(short[] samples, out double maxVolume, float[] weights)
        {
            return PtFast_GetTone(samples, samples.Length, out maxVolume, weights);
        }
    }
}