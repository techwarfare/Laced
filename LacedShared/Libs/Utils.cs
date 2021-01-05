namespace LacedShared.Libs
{
    using System;
    using CitizenFX.Core;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    public class Utils
    {
        public static void Throw(Exception _ex, string _msg = "")
        {
            string exceptionString = "Error Message: " + _ex.Message + "\n Stack:" + _ex.StackTrace;
            WriteLine($"^1[LEXEC] ^3{ exceptionString} { _msg}");
        }

        public static void WriteLine(string _msg)
        {
            Debug.WriteLine($"^1[LDGB] ^0{ _msg}");
        }

        public static void DebugLine(string _msg, string _className)
        {
            Debug.WriteLine($"^2[LDGB] ^3{ _msg} ^0");
        }

        public static string CreateRandomKey()
        {
            string randomKey = "";
            //Create a random key, maybe 15-20 characters long? Numbers included
            List<string> charList = new List<string>() { "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z" };
            List<string> numList = new List<string>() { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9" };
            for (int i = 0; i < 20; i++)
            {
                int randNum = new Random().Next(0, 1);

                if (randNum == 0)
                {
                    randNum = new Random().Next(0, 25);
                    randomKey = randomKey + charList[randNum];
                }
                else
                {
                    randNum = new Random().Next(0, 9);
                    randomKey = randomKey + numList[randNum];
                }
            }

            return randomKey;
        }
    }
}
