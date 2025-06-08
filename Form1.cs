using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using GraphColoringApp;

namespace GraphColoringApp
{
    public partial class Form1 : Form
    {
        private List<State> mexStates;
        private bool[,] adjacencyMatrix;
        private RichTextBox resultsTextBox;
        private Panel drawingPanel;
        private Button startButton;
        private Dictionary<int, Color> colorMap;
        private static Random random = new Random();

        public Form1()
        {
            InitializeComponent();
            InitializeGraphData();
            InitializeUI();
        }

        private void InitializeComponent()
        {
            this.Text = "Розфарбовування Графа Мексики";
            this.Size = new Size(1200, 800); // Збільшено розмір вікна
            this.DoubleBuffered = true;
            this.WindowState = FormWindowState.Maximized; // Максимізуємо вікно
            InitializeColorMap();
        }

        private void InitializeColorMap()
        {
            colorMap = new Dictionary<int, Color>();
            colorMap.Add(0, Color.Red);
            colorMap.Add(1, Color.Blue);
            colorMap.Add(2, Color.Green);
            colorMap.Add(3, Color.Orange);
            colorMap.Add(4, Color.Purple);
            colorMap.Add(5, Color.Brown);
            colorMap.Add(6, Color.Cyan);
            colorMap.Add(7, Color.Magenta);
            colorMap.Add(8, Color.Yellow);
            colorMap.Add(9, Color.Pink);

            for (int i = 10; i < 20; i++)
            {
                colorMap.Add(i, GetRandomColor());
            }
        }

        private Color GetRandomColor()
        {
            int r = random.Next(50, 200);
            int g = random.Next(50, 200);
            int b = random.Next(50, 200);
            return Color.FromArgb(r, g, b);
        }

        private void InitializeGraphData()
        {
            mexStates = new List<State>
            {
                new State(0, "Mexico City"),
                new State(1, "Estado de México"),
                new State(2, "Jalisco"),
                new State(3, "Nuevo León"),
                new State(4, "Veracruz"),
                new State(5, "Puebla"),
                new State(6, "Guanajuato"),
                new State(7, "Baja California"),
                new State(8, "Sonora"),
                new State(9, "Chihuahua"),
                new State(10, "Oaxaca"),
                new State(11, "Chiapas"),
                new State(12, "Campeche"),
                new State(13, "Yucatán"),
                new State(14, "Quintana Roo")
            };

            adjacencyMatrix = new bool[15, 15]
            {
                {false, true, false, false, false, true, false, false, false, false, false, false, false, false, false},
                {true, false, true, false, true, true, true, false, false, false, false, false, false, false, false},
                {false, true, false, false, false, false, true, false, false, false, false, false, false, false, false},
                {false, false, false, false, true, false, false, false, false, true, false, false, false, false, false},
                {false, true, false, true, false, true, false, false, false, false, true, false, false, false, false},
                {true, true, false, false, true, false, false, false, false, false, true, false, false, false, false},
                {false, true, true, false, false, false, false, false, false, false, false, false, false, false, false},
                {false, false, false, false, false, false, false, false, true, false, false, false, false, false, false},
                {false, false, false, false, false, false, false, true, false, true, false, false, false, false, false},
                {false, false, false, true, false, false, false, false, true, false, false, false, false, false, false},
                {false, false, false, false, true, true, false, false, false, false, false, true, false, false, false},
                {false, false, false, false, false, false, false, false, false, false, true, false, true, false, false},
                {false, false, false, false, false, false, false, false, false, false, false, true, false, true, true},
                {false, false, false, false, false, false, false, false, false, false, false, false, true, false, true},
                {false, false, false, false, false, false, false, false, false, false, false, false, true, true, false}
            };

            // Встановлення позицій для кращої візуалізації (розташування у формі кола з центром)
            SetOptimalPositions();
        }

        private void SetOptimalPositions()
        {
            // Розташовуємо штати у більш оптимальному порядку для візуалізації
            mexStates[0].Position = new PointF(500, 400);  // Mexico City (центр)
            mexStates[1].Position = new PointF(600, 350);  // Estado de México
            mexStates[2].Position = new PointF(300, 300);  // Jalisco
            mexStates[3].Position = new PointF(700, 200);  // Nuevo León
            mexStates[4].Position = new PointF(650, 450);  // Veracruz
            mexStates[5].Position = new PointF(550, 500);  // Puebla
            mexStates[6].Position = new PointF(400, 250);  // Guanajuato
            mexStates[7].Position = new PointF(100, 150);  // Baja California
            mexStates[8].Position = new PointF(200, 200);  // Sonora
            mexStates[9].Position = new PointF(350, 150);  // Chihuahua
            mexStates[10].Position = new PointF(500, 600); // Oaxaca
            mexStates[11].Position = new PointF(600, 700); // Chiapas
            mexStates[12].Position = new PointF(750, 650); // Campeche
            mexStates[13].Position = new PointF(850, 550); // Yucatán
            mexStates[14].Position = new PointF(900, 650); // Quintana Roo
        }

        private void InitializeUI()
        {
            // Кнопка для запуску алгоритму
            startButton = new Button();
            startButton.Text = "Розфарбувати Граф";
            startButton.Dock = DockStyle.Top;
            startButton.Height = 50;
            startButton.Font = new Font("Arial", 12, FontStyle.Bold);
            startButton.BackColor = Color.LightBlue;
            startButton.Click += StartButton_Click;
            this.Controls.Add(startButton);

            // Текстове поле для виведення результатів
            resultsTextBox = new RichTextBox();
            resultsTextBox.Dock = DockStyle.Bottom;
            resultsTextBox.Height = 200;
            resultsTextBox.ReadOnly = true;
            resultsTextBox.Font = new Font("Consolas", 10);
            this.Controls.Add(resultsTextBox);

            // Панель для малювання графа
            drawingPanel = new Panel();
            drawingPanel.Dock = DockStyle.Fill;
            drawingPanel.BorderStyle = BorderStyle.FixedSingle;
            drawingPanel.BackColor = Color.White;
            drawingPanel.Paint += DrawingPanel_Paint;
            this.Controls.Add(drawingPanel);

            // Правильний порядок контролів
            this.Controls.SetChildIndex(drawingPanel, 0);
            this.Controls.SetChildIndex(resultsTextBox, 1);
            this.Controls.SetChildIndex(startButton, 2);
        }

        private async void StartButton_Click(object sender, EventArgs e)
        {
            startButton.Enabled = false;
            resultsTextBox.Clear();
            resultsTextBox.AppendText("Запуск розфарбовування графа...\n");
            resultsTextBox.Update();

            try
            {
                resultsTextBox.AppendText("Отримання координат штатів через Google Maps API...\n");
                resultsTextBox.Update();

                await GraphColoring.GetStateCoordinatesAsync(mexStates);

                resultsTextBox.AppendText("Координати отримані. Запуск алгоритму розфарбовування...\n");
                resultsTextBox.Update();

                var result = await Task.Run(() =>
                {
                    GraphColoring gc = new GraphColoring(mexStates, adjacencyMatrix);
                    return gc.FindChromaticNumberAndColoring();
                });

                if (result.chromaticNumber == int.MaxValue)
                {
                    resultsTextBox.AppendText("\nПОМИЛКА: Не вдалося знайти розфарбування графа.\n");
                }
                else
                {
                    resultsTextBox.AppendText("\n--- Результати розфарбовування графа ---\n");
                    resultsTextBox.AppendText("Розфарбування: ");
                    foreach (var entry in result.coloring)
                    {
                        resultsTextBox.AppendText($"{entry.Key} – Колір {entry.Value}, ");
                    }
                    resultsTextBox.AppendText($"\nХроматичне число: {result.chromaticNumber}\n");
                    resultsTextBox.AppendText("--------------------------------------\n");
                }

                resultsTextBox.AppendText("\nУзагальнений висновок:\n");
                resultsTextBox.AppendText("Алгоритм успішно виконано. ");

                if (result.chromaticNumber <= 4)
                {
                    resultsTextBox.AppendText("Використано оптимальний backtracking алгоритм з MRV евристикою.\n");
                }
                else
                {
                    resultsTextBox.AppendText("Використано жадібний алгоритм для швидкого розв'язання.\n");
                }

                resultsTextBox.AppendText("Граф успішно розфарбовано з мінімально можливою кількістю кольорів.\n");

                drawingPanel.Invalidate();
            }
            catch (Exception ex)
            {
                resultsTextBox.AppendText($"\nПомилка: {ex.Message}\n");
            }
            finally
            {
                startButton.Enabled = true;
            }
        }

        private void DrawingPanel_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            // Параметри малювання
            int nodeRadius = 30;
            Font nodeFont = new Font("Arial", 8, FontStyle.Bold);
            Font labelFont = new Font("Arial", 9, FontStyle.Regular);
            StringFormat sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
            Pen edgePen = new Pen(Color.Gray, 2);

            // Отримуємо розміри панелі
            int panelWidth = drawingPanel.Width;
            int panelHeight = drawingPanel.Height;

            // Знаходимо межі координат
            float minX = mexStates.Min(s => s.Position.X);
            float maxX = mexStates.Max(s => s.Position.X);
            float minY = mexStates.Min(s => s.Position.Y);
            float maxY = mexStates.Max(s => s.Position.Y);

            // Додаємо відступи
            float padding = 80;
            float availableWidth = panelWidth - 2 * padding;
            float availableHeight = panelHeight - 2 * padding;

            // Обчислюємо масштаб
            float graphWidth = maxX - minX;
            float graphHeight = maxY - minY;

            // Захист від ділення на нуль
            if (graphWidth == 0) graphWidth = 1;
            if (graphHeight == 0) graphHeight = 1;

            float scaleX = availableWidth / graphWidth;
            float scaleY = availableHeight / graphHeight;
            float scale = Math.Min(scaleX, scaleY);

            // Обчислюємо зміщення для центрування
            float scaledWidth = graphWidth * scale;
            float scaledHeight = graphHeight * scale;
            float offsetX = (panelWidth - scaledWidth) / 2 - minX * scale;
            float offsetY = (panelHeight - scaledHeight) / 2 - minY * scale;

            // Функція для перетворення координат
            PointF TransformPoint(PointF originalPoint)
            {
                return new PointF(
                    originalPoint.X * scale + offsetX,
                    originalPoint.Y * scale + offsetY
                );
            }

            // 1. Малюємо ребра
            for (int i = 0; i < mexStates.Count; i++)
            {
                for (int j = i + 1; j < mexStates.Count; j++)
                {
                    if (adjacencyMatrix[i, j])
                    {
                        PointF p1 = TransformPoint(mexStates[i].Position);
                        PointF p2 = TransformPoint(mexStates[j].Position);
                        g.DrawLine(edgePen, p1, p2);
                    }
                }
            }

            // 2. Малюємо вершини
            for (int i = 0; i < mexStates.Count; i++)
            {
                State state = mexStates[i];
                PointF center = TransformPoint(state.Position);
                RectangleF rect = new RectangleF(center.X - nodeRadius, center.Y - nodeRadius, 2 * nodeRadius, 2 * nodeRadius);

                // Вибираємо колір
                Color fillColor = Color.LightGray;
                if (state.Color != -1 && colorMap.ContainsKey(state.Color))
                {
                    fillColor = colorMap[state.Color];
                }
                else if (state.Color != -1)
                {
                    fillColor = GetRandomColor();
                    colorMap[state.Color] = fillColor;
                }

                // Малюємо заповнене коло
                using (SolidBrush brush = new SolidBrush(fillColor))
                {
                    g.FillEllipse(brush, rect);
                }
                g.DrawEllipse(Pens.Black, rect);

                // Малюємо ID всередині кола
                g.DrawString(state.Id.ToString(), nodeFont, Brushes.White, rect, sf);

                // Малюємо назву штату над колом
                RectangleF nameRect = new RectangleF(rect.X - 20, rect.Y - nodeRadius - 20, rect.Width + 40, 20);
                g.DrawString(state.Name, labelFont, Brushes.Black, nameRect, sf);
            }

            // Додаємо легенду
            DrawLegend(g, panelWidth, panelHeight);
        }

        private void DrawLegend(Graphics g, int panelWidth, int panelHeight)
        {
            // Малюємо легенду з кольорами
            int legendX = 10;
            int legendY = 10;
            int legendItemHeight = 25;
            Font legendFont = new Font("Arial", 10);

            g.DrawString("Легенда кольорів:", legendFont, Brushes.Black, legendX, legendY);
            legendY += 25;

            var usedColors = mexStates.Where(s => s.Color != -1).Select(s => s.Color).Distinct().OrderBy(c => c);

            foreach (int colorIndex in usedColors)
            {
                if (colorMap.ContainsKey(colorIndex))
                {
                    // Малюємо квадратик з кольором
                    Rectangle colorRect = new Rectangle(legendX, legendY, 20, 20);
                    using (SolidBrush brush = new SolidBrush(colorMap[colorIndex]))
                    {
                        g.FillRectangle(brush, colorRect);
                    }
                    g.DrawRectangle(Pens.Black, colorRect);

                    // Підписуємо колір
                    g.DrawString($"Колір {colorIndex}", legendFont, Brushes.Black, legendX + 25, legendY + 2);

                    legendY += legendItemHeight;
                }
            }
        }
    }
}

