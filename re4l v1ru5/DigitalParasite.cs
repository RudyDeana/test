using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Linq;

namespace DigitalParasiteVirus
{
    public partial class DigitalParasite : Form
    {
        [DllImport("user32.dll")]
        static extern IntPtr GetDC(IntPtr hwnd);

        [DllImport("user32.dll")]
        static extern int ReleaseDC(IntPtr hwnd, IntPtr hdc);

        [DllImport("gdi32.dll")]
        static extern bool BitBlt(IntPtr hdc, int nXDest, int nYDest, int nWidth, int nHeight, IntPtr hdcSrc, int nXSrc, int nYSrc, uint dwRop);

        [DllImport("kernel32.dll")]
        static extern void Beep(uint dwFreq, uint dwDuration);

        [DllImport("user32.dll")]
        static extern bool SetCursorPos(int x, int y);

        private Random random = new Random();
        private Timer infectionTimer;
        private Timer parasiteTimer;
        private Timer corruptionTimer;
        
        private int infectionLevel = 0;
        private int parasiteCount = 0;
        private bool isInfected = true;
        
        // Digital parasites
        private List<DigitalBug> parasites;
        private List<Point> corruptedPixels;
        private List<string> stolenData;
        
        // Infection stages
        private string[] infectionStages = {
            "INFEZIONE INIZIALE",
            "MOLTIPLICAZIONE PARASSITI",
            "CORRUZIONE DATI",
            "INVASIONE SISTEMA", 
            "CONTROLLO TOTALE",
            "ASSIMILAZIONE COMPLETA"
        };
        
        private string[] parasiteMessages = {
            "Stiamo entrando nel tuo sistema...",
            "I parassiti si stanno moltiplicando...",
            "Stiamo rubando i tuoi dati...",
            "Il tuo computer ci appartiene ora...",
            "Resistenza è inutile...",
            "Sei stato assimilato..."
        };

        public DigitalParasite()
        {
            InitializeComponent();
            InitializeInfection();
            StartInfection();
        }

        private void InitializeComponent()
        {
            this.WindowState = FormWindowState.Maximized;
            this.FormBorderStyle = FormBorderStyle.None;
            this.TopMost = true;
            this.BackColor = Color.Black;
            this.Text = "Digital Parasite";
            this.ShowInTaskbar = false;
            this.KeyPreview = true;
        }

        private void InitializeInfection()
        {
            parasites = new List<DigitalBug>();
            corruptedPixels = new List<Point>();
            stolenData = new List<string>();
            
            // Start with a few parasites
            for (int i = 0; i < 5; i++)
            {
                parasites.Add(new DigitalBug(
                    random.Next(Screen.PrimaryScreen.Bounds.Width),
                    random.Next(Screen.PrimaryScreen.Bounds.Height),
                    random));
            }
        }

        private void StartInfection()
        {
            ShowInfectionWarning();

            // Main infection timer
            infectionTimer = new Timer();
            infectionTimer.Interval = 150;
            infectionTimer.Tick += InfectionTimer_Tick;
            infectionTimer.Start();

            // Parasite behavior timer
            parasiteTimer = new Timer();
            parasiteTimer.Interval = 100;
            parasiteTimer.Tick += ParasiteTimer_Tick;
            parasiteTimer.Start();

            // Corruption spread timer
            corruptionTimer = new Timer();
            corruptionTimer.Interval = 50;
            corruptionTimer.Tick += CorruptionTimer_Tick;
            corruptionTimer.Start();

            // Infection level progression
            Timer levelTimer = new Timer();
            levelTimer.Interval = 25000; // 25 seconds per level
            levelTimer.Tick += (s, e) => {
                infectionLevel++;
                if (infectionLevel >= infectionStages.Length)
                {
                    CompleteAssimilation();
                }
                else
                {
                    ShowInfectionProgress();
                }
            };
            levelTimer.Start();
        }

        private void ShowInfectionWarning()
        {
            MessageBox.Show(
                "🦠 DIGITAL PARASITE 🦠\n\n" +
                "ATTENZIONE: INFEZIONE RILEVATA\n\n" +
                "Un parassita digitale ha invaso il tuo sistema.\n" +
                "Non è un virus normale... è qualcosa di VIVO.\n\n" +
                "I parassiti digitali:\n" +
                "• Si moltiplicano esponenzialmente\n" +
                "• Rubano e corrompono i dati\n" +
                "• Imparano dal tuo comportamento\n" +
                "• Diventano sempre più intelligenti\n" +
                "• Non possono essere fermati\n\n" +
                "L'infezione è già iniziata...\n" +
                "Preparati all'assimilazione.",
                "Digital Parasite - Infection Alert",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }

        private void ShowInfectionProgress()
        {
            // Infection sound
            Thread soundThread = new Thread(() => {
                for (int i = 0; i < 3; i++)
                {
                    Beep((uint)(300 + infectionLevel * 200), 200);
                    Thread.Sleep(150);
                }
            });
            soundThread.IsBackground = true;
            soundThread.Start();

            MessageBox.Show(
                $"🦠 LIVELLO INFEZIONE {infectionLevel + 1} 🦠\n\n" +
                $"{infectionStages[infectionLevel]}\n\n" +
                $"{parasiteMessages[infectionLevel]}\n\n" +
                $"Parassiti attivi: {parasites.Count}\n" +
                $"Pixel corrotti: {corruptedPixels.Count}\n" +
                $"Dati rubati: {stolenData.Count}\n\n" +
                "L'infezione si sta diffondendo...",
                "Digital Parasite - Infection Progress",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);
        }

        private void InfectionTimer_Tick(object sender, EventArgs e)
        {
            if (!isInfected) return;

            switch (infectionLevel)
            {
                case 0: // Initial Infection
                    DrawInitialInfection();
                    break;
                case 1: // Parasite Multiplication
                    DrawParasiteMultiplication();
                    break;
                case 2: // Data Corruption
                    DrawDataCorruption();
                    break;
                case 3: // System Invasion
                    DrawSystemInvasion();
                    break;
                case 4: // Total Control
                    DrawTotalControl();
                    break;
                case 5: // Complete Assimilation
                    DrawCompleteAssimilation();
                    break;
            }
        }

        private void ParasiteTimer_Tick(object sender, EventArgs e)
        {
            // Update all parasites
            foreach (var parasite in parasites)
            {
                parasite.Update();
            }

            // Parasites multiply based on infection level
            if (parasites.Count < (infectionLevel + 1) * 10 && random.Next(100) < 5)
            {
                // Create new parasite near existing one
                if (parasites.Count > 0)
                {
                    var parent = parasites[random.Next(parasites.Count)];
                    parasites.Add(new DigitalBug(
                        parent.X + random.Next(-50, 51),
                        parent.Y + random.Next(-50, 51),
                        random));
                }
            }
        }

        private void CorruptionTimer_Tick(object sender, EventArgs e)
        {
            // Add corrupted pixels
            for (int i = 0; i < infectionLevel + 1; i++)
            {
                corruptedPixels.Add(new Point(
                    random.Next(Screen.PrimaryScreen.Bounds.Width),
                    random.Next(Screen.PrimaryScreen.Bounds.Height)));
            }

            // Limit corruption to prevent memory issues
            if (corruptedPixels.Count > 1000)
            {
                corruptedPixels.RemoveRange(0, 500);
            }

            // Steal fake data
            if (random.Next(100) < 10)
            {
                string[] fakeData = {
                    "password.txt", "documents/", "photos/", "browser_history.db",
                    "personal_info.doc", "bank_details.pdf", "private_keys.txt"
                };
                stolenData.Add(fakeData[random.Next(fakeData.Length)]);
            }
        }

        // LEVEL 1: Initial Infection
        private void DrawInitialInfection()
        {
            IntPtr desktop = GetDC(IntPtr.Zero);
            using (Graphics g = Graphics.FromHdc(desktop))
            {
                // Draw small parasites
                foreach (var parasite in parasites.Take(10))
                {
                    DrawParasite(g, parasite, 8);
                }

                // Initial corruption
                foreach (var pixel in corruptedPixels.Take(50))
                {
                    using (Brush brush = new SolidBrush(Color.FromArgb(100, 0, 255, 0)))
                    {
                        g.FillRectangle(brush, pixel.X, pixel.Y, 2, 2);
                    }
                }

                // Infection message
                if (parasiteCount % 100 == 0)
                {
                    Font infectionFont = new Font("Consolas", 14, FontStyle.Bold);
                    using (Brush brush = new SolidBrush(Color.FromArgb(150, 0, 255, 0)))
                    {
                        g.DrawString("🦠 INFEZIONE IN CORSO... 🦠", infectionFont, brush,
                            random.Next(Screen.PrimaryScreen.Bounds.Width - 300),
                            random.Next(Screen.PrimaryScreen.Bounds.Height - 50));
                    }
                    infectionFont.Dispose();
                }
            }
            ReleaseDC(IntPtr.Zero, desktop);
            parasiteCount++;
        }

        // LEVEL 2: Parasite Multiplication
        private void DrawParasiteMultiplication()
        {
            IntPtr desktop = GetDC(IntPtr.Zero);
            using (Graphics g = Graphics.FromHdc(desktop))
            {
                // Draw more parasites, bigger
                foreach (var parasite in parasites.Take(20))
                {
                    DrawParasite(g, parasite, 12);
                }

                // Multiplication trails
                for (int i = 0; i < 5; i++)
                {
                    using (Pen trailPen = new Pen(Color.FromArgb(100, 0, 255, 0), 2))
                    {
                        Point start = new Point(random.Next(Screen.PrimaryScreen.Bounds.Width),
                                              random.Next(Screen.PrimaryScreen.Bounds.Height));
                        Point end = new Point(start.X + random.Next(-100, 101),
                                            start.Y + random.Next(-100, 101));
                        g.DrawLine(trailPen, start, end);
                    }
                }

                // Show multiplication
                if (parasiteCount % 80 == 0)
                {
                    Font multiFont = new Font("Consolas", 16, FontStyle.Bold);
                    using (Brush brush = new SolidBrush(Color.FromArgb(200, 0, 255, 0)))
                    {
                        g.DrawString($"🦠 MOLTIPLICAZIONE: {parasites.Count} PARASSITI 🦠", 
                                   multiFont, brush,
                            random.Next(Screen.PrimaryScreen.Bounds.Width - 400),
                            random.Next(Screen.PrimaryScreen.Bounds.Height - 50));
                    }
                    multiFont.Dispose();
                }
            }
            ReleaseDC(IntPtr.Zero, desktop);
            parasiteCount++;
        }

        // LEVEL 3: Data Corruption
        private void DrawDataCorruption()
        {
            IntPtr desktop = GetDC(IntPtr.Zero);
            using (Graphics g = Graphics.FromHdc(desktop))
            {
                // Draw all parasites
                foreach (var parasite in parasites)
                {
                    DrawParasite(g, parasite, 15);
                }

                // Heavy corruption
                foreach (var pixel in corruptedPixels)
                {
                    Color corruptColor = Color.FromArgb(
                        random.Next(100, 200),
                        random.Next(256),
                        255,
                        random.Next(256));
                    
                    using (Brush brush = new SolidBrush(corruptColor))
                    {
                        g.FillRectangle(brush, pixel.X, pixel.Y, 
                                      random.Next(1, 5), random.Next(1, 5));
                    }
                }

                // Data theft visualization
                if (parasiteCount % 60 == 0)
                {
                    Font dataFont = new Font("Consolas", 12, FontStyle.Bold);
                    using (Brush brush = new SolidBrush(Color.FromArgb(255, 255, 0, 0)))
                    {
                        if (stolenData.Count > 0)
                        {
                            string stolen = stolenData[random.Next(stolenData.Count)];
                            g.DrawString($"🔓 RUBANDO: {stolen}", dataFont, brush,
                                random.Next(Screen.PrimaryScreen.Bounds.Width - 300),
                                random.Next(Screen.PrimaryScreen.Bounds.Height - 50));
                        }
                    }
                    dataFont.Dispose();
                }
            }
            ReleaseDC(IntPtr.Zero, desktop);
            parasiteCount++;
        }

        // LEVEL 4: System Invasion
        private void DrawSystemInvasion()
        {
            IntPtr desktop = GetDC(IntPtr.Zero);
            using (Graphics g = Graphics.FromHdc(desktop))
            {
                // Parasites everywhere
                foreach (var parasite in parasites)
                {
                    DrawParasite(g, parasite, random.Next(10, 25));
                }

                // System invasion lines
                for (int i = 0; i < 10; i++)
                {
                    using (Pen invasionPen = new Pen(Color.FromArgb(150, 255, 0, 0), 3))
                    {
                        g.DrawLine(invasionPen,
                            0, random.Next(Screen.PrimaryScreen.Bounds.Height),
                            Screen.PrimaryScreen.Bounds.Width, 
                            random.Next(Screen.PrimaryScreen.Bounds.Height));
                    }
                }

                // System messages
                Font systemFont = new Font("Consolas", 18, FontStyle.Bold);
                using (Brush brush = new SolidBrush(Color.FromArgb(255, 255, 0, 0)))
                {
                    string[] systemMsgs = {
                        "SISTEMA COMPROMESSO",
                        "ACCESSO NEGATO", 
                        "CONTROLLO PERDUTO",
                        "INVASIONE COMPLETA"
                    };
                    string msg = systemMsgs[random.Next(systemMsgs.Length)];
                    g.DrawString($"⚠️ {msg} ⚠️", systemFont, brush,
                        random.Next(Screen.PrimaryScreen.Bounds.Width - 400),
                        random.Next(Screen.PrimaryScreen.Bounds.Height - 50));
                }
                systemFont.Dispose();
            }
            ReleaseDC(IntPtr.Zero, desktop);
            parasiteCount++;
        }

        // LEVEL 5: Total Control
        private void DrawTotalControl()
        {
            IntPtr desktop = GetDC(IntPtr.Zero);
            using (Graphics g = Graphics.FromHdc(desktop))
            {
                // Massive parasites
                foreach (var parasite in parasites)
                {
                    DrawParasite(g, parasite, random.Next(20, 40));
                }

                // Control the cursor
                if (parasiteCount % 10 == 0)
                {
                    Point cursor = Cursor.Position;
                    SetCursorPos(
                        cursor.X + random.Next(-20, 21),
                        cursor.Y + random.Next(-20, 21));
                }

                // Screen distortion
                for (int i = 0; i < 5; i++)
                {
                    int x = random.Next(Screen.PrimaryScreen.Bounds.Width - 100);
                    int y = random.Next(Screen.PrimaryScreen.Bounds.Height - 100);
                    
                    BitBlt(desktop, x + random.Next(-5, 6), y + random.Next(-5, 6), 100, 100,
                           desktop, x, y, 0x00CC0020);
                }

                // Control message
                Font controlFont = new Font("Consolas", 24, FontStyle.Bold);
                using (Brush brush = new SolidBrush(Color.FromArgb(255, 255, 255, 0)))
                {
                    g.DrawString("🦠 CONTROLLO TOTALE ACQUISITO 🦠", controlFont, brush,
                        Screen.PrimaryScreen.Bounds.Width / 2 - 300,
                        Screen.PrimaryScreen.Bounds.Height / 2);
                }
                controlFont.Dispose();
            }
            ReleaseDC(IntPtr.Zero, desktop);
            parasiteCount++;
        }

        // LEVEL 6: Complete Assimilation
        private void DrawCompleteAssimilation()
        {
            IntPtr desktop = GetDC(IntPtr.Zero);
            using (Graphics g = Graphics.FromHdc(desktop))
            {
                // Screen completely overrun
                foreach (var parasite in parasites)
                {
                    DrawParasite(g, parasite, random.Next(30, 60));
                }

                // Complete corruption
                for (int i = 0; i < 100; i++)
                {
                    Color assimColor = Color.FromArgb(
                        random.Next(256),
                        random.Next(256),
                        255,
                        random.Next(256));
                    
                    using (Brush brush = new SolidBrush(assimColor))
                    {
                        g.FillRectangle(brush,
                            random.Next(Screen.PrimaryScreen.Bounds.Width),
                            random.Next(Screen.PrimaryScreen.Bounds.Height),
                            random.Next(5, 20),
                            random.Next(5, 20));
                    }
                }

                // Final message
                Font finalFont = new Font("Consolas", 32, FontStyle.Bold);
                using (Brush brush = new System.Drawing.Drawing2D.LinearGradientBrush(
                    new Rectangle(0, 0, 800, 100),
                    Color.Green, Color.Red, System.Drawing.Drawing2D.LinearGradientMode.Horizontal))
                {
                    g.DrawString("🦠 ASSIMILAZIONE COMPLETA 🦠", finalFont, brush,
                        Screen.PrimaryScreen.Bounds.Width / 2 - 400,
                        Screen.PrimaryScreen.Bounds.Height / 2 - 50);
                }
                finalFont.Dispose();

                // Screen chaos
                if (parasiteCount % 5 == 0)
                {
                    BitBlt(desktop, 0, 0, 
                           Screen.PrimaryScreen.Bounds.Width, 
                           Screen.PrimaryScreen.Bounds.Height,
                           desktop, 0, 0, 0x00550009);
                }
            }
            ReleaseDC(IntPtr.Zero, desktop);
            parasiteCount++;
        }

        private void DrawParasite(Graphics g, DigitalBug parasite, int size)
        {
            // Parasite body (green/red mix)
            Color parasiteColor = Color.FromArgb(
                200,
                random.Next(100, 256),
                255,
                random.Next(0, 100));
            
            using (Brush bodyBrush = new SolidBrush(parasiteColor))
            {
                g.FillEllipse(bodyBrush, parasite.X, parasite.Y, size, size);
            }

            // Parasite legs/tentacles
            using (Pen legPen = new Pen(Color.FromArgb(150, 0, 255, 0), 2))
            {
                for (int i = 0; i < 6; i++)
                {
                    double angle = i * Math.PI / 3;
                    int legLength = size / 2;
                    Point legEnd = new Point(
                        parasite.X + size / 2 + (int)(Math.Cos(angle) * legLength),
                        parasite.Y + size / 2 + (int)(Math.Sin(angle) * legLength));
                    
                    g.DrawLine(legPen, 
                        new Point(parasite.X + size / 2, parasite.Y + size / 2),
                        legEnd);
                }
            }

            // Parasite eyes
            using (Brush eyeBrush = new SolidBrush(Color.Red))
            {
                g.FillEllipse(eyeBrush, parasite.X + size / 4, parasite.Y + size / 4, 3, 3);
                g.FillEllipse(eyeBrush, parasite.X + 3 * size / 4, parasite.Y + size / 4, 3, 3);
            }
        }

        private void CompleteAssimilation()
        {
            isInfected = false;
            infectionTimer?.Stop();
            parasiteTimer?.Stop();
            corruptionTimer?.Stop();

            // Final infection sound
            Thread finalSound = new Thread(() => {
                for (int i = 0; i < 15; i++)
                {
                    Beep((uint)(200 + i * 100), 150);
                    Thread.Sleep(100);
                }
            });
            finalSound.IsBackground = true;
            finalSound.Start();

            MessageBox.Show(
                "🦠 DIGITAL PARASITE - ASSIMILAZIONE COMPLETA 🦠\n\n" +
                "L'infezione è completa.\n" +
                "Il tuo sistema è stato assimilato.\n\n" +
                $"STATISTICHE FINALI:\n" +
                $"• Parassiti generati: {parasites.Count}\n" +
                $"• Pixel corrotti: {corruptedPixels.Count}\n" +
                $"• Dati rubati: {stolenData.Count}\n\n" +
                "I parassiti digitali ora controllano tutto.\n" +
                "Hanno imparato dai tuoi dati.\n" +
                "Hanno copiato i tuoi file.\n" +
                "Hanno memorizzato le tue abitudini.\n\n" +
                "Benvenuto nella nuova realtà digitale.\n" +
                "Sei stato... ASSIMILATO.\n\n" +
                "🦠 Resistenza è stata inutile. 🦠",
                "Digital Parasite - Game Over",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);

            Application.Exit();
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                // Parasites resist escape attempts
                if (random.Next(4) == 0) // 25% chance to escape
                {
                    MessageBox.Show(
                        "🦠 FUGA RIUSCITA 🦠\n\n" +
                        "Sei riuscito a sfuggire ai parassiti...\n" +
                        "Ma hanno già rubato alcuni dati.\n\n" +
                        "La prossima volta potrebbero essere più forti.",
                        "Digital Parasite - Escape",
                        MessageBoxButtons.OK);
                    Application.Exit();
                }
                else
                {
                    MessageBox.Show(
                        "🦠 FUGA BLOCCATA 🦠\n\n" +
                        "I parassiti hanno bloccato il tuo tentativo di fuga!\n" +
                        "Stanno imparando dalle tue azioni...\n\n" +
                        "L'infezione continua.",
                        "Digital Parasite - Blocked",
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
                "🦠🦠🦠 DIGITAL PARASITE 🦠🦠🦠\n\n" +
                "⚠️ ATTENZIONE: INFEZIONE IMMINENTE ⚠️\n\n" +
                "Questo programma simula un'infezione da parassiti digitali.\n\n" +
                "I parassiti digitali sono entità che:\n" +
                "• Si moltiplicano esponenzialmente\n" +
                "• Corrompono i dati del sistema\n" +
                "• Rubano informazioni personali (fake)\n" +
                "• Prendono controllo del cursore\n" +
                "• Diventano sempre più intelligenti\n" +
                "• Resistono ai tentativi di rimozione\n\n" +
                "L'infezione progredisce attraverso 6 livelli:\n" +
                "1. Infezione Iniziale\n" +
                "2. Moltiplicazione Parassiti\n" +
                "3. Corruzione Dati\n" +
                "4. Invasione Sistema\n" +
                "5. Controllo Totale\n" +
                "6. Assimilazione Completa\n\n" +
                "Una volta iniziata, l'infezione non può essere fermata.\n\n" +
                "Vuoi essere infettato dai parassiti digitali?",
                "Digital Parasite - Infection Warning",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (result == DialogResult.Yes)
            {
                Application.Run(new DigitalParasite());
            }
            else
            {
                MessageBox.Show(
                    "🦠 INFEZIONE EVITATA 🦠\n\n" +
                    "Hai evitato l'infezione... per ora.\n\n" +
                    "Ma i parassiti digitali sono sempre in agguato.\n" +
                    "Potrebbero tornare quando meno te lo aspetti.",
                    "Digital Parasite - Safe",
                    MessageBoxButtons.OK);
            }
        }
    }

    // Helper class for digital parasites
    public class DigitalBug
    {
        public int X { get; set; }
        public int Y { get; set; }
        private int velocityX;
        private int velocityY;
        private Random random;

        public DigitalBug(int x, int y, Random rnd)
        {
            X = x;
            Y = y;
            random = rnd;
            velocityX = random.Next(-3, 4);
            velocityY = random.Next(-3, 4);
        }

        public void Update()
        {
            X += velocityX;
            Y += velocityY;

            // Bounce off screen edges
            if (X <= 0 || X >= Screen.PrimaryScreen.Bounds.Width - 20)
                velocityX = -velocityX;
            if (Y <= 0 || Y >= Screen.PrimaryScreen.Bounds.Height - 20)
                velocityY = -velocityY;

            // Random direction changes
            if (random.Next(100) < 5)
            {
                velocityX = random.Next(-3, 4);
                velocityY = random.Next(-3, 4);
            }
        }
    }
}