using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;

namespace DisarmX
{
    class Area
    {
        public int X { get; private set; }
        public int Y { get; private set; }
        public int Mines { get; private set; }
        public List<Position> MinesPosition = new List<Position>();
        public List<Position> MarkedFields = new List<Position>();
        public Field[,] Matrix { get; set; }

        public Area(int xWidth, int yWidth, int iMines)
        {
            X = xWidth;
            Y = yWidth;
            Mines = iMines;

            MinesPosition.Clear();
            MarkedFields.Clear();

            Matrix = new Field[X, Y];
            Matrix = DrawMineMatrix();

            
        }

        private Field[,] DrawMineMatrix()
        {
            Field[,] field = new Field[X, Y];

            ForEachField((i, j) =>
            {
                field[i, j] = new Field();
                field[i, j].X = i;
                field[i, j].Y = j;
            });

            Random rand = new Random();
            for (int i = 0; i < Mines; i++)
            {
                RandomMine(rand, field);
            }

            CountMines(field);
            return field;
        }

        private int RandomMine(Random rand, Field[,] field)
        {
            int randX = rand.Next(0, X);
            int randY = rand.Next(0, Y);

            if (field[randX, randY].Value != "m")
            {
                field[randX, randY].Value = "m";
                Position pos = new Position();
                pos.X = randX;
                pos.Y = randY;
                MinesPosition.Add(pos);
            }
            else
            {
                return RandomMine(rand, field);
            }
            return 1;
        }

        public string MarkField(int x, int y)
        {
            Position pos = new Position();
            pos.X = x;
            pos.Y = y;
            if (Matrix[x, y].Status == Field.Mark.NULL)
            {
                Matrix[x, y].Status = Field.Mark.EXCLAMATION;

                if(Matrix[x,y].Value == "m")
                    MarkedFields.Add(pos);

                return "!";
            }

            else if (Matrix[x, y].Status != Field.Mark.NULL)
            {
                if (Matrix[x, y].Status == Field.Mark.EXCLAMATION)
                {
                    Matrix[x, y].Status = Field.Mark.QUESTION;

                    if (Matrix[x, y].Value == "m")
                    {

                        foreach (Position item in MarkedFields)
                        {
                            if (item.X == Matrix[x, y].X && item.Y == Matrix[x, y].Y)
                            {
                                MarkedFields.Remove(item);
                                return "?";
                            }
                        }
                    }
                        

                    return "?";
                }
                else if (Matrix[x, y].Status == Field.Mark.QUESTION)
                {
                    Matrix[x, y].Status = Field.Mark.NULL;
                    return "";
                }
            }
            return "";

        }

        private void ForEachField(Action<int, int> action)
        {
            for (int i = 0; i <= X - 1; i++)
                for (int j = 0; j <= Y - 1; j++)
                    action(i, j);
        }

        private void CountMines(Field[,] field)
        {

            ForEachField((i, j) =>
            {
                if (field[i, j].Value == "m")
                {
                    int posx1, posx2, posy1, posy2;

                    posx1 = ((i - 1) < 0) ? 0 : i - 1;
                    posx2 = ((i + 1) > X - 1) ? i : i + 1;

                    posy1 = ((j - 1) < 0) ? 0 : j - 1;
                    posy2 = ((j + 1) > Y - 1) ? j : j + 1;

                    for (int k = posx1; k <= posx2; k++)
                    {
                        for (int l = posy1; l <= posy2; l++)
                        {
                            if (field[k, l].Value != "m")
                            {
                                if (field[k, l].Value == "0")
                                {
                                    field[k, l].Value = "1";
                                }
                                else
                                {
                                    int res;
                                    if (int.TryParse(field[k, l].Value, out res))
                                    {
                                        res++;
                                        field[k, l].Value = res.ToString();
                                    }
                                }
                            }
                        }
                    }

                }
            });

        }

    }
}
