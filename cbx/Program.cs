using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace cbx
{
    internal class Program
    {
        static readonly Dictionary<string, string> aliases = new()
        {
            { "ls", "dir" },
            { "clear", "cls" },
            { "mv", "move" },
            { "cp", "copy" },
            { "rm", "del" },
            { "pwd", "cd" },
            { "cat", "type" }
        };

        static readonly Dictionary<string, string> internalCmds = new()
        {
            { "about", "Displays information about cbx." },
            { "ver", "Shows cbx version and runtime environment." },
            { "clr", "Clears the console screen." },
            { "nhelp", "Lists all cbx internal commands and aliases." },
            { "exit", "Exits cbx." }
        };

        static void Main()
        {
            Console.Title = "cbx :: Cyber:Boot Executor Console";
            Console.WriteLine("cbx :: Cyber:Boot Executor v1.2");
            Console.WriteLine("© 2025 FÈUE / Prof. Nils Efverman & Prof. Joe Wells");
            Console.WriteLine("Type 'nhelp' for internal command reference.\n");

            while (true)
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write("cbx> ");
                Console.ResetColor();

                string? input = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(input))
                    continue;

                string cmd = input.Trim().ToLower();

                if (cmd == "exit")
                    break;

                if (internalCmds.ContainsKey(cmd))
                {
                    HandleInternal(cmd);
                    continue;
                }

                RunCommand(input);
            }
        }

        static void HandleInternal(string cmd)
        {
            switch (cmd)
            {
                case "about":
                    Console.WriteLine("cbx :: Cyber:Boot Executor");
                    Console.WriteLine("Part of the FÈUE system architecture.\n");
                    break;

                case "ver":
                    Console.WriteLine("cbx version 1.2 (.NET 9, Windows x64 build with GH Repo)");
                    break;

                case "clr":
                    Console.Clear();
                    break;

                case "nhelp":
                    Console.WriteLine("\n=== cbx Internal Commands ===");
                    foreach (var kv in internalCmds)
                        Console.WriteLine($"{kv.Key,-10} - {kv.Value}");

                    Console.WriteLine("\n=== Alias (Unix → Windows) ===");
                    foreach (var kv in aliases)
                        Console.WriteLine($"{kv.Key,-10} → {kv.Value}");
                    Console.WriteLine();
                    break;
            }
        }

        static void RunCommand(string command)
        {
            foreach (var alias in aliases)
            {
                if (command.StartsWith(alias.Key + " ", StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(command, alias.Key, StringComparison.OrdinalIgnoreCase))
                {
                    command = alias.Value + command[alias.Key.Length..];
                    break;
                }
            }

            try
            {
                var psi = new ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    Arguments = "/c " + command,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    StandardOutputEncoding = System.Text.Encoding.UTF8,
                    StandardErrorEncoding = System.Text.Encoding.UTF8
                };

                var proc = new Process { StartInfo = psi };
                proc.Start();

                string output = proc.StandardOutput.ReadToEnd();
                string error = proc.StandardError.ReadToEnd();
                proc.WaitForExit();

                if (!string.IsNullOrEmpty(output))
                    Console.WriteLine(output.Trim());

                if (!string.IsNullOrEmpty(error))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(error.Trim());
                    Console.ResetColor();
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Error: " + ex.Message);
                Console.ResetColor();
            }
        }
    }
}
