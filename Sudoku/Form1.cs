using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Diagnostics;
namespace Sudoku
{

    public partial class Form1 : Form
    {
        #region tanımlamalar
        static string dosya_yolu3 = @"C:\Users\ulassatr\Desktop\Sudoku2_6\Sudoku2\Sudoku\bin\Debug\metinbelgesi3.txt";
        static string dosya_yolu2 = @"C:\Users\ulassatr\Desktop\Sudoku2_6\Sudoku2\Sudoku\bin\Debug\metinbelgesi2.txt";
        static string dosya_yolu = @"C:\Users\ulassatr\Desktop\Sudoku2_6\Sudoku2\Sudoku\bin\Debug\metinbelgesi.txt";
        //İşlem yapacağımız dosyanın yolunu belirtiyoruz.
        static FileStream fs3 = new FileStream(dosya_yolu3, FileMode.OpenOrCreate, FileAccess.Write);
        static FileStream fs2 = new FileStream(dosya_yolu2, FileMode.OpenOrCreate, FileAccess.Write);
        static FileStream fs = new FileStream(dosya_yolu, FileMode.OpenOrCreate, FileAccess.Write);
        int hız = 1000;
        StreamReader sw1;
        StreamReader sw4;
        StreamReader sw5;
        //Bir file stream nesnesi oluşturuyoruz. 1.parametre dosya yolunu,
        //2.parametre dosya varsa açılacağını yoksa oluşturulacağını belirtir,
        //3.parametre dosyaya erişimin veri yazmak için olacağını gösterir.
        StreamWriter sw3 = new StreamWriter(fs3);
        StreamWriter sw2 = new StreamWriter(fs2);
        StreamWriter sw = new StreamWriter(fs);
        char[,] data = new char[9, 9];
        char[,] data2 = new char[9, 9];
        char[,] data3 = new char[9, 9];
        char[] ihtimaller = new char[9] { '1', '2', '3', '4', '5', '6', '7', '8', '9' };
        List<TextBox> myTextBoxes = new List<TextBox>();
        int adım3 = 0;
        int adım = 0;
        int adım2 = 0;
        Stopwatch stopWatch3 = new Stopwatch();
        Stopwatch stopWatch2 = new Stopwatch();
        Stopwatch stopWatch = new Stopwatch();
        private Thread th1;
        private Thread th2;
        private Thread th3;
        ArrayList elemanlarim = new ArrayList();
        ArrayList elemanlarim2 = new ArrayList();
        ArrayList elemanlarim3 = new ArrayList();
        #endregion

        public Form1()
        {
            CheckForIllegalCrossThreadCalls = false;
            InitializeComponent();
        }

        public void StartMultipleThread()
        {
            Thread th1 = new Thread(new ThreadStart(run));
            Thread th2 = new Thread(new ThreadStart(run2));
            Thread th3 = new Thread(new ThreadStart(run3));
            th1.Start();
            th2.Start();
            th3.Start();

            this.th1 = th1;
            this.th2 = th2;
            this.th3 = th3;

        }

        #region Solution1
        public void File_Reader()
        {
            List<TextBox> myTextBoxes = new List<TextBox>();
            for (int i = 1; i < 82; i++)
            {
                myTextBoxes.Add((TextBox)Controls.Find("textBox" + i, true)[0]);
            }
            this.myTextBoxes = myTextBoxes;
            StreamReader reader = new StreamReader("sudoku.txt");
            string contents = reader.ReadToEnd();
            char[,] data = new char[9, 9];

            var lines = contents.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
            int row = 0;
            foreach (string line in lines)
            {
                int column = 0;
                foreach (char character in line)
                {
                    data[row, column] = character;
                    column++;
                }
                row++;
            }
            reader.Close();
            for (int j = -1; j < 80; j++)
            {
                for (int a = 0; a < 9; a++)
                {
                    for (int b = 0; b < 9; b++)
                    {
                        j++;
                        myTextBoxes[j].Text = data[a, b].ToString();
                        if (data[a, b] == '*')
                        {
                            myTextBoxes[j].Text = " ";
                        }
                    }

                }
            }
            this.data = data;
        }

        bool check(char[,] data, int row, int col)
        {
            for (int i = 0; i < 9; ++i)
            {
                int p = 3 * (row / 3) + i % 3, q = 3 * (col / 3) + i / 3;
                if (i != row && data[i, col] == data[row, col] // col
                  || i != col && data[row, i] == data[row, col] // row
                  || (p != row || q != col) && data[p, q] == data[row, col])
                { // box
                    return false;
                }
            }
            return true;
        }

        bool solve(char[,] data, int row, int col)
        {
            stopWatch.Start();
            bool complete = false;
            int i, j = col;

            for (i = row; i < 9; ++i)
            {
                for (; j < 9; ++j)
                {
                    if (data[i, j] == '*')
                    {
                        int nextcol = (j + 1) % 9, nextrow = i;
                        if (nextcol == '*')
                        {
                            nextrow = i + 1;
                            complete = nextrow == 9;
                        }
                        for (int v = 0; v < 9; ++v)
                        {
                            data[i, j] = ihtimaller[v];
                            adım++;
                            Data_Writer(data);
                            // backtracking prune
                            if (check(data, i, j))
                            {
                                if (complete || solve(data, nextrow, nextcol))
                                {
                                    label2.Text = "Adım Sayısı: " + adım.ToString();
                                    stopWatch.Stop();
                                    return true;
                                }
                            }
                        }

                        data[i, j] = '*';
                        return false;
                    }

                }
                j = 0;
            }


            return true;
        }

        public void run()
        {
            if (solve(data, 0, 8))
            {
                for (int j = -1; j < 80; j++)
                {
                    for (int a = 0; a < 9; a++)
                    {
                        for (int b = 0; b < 9; b++)
                        {
                            j++;

                            myTextBoxes[j].Text = data[a, b].ToString();
                            if (data[a, b] == '*')
                            {
                                myTextBoxes[j].Text = " ";
                            }
                        }

                    }
                }
            }
            else
            { MessageBox.Show("Çözümü olmayan bir sudoku denediniz.!", "Bilgilendirme Penceresi"); }
            File_Closing();
            Data_Reader();
            animation(elemanlarim);
            if (th2.IsAlive || th3.IsAlive)
            {
                th2.Abort();
                th3.Abort();
                TimeSpan ts = stopWatch.Elapsed;
                string elapsedTime = String.Format("{0:00}.{1:00}", ts.Seconds,
    ts.Milliseconds);
                label9.Text = "Süre: " + elapsedTime;
                label3.Text = "Adım Sayısı: " + adım2.ToString();
                label4.Text = "Adım Sayısı: " + adım3.ToString();
            }

        }

        public void Data_Reader()
        {
            FileStream fs1 = new FileStream(dosya_yolu, FileMode.Open, FileAccess.Read);
            StreamReader sw1 = new StreamReader(fs1);
            int i = 1;
            string yazi = sw1.ReadLine();
            string[] dizi = new string[9];
            while (yazi != null)
            {
                if (yazi == "")
                {
                    yazi = sw1.ReadLine();
                    if (yazi == "")
                    {
                        break;
                    }
                    continue;
                }

                if (i % 9 == 0)
                {
                    dizi[i - 1] = yazi;
                    for (int j = 0; j < 9; j++)
                    {
                        elemanlarim.Add(dizi[j]);
                    }
                    Array.Clear(dizi, 0, dizi.Length);

                    i = 0;

                }
                else if (i % 9 != 0)
                {
                    dizi[i - 1] = yazi;
                }
                yazi = sw1.ReadLine();
                i++;
            }

            this.sw1 = sw1;
        }

        public void Data_Writer(char[,] data)
        {
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    sw.Write(" ");
                    sw.Write(data[i, j]);

                }
                sw.WriteLine();
            }
            sw.WriteLine();
        }

        public void File_Closing()
        {
            //Dosyaya ekleyeceğimiz iki satırlık yazıyı WriteLine() metodu ile yazacağız.
            sw.Flush();
            //Veriyi tampon bölgeden dosyaya aktardık.

            sw.Close();
            fs.Close();
        }

        public void animation(ArrayList elemanlarim)
        {
            int a = 0;
            foreach (var index in elemanlarim)
            {
                if (a % 9 == 0)
                {
                    label28.Text = elemanlarim[a].ToString();
                }
                if (a % 9 == 1)
                {
                    label29.Text = elemanlarim[a].ToString();
                }
                if (a % 9 == 2)
                {
                    label30.Text = elemanlarim[a].ToString();
                }
                if (a % 9 == 3)
                {
                    label31.Text = elemanlarim[a].ToString();
                }
                if (a % 9 == 4)
                {
                    label32.Text = elemanlarim[a].ToString();
                }
                if (a % 9 == 5)
                {
                    label33.Text = elemanlarim[a].ToString();
                }
                if (a % 9 == 6)
                {
                    label34.Text = elemanlarim[a].ToString();
                }
                if (a % 9 == 7)
                {
                    label35.Text = elemanlarim[a].ToString();
                }
                if (a % 9 == 8)
                {
                    label36.Text = elemanlarim[a].ToString();
                }
                for (int i = 0; i < hız; i++) ;
                a++;
            }
        }

        #endregion

        #region Solution2
        public void File_Reader2()
        {
            List<TextBox> myTextBoxes = new List<TextBox>();
            for (int i = 1; i < 82; i++)
            {
                myTextBoxes.Add((TextBox)Controls.Find("textBox" + i, true)[0]);
            }
            this.myTextBoxes = myTextBoxes;
            StreamReader reader = new StreamReader("sudoku.txt");
            string contents = reader.ReadToEnd();
            char[,] data2 = new char[9, 9];

            var lines = contents.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
            int row = 0;
            foreach (string line in lines)
            {
                int column = 0;
                foreach (char character in line)
                {
                    data2[row, column] = character;
                    column++;
                }
                row++;
            }
            reader.Close();
            for (int j = -1; j < 80; j++)
            {
                for (int a = 0; a < 9; a++)
                {
                    for (int b = 0; b < 9; b++)
                    {
                        j++;
                        myTextBoxes[j].Text = data2[a, b].ToString();
                        if (data2[a, b] == '*')
                        {
                            myTextBoxes[j].Text = " ";
                        }
                    }

                }
            }
            this.data2 = data2;
        }

        public void solveSudoku(char[,] data2)
        {
            stopWatch2.Start();
            solve2(data2, 0);
        }

        bool solve2(char[,] data2, int ind)
        {
            if (ind == 81)
            {
                stopWatch2.Stop();
                return true;
                // solved
            }

            int row = ind / 9;
            int col = ind % 9;

            // Advance forward on cells that are prefilled
            if (data2[row, col] != '*') return solve2(data2, ind + 1);

            else
            {
                // we are positioned on something we need to fill in.
                // Try all possibilities

                for (int i = 0; i < 9; i++)
                {
                    if (check2(data2, row, col, ihtimaller[i]))
                    {
                        data2[row, col] = ihtimaller[i];
                        Data_Writer2(data2);
                        if (solve2(data2, ind + 1))
                        {

                            return true;
                        }
                        data2[row, col] = '*';
                        if (th2.IsAlive)
                            adım2++;
                        // unmake move
                    }
                }
            }

            // no solution
            return false;
        }

        public void Data_Writer2(char[,] data)
        {
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    sw2.Write(" ");
                    sw2.Write(data[i, j]);

                }
                sw2.WriteLine();
            }
            sw2.WriteLine();
        }

        public bool check2(char[,] data2, int row, int col, int c)
        {
            // check columns/rows
            for (int i = 8; i >= 0; i--)
            {

                if (data2[i, col] == c) return false;
            }
            for (int i = 0; i < 9; i++)
            {
                if (data2[row, i] == c) return false;

            }

            int rowStart = row - row % 3;
            int colStart = col - col % 3;

            for (int m = 2; m >= 0; m--)
            {
                for (int k = 0; k < 3; k++)
                {
                    if (data2[rowStart + k, colStart + m] == c) return false;
                }
            }

            return true;
        }

        public void run2()
        {
            solveSudoku(data2);
            label3.Text = "Adım Sayısı: " + adım2.ToString();
            File_Closing2();
            Data_Reader2();
            animation2(elemanlarim2);
            if (th1.IsAlive || th3.IsAlive)
            {

                th1.Abort();
                th3.Abort();
                TimeSpan ts2 = stopWatch2.Elapsed;
                string elapsedTime2 = String.Format("{0:00}.{1:00}", ts2.Seconds,
ts2.Milliseconds);
                label1.Text = "Süre: " + elapsedTime2;
                label2.Text = "Adım Sayısı: " + adım.ToString();
                label4.Text = "Adım Sayısı: " + adım3.ToString();
            }


            for (int j = -1; j < 80; j++)
            {
                for (int a = 0; a < 9; a++)
                {
                    for (int b = 0; b < 9; b++)
                    {
                        j++;
                        myTextBoxes[j].Text = data2[a, b].ToString();
                        if (data2[a, b] == '*')
                        {
                            myTextBoxes[j].Text = " ";
                        }
                    }

                }
            }


        }

        public void File_Closing2()
        {
            //Dosyaya ekleyeceğimiz iki satırlık yazıyı WriteLine() metodu ile yazacağız.
            //  sw2.Flush();
            //Veriyi tampon bölgeden dosyaya aktardık.

            sw2.Close();
            fs2.Close();
        }

        public void Data_Reader2()
        {
            FileStream fs4 = new FileStream(dosya_yolu2, FileMode.Open, FileAccess.Read);
            StreamReader sw4 = new StreamReader(fs4);
            int i = 1;
            string yazi = sw4.ReadLine();
            string[] dizi = new string[9];
            while (yazi != null)
            {
                if (yazi == "")
                {
                    yazi = sw4.ReadLine();
                    if (yazi == "")
                    {
                        break;
                    }
                    continue;
                }

                if (i % 9 == 0)
                {
                    dizi[i - 1] = yazi;
                    for (int j = 0; j < 9; j++)
                    {
                        elemanlarim2.Add(dizi[j]);
                    }
                    Array.Clear(dizi, 0, dizi.Length);

                    i = 0;

                }
                else if (i % 9 != 0)
                {
                    dizi[i - 1] = yazi;
                }
                yazi = sw4.ReadLine();
                i++;


            }
            this.sw4 = sw4;
        }

        public void animation2(ArrayList elemanlarim2)
        {
            int a = 0;
            foreach (var index in elemanlarim2)
            {
                if (a % 9 == 0)
                {
                    label19.Text = elemanlarim2[a].ToString();
                }
                if (a % 9 == 1)
                {
                    label20.Text = elemanlarim2[a].ToString();
                }
                if (a % 9 == 2)
                {
                    label21.Text = elemanlarim2[a].ToString();
                }
                if (a % 9 == 3)
                {
                    label22.Text = elemanlarim2[a].ToString();
                }
                if (a % 9 == 4)
                {
                    label23.Text = elemanlarim2[a].ToString();
                }
                if (a % 9 == 5)
                {
                    label24.Text = elemanlarim2[a].ToString();
                }
                if (a % 9 == 6)
                {
                    label25.Text = elemanlarim2[a].ToString();
                }
                if (a % 9 == 7)
                {
                    label26.Text = elemanlarim2[a].ToString();
                }
                if (a % 9 == 8)
                {
                    label27.Text = elemanlarim2[a].ToString();
                }
                for (int i = 0; i < hız; i++) ;

                a++;
            }
        }

        #endregion

        #region Solution3
        public void File_Reader3()
        {
            List<TextBox> myTextBoxes = new List<TextBox>();
            for (int i = 1; i < 82; i++)
            {
                myTextBoxes.Add((TextBox)Controls.Find("textBox" + i, true)[0]);
            }
            this.myTextBoxes = myTextBoxes;
            StreamReader reader = new StreamReader("sudoku.txt");
            string contents = reader.ReadToEnd();
            char[,] data3 = new char[9, 9];

            var lines = contents.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
            int row = 0;
            foreach (string line in lines)
            {
                int column = 0;
                foreach (char character in line)
                {
                    data3[row, column] = character;
                    column++;
                }
                row++;
            }
            reader.Close();
            for (int j = -1; j < 80; j++)
            {
                for (int a = 0; a < 9; a++)
                {
                    for (int b = 0; b < 9; b++)
                    {
                        j++;
                        myTextBoxes[j].Text = data3[a, b].ToString();
                        if (data3[a, b] == '*')
                        {
                            myTextBoxes[j].Text = " ";
                        }
                    }

                }
            }
            this.data3 = data3;
        }

        public void solveSudoku3(char[,] data3)
        {
            stopWatch3.Start();
            solve3(data3, 0);
        }

        bool solve3(char[,] data3, int ind)
        {
            if (ind == 81)
            {
                stopWatch3.Stop();
                return true;
                // solved
            }
            int row = ind / 9;
            int col = ind % 9;

            // Advance forward on cells that are prefilled
            if (data3[row, col] != '*') return solve3(data3, ind + 1);

            else
            {
                // we are positioned on something we need to fill in.
                // Try all possibilities

                for (int i = 8; i >= 0; i--)
                {
                    if (check3(data3, row, col, ihtimaller[i]))
                    {
                        data3[row, col] = ihtimaller[i];
                        Data_Writer3(data3);
                        if (solve3(data3, ind + 1))
                        {

                            return true;
                        }
                        data3[row, col] = '*';
                        adım3++;
                        // unmake move
                    }
                }
            }
            // no solution
            return false;
        }

        public bool check3(char[,] data3, int row, int col, int c)
        {
            // check columns/rows
            for (int i = 8; i >= 0; i--)
            {
                if (data3[row, i] == c) return false;
                if (data3[i, col] == c) return false;

            }

            int rowStart = row - row % 3;
            int colStart = col - col % 3;

            for (int m = 2; m >= 0; m--)
            {
                for (int k = 2; k >= 0; k--)
                {
                    if (data3[rowStart + k, colStart + m] == c) return false;

                }
            }

            return true;
        }

        public void run3()
        {
            solveSudoku3(data3);
            label4.Text = "Adım Sayısı: " + adım3.ToString();
            File_Closing3();
            Data_Reader3();
            animation3(elemanlarim3);
            if (th1.IsAlive || th2.IsAlive)
            {
                th1.Abort();
                th2.Abort();
                TimeSpan ts3 = stopWatch3.Elapsed;
                string elapsedTime3 = String.Format("{0:00}.{1:00}", ts3.Seconds,
ts3.Milliseconds);
                label8.Text = "Süre: " + elapsedTime3;
                label2.Text = "Adım Sayısı: " + adım.ToString();
                label3.Text = "Adım Sayısı: " + adım2.ToString();
            }

            for (int j = -1; j < 80; j++)
            {
                for (int a = 0; a < 9; a++)
                {
                    for (int b = 0; b < 9; b++)
                    {
                        j++;
                        myTextBoxes[j].Text = data3[a, b].ToString();
                        if (data3[a, b] == '*')
                        {
                            myTextBoxes[j].Text = " ";
                        }
                    }

                }
            }


        }
        //Metin belgelerine yazan kod
        public void Data_Writer3(char[,] data)
        {
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    sw3.Write(" ");
                    sw3.Write(data[i, j]);

                }
                sw3.WriteLine();
            }
            sw3.WriteLine();
        }

        public void File_Closing3()
        {
            //Dosyaya ekleyeceğimiz iki satırlık yazıyı WriteLine() metodu ile yazacağız.
            // sw3.Flush();
            //Veriyi tampon bölgeden dosyaya aktardık.

            sw3.Close();
            fs3.Close();
        }

        public void Data_Reader3()
        {
            FileStream fs5 = new FileStream(dosya_yolu3, FileMode.Open, FileAccess.Read);
            StreamReader sw5 = new StreamReader(fs5);

            int i = 1;
            string yazi = sw5.ReadLine();
            string[] dizi = new string[9];
            while (yazi != null)
            {
                if (yazi == "")
                {
                    yazi = sw5.ReadLine();
                    if (yazi == "")
                    {
                        break;
                    }
                    continue;
                }

                if (i % 9 == 0)
                {
                    dizi[i - 1] = yazi;
                    for (int j = 0; j < 9; j++)
                    {
                        elemanlarim3.Add(dizi[j]);
                    }
                    Array.Clear(dizi, 0, dizi.Length);

                    i = 0;

                }
                else if (i % 9 != 0)
                {
                    dizi[i - 1] = yazi;
                }
                yazi = sw5.ReadLine();
                i++;
            }
            this.sw5 = sw5;
        }

        public void animation3(ArrayList elemanlarim3)
        {
            int a = 0;
            foreach (var index in elemanlarim3)
            {


                if (a % 9 == 0)
                {
                    label10.Text = elemanlarim3[a].ToString();
                }
                if (a % 9 == 1)
                {
                    label11.Text = elemanlarim3[a].ToString();
                }
                if (a % 9 == 2)
                {
                    label12.Text = elemanlarim3[a].ToString();
                }
                if (a % 9 == 3)
                {
                    label13.Text = elemanlarim3[a].ToString();
                }
                if (a % 9 == 4)
                {
                    label14.Text = elemanlarim3[a].ToString();
                }
                if (a % 9 == 5)
                {
                    label15.Text = elemanlarim3[a].ToString();
                }
                if (a % 9 == 6)
                {
                    label16.Text = elemanlarim3[a].ToString();
                }
                if (a % 9 == 7)
                {
                    label17.Text = elemanlarim3[a].ToString();
                }
                if (a % 9 == 8)
                {
                    label18.Text = elemanlarim3[a].ToString();
                }
                for (int i = 0; i < hız; i++) ;
                a++;
            }

        }
        #endregion

        #region Butonlar
        private void button1_Click(object sender, EventArgs e)
        {
            File_Reader();
            File_Reader2();
            File_Reader3();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            StartMultipleThread();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Process.Start("notepad.exe", "C:\\Users\\ulassatr\\Desktop\\Sudoku2_6\\Sudoku2\\Sudoku\bin\\Debug\\metinbelgesi.txt");

        }

        private void button4_Click(object sender, EventArgs e)
        {
            Process.Start("notepad.exe", "C:\\Users\\ulassatr\\Desktop\\Sudoku2_6\\Sudoku2\\Sudoku\bin\\Debug\\metinbelgesi2.txt");
        }

        private void button9_Click(object sender, EventArgs e)
        {
            Process.Start("notepad.exe", "C:\\Users\\ulassatr\\Desktop\\Sudoku2_6\\Sudoku2\\Sudoku\bin\\Debug\\metinbelgesi3.txt");
        }

        private void anime_Click(object sender, EventArgs e)
        {
            hız = hız * 10;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            hız = hız / 10;
        }

        private void button6_Click(object sender, EventArgs e)
        {
            hız = 1000000000;
        }
        #endregion
    }
}
