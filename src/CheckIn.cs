using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace DailyCheckInJournal
{
    internal static class CheckIn
    {
        [STAThread]
        private static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new CheckInForm());
        }
    }

    internal sealed class CheckInForm : Form
    {
        private readonly string root;
        private readonly string date;
        private readonly string filePath;
        private readonly TextBox checkInTextBox;
        private readonly TextBox noteTextBox;
        private readonly Label statusLabel;
        private readonly Button saveButton;
        private readonly Button openLogsButton;
        private readonly PlaceholderText checkInPlaceholder;
        private readonly PlaceholderText notePlaceholder;

        public CheckInForm()
        {
            root = ProjectPaths.FindProjectRoot();
            date = DateTime.Now.ToString("yyyy-MM-dd");
            filePath = Path.Combine(root, "logs", date + ".md");

            Text = "Daily Check-In Journal";
            StartPosition = FormStartPosition.CenterScreen;
            MinimumSize = new Size(520, 360);
            Size = new Size(560, 400);
            Font = new Font("Segoe UI", 9F);

            var titleLabel = new Label
            {
                AutoSize = true,
                Font = new Font(Font.FontFamily, 14F, FontStyle.Bold),
                Location = new Point(20, 18),
                Text = "Daily Check-In"
            };

            var dateLabel = new Label
            {
                AutoSize = true,
                Location = new Point(22, 54),
                Text = "Date: " + date
            };

            var checkInLabel = new Label
            {
                AutoSize = true,
                Location = new Point(22, 88),
                Text = "Check-in"
            };

            checkInTextBox = new TextBox
            {
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                Location = new Point(24, 110),
                Size = new Size(492, 24)
            };
            checkInPlaceholder = new PlaceholderText(checkInTextBox, "예: README 정리하고 체크인 UI를 다듬었다");

            var noteLabel = new Label
            {
                AutoSize = true,
                Location = new Point(22, 150),
                Text = "Note (optional)"
            };

            noteTextBox = new TextBox
            {
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
                Location = new Point(24, 172),
                Multiline = true,
                ScrollBars = ScrollBars.Vertical,
                Size = new Size(492, 92)
            };
            notePlaceholder = new PlaceholderText(noteTextBox, "예: 내일은 GitHub 원격 저장소 연결하기");

            saveButton = new Button
            {
                Anchor = AnchorStyles.Bottom | AnchorStyles.Right,
                Location = new Point(296, 286),
                Size = new Size(106, 32),
                Text = "Save"
            };
            saveButton.Click += SaveButton_Click;

            openLogsButton = new Button
            {
                Anchor = AnchorStyles.Bottom | AnchorStyles.Right,
                Location = new Point(410, 286),
                Size = new Size(106, 32),
                Text = "Open logs"
            };
            openLogsButton.Click += OpenLogsButton_Click;

            statusLabel = new Label
            {
                Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
                AutoEllipsis = true,
                Location = new Point(24, 328),
                Size = new Size(492, 20),
                Text = BuildInitialStatus()
            };

            Controls.Add(titleLabel);
            Controls.Add(dateLabel);
            Controls.Add(checkInLabel);
            Controls.Add(checkInTextBox);
            Controls.Add(noteLabel);
            Controls.Add(noteTextBox);
            Controls.Add(saveButton);
            Controls.Add(openLogsButton);
            Controls.Add(statusLabel);

            AcceptButton = saveButton;
        }

        private string BuildInitialStatus()
        {
            if (File.Exists(filePath))
            {
                saveButton.Enabled = false;
                return "Today's check-in already exists.";
            }

            return "Ready to create logs\\" + date + ".md";
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            string checkIn = checkInPlaceholder.Value.Trim();
            string note = notePlaceholder.Value.Trim();

            if (string.IsNullOrWhiteSpace(checkIn))
            {
                MessageBox.Show(this, "Write a short check-in first.", "Daily Check-In", MessageBoxButtons.OK, MessageBoxIcon.Information);
                checkInTextBox.Focus();
                return;
            }

            if (File.Exists(filePath))
            {
                saveButton.Enabled = false;
                statusLabel.Text = "Today's check-in already exists.";
                MessageBox.Show(this, "A check-in already exists for " + date + ".", "Daily Check-In", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            string logsDir = Path.GetDirectoryName(filePath);
            Directory.CreateDirectory(logsDir);

            var lines = new List<string>
            {
                "# " + date,
                "",
                "- Check-in: " + checkIn
            };

            if (!string.IsNullOrWhiteSpace(note))
            {
                lines.Add("- Note: " + note);
            }

            File.WriteAllLines(filePath, lines.ToArray(), new UTF8Encoding(false));

            saveButton.Enabled = false;
            statusLabel.Text = "Created " + filePath;

            MessageBox.Show(
                this,
                "Check-in saved.\n\nCommit with:\ngit add logs/" + date + ".md\ngit commit -m \"Add check-in for " + date + "\"",
                "Daily Check-In",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }

        private void OpenLogsButton_Click(object sender, EventArgs e)
        {
            string logsDir = Path.Combine(root, "logs");
            Directory.CreateDirectory(logsDir);
            Process.Start("explorer.exe", logsDir);
        }
    }

    internal sealed class PlaceholderText
    {
        private readonly TextBox textBox;
        private readonly string placeholder;
        private readonly Color normalColor;
        private bool showingPlaceholder;

        public PlaceholderText(TextBox textBox, string placeholder)
        {
            this.textBox = textBox;
            this.placeholder = placeholder;
            normalColor = textBox.ForeColor;

            textBox.GotFocus += TextBox_GotFocus;
            textBox.LostFocus += TextBox_LostFocus;

            ShowPlaceholder();
        }

        public string Value
        {
            get
            {
                return showingPlaceholder ? "" : textBox.Text;
            }
        }

        private void TextBox_GotFocus(object sender, EventArgs e)
        {
            if (showingPlaceholder)
            {
                textBox.Text = "";
                textBox.ForeColor = normalColor;
                showingPlaceholder = false;
            }
        }

        private void TextBox_LostFocus(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBox.Text))
            {
                ShowPlaceholder();
            }
        }

        private void ShowPlaceholder()
        {
            showingPlaceholder = true;
            textBox.Text = placeholder;
            textBox.ForeColor = SystemColors.GrayText;
        }
    }

    internal static class ProjectPaths
    {
        public static string FindProjectRoot()
        {
            string fromCurrent = WalkForRoot(Directory.GetCurrentDirectory());

            if (fromCurrent != null)
            {
                return fromCurrent;
            }

            string exeDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string fromExe = WalkForRoot(exeDir);

            return fromExe ?? Directory.GetCurrentDirectory();
        }

        private static string WalkForRoot(string start)
        {
            var dir = new DirectoryInfo(start);

            while (dir != null)
            {
                bool hasReadme = File.Exists(Path.Combine(dir.FullName, "README.md"));
                bool hasGit = File.Exists(Path.Combine(dir.FullName, ".git")) || Directory.Exists(Path.Combine(dir.FullName, ".git"));

                if (hasReadme && hasGit)
                {
                    return dir.FullName;
                }

                dir = dir.Parent;
            }

            return null;
        }
    }
}
