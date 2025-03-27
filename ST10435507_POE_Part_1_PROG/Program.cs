using System;
using System.IO;
using NAudio.Wave;

class CyberSecurityBot
{
    static void Main()
    {
        PlayGreeting();
        FormatConsole();
        DisplayAsciiArt();
        GreetUser();
        ChatbotResponses();
    }

    static void PlayGreeting()
    {
        string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "greeting.wav");
        Console.WriteLine("Looking for sound file at: " + path);

        if (File.Exists(path))
        {
            try
            {
                using (var audioFile = new AudioFileReader(path))
                using (var outputDevice = new WaveOutEvent())
                {
                    outputDevice.Init(audioFile);
                    outputDevice.Play();

                    // Keep the program running until the sound finishes
                    while (outputDevice.PlaybackState == PlaybackState.Playing)
                    {
                        System.Threading.Thread.Sleep(100);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error playing sound: " + ex.Message);
            }
        }
        else
        {
            Console.WriteLine("Warning: Sound file not found at " + path);
        }
    }

    static void DisplayAsciiArt()
    {
        Console.WriteLine(@"

  ___   _   __  ___  ______  _____                      _ _         
 / _ \ | | / / /   ||___  / /  ___|                    (_) |        
/ /_\ \| |/ / / /| |   / /  \ `--.  ___  ___ _   _ _ __ _| |_ _   _ 
|  _  ||    \/ /_| |  / /    `--. \/ _ \/ __| | | | '__| | __| | | |
| | | || |\  \___  |./ /    /\__/ /  __/ (__| |_| | |  | | |_| |_| |
\_| |_/\_| \_/   |_/\_/     \____/ \___|\___|\__,_|_|  |_|\__|\__, |
                                                               __/ |
                                                              |___/ 
  ");
    }

    static void FormatConsole()
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("=================================");
        Console.WriteLine("   Cybersecurity Awareness Bot   ");
        Console.WriteLine("=================================");
        Console.ResetColor();
    }

    static void GreetUser()
    {
        Console.Write("Enter your name: ");
        string name = Console.ReadLine();
        Console.WriteLine($"Hello, {name}! I'm here to help you with cybersecurity awareness.");
    }

    static void ChatbotResponses()
    {
        while (true)
        {
            Console.Write("\nAsk me a cybersecurity question: ");
            string question = Console.ReadLine().ToLower();

            if (question.Contains("how are you"))
            {
                Console.WriteLine("I'm just a bot, but I'm functioning as expected!");
            }
            else if (question.Contains("purpose"))
            {
                Console.WriteLine("I educate users about cybersecurity awareness.");
            }
            else if (question.Contains("password"))
            {
                Console.WriteLine("Use a strong password with uppercase, lowercase, numbers, and symbols.");
            }
            else if (question.Contains("phishing"))
            {
                Console.WriteLine("Be cautious of emails or messages asking for personal information.");
            }
            else if (question.Contains("safe browsing"))
            {
                Console.WriteLine("Avoid clicking on suspicious links and use a secure browser.");
            }
            else if (question == "exit")
            {
                Console.WriteLine("Goodbye!");
                break;
            }
            else
            {
                Console.WriteLine("I didn’t quite understand that. Could you rephrase?");
            }
        }
    }
}

