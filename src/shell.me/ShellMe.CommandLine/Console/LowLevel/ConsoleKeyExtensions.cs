using System;

namespace ShellMe.CommandLine.Console.LowLevel
{
    public static class ConsoleKeyExtensions
    {
        public static bool IsPrintable(this ConsoleKeyInfo keyInfo)
        {
            return keyInfo.Key == ConsoleKey.A ||
                   keyInfo.Key == ConsoleKey.B ||
                   keyInfo.Key == ConsoleKey.C ||
                   keyInfo.Key == ConsoleKey.D ||
                   keyInfo.Key == ConsoleKey.E ||
                   keyInfo.Key == ConsoleKey.F ||
                   keyInfo.Key == ConsoleKey.G ||
                   keyInfo.Key == ConsoleKey.H ||
                   keyInfo.Key == ConsoleKey.I ||
                   keyInfo.Key == ConsoleKey.J ||
                   keyInfo.Key == ConsoleKey.K ||
                   keyInfo.Key == ConsoleKey.L ||
                   keyInfo.Key == ConsoleKey.M ||
                   keyInfo.Key == ConsoleKey.N ||
                   keyInfo.Key == ConsoleKey.O ||
                   keyInfo.Key == ConsoleKey.P ||
                   keyInfo.Key == ConsoleKey.Q ||
                   keyInfo.Key == ConsoleKey.R ||
                   keyInfo.Key == ConsoleKey.S ||
                   keyInfo.Key == ConsoleKey.T ||
                   keyInfo.Key == ConsoleKey.U ||
                   keyInfo.Key == ConsoleKey.V ||
                   keyInfo.Key == ConsoleKey.W ||
                   keyInfo.Key == ConsoleKey.X ||
                   keyInfo.Key == ConsoleKey.Y ||
                   keyInfo.Key == ConsoleKey.Z ||
                   keyInfo.Key == ConsoleKey.D ||
                   keyInfo.Key == ConsoleKey.D1 ||
                   keyInfo.Key == ConsoleKey.D2 ||
                   keyInfo.Key == ConsoleKey.D3 ||
                   keyInfo.Key == ConsoleKey.D4 ||
                   keyInfo.Key == ConsoleKey.D5 ||
                   keyInfo.Key == ConsoleKey.D6 ||
                   keyInfo.Key == ConsoleKey.D7 ||
                   keyInfo.Key == ConsoleKey.D8 ||
                   keyInfo.Key == ConsoleKey.D9 ||
                   keyInfo.Key == ConsoleKey.D0 ||
                   keyInfo.Key == ConsoleKey.OemComma ||
                   keyInfo.Key == ConsoleKey.OemMinus ||
                   keyInfo.Key == ConsoleKey.OemPlus ||
                   keyInfo.Key == ConsoleKey.OemPeriod ||
                   keyInfo.Key == ConsoleKey.Oem2 ||
                   keyInfo.Key == ConsoleKey.Oem4 ||
                   keyInfo.Key == ConsoleKey.Oem5 ||
                   keyInfo.Key == ConsoleKey.Oem6 ||
                   keyInfo.Key == ConsoleKey.Oem102 ||
                   keyInfo.Key == ConsoleKey.Spacebar;
        }
    }
}
