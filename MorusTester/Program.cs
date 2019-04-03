using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MorusManaged;

namespace MorusTester
{
    public class Program
    {
        const int MAXTESTS = TestCases.MAXTESTS;

        static void Main(string[] args)
        {
            TestCases tc = new TestCases();

            Test640128(tc);
            Test1280128(tc);
            Test1280256(tc);

            while (Console.KeyAvailable) Console.ReadKey(true);
            Console.WriteLine("Press ENTER to quit...");
            Console.ReadLine();
        }

        static void Test640128(TestCases tc)
        {
            byte[] output = null;
            byte[] returnTrip = null;
            WriteThis("640 128 Tests ".PadRight(40, '='), true, WType.Heading);
            for (int i = 0; i < MAXTESTS; i++) //< tc.TestCases1280256.Length; i++)
            {
                WriteThis("Test #" + i.ToString("00") + ": ", false, WType.Heading);
                TestCases.TestCase t = tc.TestCases640128[i];

                if (Morus_640_128.Encrypt(t.AD, t.P, t.K, t.IV, out output) != 0)
                    WriteThis("*Bad Return Result!*", true, WType.Error);
                else if (TestCases.BytesToHexString(output).CompareTo(
                    TestCases.BytesToHexString(t.C) + TestCases.BytesToHexString(t.T)) != 0)
                {
                    WriteThis("CipherText/Tag *FAIL*!", true, WType.Error);
                }
                else
                {
                    WriteThis("CipherText/Tag, ", false, WType.Awesome);
                    if (Morus_640_128.Decrypt(t.AD, output, t.K, t.IV, out returnTrip) != 0)
                        WriteThis("Tag Verify *FAIL*!", true, WType.Error);
                    else
                    {
                        WriteThis("Tag Verify, ", false, WType.Awesome);
                        string expected = TestCases.BytesToHexString(t.P);
                        string whatCameOut = TestCases.BytesToHexString(returnTrip);
                        if (expected.CompareTo(whatCameOut) != 0)
                            WriteThis("Plaintext *FAIL*!", true, WType.Error);
                        else
                            WriteThis("Plaintext. -PASS-", true, WType.Awesome);
                    }
                }
            }

            WriteThis("640 128 Detached Tests".PadRight(40, '-'), true, WType.Heading);
            for (int i = 0; i < MAXTESTS; i++)
            {
                WriteThis("Test #" + i.ToString("00") + ": ", false, WType.Heading);
                TestCases.TestCase t = tc.TestCases640128[i];
                output = null;
                returnTrip = null;
                if (Morus_640_128.Encrypt_Detached(t.AD, t.P, t.K, t.IV, out output, out byte[] outTag) != 0)
                    WriteThis("*Bad Return Result!*", true, WType.Error);
                else if (TestCases.BytesToHexString(output).CompareTo(TestCases.BytesToHexString(t.C)) != 0)
                {
                    WriteThis("CipherText *FAIL*!", true, WType.Error);
                }
                else
                {
                    WriteThis("CipherText, ", false, WType.Awesome);
                    if (TestCases.BytesToHexString(outTag).CompareTo(TestCases.BytesToHexString(t.T)) != 0)
                    {
                        WriteThis("Detached Tag *FAIL*!", true, WType.Error);
                    }
                    else
                    {
                        WriteThis("Detached Tag, ", false, WType.Awesome);
                        if (Morus_640_128.Decrypt_Detached(t.AD, output, outTag, t.K, t.IV, out returnTrip) != 0)
                            WriteThis("Tag Verify *FAIL*!", true, WType.Error);
                        else
                        {
                            WriteThis("Tag Verify, ", false, WType.Awesome);
                            string expected = TestCases.BytesToHexString(t.P);
                            string whatCameOut = TestCases.BytesToHexString(returnTrip);
                            if (expected.CompareTo(whatCameOut) != 0)
                                WriteThis("Plaintext *FAIL*!", true, WType.Error);
                            else
                                WriteThis("Plaintext. -PASS-", true, WType.Awesome);
                        }
                    }
                }
            }

            WriteThis("640 128 Streams Tests ".PadRight(40, '-'), true, WType.Heading);
            for (int i = 0; i < MAXTESTS; i++) //< tc.TestCases1280256.Length; i++)
            {
                WriteThis("Stream Test #" + i.ToString("00") + ": ", false, WType.Heading);
                TestCases.TestCase t = tc.TestCases640128[i];
                MemoryStream outputStream = new MemoryStream((t.P?.Length ?? 0) + 16);
                MemoryStream AD = t.AD != null ? new MemoryStream(t.AD) : null;
                MemoryStream P = t.P != null ? new MemoryStream(t.P) : null;
                if (Morus_640_128.EncryptStream(AD, P, t.K, t.IV, outputStream) != 0)
                    WriteThis("*Bad Return Result!", true, WType.Error);
                else if (TestCases.BytesToHexString(outputStream.ToArray()).CompareTo(
                    TestCases.BytesToHexString(t.C) + TestCases.BytesToHexString(t.T)) != 0)
                {
                    WriteThis("CipherText/Tag *FAIL*!", true, WType.Error);
                }
                else
                {
                    outputStream.Seek(0, SeekOrigin.Begin);
                    AD?.Seek(0, SeekOrigin.Begin);
                    MemoryStream returnStream = new MemoryStream(new byte[t.P?.Length ?? 0], true);
                    WriteThis("CipherText/Tag, ", false, WType.Awesome);
                    if (Morus_640_128.DecryptStream(AD, outputStream, t.K, t.IV, returnStream) != 0)
                        WriteThis("Tag Verify *FAIL*!", true, WType.Error);
                    else
                    {
                        WriteThis("Tag Verify, ", false, WType.Awesome);
                        string expected = TestCases.BytesToHexString(t.P);
                        string whatCameOut = TestCases.BytesToHexString(returnStream.ToArray());
                        if (expected.CompareTo(whatCameOut) != 0)
                            WriteThis("Plaintext *FAIL*!", true, WType.Error);
                        else
                            WriteThis("Plaintext. -PASS-", true, WType.Awesome);
                    }
                }
            }
        }

        static void Test1280128(TestCases tc)
        {
            byte[] output = null;
            byte[] returnTrip = null;
            WriteThis("1280 128 Tests ".PadRight(40, '='), true, WType.Heading);
            for (int i = 0; i < MAXTESTS; i++) //< tc.TestCases1280256.Length; i++)
            {
                WriteThis("Test #" + i.ToString("00") + ": ", false, WType.Heading);
                TestCases.TestCase t = tc.TestCases1280128[i];

                if (Morus_1280_128.Encrypt(t.AD, t.P, t.K, t.IV, out output) != 0)
                    WriteThis("*Bad Return Result!*", true, WType.Error);
                else if (TestCases.BytesToHexString(output).CompareTo(
                    TestCases.BytesToHexString(t.C) + TestCases.BytesToHexString(t.T)) != 0)
                {
                    WriteThis("CipherText/Tag *FAIL*!", true, WType.Error);
                }
                else
                {
                    WriteThis("CipherText/Tag, ", false, WType.Awesome);
                    if (Morus_1280_128.Decrypt(t.AD, output, t.K, t.IV, out returnTrip) != 0)
                        WriteThis("Tag Verify *FAIL*!", true, WType.Error);
                    else
                    {
                        WriteThis("Tag Verify, ", false, WType.Awesome);
                        string expected = TestCases.BytesToHexString(t.P);
                        string whatCameOut = TestCases.BytesToHexString(returnTrip);
                        if (expected.CompareTo(whatCameOut) != 0)
                            WriteThis("Plaintext *FAIL*!", true, WType.Error);
                        else
                            WriteThis("Plaintext. -PASS-", true, WType.Awesome);
                    }
                }
            }

            WriteThis("1280 128 Detached Tests ".PadRight(40, '-'), true, WType.Heading);
            for (int i = 0; i < MAXTESTS; i++)
            {
                WriteThis("Test #" + i.ToString("00") + ": ", false, WType.Heading);
                TestCases.TestCase t = tc.TestCases1280128[i];
                output = null;
                returnTrip = null;
                if (Morus_1280_128.Encrypt_Detached(t.AD, t.P, t.K, t.IV, out output, out byte[] outTag) != 0)
                    WriteThis("*Bad Return Result!*", true, WType.Error);
                else if (TestCases.BytesToHexString(output).CompareTo(TestCases.BytesToHexString(t.C)) != 0)
                {
                    WriteThis("CipherText *FAIL*!", true, WType.Error);
                }
                else
                {
                    WriteThis("CipherText, ", false, WType.Awesome);
                    if (TestCases.BytesToHexString(outTag).CompareTo(TestCases.BytesToHexString(t.T)) != 0)
                    {
                        WriteThis("Detached Tag *FAIL*!", true, WType.Error);
                    }
                    else
                    {
                        WriteThis("Detached Tag, ", false, WType.Awesome);
                        if (Morus_1280_128.Decrypt_Detached(t.AD, output, outTag, t.K, t.IV, out returnTrip) != 0)
                            WriteThis("Tag Verify *FAIL*!", true, WType.Error);
                        else
                        {
                            WriteThis("Tag Verify, ", false, WType.Awesome);
                            string expected = TestCases.BytesToHexString(t.P);
                            string whatCameOut = TestCases.BytesToHexString(returnTrip);
                            if (expected.CompareTo(whatCameOut) != 0)
                                WriteThis("Plaintext *FAIL*!", true, WType.Error);
                            else
                                WriteThis("Plaintext. -PASS-", true, WType.Awesome);
                        }
                    }
                }
            }

            WriteThis("1280 128 Streams Tests ".PadRight(40, '-'), true, WType.Heading);
            for (int i = 0; i < MAXTESTS; i++) //< tc.TestCases1280256.Length; i++)
            {
                WriteThis("Stream Test #" + i.ToString("00") + ": ", false, WType.Heading);
                TestCases.TestCase t = tc.TestCases1280128[i];
                MemoryStream outputStream = new MemoryStream((t.P?.Length ?? 0) + 16);
                MemoryStream AD = t.AD != null ? new MemoryStream(t.AD) : null;
                MemoryStream P = t.P != null ? new MemoryStream(t.P) : null;
                if (Morus_1280_128.EncryptStream(AD, P, t.K, t.IV, outputStream) != 0)
                    WriteThis("Bad Return Result!", true, WType.Error);
                else if (TestCases.BytesToHexString(outputStream.ToArray()).CompareTo(
                    TestCases.BytesToHexString(t.C) + TestCases.BytesToHexString(t.T)) != 0)
                {
                    WriteThis("CipherText/Tag *FAIL*!", true, WType.Error);
                }
                else
                {
                    outputStream.Seek(0, SeekOrigin.Begin);
                    AD?.Seek(0, SeekOrigin.Begin);
                    MemoryStream returnStream = new MemoryStream(new byte[t.P?.Length ?? 0], true);
                    WriteThis("CipherText/Tag, ", false, WType.Awesome);
                    if (Morus_1280_128.DecryptStream(AD, outputStream, t.K, t.IV, returnStream) != 0)
                        WriteThis("Tag Verify *FAIL*!", true, WType.Error);
                    else
                    {
                        WriteThis("Tag Verify, ", false, WType.Awesome);
                        string expected = TestCases.BytesToHexString(t.P);
                        string whatCameOut = TestCases.BytesToHexString(returnStream.ToArray());
                        if (expected.CompareTo(whatCameOut) != 0)
                            WriteThis("Plaintext *FAIL*!", true, WType.Error);
                        else
                            WriteThis("Plaintext. -PASS-", true, WType.Awesome);
                    }
                }
            }
        }

        static void Test1280256(TestCases tc)
        {
            byte[] output = null;
            byte[] returnTrip = null;
            WriteThis("1280 256 Tests ".PadRight(40, '='), true, WType.Heading);
            for (int i=0; i < MAXTESTS; i++) //< tc.TestCases1280256.Length; i++)
            {
                WriteThis(" Test #" + i.ToString("00") + ": ", false, WType.Heading);
                TestCases.TestCase t = tc.TestCases1280256[i];

                if (Morus_1280_256.Encrypt(t.AD, t.P, t.K, t.IV, out output) != 0)
                    WriteThis(" *Bad Return Result!*", true, WType.Error);
                else if (TestCases.BytesToHexString(output).CompareTo(
                    TestCases.BytesToHexString(t.C) + TestCases.BytesToHexString(t.T)) !=0)
                {
                    WriteThis("CipherText/Tag *FAIL*!", true, WType.Error);
                }
                else
                {
                    WriteThis("CipherText/Tag, ", false, WType.Awesome);
                    if (Morus_1280_256.Decrypt(t.AD, output, t.K, t.IV, out returnTrip) != 0)
                        WriteThis("Tag Verify *FAIL*!", true, WType.Error);
                    else
                    {
                        WriteThis("Tag Verify, ", false, WType.Awesome);
                        string expected = TestCases.BytesToHexString(t.P);
                        string whatCameOut = TestCases.BytesToHexString(returnTrip);
                        if (expected.CompareTo(whatCameOut) != 0)
                            WriteThis("Plaintext *FAIL*!", true, WType.Error);
                        else
                            WriteThis("Plaintext. -PASS-", true, WType.Awesome);
                    }
                }
            }

            WriteThis("1280 256 Detached Tests ".PadRight(40, '-'), true, WType.Heading);
            for (int i = 0; i < MAXTESTS; i++)
            {
                WriteThis(" Test #" + i.ToString("00") + ": ", false, WType.Heading);
                TestCases.TestCase t = tc.TestCases1280256[i];
                output = null;
                returnTrip = null;
                if (Morus_1280_256.Encrypt_Detached(t.AD, t.P, t.K, t.IV, out output, out byte[] outTag) != 0)
                    WriteThis(" *Bad Return Result!*", true, WType.Error);
                else if (TestCases.BytesToHexString(output).CompareTo(TestCases.BytesToHexString(t.C)) != 0)
                {
                    WriteThis("CipherText *FAIL*!", true, WType.Error);
                }
                else
                {
                    WriteThis("CipherText, ", false, WType.Awesome);
                    if (TestCases.BytesToHexString(outTag).CompareTo(TestCases.BytesToHexString(t.T)) != 0)
                    {
                        WriteThis("Detached Tag *FAIL*!", true, WType.Error);
                    }
                    else
                    {
                        WriteThis("Detached Tag, ", false, WType.Awesome);
                        if (Morus_1280_256.Decrypt_Detached(t.AD, output, outTag, t.K, t.IV, out returnTrip) != 0)
                            WriteThis("Tag Verify *FAIL*!", true, WType.Error);
                        else
                        {
                            WriteThis("Tag Verify, ", false, WType.Awesome);
                            string expected = TestCases.BytesToHexString(t.P);
                            string whatCameOut = TestCases.BytesToHexString(returnTrip);
                            if (expected.CompareTo(whatCameOut) != 0)
                                WriteThis("Plaintext *FAIL*!", true, WType.Error);
                            else
                                WriteThis("Plaintext. -PASS-", true, WType.Awesome);
                        }
                    }
                }
            }

            WriteThis("1280 256 Streams Tests ==========================================================================", true, WType.Heading);
            for (int i = 0; i < MAXTESTS; i++) //< tc.TestCases1280256.Length; i++)
            {
                WriteThis(" Stream Test #" + i.ToString("00") + ": ", false, WType.Heading);
                TestCases.TestCase t = tc.TestCases1280256[i];
                MemoryStream outputStream = new MemoryStream((t.P?.Length ?? 0) + 16);
                MemoryStream AD = t.AD != null ? new MemoryStream(t.AD) : null;
                MemoryStream P = t.P != null ? new MemoryStream(t.P) : null;
                if (Morus_1280_256.EncryptStream(AD, P, t.K, t.IV, outputStream) != 0)
                    WriteThis(" Bad Return Result!", true, WType.Error);
                else if (TestCases.BytesToHexString(outputStream.ToArray()).CompareTo(
                    TestCases.BytesToHexString(t.C) + TestCases.BytesToHexString(t.T)) != 0)
                {
                    WriteThis("CipherText/Tag *FAIL*!", true, WType.Error);
                }
                else
                {
                    outputStream.Seek(0, SeekOrigin.Begin);
                    AD?.Seek(0, SeekOrigin.Begin);
                    MemoryStream returnStream = new MemoryStream(new byte[t.P?.Length ?? 0], true);
                    WriteThis("CipherText/Tag, ", false, WType.Awesome);
                    if (Morus_1280_256.DecryptStream(AD, outputStream, t.K, t.IV, returnStream) != 0)
                        WriteThis("Tag Verify *FAIL*!", true, WType.Error);
                    else
                    {
                        WriteThis("Tag Verify, ", false, WType.Awesome);
                        string expected = TestCases.BytesToHexString(t.P);
                        string whatCameOut = TestCases.BytesToHexString(returnStream.ToArray());
                        if (expected.CompareTo(whatCameOut) != 0)
                            WriteThis("Plaintext *FAIL*!", true, WType.Error);
                        else
                            WriteThis("Plaintext. -PASS-", true, WType.Awesome);
                    }
                }
            }
        }

        public enum WType : byte
        {
            Normal = 0,
            Error = 1,
            Info = 2,
            Awesome = 4,
            Heading = 8
        }

        public static void WriteThis(string message, bool crlf = true, WType type = WType.Normal)
        {
            ConsoleColor temp = Console.ForegroundColor;

            switch (type)
            {
                case WType.Error: Console.ForegroundColor = ConsoleColor.Red; break;
                case WType.Awesome: Console.ForegroundColor = ConsoleColor.Green; break;
                case WType.Heading: Console.ForegroundColor = ConsoleColor.Cyan; break;
                case WType.Info: Console.ForegroundColor = ConsoleColor.Yellow; break;
                case WType.Normal:
                default:
                    Console.ForegroundColor = ConsoleColor.Gray; break;
            }
            Console.Write(message);
            if (crlf) Console.WriteLine();
            Console.ForegroundColor = temp;
            Debug.Print(message);
        }
    }
}
