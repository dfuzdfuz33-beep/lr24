using System;
using System.Drawing;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace PetrenkoLab24
{
    public class Form1 : Form
    {
        // ── Потоки ──
        private Thread thread1; // CAST
        private Thread thread2; // MD-4
        private Thread thread3; // SEAL

        // ── Панелі виводу ──
        private Panel panel1;
        private Panel panel2;
        private RichTextBox rtb3;

        // ── Кнопки ──
        private Button btnStart1, btnStop1;
        private Button btnStart2, btnStop2;
        private Button btnStart3, btnStop3;
        private Button btnStartAll, btnStopAll;

        // ── Стан потоків ──
        private volatile bool running1 = false;
        private volatile bool running2 = false;
        private volatile bool running3 = false;

        public Form1()
        {
            this.Text = "Лаб. 24 — Багатопотоковість (Варіант 16)";
            this.Width = 900;
            this.Height = 620;
            this.MinimumSize = new Size(700, 500);

            BuildUI();
            CreateThreads();
        }

        // ════════════════════════════════════════════
        //  Побудова інтерфейсу
        // ════════════════════════════════════════════
        private void BuildUI()
        {
            int panelTop = 10;
            int panelH = 300;
            int gap = 10;

            // ── Заголовки колонок ──
            AddLabel("CAST (блоковий)", 10, panelTop - 2, 250, bold: true);
            AddLabel("MD-4 (хешування)", 310, panelTop - 2, 250, bold: true);
            AddLabel("SEAL (потоковий)", 610, panelTop - 2, 250, bold: true);

            panelTop += 18;

            // ── Панель 1: CAST ──
            panel1 = new Panel();
            panel1.Location = new Point(10, panelTop);
            panel1.Size = new Size(270, panelH);
            panel1.BorderStyle = BorderStyle.FixedSingle;
            panel1.BackColor = Color.White;
            Controls.Add(panel1);

            // ── Панель 2: MD-4 ──
            panel2 = new Panel();
            panel2.Location = new Point(300, panelTop);
            panel2.Size = new Size(270, panelH);
            panel2.BorderStyle = BorderStyle.FixedSingle;
            panel2.BackColor = Color.White;
            Controls.Add(panel2);

            // ── RTB 3: SEAL ──
            rtb3 = new RichTextBox();
            rtb3.Location = new Point(590, panelTop);
            rtb3.Size = new Size(285, panelH);
            rtb3.BorderStyle = BorderStyle.FixedSingle;
            rtb3.BackColor = Color.White;
            rtb3.ReadOnly = true;
            rtb3.Font = new Font("Consolas", 8);
            Controls.Add(rtb3);

            // ── Рядок кнопок Запустити ──
            int row1 = panelTop + panelH + gap;
            btnStart1 = AddButton("Запустити 1 потік (CAST)", 10, row1, 270, Color.DodgerBlue, btnStart1_Click);
            btnStart2 = AddButton("Запустити 2 потік (MD-4)", 300, row1, 270, Color.DodgerBlue, btnStart2_Click);
            btnStart3 = AddButton("Запустити 3 потік (SEAL)", 590, row1, 285, Color.DodgerBlue, btnStart3_Click);

            // ── Рядок кнопок Зупинити ──
            int row2 = row1 + 38;
            btnStop1 = AddButton("Зупинити 1 потік", 10, row2, 270, Color.Tomato, btnStop1_Click);
            btnStop2 = AddButton("Зупинити 2 потік", 300, row2, 270, Color.Tomato, btnStop2_Click);
            btnStop3 = AddButton("Зупинити 3 потік", 590, row2, 285, Color.Tomato, btnStop3_Click);

            // ── Запустити всі ──
            int row3 = row2 + 38;
            btnStartAll = AddButton("Запустити всі потоки", 10, row3, 865, Color.SeaGreen, btnStartAll_Click);

            // ── Зупинити всі ──
            int row4 = row3 + 38;
            btnStopAll = AddButton("Зупинити всі потоки", 10, row4, 865, Color.Firebrick, btnStopAll_Click);

            UpdateButtons();

            this.FormClosed += Form1_FormClosed;
        }

        private Label AddLabel(string text, int x, int y, int w, bool bold = false)
        {
            var lbl = new Label();
            lbl.Text = text;
            lbl.Location = new Point(x, y);
            lbl.Width = w;
            lbl.AutoSize = false;
            lbl.Font = bold ? new Font("Arial", 9, FontStyle.Bold) : new Font("Arial", 9);
            Controls.Add(lbl);
            return lbl;
        }

        private Button AddButton(string text, int x, int y, int w, Color color, EventHandler handler)
        {
            var btn = new Button();
            btn.Text = text;
            btn.Location = new Point(x, y);
            btn.Width = w;
            btn.Height = 32;
            btn.BackColor = color;
            btn.ForeColor = Color.White;
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0;
            btn.Font = new Font("Arial", 9, FontStyle.Bold);
            btn.Click += handler;
            Controls.Add(btn);
            return btn;
        }

        // ════════════════════════════════════════════
        //  Створення потоків
        // ════════════════════════════════════════════
        private void CreateThreads()
        {
            thread1 = new Thread(DrawCAST) { IsBackground = true, Name = "CAST" };
            thread2 = new Thread(DrawMD4) { IsBackground = true, Name = "MD4" };
            thread3 = new Thread(RunSEAL) { IsBackground = true, Name = "SEAL" };
        }

        // ════════════════════════════════════════════
        //  Метод 1 — CAST: малює прямокутники з ключем
        //  CAST — симетричний блоковий шифр (64-біт блок, 128-біт ключ)
        //  Візуалізація: кожен прямокутник = один раунд шифрування CAST
        //  Координати та колір визначаються псевдо-ключем (імітація раундів)
        // ════════════════════════════════════════════
        private void DrawCAST()
        {
            Random rnd = new Random();
            // Імітація 128-бітного ключа CAST (16 байт)
            byte[] key = new byte[16];
            rnd.NextBytes(key);

            Graphics g = panel1.CreateGraphics();
            int round = 0;

            while (running1)
            {
                try
                {
                    Thread.Sleep(80);

                    // CAST має 16 раундів — обчислюємо параметри через ключ
                    int r = round % 16;
                    // XOR суміжних байтів ключа = імітація раундової функції
                    int w = Math.Abs((key[r] ^ key[(r + 1) % 16])) % (panel1.Width - 20) + 10;
                    int h = Math.Abs((key[(r + 2) % 16] ^ key[(r + 3) % 16])) % (panel1.Height - 20) + 10;
                    int x = rnd.Next(0, panel1.Width - w);
                    int y = rnd.Next(0, panel1.Height - h);

                    // Колір залежить від раунду (ротація ключа)
                    Color c = Color.FromArgb(
                        150,
                        key[r % 16],
                        key[(r + 5) % 16],
                        key[(r + 10) % 16]);

                    // Оновлюємо ключ (імітація key schedule CAST)
                    key[r] = (byte)(key[r] ^ (byte)(round & 0xFF));
                    round++;

                    using (Pen pen = new Pen(c, 1.5f))
                    using (SolidBrush brush = new SolidBrush(Color.FromArgb(30, c)))
                    {
                        g.DrawRectangle(pen, x, y, w, h);
                        g.FillRectangle(brush, x, y, w, h);
                    }

                    // Підпис раунду
                    if (round % 50 == 0)
                    {
                        g.Clear(Color.White);
                        g.DrawString($"CAST раунд #{round}", new Font("Arial", 8), Brushes.DimGray, 5, 5);
                    }
                }
                catch { break; }
            }
            g.Dispose();
        }

        // ════════════════════════════════════════════
        //  Метод 2 — MD-4: малює еліпси, візуалізує хешування
        //  MD-4 — алгоритм хешування (128 біт, 3 раунди по 16 операцій)
        //  Візуалізація: кожен еліпс = один блок даних, що хешується
        //  Розмір/позиція залежать від проміжного значення хешу
        // ════════════════════════════════════════════
        private void DrawMD4()
        {
            Graphics g = panel2.CreateGraphics();
            int blockNum = 0;

            // Початкові значення MD-4 (магічні константи з RFC 1320)
            uint A = 0x67452301;
            uint B = 0xEFCDAB89;
            uint C = 0x98BADCFE;
            uint D = 0x10325476;

            Random rnd = new Random();

            while (running2)
            {
                try
                {
                    Thread.Sleep(80);

                    // Імітація одного раунду MD-4 (функція F + зсув)
                    uint msg = (uint)rnd.Next();
                    uint F = (B & C) | (~B & D);            // MD-4 раунд 1: F(B,C,D)
                    A = MD4LeftRotate(A + F + msg, 3);       // зсув на 3 біти
                    // Перестановка регістрів
                    uint tmp = D; D = C; C = B; B = A; A = tmp;

                    blockNum++;

                    // Координати з регістрів A,B,C,D
                    int x = (int)(A % (uint)(panel2.Width - 40));
                    int y = (int)(B % (uint)(panel2.Height - 40));
                    int w = (int)(C % 100) + 20;
                    int h = (int)(D % 80) + 15;

                    Color c = Color.FromArgb(
                        180,
                        (int)(A & 0xFF),
                        (int)(B & 0xFF),
                        (int)(C & 0xFF));

                    using (Pen pen = new Pen(c, 1.5f))
                    using (SolidBrush brush = new SolidBrush(Color.FromArgb(30, c)))
                    {
                        g.DrawEllipse(pen, x, y, w, h);
                        g.FillEllipse(brush, x, y, w, h);
                    }

                    if (blockNum % 40 == 0)
                    {
                        g.Clear(Color.White);
                        g.DrawString($"MD-4 блок #{blockNum}", new Font("Arial", 8), Brushes.DimGray, 5, 5);
                    }
                }
                catch { break; }
            }
            g.Dispose();
        }

        // MD-4 допоміжна: циклічний лівий зсув
        private static uint MD4LeftRotate(uint x, int n) => (x << n) | (x >> (32 - n));

        // ════════════════════════════════════════════
        //  Метод 3 — SEAL: генерує псевдовипадковий потік
        //  SEAL — Software-Efficient Algorithm, потоковий шифр
        //  Виводить псевдовипадкові байти (гамму) у текстове поле
        //  Алгоритм базується на SHA-подібних перетвореннях
        // ════════════════════════════════════════════
        private void RunSEAL()
        {
            // Ключ SEAL (160 біт = 5 x uint, як у специфікації)
            uint[] K = new uint[] { 0xDEADBEEF, 0xCAFEBABE, 0x12345678, 0xABCDEF01, 0xFEDCBA98 };
            uint n = 0; // лічильник позиції в потоці
            int chunkNum = 0;

            while (running3)
            {
                try
                {
                    Thread.Sleep(150);

                    // SEAL генерує блоки по 512 байт
                    // Спрощена імітація: XOR з ключем + зсуви (як у SEAL 3.0)
                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine($"── Блок #{++chunkNum} (n={n}) ──");

                    for (int i = 0; i < 8; i++)
                    {
                        // Имітація SEAL: A,B,C,D з лічильника і ключа
                        uint a = n ^ K[0];
                        uint b = (n + 1) ^ K[1];
                        uint c = MD4LeftRotate(a ^ b, 9) ^ K[2];
                        uint d = MD4LeftRotate(b ^ c, 13) ^ K[3];

                        // 4 байти гами
                        byte g0 = (byte)((a ^ d) & 0xFF);
                        byte g1 = (byte)((b ^ c) & 0xFF);
                        byte g2 = (byte)(((a >> 8) ^ (d >> 16)) & 0xFF);
                        byte g3 = (byte)(((b >> 16) ^ (c >> 8)) & 0xFF);

                        sb.Append($"{g0:X2} {g1:X2} {g2:X2} {g3:X2}  ");
                        n++;
                    }

                    string output = sb.ToString();

                    if (rtb3.IsHandleCreated)
                    {
                        rtb3.Invoke((MethodInvoker)delegate
                        {
                            if (rtb3.Lines.Length > 200)
                                rtb3.Clear();
                            rtb3.AppendText(output + "\n");
                            rtb3.ScrollToCaret();
                        });
                    }
                }
                catch { break; }
            }
        }

        // ════════════════════════════════════════════
        //  Обробники кнопок
        // ════════════════════════════════════════════
        private void btnStart1_Click(object sender, EventArgs e)
        {
            if (!running1)
            {
                running1 = true;
                if (thread1 == null || !thread1.IsAlive)
                {
                    thread1 = new Thread(DrawCAST) { IsBackground = true };
                    thread1.Start();
                }
                UpdateButtons();
            }
        }

        private void btnStop1_Click(object sender, EventArgs e)
        {
            running1 = false;
            UpdateButtons();
        }

        private void btnStart2_Click(object sender, EventArgs e)
        {
            if (!running2)
            {
                running2 = true;
                if (thread2 == null || !thread2.IsAlive)
                {
                    thread2 = new Thread(DrawMD4) { IsBackground = true };
                    thread2.Start();
                }
                UpdateButtons();
            }
        }

        private void btnStop2_Click(object sender, EventArgs e)
        {
            running2 = false;
            UpdateButtons();
        }

        private void btnStart3_Click(object sender, EventArgs e)
        {
            if (!running3)
            {
                running3 = true;
                if (thread3 == null || !thread3.IsAlive)
                {
                    thread3 = new Thread(RunSEAL) { IsBackground = true };
                    thread3.Start();
                }
                UpdateButtons();
            }
        }

        private void btnStop3_Click(object sender, EventArgs e)
        {
            running3 = false;
            UpdateButtons();
        }

        private void btnStartAll_Click(object sender, EventArgs e)
        {
            btnStart1_Click(null, null);
            btnStart2_Click(null, null);
            btnStart3_Click(null, null);
        }

        private void btnStopAll_Click(object sender, EventArgs e)
        {
            running1 = false;
            running2 = false;
            running3 = false;
            UpdateButtons();
        }

        private void UpdateButtons()
        {
            btnStop1.Enabled = running1;
            btnStop2.Enabled = running2;
            btnStop3.Enabled = running3;
            btnStart1.Enabled = !running1;
            btnStart2.Enabled = !running2;
            btnStart3.Enabled = !running3;
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            running1 = false;
            running2 = false;
            running3 = false;
        }
    }
}
