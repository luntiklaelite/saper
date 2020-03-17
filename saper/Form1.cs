using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace saper
{
    public partial class Form1 : Form
    {
        Random rand;
        int columns, rows, bombs, time, openedKl, flags;
        kletka[,] kletki;
        int widthcol, heightrow;

        private void timer1_Tick(object sender, EventArgs e)
        {
            time++;
            label_time.Text = "Прошло времени:\n" + time + " с";
        }

        private void новаяИграToolStripMenuItem_Click(object sender, EventArgs e)
        {
            newGame();
        }

        private void gameView_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (!timer1.Enabled)
                timer1.Start();
           
            if (e.Button == MouseButtons.Left)
            {
                if (kletki[e.ColumnIndex, e.RowIndex].isOpen)
                    return;
                if (kletki[e.ColumnIndex, e.RowIndex].isBomb)
                {
                    gameView.SelectedCells[0].Style.BackColor = Color.Red;
                    gameView.SelectedCells[0].Style.SelectionBackColor = gameView.SelectedCells[0].Style.BackColor;
                    loseGame();
                    return;
                }
                openNeibNear(e.ColumnIndex, e.RowIndex);
                gameView.SelectedCells[0].Style.SelectionBackColor = gameView.SelectedCells[0].Style.BackColor;
            }
            else if(e.Button == MouseButtons.Right)
            {
                if (!kletki[e.ColumnIndex, e.RowIndex].flag)
                {
                    flags++;
                    kletki[e.ColumnIndex, e.RowIndex].flag = true;
                    gameView.Rows[e.RowIndex].Cells[e.ColumnIndex].Style.BackColor = Color.Green;
                    label_bomb.Text = "Бомб: " + (bombs - flags);
                }
                else
                {
                    flags--;
                    kletki[e.ColumnIndex, e.RowIndex].flag = false;
                    gameView.Rows[e.RowIndex].Cells[e.ColumnIndex].Style.BackColor = SystemColors.InactiveCaption;
                    label_bomb.Text = "Бомб: " + (bombs - flags);
                }
            }
        }

        public Form1()
        {
            InitializeComponent();
            rand = new Random();
            newGame();
        }

        public void openAllMap()
        {
            for (int i = 0; i < columns; i++)
            {
                for (int k = 0; k < rows; k++)
                {
                    if (kletki[i, k].isBomb)
                        gameView.Rows[k].Cells[i].Value = "+";
                }
            }
        }

        public void openNeibNear(int col, int row)
        {
            if (col < 0 || row < 0 || col >= columns || row >= rows || kletki[col, row].isOpen)
                return;
            gameView.Rows[row].Cells[col].Style.BackColor = Color.AliceBlue;
            if(kletki[col, row].flag)
            {
                flags--;
                kletki[col, row].flag = false;
                label_bomb.Text = "Бомб: " + (bombs - flags);
            }
            kletki[col, row].isOpen = true;
            openedKl++;
            if (openedKl >= rows * columns - bombs)
            {
                winGame();
                return;
            }
            if (kletki[col, row].near > 0)
            {
                gameView.Rows[row].Cells[col].Value = kletki[col, row].near;
            }
            else if(kletki[col, row].near == 0)
            {
                openNeibNear(col, row + 1);
                openNeibNear(col, row - 1);
                openNeibNear(col - 1, row);
                openNeibNear(col + 1, row);
            }
        }

        public void setNeibNear(int col, int row)
        {
            for(int i = -1; i < 2; i++)
            {
                if (col + i < 0 || col + i >= columns)
                    continue;
                for(int k = -1; k < 2; k++)
                {
                    if (row + k < 0 || row + k >= rows || (i == 0 && k == 0))
                        continue;
                    if (kletki[col + i, row + k].isBomb)
                        continue;
                    kletki[col + i, row + k].near++;
                    //gameView.Rows[row+k].Cells[col+i].Value = kletki[col + i, row + k].near;
                }
            }
        }

        private void winGame()
        {
            timer1.Stop();
            openAllMap();
            MessageBox.Show("Поздравляем! Вы выйграли.\nВремени потрачено: " + time);
            newGame();
        }

        private void loseGame()
        {
            timer1.Stop();
            openAllMap();
            MessageBox.Show("Вы проиграли!\nВремени потрачено: " + time);
            newGame();
        }

        private void newGame() 
        {
            timer1.Stop();
            flags = 0;
            openedKl = 0;
            time = 0;
            bombs = 9;
            columns = 16;
            rows = 16;
            widthcol = 20;
            heightrow = 20;
            label_bomb.Text = "Бомб: " + bombs;
            label_time.Text = "Прошло времени:\n" + time + " с";
            kletki = new kletka[columns, rows];
            gameView.Rows.Clear();
            gameView.Columns.Clear();


            for (int i = 0; i < columns; i++)
            {
                gameView.Columns.Add("col" + i, "");
                gameView.Columns[i].Width = widthcol;
            }
            for (int i = 0; i < rows; i++)
            {
                gameView.Rows.Add("");
                gameView.Rows[i].Height = heightrow;
                gameView.Rows[i].DefaultCellStyle.BackColor = SystemColors.InactiveCaption;
            }


            gameView.Size = new Size(gameView.Columns[0].Width * gameView.Columns.Count + 3, gameView.Rows[0].Height * gameView.Rows.Count + 3);
            Size = new Size(gameView.Size.Width + label_time.Width, gameView.Size.Height + label_bomb.Height + label_time.Height + 100);
            

            for (int i = 0; i < columns; i++)
            {
                for (int k = 0; k < rows; k++)
                {
                    kletki[i,k] = new kletka();
                }
            }
            for (int i = 0; i < bombs; i++)
            {
                int randCol = rand.Next(columns);
                int randRow = rand.Next(rows);
                if (!kletki[randCol, randRow].isBomb)
                {
                    kletki[randCol, randRow].isBomb = true;
                    //gameView.Rows[randRow].Cells[randCol].Value = "+";

                    setNeibNear(randCol, randRow);
                    
                }
                else
                    i--;
            }
        }
    }
}
