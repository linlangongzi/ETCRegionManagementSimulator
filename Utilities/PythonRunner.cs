﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETCRegionManagementSimulator.Utilities
{
    public class PythonRunner
    {
        public static void RunPythonScript(string scriptPath, string args = "")
        {
            ProcessStartInfo start = new ProcessStartInfo();
            start.FileName = "python"; // Specify python.exe path if not added to PATH
            start.Arguments = string.Format("\"{0}\" {1}", scriptPath, args); // arguments is path to .py file and any cmd line args
            start.UseShellExecute = false; // Do not use OS shell
            start.CreateNoWindow = true; // We don't need new window
            start.RedirectStandardOutput = true; // Any output, generated by application will be redirected back
            start.RedirectStandardError = true; // Any error in standard output will be redirected back (for example exceptions)

            using (Process process = Process.Start(start))
            {
                using (StreamReader reader = process.StandardOutput)
                {
                    string stderr = process.StandardError.ReadToEnd(); // Here are the exceptions from our Python script
                    string result = reader.ReadToEnd(); // Here is the result of StdOut(for example: print "test")
                    Debug.WriteLine(result);
                    Debug.WriteLine(stderr);
                }
            }
        }
    }
}
