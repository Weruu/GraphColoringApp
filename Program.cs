using GraphColoringApp; // Переконайтеся, що це ваш простір імен
using System;
using System.Windows.Forms;

namespace GraphColoringApp // Ваш простір імен
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1()); // Цей рядок запускає вікно
        }
    }
}