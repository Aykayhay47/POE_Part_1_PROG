using System;
using System.IO;
using System.Collections.Generic;
using System.Threading;
using NAudio.Wave;

class CyberSecurityBot
{
    static string userName = "";
    static string userInterest = "";
    static string lastTopic = "";

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

                    while (outputDevice.PlaybackState == PlaybackState.Playing)
                    {
                        Thread.Sleep(100);
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
        Console.ForegroundColor = ConsoleColor.Green;
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
        Console.ResetColor();
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
        Console.Write("Bot - Enter your name: ");
        userName = Console.ReadLine();
        Console.WriteLine($"Bot - Hello, {userName}! I'm here to help you with cybersecurity awareness.");
    }

    static void ChatbotResponses()
    {
        var phishingTips = new List<string>
        {
            "Bot - Never click on suspicious links.",
            "Bot - Phishers often mimic trusted websites.",
            "Bot - Hover over links to preview the URL before clicking."
        };

        while (true)
        {
            Console.Write("\nBot - Ask me a cybersecurity question: ");
            string question = Console.ReadLine().ToLower();

            if (string.IsNullOrWhiteSpace(question))
            {
                Console.WriteLine("Bot - Please type something.");
                continue;
            }

            if (question == "exit")
            {
                Console.WriteLine($"Goodbye, {userName}! Stay safe online.");
                break;
            }

            if (question.Contains("how are you"))
            {
                Console.WriteLine("I'm functioning as expected, thank you!");
                lastTopic = "status";
            }
            else if (question.Contains("purpose"))
            {
                Console.WriteLine("Bot - I educate users about cybersecurity awareness.");
                lastTopic = "purpose";
            }
            else if (question.Contains("password"))
            {
                Console.WriteLine("Bot - Use strong passwords with uppercase, lowercase, numbers, and symbols.");
                lastTopic = "password";
            }
            else if (question.Contains("phishing"))
            {
                var rand = new Random();
                Console.WriteLine(phishingTips[rand.Next(phishingTips.Count)]);
                lastTopic = "phishing";
            }
            else if (question.Contains("scam"))
            {
                Console.WriteLine("Bot - Avoid scams by verifying sources and ignoring suspicious messages.");
                lastTopic = "scam";
            }
            else if (question.Contains("privacy"))
            {
                Console.WriteLine("Bot - Protect your privacy by limiting what you share online.");
                userInterest = "privacy";
                lastTopic = "privacy";
            }
            else if (question.Contains("cyberattack"))
            {
                Console.WriteLine("Bot - Cyberattacks can happen anytime. Always keep your software updated.");
                lastTopic = "cyberattack";
            }
            else if (question.Contains("encryption"))
            {
                Console.WriteLine("Bot - Encryption secures your data so that only authorized people can access it.");
                lastTopic = "encryption";
            }
            else if (question.Contains("tell me more"))
            {
                if (lastTopic == "phishing")
                {
                    Console.WriteLine("Bot - Phishing emails often impersonate banks or services. Always double-check the sender.");
                }
                else if (lastTopic == "privacy")
                {
                    Console.WriteLine("Bot - Use private browsing, check app permissions, and don’t overshare on social media.");
                }
                else
                {
                    Console.WriteLine("Bot - I'm not sure what you'd like to know more about. Please ask a specific topic.");
                }
            }
            else if (question.Contains("worried") || question.Contains("frustrated") || question.Contains("scared"))
            {
                Console.WriteLine("Bot - It's okay to feel that way. Cybersecurity can feel overwhelming, but you're not alone.");
            }
            else
            {
                Console.WriteLine("Bot - I didn’t quite understand that. Could you rephrase or ask about a cybersecurity topic?");
            }
        }
    }
}
