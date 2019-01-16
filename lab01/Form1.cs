using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Globalization;


namespace lab01
{
    // Точка с вещественными координатами
    public class CPoint{
        public double x;
        public double y;

        public CPoint(double a_x, double a_y)
        {
            x = a_x;
            y = a_y;
        }

        public static bool operator !=(CPoint a_p1, CPoint a_p2)
        {
            bool val = false;
            if (a_p1.x != a_p2.x || a_p1.y != a_p2.y)
            {
                val = true;
            }
            return val;
        }

        public static bool operator ==(CPoint a_p1, CPoint a_p2)
        {
            bool val = false;
            if (a_p1.x == a_p2.x && a_p1.y == a_p2.y)
            {
                val = true;
            }
            return val;
        }

    }

    public partial class Form1 : Form
    {
        // Список для хранения точек
        List<CPoint> Points;
        // Объект для рисования
        Graphics g;
        // Шаг для рисования в пикселях
        int step;

        public Form1()
        {
            InitializeComponent();
        }

        private void загрузитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Диалог открытия файла
            OpenFileDialog LoadDlg = new OpenFileDialog();
            // Установка начальных значений и фильтров
            LoadDlg.InitialDirectory = ".\\test";
            LoadDlg.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            LoadDlg.FilterIndex = 2;
            LoadDlg.RestoreDirectory = true;

            // Имя файла с точками
            string fname = null;
            if (LoadDlg.ShowDialog() == DialogResult.OK)
            {
                fname = LoadDlg.FileName;

                // Получение пути к файлу по имени
                string fpath = Path.GetFullPath(fname);
                // Поток для чтение из файла
                FileStream f = new FileStream(fpath, FileMode.Open, FileAccess.Read);

                // Поток для чтения из файла
                StreamReader rd = new StreamReader(f);
                // Массив координат одной точки
                string[] Coordinates = new string[2];

                // Количество строк в файле
                var lines = File.ReadAllLines(fpath);
                rd.Close();
                int n = lines.Length;
                Points = new List<CPoint>(n);           

                for (int i = 0; i < n; i++)
                {
                    Coordinates = lines[i].Split(',');
                    Points.Insert(i, new CPoint(Double.Parse(Coordinates[0], CultureInfo.InvariantCulture),
                                           Double.Parse(Coordinates[1], CultureInfo.InvariantCulture)));
                }

                // Рисование осей координат
                DrawAxis(Points);
                // Рисование точек
                InitDrawPoints(Points);

            } //endif
        }

        public void DrawAxis(List<CPoint> a_Points)
        {
            // Максимальные значения
            double max_X = Math.Abs(a_Points[0].x);
            double max_Y = Math.Abs(a_Points[0].y);

            // Нахождение максимумов по осям X и Y
            for (int i = 1; i < a_Points.Count; i++)
            {
                if (max_X < Math.Abs(a_Points[i].x))
                {
                    max_X = Math.Abs(a_Points[i].x);
                }

                if (max_Y < Math.Abs(a_Points[i].y))
                {
                    max_Y = Math.Abs(a_Points[i].y);
                }
            }

            if (g != null)
            {
                g.Clear(Form1.DefaultBackColor);
            }

            // Рисование: НАЧАЛО
            g = pictureBox1.CreateGraphics();
            // Ось X
            g.DrawLine(new Pen(Brushes.Black, 1), new Point(0, pictureBox1.Height / 2), new Point(pictureBox1.Width, pictureBox1.Height / 2));
            // Ось Y
            g.DrawLine(new Pen(Brushes.Black, 1), new Point(pictureBox1.Width/2, pictureBox1.Height), new Point(pictureBox1.Width / 2, 0));

            // Нанесение шкалы деления
            int mX = Convert.ToInt32(max_X + 0.5);
            int mY = Convert.ToInt32(max_Y + 0.5);

            int max = Math.Max(mX, mY);
            // размер шага в пикселях для рисования
            step = (pictureBox1.Width / 2) / (max + 1);

            // Координаты центра
            int originX = pictureBox1.Width / 2;
            int originY = pictureBox1.Height / 2;
            string str = null;

            // Параметры подписей
            Font font = new Font("Arial", 10);
            SolidBrush br = new SolidBrush(Color.Black);
            for (int i = 1; i < max + 1; i++)
            {
                g.DrawLine(new Pen(Brushes.Black, 1), new Point(originX + step * i, originY-2), new Point(originX + step * i, originY+2));
                if ((i % 2) == 0)
                {
                    str = Convert.ToString(i);
                    g.DrawString(str, font, br, new Point(originX + step * i - 4, originY + 3));
                }

                g.DrawLine(new Pen(Brushes.Black, 1), new Point(originX - step * i, originY-2), new Point(originX - step * i, originY+2));
                if ((i % 2) == 0)
                {
                    str = Convert.ToString(-i);
                    g.DrawString(str, font, br, new Point(originX - step * i - 5, originY + 3));
                }
            }
            for (int i = 1; i < max + 1; i++)
            {
                g.DrawLine(new Pen(Brushes.Black, 1), new Point(originX-2, originY - step * i), new Point(originX+2, originY - step * i));
                if ((i % 2) == 0)
                {
                    str = Convert.ToString(i);
                    g.DrawString(str, font, br, new Point(originX - 17, originY - step * i - 6));
                }

                g.DrawLine(new Pen(Brushes.Black, 1), new Point(originX-2, originY + step * i), new Point(originX+2, originY + step * i));
                if ((i % 2) == 0)
                {
                    str = Convert.ToString(-i);
                    g.DrawString(str, font, br, new Point(originX - 17, originY + step * i - 6));
                }
            }
        }

        public void InitDrawPoints(List<CPoint> a_Points)
        {
            // Координаты центра
            int originX = pictureBox1.Width / 2;
            int originY = pictureBox1.Height / 2;

            // Максимальные значения
            double max_X = Math.Abs(a_Points[0].x);
            double max_Y = Math.Abs(a_Points[0].y);
            // Нахождение максимумов по осям X и Y
            for (int i = 1; i < a_Points.Count; i++)
            {
                if (max_X < Math.Abs(a_Points[i].x))
                {
                    max_X = Math.Abs(a_Points[i].x);
                }

                if (max_Y < Math.Abs(a_Points[i].y))
                {
                    max_Y = Math.Abs(a_Points[i].y);
                }
            }
            // Определение шага
            int mX = Convert.ToInt32(max_X + 0.5);
            int mY = Convert.ToInt32(max_Y + 0.5);
            int max = Math.Max(mX, mY);
            // размер шага в пикселях для рисования
            //int step = (pictureBox1.Width / 2) / (max + 1);

            // Рисование: НАЧАЛО
            //Graphics g = pictureBox1.CreateGraphics();
            int x, y; //целые отрезки
            int rem_X, rem_Y; //части последнего отрезка
            for(int i = 0; i < a_Points.Count; i++)
            {
                x = Convert.ToInt32(Math.Truncate(a_Points[i].x));
                y = Convert.ToInt32(Math.Truncate(a_Points[i].y));
                rem_X = Convert.ToInt32((a_Points[i].x - x)*step);
                rem_Y = Convert.ToInt32((a_Points[i].y - y)*step);
                g.FillEllipse(new SolidBrush(Color.Black), (originX + x * step + rem_X)-4, (originY - y * step - rem_Y)-4, 8, 8);
            }
        }

        public void PaintPoints(List<CPoint> a_Points, SolidBrush a_br) {
            // Координаты центра
            int originX = pictureBox1.Width / 2;
            int originY = pictureBox1.Height / 2;

            // Рисование: НАЧАЛО
            //Graphics g = pictureBox1.CreateGraphics();
            int x, y; //целые отрезки
            int rem_X, rem_Y; //части последнего отрезка
            for (int i = 0; i < a_Points.Count; i++)
            {
                x = Convert.ToInt32(Math.Truncate(a_Points[i].x));
                y = Convert.ToInt32(Math.Truncate(a_Points[i].y));
                rem_X = Convert.ToInt32((a_Points[i].x - x) * step);
                rem_Y = Convert.ToInt32((a_Points[i].y - y) * step);
                g.FillEllipse(a_br, (originX + x * step + rem_X) - 4, (originY - y * step - rem_Y) - 4, 8, 8);
            }
        }

        private void запускАлгоритмаToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Коэффициент для проверки условия
            double coeff = 0.5;

            // Список центров
            List<CPoint> cores = new List<CPoint>();

            // Выбор центра первого кластера (первый шаг алгоритма)
            Random rnd = new Random();
            // Индекс первого кластера в списке
            int num = rnd.Next(0, Points.Count);

            // Координаты центра первого кластера
            CPoint z1 = new CPoint(Points[num].x, Points[num].y);
            cores.Add(z1);

            //Расстояние между точками и центром первого кластера
            double dst = 0;
            double dst_1;

            // Нахождение центра второго кластера - второй шаг алгоритма
            CPoint z2 = new CPoint(0,0);
            for (int i=0; i < Points.Count; i++)
            {
                if (Points.IndexOf(z1) == -1)
                {
                    dst_1 = Math.Sqrt(Math.Pow((z1.x - Points[i].x),2) + Math.Pow((z1.y - Points[i].y), 2));
                    if (dst_1 > dst)
                    {
                        dst = dst_1;
                        z2.x = Points[i].x;
                        z2.y = Points[i].y;
                    }
                }
            }
            // Добавление ценра второго кластера в список
            cores.Add(z2);

            // Переменная для накопления величин d на каждой итерации, сначала равна расстоянию между двумя центрами z1 и z2
            double d_sum = Math.Sqrt(Math.Pow((z1.x - z2.x), 2) + Math.Pow((z1.y - z2.y), 2));
            // Текущее максимальное расстояние (рассчитывается как среднее), сначала равно расстоянию между двумя центрами z1 и z2
            double d_average = Math.Sqrt(Math.Pow((z1.x - z2.x), 2) + Math.Pow((z1.y - z2.y), 2));
            // Список кластеров
            List<List<CPoint>> Clusters = new List<List<CPoint>>();

            // Циклическая часть алгоритма
            for (int t = 2; t < Points.Count + 1; t++)
            {
                // Инициализация списка кластеров
                Clusters = new List<List<CPoint>>(cores.Count);
                // Заполнение списка кластеров пустыми списками точек
                for (int k = 0; k < cores.Count; k++)
                {
                    Clusters.Add(new List<CPoint>());
                }

                // Массив минимальных расстояний до центров
                double[] destinations = new double[Points.Count - cores.Count];
                // Доп. параметр для заполнения destinations
                int dst_index = 0;

                // Список потенциально новых центров
                List<CPoint> X = new List<CPoint>(Points.Count - cores.Count);

                if (Points.Count != cores.Count)
                {
                    for (int i = 0; i < Points.Count; i++)
                    {
                        dst = 0;
                        double[] dst_min = new double[cores.Count];
                        // Индекс кластера
                        int ind_cl = 0;

                        if (cores.FindIndex(x => x == Points[i]) == -1)
                        {
                            // Перебор всех текущих центров и вычисление ближайшего центра
                            for (int j = 0; j < cores.Count; j++)
                            {
                                // Вычисление минимального расстояния до центра
                                dst = Math.Sqrt(Math.Pow((cores[j].x - Points[i].x), 2) + Math.Pow((cores[j].y - Points[i].y), 2));
                                dst_min[j] = dst;

                                if (Clusters[j].Count == 0)
                                {
                                    // Добавление центра в текущий список кластеров
                                    Clusters[j].Add(new CPoint(cores[j].x, cores[j].y));
                                    //Clusters[j].Insert(0, new CPoint(cores[j].x, cores[j].y));
                                }

                            } // Следующий центр

                            dst = dst_min.Min();
                            ind_cl = Array.IndexOf(dst_min, dst);

                            // Добавление точки в кластер с номером ind_cl
                            Clusters[ind_cl].Add(new CPoint(Points[i].x, Points[i].y));

                            destinations[dst_index] = dst;
                            X.Insert(dst_index, Points[i]);
                            dst_index++;

                            // Добавление центра в кластер с номером ind_cl
                            //Clusters[ind_cl].Insert(0, new CPoint(cores[ind_cl].x, cores[ind_cl].y));
                        }
                        else { continue; }
                    } // Следущая точка

                    double d = 0;
                    if (destinations.Length != 0)
                    {
                        d = destinations.Max();
                    }
                    int ind_max = Array.IndexOf(destinations, d);

                    if (d >= d_average * coeff)
                    {
                        // Новый центр
                        cores.Add(X[ind_max]);
                        // вычисление текущего d среднего
                        d_sum += d;
                        d_average = (double)d_sum / t;
                    }
                    else
                    {
                        break;
                    }

                } else  if (Points.Count == cores.Count) {
                    for (int j = 0; j < cores.Count; j++)
                    {
                        Clusters[j].Add(new CPoint(cores[j].x, cores[j].y));
                    }
                } // Проверка на совпадения начального количества точек и ядер 

            } // Следующая итерация алгоритма

            // Создание массива кистей разных цветов, длина массива равна итоговому количеству кластеров
            /*SolidBrush[] brushes = new SolidBrush[Clusters.Count];
            Color colour;
            int col;
            Random rnd_col = new Random();
            for (int i = 0; i < brushes.Length; i++)
            {
                col = rnd_col.Next(256);
                colour = Color.FromArgb(255, 0, col, col);
                brushes[i] = new SolidBrush(colour);
            }*/

            Color[] colours = new Color[15]{ Color.Blue, Color.Yellow, Color.Red, Color.Pink, Color.Brown,
                                             Color.Green, Color.Gray, Color.Aqua, Color.DarkBlue, Color.DarkGreen,
                                             Color.Orange, Color.DarkSlateGray, Color.BurlyWood, Color.Coral, Color.Cornsilk};

            // Раскрашивание кластеров
            for (int j = 0; j < Clusters.Count; j++)
            {
                PaintPoints(Clusters[j], new SolidBrush(colours[j]));
            }
        }
    }
}
