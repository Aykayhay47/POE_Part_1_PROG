using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using NAudio.Wave;

namespace AK47_CyberSecurity_POE
{
    public partial class MainWindow : Window
    {
        private static readonly Random rng = new();
        private string userName = "User";
        private List<TaskItem> tasks = new();
        private DispatcherTimer reminderTimer = new();

        private List<QuizQuestion> quizQuestions = new();
        private int currentQuestionIndex = 0;
        private int quizScore = 0;
        private int incorrectAnswers = 0;

        private List<string> activityLog = new();
        private const int MaxLogEntries = 10;

        public MainWindow()
        {
            InitializeComponent();
            AppendChat("Please enter your name to begin...");
            ChatInput.KeyDown += CaptureNameOnEnter;

            reminderTimer.Interval = TimeSpan.FromMinutes(1);
            reminderTimer.Tick += ReminderTimer_Tick;
            reminderTimer.Start();

            InitializeQuiz();
            TaskListBox.SelectionChanged += TaskListBox_SelectionChanged;
            RemoveTaskButton.IsEnabled = false;
        }

        private void CaptureNameOnEnter(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                string input = ChatInput.Text.Trim();
                if (!string.IsNullOrEmpty(input))
                {
                    userName = input;
                    ChatInput.Text = "";
                    ChatInput.KeyDown -= CaptureNameOnEnter;

                    AppendChat($"Welcome, {userName}!");
                    PlayGreeting();
                    AppendChat($"AK47: Hello {userName}, I'm your cybersecurity bot.");
                    ShowMenu();
                }
            }
        }

        private void AppendChat(string message, bool log = true)
        {
            ChatOutput.Text += message + "\n";
            ChatOutput.ScrollToEnd();
            if (log) AddToActivityLog(message);
        }

        private void AddToActivityLog(string message)
        {
            string timestamp = DateTime.Now.ToString("HH:mm:ss");
            activityLog.Add($"[{timestamp}] {message}");
            if (activityLog.Count > MaxLogEntries)
                activityLog.RemoveAt(0);
        }

        private void ShowMenu()
        {
            AppendChat("Choose a question by number:");
            AppendChat("• 1. How are you?");
            AppendChat("• 2. What's your purpose?");
            AppendChat("• 3. What can I ask you about?");
            AppendChat("• 4. What is phishing?");
            AppendChat("• 5. How do I create a strong password?");
            AppendChat("• 6. Is public Wi-Fi safe?");
            AppendChat("• 7. What is two-factor authentication?");
            AppendChat("• 8. How can I spot a scam email?");
            AppendChat("Type 'start quiz' or 'add task' to try those features.");
        }

        private void Send_Click(object sender, RoutedEventArgs e)
        {
            string message = ChatInput.Text.Trim();
            if (string.IsNullOrWhiteSpace(message)) return;

            AppendChat($"{userName}: {message}");
            ChatInput.Text = "";

            var intent = RecognizeIntent(message.ToLower());
            switch (intent)
            {
                case UserIntent.AddTask:
                    AppendChat("AK47: Use the Task Assistant section to add a task.");
                    break;
                case UserIntent.RemoveTask:
                    AppendChat("AK47: Select a task and click 'Remove Task'.");
                    break;
                case UserIntent.StartQuiz:
                    StartQuiz();
                    break;
                case UserIntent.ShowMenu:
                    ShowMenu();
                    break;
                case UserIntent.ShowActivityLog:
                    ShowActivityLog();
                    break;
                case UserIntent.AskCybersecurity:
                    HandleCybersecurityQuery(message);
                    break;
                default:
                    AppendChat("AK47: I didn't understand. Try selecting a menu number or type a command.");
                    break;
            }
        }

        private enum UserIntent
        {
            AddTask,
            RemoveTask,
            StartQuiz,
            ShowMenu,
            ShowActivityLog,
            AskCybersecurity,
            Unknown
        }

        private UserIntent RecognizeIntent(string input)
        {
            if (new[] { "add task", "new task", "reminder" }.Any(input.Contains)) return UserIntent.AddTask;
            if (input.Contains("remove")) return UserIntent.RemoveTask;
            if (input.Contains("quiz")) return UserIntent.StartQuiz;
            if (input.Contains("menu") || input.Contains("help")) return UserIntent.ShowMenu;
            if (input.Contains("log")) return UserIntent.ShowActivityLog;
            if (input.Any(char.IsDigit)) return UserIntent.AskCybersecurity;

            return UserIntent.Unknown;
        }

        private void HandleCybersecurityQuery(string input)
        {
            string response = input switch
            {
                "1" => "AK47: I'm just code, but I'm running strong 💪",
                "2" => "AK47: My job is to help you learn cybersecurity 🧠",
                "3" => "AK47: Ask about passwords, scams, phishing, 2FA, etc.",
                "4" => "AK47: Phishing is when someone pretends to be trustworthy to steal info.",
                "5" => "AK47: Use long passwords (12+), with letters, numbers & symbols.",
                "6" => "AK47: Avoid public Wi-Fi for banking unless you're using a VPN.",
                "7" => "AK47: 2FA means two-factor authentication, like a code + password.",
                "8" => "AK47: Scam emails often have urgency, poor grammar & weird links.",
                _ => "AK47: Please enter a number between 1 and 8."
            };
            AppendChat(response);
        }

        private void ShowActivityLog()
        {
            AppendChat("AK47: Here's your recent activity:");
            foreach (var log in activityLog)
                AppendChat(log, false);
        }

        private void ClearChat_Click(object sender, RoutedEventArgs e)
        {
            ChatOutput.Text = "";
            AppendChat("AK47: Chat cleared.");
        }

        private void ShowMenu_Click(object sender, RoutedEventArgs e) => ShowMenu();

        private void TaskListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            RemoveTaskButton.IsEnabled = TaskListBox.SelectedItem != null;
        }

        private void AddTask_Click(object sender, RoutedEventArgs e)
        {
            string title = TaskTitleInput.Text.Trim();
            string desc = TaskDescriptionInput.Text.Trim();

            if (string.IsNullOrWhiteSpace(title))
            {
                AppendChat("AK47: Enter a task title.");
                return;
            }

            DateTime? reminder = null;
            if (int.TryParse(ReminderDaysInput.Text.Trim(), out int days))
                reminder = DateTime.Now.AddDays(days);

            var task = new TaskItem { Title = title, Description = desc, ReminderDate = reminder };
            tasks.Add(task);
            TaskListBox.Items.Add(task);

            AppendChat($"AK47: Task '{title}' added. {(reminder.HasValue ? $"Reminder set for {days} day(s)." : "")}");
        }

        private void RemoveTask_Click(object sender, RoutedEventArgs e)
        {
            if (TaskListBox.SelectedItem is TaskItem task)
            {
                tasks.Remove(task);
                TaskListBox.Items.Remove(task);
                AppendChat($"AK47: Task '{task.Title}' removed.");
            }
        }

        private void ReminderTimer_Tick(object sender, EventArgs e)
        {
            foreach (var task in tasks)
            {
                if (task.ReminderDate.HasValue && task.ReminderDate.Value.Date == DateTime.Today)
                    AppendChat($"⏰ Reminder: Task '{task.Title}' is due today!");
            }
        }

        private void InitializeQuiz()
        {
            quizQuestions = new()
            {
                new() { QuestionText = "What is phishing?", Choices = ["Fishing scam", "Tricking users for data", "Spamming ads"], CorrectAnswerIndex = 1, Explanation = "Phishing is tricking users for sensitive info." },
                new() { QuestionText = "Strong password length?", Choices = ["6", "8", "12"], CorrectAnswerIndex = 2, Explanation = "At least 12 characters is best!" },
                new() { QuestionText = "What is 2FA?", Choices = ["Two-Factor Authentication", "Two-Face Access", "Second Form Authorization"], CorrectAnswerIndex = 0, Explanation = "2FA = extra login security step." },
                new() { QuestionText = "Safe to use public Wi-Fi?", Choices = ["Always", "Never", "Only with VPN"], CorrectAnswerIndex = 2, Explanation = "Use VPN on public Wi-Fi!" },
                new() { QuestionText = "Scam email signs?", Choices = ["Spelling errors", "Professional layout", "Known sender"], CorrectAnswerIndex = 0, Explanation = "Scam emails often have poor grammar." },
            };
        }

        private void StartQuiz()
        {
            QuizPanel.Visibility = Visibility.Visible;
            RestartQuizButton.Visibility = Visibility.Collapsed;
            currentQuestionIndex = 0;
            quizScore = 0;
            incorrectAnswers = 0;
            DisplayCurrentQuestion();
        }

        private void DisplayCurrentQuestion()
        {
            throw new NotImplementedException();
        }

        private void RestartQuizButton_Click(object sender, RoutedEventArgs e)
        {
            StartQuiz();
        }

        private void HandleQuizAnswer_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is int index)
            {
                var q = quizQuestions[currentQuestionIndex];

                if (index == q.CorrectAnswerIndex)
                {
                    quizScore++;
                    QuizFeedback.Text = "✅ Correct! " + q.Explanation;
                }
                else
                {
                    incorrectAnswers++;
                    QuizFeedback.Text = "❌ Incorrect. " + q.Explanation;
                }

                QuizFeedback.Visibility = Visibility.Visible;

                // Disable buttons to prevent multiple answers
                foreach (Button button in AnswerPanel.Children.OfType<Button>())
                {
                    button.IsEnabled = false;
                }

                currentQuestionIndex++;

                Task.Delay(1500).ContinueWith(_ =>
                {
                    Dispatcher.Invoke(DisplayCurrentQuestion);
                });
            }
        }



        private void QuizAnswer_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is int index)
            {
                var q = quizQuestions[currentQuestionIndex];
                if (index == q.CorrectAnswerIndex)
                {
                    quizScore++;
                    QuizFeedback.Text = "✅ Correct! " + q.Explanation;
                }
                else
                {
                    incorrectAnswers++;
                    QuizFeedback.Text = "❌ Incorrect. " + q.Explanation;
                }

                QuizFeedback.Visibility = Visibility.Visible;
                currentQuestionIndex++;
                Task.Delay(1500).ContinueWith(_ => Dispatcher.Invoke(DisplayCurrentQuestion));
            }
        }

        private void PlayGreeting()
        {
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "greeting.wav");

            if (!File.Exists(path))
            {
                AppendChat("[Warning: greeting.wav not found]");
                return;
            }

            Task.Run(() =>
            {
                try
                {
                    var audio = new AudioFileReader(path);
                    var output = new WaveOutEvent();

                    output.Init(audio);
                    output.Play();

                    while (output.PlaybackState == PlaybackState.Playing)
                    {
                        Thread.Sleep(100);
                    }

                    output.Dispose();
                    audio.Dispose();
                }
                catch (Exception ex)
                {
                    Dispatcher.Invoke(() => AppendChat("Error playing audio: " + ex.Message));
                }
            });
        }


        public class TaskItem
        {
            public string Title { get; set; } = string.Empty;
            public string Description { get; set; } = string.Empty;
            public DateTime? ReminderDate { get; set; }

            public override string ToString()
            {
                string reminder = ReminderDate.HasValue ? $"⏰ Due: {ReminderDate.Value.ToShortDateString()}" : "No reminder set.";
                return $"📌 {Title}\n📝 {Description}\n{reminder}";
            }
        }

        public class QuizQuestion
        {
            public string QuestionText { get; set; } = string.Empty;
            public List<string> Choices { get; set; } = new();
            public int CorrectAnswerIndex { get; set; }
            public string Explanation { get; set; } = string.Empty;
        }
    }
}
