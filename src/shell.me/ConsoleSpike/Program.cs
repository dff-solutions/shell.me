using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConsoleSpike
{
    class Program
    {
        static void Main(string[] args)
        {
            var console = new NativeConsole();

            var consoleWrapper = new ConsoleController(console);

            consoleWrapper.WriteLine("This is it!");

            while (true)
            {
                consoleWrapper.ReadLine();
                var color = consoleWrapper.ForegroundColor;
                consoleWrapper.ForegroundColor = ConsoleColor.Red;
                consoleWrapper.WriteLine("foo");
                consoleWrapper.ForegroundColor = color;
            }

            //ConsoleKeyInfo pressedKey = default(ConsoleKeyInfo);
            //while (pressedKey.Key != ConsoleKey.Escape)
            //{
            //    pressedKey = console.Read();
            //    if (pressedKey.IsPrintable())
            //        console.WriteAtCursorAndMove(pressedKey.KeyChar);
            //}
        }
    }
}
