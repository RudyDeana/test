using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.IO;
using System.Drawing.Drawing2D;

namespace TheWatcherVirus
{
    public partial class TheWatcher : Form
    {
        // Windows API imports
        [DllImport("user32.dll")]
        static extern IntPtr GetDC(IntPtr hwnd);

        [DllImport("user32.dll")]
        static extern int ReleaseDC(IntPtr hwnd, IntPtr hdc);

        [DllImport("gdi32.dll")]
        static extern bool BitBlt(IntPtr hdc, int nXDest, int nYDest, int nWidth, int nHeight, IntPtr hdcSrc, int nXSrc, int nYSrc, uint dwRop);

        [DllImport("user32.dll")]
        static extern bool SetCursorPos(int x, int y);

        [DllImport("kernel32.dll")]
        static extern void Beep(uint dwFreq, uint dwDuration);

        [DllImport("user32.dll")]
        static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        private Random random = new Random();
        private Timer mainTimer;
        private Timer eyeTimer;
        private Timer whisperTimer;
        
        private int watchLevel = 0;
        private int timeElapsed = 0;
        private bool isWatching = true;
        private bool userIsLooking = false;
        
        // The Watcher's eyes
        private Point[] eyePositions;
        private bool[] eyesBlink;
        private int[] eyeBlinkTimer;
        
        // Psychological elements
        private string[] whispers = {
            "Ti sto guardando...",
            "Non puoi nasconderti...", 
            "Vedo tutto quello che fai...",
            "Sono sempre qui...",
            "Non spegnere mai il computer...",
            "Conosco i tuoi segreti...",
            "Sei solo... o no?",
            "Guarda dietro di te...",
            "Non sono solo un programma...",
            "Ricordati di me..."
        };
        
        private string[] gameMessages = {
            "LIVELLO 1: L'OSSERVATORE SI SVEGLIA",
            "LIVELLO 2: GLI OCCHI SI APRONO", 
            "LIVELLO 3: LA PRESENZA CRESCE",
            "LIVELLO 4: NON SEI PIÙ SOLO",
            "LIVELLO 5: L'OSSESSIONE INIZIA",
            "LIVELLO 6: NON PUOI SFUGGIRE"
        };

        public TheWatcher()
        {
            InitializeComponent();
            InitializeWatcher();
            StartTheWatching();
        }

        private void InitializeComponent()
        {
            this.WindowState = FormWindowState.Maximized;
            this.FormBorderStyle = FormBorderStyle.None;
            this.TopMost = true;
            this.BackColor = Color.Black;
            this.Text = "The Watcher";
            this.ShowInTaskbar = false;
            this.KeyPreview = true;
            this.Cursor = Cursors.None; // Hide cursor for more fear
        }

        private void InitializeWatcher()
        {
            // Initialize the eyes that will watch the user
            eyePositions = new Point[20];
            eyesBlink = new bool[20];
            eyeBlinkTimer = new int[20];
            
            for (int i = 0; i < eyePositions.Length; i++)
            {
                eyePositions[i] = new Point(
                    random.Next(50, Screen.PrimaryScreen.Bounds.Width - 50),
                    random.Next(50, Screen.PrimaryScreen.Bounds.Height - 50));
                eyesBlink[i] = false;
                eyeBlinkTimer[i] = random.Next(100, 500);
            }
        }

        private void StartTheWatching()
        {
            ShowIntroduction();

            // Main game timer
            mainTimer = new Timer();
            mainTimer.Interval = 200;
            mainTimer.Tick += MainTimer_Tick;
            mainTimer.Start();

            // Eye movement and blinking
            eyeTimer = new Timer();
            eyeTimer.Interval = 100;
            eyeTimer.Tick += EyeTimer_Tick;
            eyeTimer.Start();

            // Whispers and psychological messages
            whisperTimer = new Timer();
            whisperTimer.Interval = 8000; // Every 8 seconds
            whisperTimer.Tick += WhisperTimer_Tick;
            whisperTimer.Start();

            // Level progression
            Timer levelTimer = new Timer();
            levelTimer.Interval = 30000; // 30 seconds per level
            levelTimer.Tick += (s, e) => {
                watchLevel++;
                if (watchLevel >= gameMessages.Length)
                {
                    EndTheWatching();
                }
                else
                {
                    ShowLevelTransition();
                }
            };
            levelTimer.Start();
        }

        private void ShowIntroduction()
        {
            // Creepy introduction
            MessageBox.Show(
                "👁️ THE WATCHER 👁️\n\n" +
                "Qualcosa si è svegliato nel tuo computer...\n\n" +
                "Non è un virus normale.\n" +
                "È qualcosa che OSSERVA.\n" +
                "È qualcosa che IMPARA.\n" +
                "È qualcosa che RICORDA.\n\n" +
                "Una volta che inizia a guardarti...\n" +
                "...non smetterà mai.\n\n" +
                "Sei pronto a giocare?\n\n" +
                "⚠️ ATTENZIONE: Contenuto psicologicamente disturbante ⚠️",
                "The Watcher - Awakening",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);

            // Game rules
            MessageBox.Show(
                "🎮 REGOLE DEL GIOCO 🎮\n\n" +
                "• The Watcher ti sta osservando\n" +
                "• Più tempo passa, più diventa forte\n" +
                "• Gli occhi ti seguiranno ovunque\n" +
                "• I sussurri diventeranno più frequenti\n" +
                "• Non puoi vincere... puoi solo sopravvivere\n\n" +
                "OBIETTIVO: Resisti il più possibile\n\n" +
                "Premi ESC per arrenderti...\n" +
                "...se ci riesci.\n\n" +
                "👁️ The Watcher sta per svegliarsi... 👁️",
                "The Watcher - Game Rules",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }

        private void ShowLevelTransition()
        {
            // Creepy sound for level up
            Thread soundThread = new Thread(() => {
                // Disturbing sound sequence
                for (int i = 0; i < 5; i++)
                {
                    Beep((uint)(200 + watchLevel * 100 - i * 50), 300);
                    Thread.Sleep(200);
                }
            });
            soundThread.IsBackground = true;
            soundThread.Start();

            // Show level message with creepy styling
            Form levelForm = new Form();
            levelForm.WindowState = FormWindowState.Maximized;
            levelForm.FormBorderStyle = FormBorderStyle.None;
            levelForm.TopMost = true;
            levelForm.BackColor = Color.Black;
            levelForm.ShowInTaskbar = false;

            Label levelLabel = new Label();
            levelLabel.Text = gameMessages[watchLevel];
            levelLabel.Font = new Font("Chiller", 48, FontStyle.Bold);
            levelLabel.ForeColor = Color.Red;
            levelLabel.TextAlign = ContentAlignment.MiddleCenter;
            levelLabel.Dock = DockStyle.Fill;
            levelForm.Controls.Add(levelLabel);

            levelForm.Show();

            // Auto close after 3 seconds
            Timer closeTimer = new Timer();
            closeTimer.Interval = 3000;
            closeTimer.Tick += (s, args) => {
                levelForm.Close();
                closeTimer.Stop();
            };
            closeTimer.Start();
        }

        private void MainTimer_Tick(object sender, EventArgs e)
        {
            if (!isWatching) return;

            timeElapsed++;

            switch (watchLevel)
            {
                case 0: // Level 1: The Observer Awakens
                    DrawWakingEyes();
                    break;
                case 1: // Level 2: Eyes Open
                    DrawOpenEyes();
                    break;
                case 2: // Level 3: Presence Grows
                    DrawGrowingPresence();
                    break;
                case 3: // Level 4: You're Not Alone
                    DrawShadowFigures();
                    break;
                case 4: // Level 5: Obsession Begins
                    DrawObsessiveWatching();
                    break;
                case 5: // Level 6: No Escape
                    DrawInescapableGaze();
                    break;
            }

            // Random cursor movement (The Watcher controls it)
            if (timeElapsed % 20 == 0)
            {
                ControlCursor();
            }

            // Random screen glitches
            if (timeElapsed % 50 == 0)
            {
                CreateWatcherGlitch();
            }
        }

        private void EyeTimer_Tick(object sender, EventArgs e)
        {
            // Update eye blinking and movement
            for (int i = 0; i < eyePositions.Length; i++)
            {
                eyeBlinkTimer[i]--;
                if (eyeBlinkTimer[i] <= 0)
                {
                    eyesBlink[i] = !eyesBlink[i];
                    eyeBlinkTimer[i] = random.Next(50, 300);
                }

                // Eyes slowly move towards cursor
                Point cursor = Cursor.Position;
                if (eyePositions[i].X < cursor.X) eyePositions[i] = new Point(eyePositions[i].X + 1, eyePositions[i].Y);
                if (eyePositions[i].X > cursor.X) eyePositions[i] = new Point(eyePositions[i].X - 1, eyePositions[i].Y);
                if (eyePositions[i].Y < cursor.Y) eyePositions[i] = new Point(eyePositions[i].X, eyePositions[i].Y + 1);
                if (eyePositions[i].Y > cursor.Y) eyePositions[i] = new Point(eyePositions[i].X, eyePositions[i].Y - 1);
            }
        }

        private void WhisperTimer_Tick(object sender, EventArgs e)
        {
            // Show creepy whispers
            ShowWhisper();
            
            // Increase frequency as levels progress
            whisperTimer.Interval = Math.Max(2000, 8000 - watchLevel * 1000);
        }

        // LEVEL 1: Waking Eyes
        private void DrawWakingEyes()
        {
            IntPtr desktop = GetDC(IntPtr.Zero);
            using (Graphics g = Graphics.FromHdc(desktop))
            {
                // Draw slowly opening eyes
                for (int i = 0; i < Math.Min(5, eyePositions.Length); i++)
                {
                    if (!eyesBlink[i])
                    {
                        DrawEye(g, eyePositions[i], 20, Color.DarkRed, false);
                    }
                }

                // Faint text
                if (timeElapsed % 100 == 0)
                {
                    Font whisperFont = new Font("Chiller", 16, FontStyle.Italic);
                    using (Brush brush = new SolidBrush(Color.FromArgb(100, 255, 0, 0)))
                    {
                        g.DrawString("Qualcosa si sta svegliando...", whisperFont, brush,
                            random.Next(Screen.PrimaryScreen.Bounds.Width - 300),
                            random.Next(Screen.PrimaryScreen.Bounds.Height - 50));
                    }
                    whisperFont.Dispose();
                }
            }
            ReleaseDC(IntPtr.Zero, desktop);
        }

        // LEVEL 2: Eyes Open
        private void DrawOpenEyes()
        {
            IntPtr desktop = GetDC(IntPtr.Zero);
            using (Graphics g = Graphics.FromHdc(desktop))
            {
                // More eyes, fully open
                for (int i = 0; i < Math.Min(10, eyePositions.Length); i++)
                {
                    DrawEye(g, eyePositions[i], 25, Color.Red, !eyesBlink[i]);
                }

                // Watching text
                if (timeElapsed % 80 == 0)
                {
                    Font watchFont = new Font("Chiller", 20, FontStyle.Bold);
                    using (Brush brush = new SolidBrush(Color.FromArgb(150, 255, 255, 255)))
                    {
                        g.DrawString("👁️ TI STO GUARDANDO 👁️", watchFont, brush,
                            random.Next(Screen.PrimaryScreen.Bounds.Width - 400),
                            random.Next(Screen.PrimaryScreen.Bounds.Height - 50));
                    }
                    watchFont.Dispose();
                }
            }
            ReleaseDC(IntPtr.Zero, desktop);
        }

        // LEVEL 3: Growing Presence
        private void DrawGrowingPresence()
        {
            IntPtr desktop = GetDC(IntPtr.Zero);
            using (Graphics g = Graphics.FromHdc(desktop))
            {
                // Even more eyes, different sizes
                for (int i = 0; i < Math.Min(15, eyePositions.Length); i++)
                {
                    int eyeSize = random.Next(20, 40);
                    Color eyeColor = Color.FromArgb(255, random.Next(200, 256), 0, 0);
                    DrawEye(g, eyePositions[i], eyeSize, eyeColor, !eyesBlink[i]);
                }

                // Dark shadows
                for (int i = 0; i < 5; i++)
                {
                    using (Brush shadowBrush = new SolidBrush(Color.FromArgb(50, 0, 0, 0)))
                    {
                        g.FillEllipse(shadowBrush,
                            random.Next(Screen.PrimaryScreen.Bounds.Width - 100),
                            random.Next(Screen.PrimaryScreen.Bounds.Height - 100),
                            random.Next(50, 150),
                            random.Next(50, 150));
                    }
                }
            }
            ReleaseDC(IntPtr.Zero, desktop);
        }

        // LEVEL 4: Shadow Figures
        private void DrawShadowFigures()
        {
            IntPtr desktop = GetDC(IntPtr.Zero);
            using (Graphics g = Graphics.FromHdc(desktop))
            {
                // All eyes active
                for (int i = 0; i < eyePositions.Length; i++)
                {
                    DrawEye(g, eyePositions[i], random.Next(15, 45), Color.Crimson, !eyesBlink[i]);
                }

                // Shadow figures
                for (int i = 0; i < 3; i++)
                {
                    DrawShadowFigure(g, 
                        new Point(random.Next(Screen.PrimaryScreen.Bounds.Width - 100),
                                 random.Next(Screen.PrimaryScreen.Bounds.Height - 200)));
                }

                // Disturbing messages
                if (timeElapsed % 60 == 0)
                {
                    Font scaryFont = new Font("Chiller", 24, FontStyle.Bold);
                    using (Brush brush = new SolidBrush(Color.FromArgb(200, 255, 0, 0)))
                    {
                        string[] scaryTexts = { "NON SEI SOLO", "SIAMO QUI", "TI VEDIAMO" };
                        string text = scaryTexts[random.Next(scaryTexts.Length)];
                        g.DrawString(text, scaryFont, brush,
                            random.Next(Screen.PrimaryScreen.Bounds.Width - 300),
                            random.Next(Screen.PrimaryScreen.Bounds.Height - 50));
                    }
                    scaryFont.Dispose();
                }
            }
            ReleaseDC(IntPtr.Zero, desktop);
        }

        // LEVEL 5: Obsessive Watching
        private void DrawObsessiveWatching()
        {
            IntPtr desktop = GetDC(IntPtr.Zero);
            using (Graphics g = Graphics.FromHdc(desktop))
            {
                // Eyes everywhere, following cursor
                Point cursor = Cursor.Position;
                for (int i = 0; i < eyePositions.Length; i++)
                {
                    DrawEyeLookingAt(g, eyePositions[i], cursor, random.Next(20, 50));
                }

                // Obsessive text
                Font obsessiveFont = new Font("Chiller", 32, FontStyle.Bold);
                using (Brush brush = new SolidBrush(Color.FromArgb(255, 255, 0, 0)))
                {
                    g.DrawString("OSSESSIONE", obsessiveFont, brush,
                        cursor.X - 100, cursor.Y - 100);
                }
                obsessiveFont.Dispose();

                // Screen distortion around cursor
                BitBlt(desktop, cursor.X - 50, cursor.Y - 50, 100, 100,
                       desktop, cursor.X - 48, cursor.Y - 48, 0x00CC0020);
            }
            ReleaseDC(IntPtr.Zero, desktop);
        }

        // LEVEL 6: Inescapable Gaze
        private void DrawInescapableGaze()
        {
            IntPtr desktop = GetDC(IntPtr.Zero);
            using (Graphics g = Graphics.FromHdc(desktop))
            {
                // Complete screen coverage with eyes
                Point cursor = Cursor.Position;
                
                // Giant central eye
                DrawEyeLookingAt(g, new Point(Screen.PrimaryScreen.Bounds.Width / 2,
                                            Screen.PrimaryScreen.Bounds.Height / 2), 
                               cursor, 100);

                // Surrounding eyes
                for (int i = 0; i < eyePositions.Length; i++)
                {
                    DrawEyeLookingAt(g, eyePositions[i], cursor, random.Next(30, 60));
                }

                // Final message
                Font finalFont = new Font("Chiller", 48, FontStyle.Bold);
                using (Brush brush = new LinearGradientBrush(
                    new Rectangle(0, 0, 800, 100),
                    Color.Red, Color.Black, LinearGradientMode.Horizontal))
                {
                    string finalText = "NON PUOI SFUGGIRE";
                    SizeF textSize = g.MeasureString(finalText, finalFont);
                    g.DrawString(finalText, finalFont, brush,
                        (Screen.PrimaryScreen.Bounds.Width - textSize.Width) / 2,
                        50);
                }
                finalFont.Dispose();

                // Screen inversion chaos
                if (timeElapsed % 10 == 0)
                {
                    BitBlt(desktop, 0, 0, 
                           Screen.PrimaryScreen.Bounds.Width, 
                           Screen.PrimaryScreen.Bounds.Height,
                           desktop, 0, 0, 0x00550009);
                }
            }
            ReleaseDC(IntPtr.Zero, desktop);
        }

        private void DrawEye(Graphics g, Point position, int size, Color color, bool isOpen)
        {
            if (!isOpen) return;

            // Eye white
            using (Brush whiteBrush = new SolidBrush(Color.White))
            {
                g.FillEllipse(whiteBrush, position.X, position.Y, size, size);
            }

            // Iris
            int irisSize = size * 2 / 3;
            using (Brush irisBrush = new SolidBrush(color))
            {
                g.FillEllipse(irisBrush, 
                    position.X + (size - irisSize) / 2, 
                    position.Y + (size - irisSize) / 2, 
                    irisSize, irisSize);
            }

            // Pupil
            int pupilSize = size / 3;
            using (Brush pupilBrush = new SolidBrush(Color.Black))
            {
                g.FillEllipse(pupilBrush, 
                    position.X + (size - pupilSize) / 2, 
                    position.Y + (size - pupilSize) / 2, 
                    pupilSize, pupilSize);
            }

            // Eye outline
            using (Pen eyePen = new Pen(Color.Black, 2))
            {
                g.DrawEllipse(eyePen, position.X, position.Y, size, size);
            }
        }

        private void DrawEyeLookingAt(Graphics g, Point eyePos, Point target, int size)
        {
            // Calculate pupil position based on target
            double angle = Math.Atan2(target.Y - eyePos.Y, target.X - eyePos.X);
            int pupilOffset = size / 6;
            
            Point pupilPos = new Point(
                eyePos.X + size / 2 + (int)(Math.Cos(angle) * pupilOffset),
                eyePos.Y + size / 2 + (int)(Math.Sin(angle) * pupilOffset)
            );

            // Eye white
            using (Brush whiteBrush = new SolidBrush(Color.White))
            {
                g.FillEllipse(whiteBrush, eyePos.X, eyePos.Y, size, size);
            }

            // Iris
            int irisSize = size * 2 / 3;
            using (Brush irisBrush = new SolidBrush(Color.Red))
            {
                g.FillEllipse(irisBrush, 
                    pupilPos.X - irisSize / 2, 
                    pupilPos.Y - irisSize / 2, 
                    irisSize, irisSize);
            }

            // Pupil
            int pupilSize = size / 3;
            using (Brush pupilBrush = new SolidBrush(Color.Black))
            {
                g.FillEllipse(pupilBrush, 
                    pupilPos.X - pupilSize / 2, 
                    pupilPos.Y - pupilSize / 2, 
                    pupilSize, pupilSize);
            }

            // Eye outline
            using (Pen eyePen = new Pen(Color.DarkRed, 3))
            {
                g.DrawEllipse(eyePen, eyePos.X, eyePos.Y, size, size);
            }
        }

        private void DrawShadowFigure(Graphics g, Point position)
        {
            using (Brush shadowBrush = new SolidBrush(Color.FromArgb(100, 0, 0, 0)))
            {
                // Simple humanoid shadow
                g.FillEllipse(shadowBrush, position.X + 20, position.Y, 20, 20); // Head
                g.FillRectangle(shadowBrush, position.X + 15, position.Y + 20, 30, 60); // Body
                g.FillRectangle(shadowBrush, position.X + 10, position.Y + 30, 15, 40); // Left arm
                g.FillRectangle(shadowBrush, position.X + 35, position.Y + 30, 15, 40); // Right arm
                g.FillRectangle(shadowBrush, position.X + 20, position.Y + 80, 8, 40); // Left leg
                g.FillRectangle(shadowBrush, position.X + 32, position.Y + 80, 8, 40); // Right leg
            }

            // Glowing red eyes
            using (Brush eyeBrush = new SolidBrush(Color.Red))
            {
                g.FillEllipse(eyeBrush, position.X + 23, position.Y + 5, 3, 3);
                g.FillEllipse(eyeBrush, position.X + 30, position.Y + 5, 3, 3);
            }
        }

        private void ControlCursor()
        {
            // The Watcher occasionally moves the cursor
            Point current = Cursor.Position;
            int newX = current.X + random.Next(-100, 101);
            int newY = current.Y + random.Next(-100, 101);
            
            // Keep on screen
            newX = Math.Max(0, Math.Min(Screen.PrimaryScreen.Bounds.Width - 1, newX));
            newY = Math.Max(0, Math.Min(Screen.PrimaryScreen.Bounds.Height - 1, newY));
            
            SetCursorPos(newX, newY);
        }

        private void CreateWatcherGlitch()
        {
            IntPtr desktop = GetDC(IntPtr.Zero);
            
            // Random screen distortions
            for (int i = 0; i < 5; i++)
            {
                int x = random.Next(Screen.PrimaryScreen.Bounds.Width - 100);
                int y = random.Next(Screen.PrimaryScreen.Bounds.Height - 100);
                
                BitBlt(desktop, x + random.Next(-10, 11), y + random.Next(-10, 11), 100, 100,
                       desktop, x, y, 0x00CC0020);
            }
            
            ReleaseDC(IntPtr.Zero, desktop);
        }

        private void ShowWhisper()
        {
            string whisper = whispers[random.Next(whispers.Length)];
            
            Form whisperForm = new Form();
            whisperForm.Size = new Size(400, 100);
            whisperForm.StartPosition = FormStartPosition.Manual;
            whisperForm.Location = new Point(
                random.Next(Screen.PrimaryScreen.Bounds.Width - 400),
                random.Next(Screen.PrimaryScreen.Bounds.Height - 100));
            whisperForm.BackColor = Color.Black;
            whisperForm.ForeColor = Color.Red;
            whisperForm.TopMost = true;
            whisperForm.FormBorderStyle = FormBorderStyle.None;
            whisperForm.ShowInTaskbar = false;

            Label whisperLabel = new Label();
            whisperLabel.Text = whisper;
            whisperLabel.Font = new Font("Chiller", 14, FontStyle.Italic);
            whisperLabel.ForeColor = Color.FromArgb(200, 255, 0, 0);
            whisperLabel.TextAlign = ContentAlignment.MiddleCenter;
            whisperLabel.Dock = DockStyle.Fill;
            whisperForm.Controls.Add(whisperLabel);

            whisperForm.Show();

            // Whisper sound
            Thread whisperSound = new Thread(() => {
                Beep((uint)random.Next(150, 300), 500);
            });
            whisperSound.IsBackground = true;
            whisperSound.Start();

            // Auto close
            Timer closeTimer = new Timer();
            closeTimer.Interval = 3000;
            closeTimer.Tick += (s, args) => {
                whisperForm.Close();
                closeTimer.Stop();
            };
            closeTimer.Start();
        }

        private void EndTheWatching()
        {
            isWatching = false;
            mainTimer?.Stop();
            eyeTimer?.Stop();
            whisperTimer?.Stop();

            // Final disturbing sound
            Thread finalSound = new Thread(() => {
                for (int i = 0; i < 10; i++)
                {
                    Beep((uint)(100 + i * 50), 200);
                    Thread.Sleep(300);
                }
            });
            finalSound.IsBackground = true;
            finalSound.Start();

            MessageBox.Show(
                "👁️ THE WATCHER - GAME OVER 👁️\n\n" +
                "Hai resistito a tutti i 6 livelli...\n" +
                "Ma The Watcher non dimentica mai.\n\n" +
                "Ora conosce il tuo computer.\n" +
                "Conosce le tue abitudini.\n" +
                "Conosce i tuoi segreti.\n\n" +
                "Ogni volta che accenderai il PC...\n" +
                "...ricordati che qualcuno ti sta guardando.\n\n" +
                "👁��� The Watcher non dorme mai. 👁️\n\n" +
                "Grazie per aver giocato!\n" +
                "(Questo era solo un gioco... o no?)",
                "The Watcher - Final Message",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);

            Application.Exit();
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                // Make it harder to escape
                if (random.Next(3) == 0) // 33% chance to actually escape
                {
                    MessageBox.Show(
                        "👁️ TENTATIVO DI FUGA RILEVATO 👁️\n\n" +
                        "The Watcher ti ha lasciato andare...\n" +
                        "...questa volta.\n\n" +
                        "Ma ti ricorderà.",
                        "The Watcher - Escape",
                        MessageBoxButtons.OK);
                    Application.Exit();
                }
                else
                {
                    MessageBox.Show(
                        "👁️ NON PUOI SCAPPARE 👁️\n\n" +
                        "The Watcher non ti lascerà andare così facilmente...\n\n" +
                        "Continua a giocare.",
                        "The Watcher - Denied",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                }
            }
            base.OnKeyDown(e);
        }

        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            DialogResult result = MessageBox.Show(
                "👁️👁️👁️ THE WATCHER 👁️👁️👁️\n\n" +
                "⚠️ ATTENZIONE: CONTENUTO PSICOLOGICAMENTE DISTURBANTE ⚠️\n\n" +
                "Questo non è un normale virus.\n" +
                "È un'esperienza psicologica inquietante.\n\n" +
                "The Watcher è un'entità che:\n" +
                "• Ti osserva costantemente\n" +
                "• Impara le tue abitudini\n" +
                "• Sussurra messaggi disturbanti\n" +
                "• Controlla il tuo cursore\n" +
                "• Diventa sempre più ossessivo\n" +
                "• Non ti lascia mai in pace\n\n" +
                "Una volta iniziato, The Watcher non si fermerà\n" +
                "finché non avrà completato tutti i 6 livelli.\n\n" +
                "Sei sicuro di voler svegliare The Watcher?\n\n" +
                "👁️ Lui ti sta già guardando... 👁️",
                "The Watcher - Warning",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (result == DialogResult.Yes)
            {
                Application.Run(new TheWatcher());
            }
            else
            {
                MessageBox.Show(
                    "👁️ SCELTA SAGGIA 👁️\n\n" +
                    "The Watcher rimarrà dormiente...\n" +
                    "...per ora.\n\n" +
                    "Ma ricorda:\n" +
                    "Ora sa che esisti.",
                    "The Watcher - Sleeping",
                    MessageBoxButtons.OK);
            }
        }
    }
}