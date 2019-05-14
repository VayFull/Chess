using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using Chess.Properties;

namespace Chess
{
    internal partial class Chess : Form
    {
        public Chess()
        {
            Controller.form = this;
            Controller.Initializer();
            foreach (var element in Controller.list) Controls.Add(element);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Image img = Resources.Grey;
            e.Graphics.DrawImage(img, 510, 90);
        }
    }


    public enum Site
    {
        NoSite,
        White,
        Black
    }

    public enum Condition
    {
        Soldier,
        Horse,
        Officer,
        Rook,
        Queen,
        King,
        NotFigure
    }

    public static class Controller
    {
        public static bool IsWhiteTurn = true;
        public static List<Button> list = new List<Button>();
        public static Form form;
        public static Figure[,] FieldOfFigures = new Figure[8, 8];

        public static void Initializer()
        {
            var label = new Label(); //Кто ходит?
            form.Size = new Size(1920, 1080);
            form.FormBorderStyle = FormBorderStyle.None;
            form.WindowState = FormWindowState.Maximized;
            for (var y = 0; y < 8; y++)
            for (var x = 0; x < 8; x++)
            {
                var xPos = 40 + 510 + x * 100;
                var yPos = 40 + 90 + y * 100;
                var currPosition = new Point(xPos, yPos);
                if (y == 1)
                    FieldOfFigures[x, y] = new Soldier(Site.Black, currPosition);
                if (y == 6)
                    FieldOfFigures[x, y] = new Soldier(Site.White, currPosition);
                if (y == 0)
                {
                    if (x == 0 || x == 7) FieldOfFigures[x, y] = new Rook(Site.Black, currPosition);
                    if (x == 1 || x == 6) FieldOfFigures[x, y] = new Horse(Site.Black, currPosition);
                    if (x == 2 || x == 5) FieldOfFigures[x, y] = new Officer(Site.Black, currPosition);
                    if (x == 3) FieldOfFigures[x, y] = new Queen(Site.Black, currPosition);
                    if (x == 4) FieldOfFigures[x, y] = new King(Site.Black, currPosition);
                }

                if (y == 7)
                {
                    if (x == 0 || x == 7) FieldOfFigures[x, y] = new Rook(Site.White, currPosition);
                    if (x == 1 || x == 6) FieldOfFigures[x, y] = new Horse(Site.White, currPosition);
                    if (x == 2 || x == 5) FieldOfFigures[x, y] = new Officer(Site.White, currPosition);
                    if (x == 3) FieldOfFigures[x, y] = new Queen(Site.White, currPosition);
                    if (x == 4) FieldOfFigures[x, y] = new King(Site.White, currPosition);
                }

                if (FieldOfFigures[x, y] == null) FieldOfFigures[x, y] = new Figure {condition = Condition.NotFigure};
            }

            foreach (var element in FieldOfFigures) element.UpdateList(FieldOfFigures);

            Thread.Sleep(50);
        }
    }

    public class Soldier : Figure
    {
        public Soldier(Site site, Point position)
        {
            image = new Button();
            CurrPosition = position;
            image.Location = CurrPosition;
            image.Size = Resources.BlackSoldier.Size;
            if (site == Site.Black) image.Image = Resources.BlackSoldier;
            else image.Image = Resources.Soldier;
            image.Show();
            condition = Condition.Soldier;
            Controller.list.Add(image);
            this.site = site;
            image.Click += Image_Click;
        }


        public override void UpdateList(Figure[,] figures)
        {
            AvailablePoints.Clear();
            var x = (CurrPosition.X - 550) / 100;
            var y = (CurrPosition.Y - 130) / 100;

            if (site == Site.Black) //для черных
            {
                if (y == 1) //для старта
                {
                    if (figures[x, y + 1].condition == Condition.NotFigure)
                        AvailablePoints.Add(new Point(CurrPosition.X, CurrPosition.Y + 100));

                    if (figures[x, y + 2].condition == Condition.NotFigure &&
                        figures[x, y + 1].condition == Condition.NotFigure)
                        AvailablePoints.Add(new Point(CurrPosition.X, CurrPosition.Y + 200));
                }

                if (y != 7 && y != 1) //для обычного случая
                    if (figures[x, y + 1].condition == Condition.NotFigure)
                        AvailablePoints.Add(new Point(CurrPosition.X, CurrPosition.Y + 100));

                if (y != 7) //Для атаки
                {
                    if (x != 7 && figures[x + 1, y + 1].site == Site.White)
                        AvailablePoints.Add(new Point(CurrPosition.X + 100, CurrPosition.Y + 100));
                    if (x != 0 && figures[x - 1, y + 1].site == Site.White)
                        AvailablePoints.Add(new Point(CurrPosition.X - 100, CurrPosition.Y + 100));
                }
            }

            if (site == Site.White)
            {
                if (y == 6) //для старта
                {
                    if (figures[x, y - 1].condition == Condition.NotFigure)
                        AvailablePoints.Add(new Point(CurrPosition.X, CurrPosition.Y - 100));
                    if (figures[x, y - 2].condition == Condition.NotFigure &&
                        figures[x, y - 1].condition == Condition.NotFigure)
                        AvailablePoints.Add(new Point(CurrPosition.X, CurrPosition.Y - 200));
                }

                if (y != 6 && y != 0) //для обычного случая
                    if (figures[x, y - 1].condition == Condition.NotFigure)
                        AvailablePoints.Add(new Point(CurrPosition.X, CurrPosition.Y - 100));

                if (y != 0) //Для атаки
                {
                    if (x != 7 && figures[x + 1, y - 1].site == Site.Black)
                        AvailablePoints.Add(new Point(CurrPosition.X + 100, CurrPosition.Y - 100));
                    if (x != 0 && figures[x - 1, y - 1].site == Site.Black)
                        AvailablePoints.Add(new Point(CurrPosition.X - 100, CurrPosition.Y - 100));
                }
            }
        } //метод, который позволяет получить текущий лист доступных ходов

        public void Image_Click(object sender, EventArgs e)
        {
            if (Controller.IsWhiteTurn && site == Site.White)
            {
                foreach (var element in Controller.FieldOfFigures) element.UpdateList(Controller.FieldOfFigures);
                var rnd = new Random().Next(AvailablePoints.Count);
                if (AvailablePoints.Count != 0)
                {
                    var x = (CurrPosition.X - 550) / 100;
                    var y = (CurrPosition.Y - 130) / 100;
                    var newX = (AvailablePoints[rnd].X - 550) / 100;
                    var newY = (AvailablePoints[rnd].Y - 130) / 100;
                    if (Controller.FieldOfFigures[newX, newY].condition != Condition.NotFigure)
                        Controller.form.Controls.Remove(Controller.FieldOfFigures[newX, newY].image);
                    Controller.form.Controls.Remove(image);
                    Controller.FieldOfFigures[newX, newY] = new Soldier(site = site, AvailablePoints[rnd]);
                    Controller.form.Controls.Add(Controller.FieldOfFigures[newX, newY].image);
                    Controller.FieldOfFigures[x, y] = new Figure {condition = Condition.NotFigure};
                    Controller.IsWhiteTurn = false;
                }
            }
            else
            {
                if (Controller.IsWhiteTurn == false && site == Site.Black)
                {
                    foreach (var element in Controller.FieldOfFigures) element.UpdateList(Controller.FieldOfFigures);
                    var rnd = new Random().Next(AvailablePoints.Count);
                    if (AvailablePoints.Count != 0)
                    {
                        var x = (CurrPosition.X - 550) / 100;
                        var y = (CurrPosition.Y - 130) / 100;
                        var newX = (AvailablePoints[rnd].X - 550) / 100;
                        var newY = (AvailablePoints[rnd].Y - 130) / 100;
                        if (Controller.FieldOfFigures[newX, newY].condition != Condition.NotFigure)
                            Controller.form.Controls.Remove(Controller.FieldOfFigures[newX, newY].image);
                        Controller.form.Controls.Remove(image);
                        Controller.FieldOfFigures[newX, newY] = new Soldier(site = site, AvailablePoints[rnd]);
                        Controller.form.Controls.Add(Controller.FieldOfFigures[newX, newY].image);
                        Controller.FieldOfFigures[x, y] = new Figure {condition = Condition.NotFigure};
                        Controller.IsWhiteTurn = true;
                    }
                }
            }
        }
    }

    public class Horse : Figure
    {
        public Horse(Site site, Point position)
        {
            image = new Button();
            CurrPosition = position;
            image.Location = CurrPosition;
            image.Size = Resources.BlackHorse.Size;
            if (site == Site.Black) image.Image = Resources.BlackHorse;
            else image.Image = Resources.Horse;
            condition = Condition.Horse;
            Controller.list.Add(image);
            this.site = site;
            image.Click += Image_Click;
        }

        public override void UpdateList(Figure[,] figures)
        {
            AvailablePoints.Clear();
            var x = (CurrPosition.X - 550) / 100;
            var y = (CurrPosition.Y - 130) / 100;

            if (y == 0) //для самой верхней строки
            {
                if (x == 0)
                {
                    if (figures[x + 2, y + 1].site != site)
                        AvailablePoints.Add(new Point(CurrPosition.X + 200, CurrPosition.Y + 100));
                    if (figures[x + 1, y + 2].site != site)
                        AvailablePoints.Add(new Point(CurrPosition.X + 100, CurrPosition.Y + 200));
                }

                if (x == 7)
                {
                    if (figures[x - 2, y + 1].site != site)
                        AvailablePoints.Add(new Point(CurrPosition.X - 200, CurrPosition.X + 100));
                    if (figures[x - 1, y + 2].site != site)
                        AvailablePoints.Add(new Point(CurrPosition.X - 100, CurrPosition.Y + 200));
                }

                if (x == 1)
                {
                    if (figures[x + 1, y + 2].site != site)
                        AvailablePoints.Add(new Point(CurrPosition.X + 100, CurrPosition.Y + 200));
                    if (figures[x - 1, y + 2].site != site)
                        AvailablePoints.Add(new Point(CurrPosition.X - 100, CurrPosition.Y + 200));
                    if (figures[x + 2, y + 1].site != site)
                        AvailablePoints.Add(new Point(CurrPosition.X + 200, CurrPosition.Y + 100));
                }

                if (x == 6)
                {
                    if (figures[x + 1, y + 2].site != site)
                        AvailablePoints.Add(new Point(CurrPosition.X + 100, CurrPosition.Y + 200));
                    if (figures[x - 1, y + 2].site != site)
                        AvailablePoints.Add(new Point(CurrPosition.X - 100, CurrPosition.Y + 200));
                    if (figures[x - 2, y + 1].site != site)
                        AvailablePoints.Add(new Point(CurrPosition.X - 200, CurrPosition.Y + 100));
                }

                if (x != 0 && x != 1 && x != 6 && x != 7)
                {
                    if (figures[x - 2, y + 1].site != site)
                        AvailablePoints.Add(new Point(CurrPosition.X - 200, CurrPosition.Y + 100));
                    if (figures[x + 2, y + 1].site != site)
                        AvailablePoints.Add(new Point(CurrPosition.X + 200, CurrPosition.Y + 100));
                    if (figures[x + 1, y + 2].site != site)
                        AvailablePoints.Add(new Point(CurrPosition.X + 100, CurrPosition.Y + 200));
                    if (figures[x - 1, y + 2].site != site)
                        AvailablePoints.Add(new Point(CurrPosition.X - 100, CurrPosition.Y + 200));
                }
            }

            if (y == 7) //для самой нижней строки
            {
                if (x == 0)
                {
                    if (figures[x + 2, y - 1].site != site)
                        AvailablePoints.Add(new Point(CurrPosition.X + 200, CurrPosition.Y - 100));
                    if (figures[x + 1, y - 2].site != site)
                        AvailablePoints.Add(new Point(CurrPosition.X + 100, CurrPosition.Y - 200));
                }

                if (x == 7)
                {
                    if (figures[x - 2, y - 1].site != site)
                        AvailablePoints.Add(new Point(CurrPosition.X - 200, CurrPosition.X - 100));
                    if (figures[x - 1, y - 2].site != site)
                        AvailablePoints.Add(new Point(CurrPosition.X - 100, CurrPosition.Y - 200));
                }

                if (x == 1)
                {
                    if (figures[x - 1, y - 2].site != site)
                        AvailablePoints.Add(new Point(CurrPosition.X - 100, CurrPosition.Y - 200));
                    if (figures[x + 1, y - 2].site != site)
                        AvailablePoints.Add(new Point(CurrPosition.X + 100, CurrPosition.Y - 200));
                    if (figures[x + 2, y - 1].site != site)
                        AvailablePoints.Add(new Point(CurrPosition.X + 200, CurrPosition.Y - 100));
                }

                if (x == 6)
                {
                    if (figures[x + 1, y - 2].site != site)
                        AvailablePoints.Add(new Point(CurrPosition.X + 100, CurrPosition.Y - 200));
                    if (figures[x - 1, y - 2].site != site)
                        AvailablePoints.Add(new Point(CurrPosition.X - 100, CurrPosition.Y - 200));
                    if (figures[x - 2, y - 1].site != site)
                        AvailablePoints.Add(new Point(CurrPosition.X - 200, CurrPosition.Y - 100));
                }

                if (x != 0 && x != 1 && x != 6 && x != 7)
                {
                    if (figures[x - 2, y - 1].site != site)
                        AvailablePoints.Add(new Point(CurrPosition.X - 200, CurrPosition.Y - 100));
                    if (figures[x + 2, y - 1].site != site)
                        AvailablePoints.Add(new Point(CurrPosition.X + 200, CurrPosition.Y - 100));
                    if (figures[x + 1, y - 2].site != site)
                        AvailablePoints.Add(new Point(CurrPosition.X + 100, CurrPosition.Y - 200));
                    if (figures[x - 1, y - 2].site != site)
                        AvailablePoints.Add(new Point(CurrPosition.X - 100, CurrPosition.Y - 200));
                }
            }

            if (y == 1)
            {
                if (x == 0)
                {
                    if (figures[x + 2, y - 1].site != site)
                        AvailablePoints.Add(new Point(CurrPosition.X + 200, CurrPosition.Y - 100));
                    if (figures[x + 2, y + 1].site != site)
                        AvailablePoints.Add(new Point(CurrPosition.X + 200, CurrPosition.Y + 100));
                    if (figures[x + 1, y + 2].site != site)
                        AvailablePoints.Add(new Point(CurrPosition.X + 100, CurrPosition.Y + 200));
                }

                if (x == 7)
                {
                    if (figures[x - 2, y - 1].site != site)
                        AvailablePoints.Add(new Point(CurrPosition.X - 200, CurrPosition.Y - 100));
                    if (figures[x - 2, y + 1].site != site)
                        AvailablePoints.Add(new Point(CurrPosition.X - 200, CurrPosition.Y + 100));
                    if (figures[x - 1, y + 2].site != site)
                        AvailablePoints.Add(new Point(CurrPosition.X - 100, CurrPosition.Y + 200));
                }

                if (x == 1)
                {
                    if (figures[x + 2, y - 1].site != site)
                        AvailablePoints.Add(new Point(CurrPosition.X + 200, CurrPosition.Y - 100));
                    if (figures[x + 2, y + 1].site != site)
                        AvailablePoints.Add(new Point(CurrPosition.X + 200, CurrPosition.Y + 100));
                    if (figures[x - 1, y + 2].site != site)
                        AvailablePoints.Add(new Point(CurrPosition.X - 100, CurrPosition.Y + 200));
                    if (figures[x + 1, y + 2].site != site)
                        AvailablePoints.Add(new Point(CurrPosition.X + 100, CurrPosition.Y + 200));
                }

                if (x == 6)
                {
                    if (figures[x - 2, y - 1].site != site)
                        AvailablePoints.Add(new Point(CurrPosition.X - 200, CurrPosition.Y - 100));
                    if (figures[x - 2, y + 1].site != site)
                        AvailablePoints.Add(new Point(CurrPosition.X - 200, CurrPosition.Y + 100));
                    if (figures[x - 1, y + 2].site != site)
                        AvailablePoints.Add(new Point(CurrPosition.X - 100, CurrPosition.Y + 200));
                    if (figures[x + 1, y + 2].site != site)
                        AvailablePoints.Add(new Point(CurrPosition.X + 100, CurrPosition.Y + 200));
                }

                if (x >= 2 && x <= 5)
                {
                    if (figures[x - 2, y + 1].site != site)
                        AvailablePoints.Add(new Point(CurrPosition.X - 200, CurrPosition.Y + 100));
                    if (figures[x - 2, y - 1].site != site)
                        AvailablePoints.Add(new Point(CurrPosition.X - 200, CurrPosition.Y - 100));
                    if (figures[x + 1, y + 2].site != site)
                        AvailablePoints.Add(new Point(CurrPosition.X + 100, CurrPosition.Y + 200));
                    if (figures[x - 1, y + 2].site != site)
                        AvailablePoints.Add(new Point(CurrPosition.X - 100, CurrPosition.Y + 200));
                    if (figures[x + 2, y + 1].site != site)
                        AvailablePoints.Add(new Point(CurrPosition.X + 200, CurrPosition.Y + 100));
                    if (figures[x + 2, y - 1].site != site)
                        AvailablePoints.Add(new Point(CurrPosition.X + 200, CurrPosition.Y - 100));
                }
            }

            if (y == 6)
            {
                if (x == 0)
                {
                    if (figures[x + 2, y + 1].site != site)
                        AvailablePoints.Add(new Point(CurrPosition.X + 200, CurrPosition.Y + 100));
                    if (figures[x + 2, y - 1].site != site)
                        AvailablePoints.Add(new Point(CurrPosition.X + 200, CurrPosition.Y - 100));
                    if (figures[x + 1, y - 2].site != site)
                        AvailablePoints.Add(new Point(CurrPosition.X + 100, CurrPosition.Y - 200));
                }

                if (x == 7)
                {
                    if (figures[x - 2, y + 1].site != site)
                        AvailablePoints.Add(new Point(CurrPosition.X - 200, CurrPosition.Y + 100));
                    if (figures[x - 2, y - 1].site != site)
                        AvailablePoints.Add(new Point(CurrPosition.X - 200, CurrPosition.Y - 100));
                    if (figures[x - 1, y - 2].site != site)
                        AvailablePoints.Add(new Point(CurrPosition.X - 100, CurrPosition.Y - 200));
                }

                if (x == 1)
                {
                    if (figures[x + 2, y + 1].site != site)
                        AvailablePoints.Add(new Point(CurrPosition.X + 200, CurrPosition.Y + 100));
                    if (figures[x + 2, y - 1].site != site)
                        AvailablePoints.Add(new Point(CurrPosition.X + 200, CurrPosition.Y - 100));
                    if (figures[x - 1, y - 2].site != site)
                        AvailablePoints.Add(new Point(CurrPosition.X - 100, CurrPosition.Y - 200));
                    if (figures[x + 1, y - 2].site != site)
                        AvailablePoints.Add(new Point(CurrPosition.X + 100, CurrPosition.Y - 200));
                }

                if (x == 6)
                {
                    if (figures[x - 2, y + 1].site != site)
                        AvailablePoints.Add(new Point(CurrPosition.X - 200, CurrPosition.Y + 100));
                    if (figures[x - 2, y - 1].site != site)
                        AvailablePoints.Add(new Point(CurrPosition.X - 200, CurrPosition.Y - 100));
                    if (figures[x - 1, y - 2].site != site)
                        AvailablePoints.Add(new Point(CurrPosition.X - 100, CurrPosition.Y - 200));
                    if (figures[x + 1, y - 2].site != site)
                        AvailablePoints.Add(new Point(CurrPosition.X + 100, CurrPosition.Y - 200));
                }

                if (x >= 2 && x <= 5)
                {
                    if (figures[x - 2, y - 1].site != site)
                        AvailablePoints.Add(new Point(CurrPosition.X - 200, CurrPosition.Y - 100));
                    if (figures[x - 2, y + 1].site != site)
                        AvailablePoints.Add(new Point(CurrPosition.X - 200, CurrPosition.Y + 100));
                    if (figures[x + 1, y - 2].site != site)
                        AvailablePoints.Add(new Point(CurrPosition.X + 100, CurrPosition.Y - 200));
                    if (figures[x - 1, y - 2].site != site)
                        AvailablePoints.Add(new Point(CurrPosition.X - 100, CurrPosition.Y - 200));
                    if (figures[x + 2, y - 1].site != site)
                        AvailablePoints.Add(new Point(CurrPosition.X + 200, CurrPosition.Y - 100));
                    if (figures[x + 2, y + 1].site != site)
                        AvailablePoints.Add(new Point(CurrPosition.X + 200, CurrPosition.Y + 100));
                }
            }

            if (y >= 2 && y <= 5)
            {
                if (x == 0)
                {
                    if (figures[x + 2, y + 1].site != site)
                        AvailablePoints.Add(new Point(CurrPosition.X + 200, CurrPosition.Y + 100));
                    if (figures[x + 2, y - 1].site != site)
                        AvailablePoints.Add(new Point(CurrPosition.X + 200, CurrPosition.Y - 100));
                    if (figures[x + 1, y + 2].site != site)
                        AvailablePoints.Add(new Point(CurrPosition.X + 100, CurrPosition.Y + 200));
                    if (figures[x + 1, y - 2].site != site)
                        AvailablePoints.Add(new Point(CurrPosition.X + 100, CurrPosition.Y - 200));
                }

                if (x == 7)
                {
                    if (figures[x - 2, y + 1].site != site)
                        AvailablePoints.Add(new Point(CurrPosition.X - 200, CurrPosition.Y + 100));
                    if (figures[x - 2, y - 1].site != site)
                        AvailablePoints.Add(new Point(CurrPosition.X - 200, CurrPosition.Y - 100));
                    if (figures[x - 1, y + 2].site != site)
                        AvailablePoints.Add(new Point(CurrPosition.X - 100, CurrPosition.Y + 200));
                    if (figures[x - 1, y - 2].site != site)
                        AvailablePoints.Add(new Point(CurrPosition.X - 100, CurrPosition.Y - 200));
                }

                if (x == 1)
                {
                    if (figures[x + 2, y + 1].site != site)
                        AvailablePoints.Add(new Point(CurrPosition.X + 200, CurrPosition.Y + 100));
                    if (figures[x + 2, y - 1].site != site)
                        AvailablePoints.Add(new Point(CurrPosition.X + 200, CurrPosition.Y - 100));
                    if (figures[x + 1, y + 2].site != site)
                        AvailablePoints.Add(new Point(CurrPosition.X + 100, CurrPosition.Y + 200));
                    if (figures[x + 1, y - 2].site != site)
                        AvailablePoints.Add(new Point(CurrPosition.X + 100, CurrPosition.Y - 200));
                    if (figures[x - 1, y - 2].site != site)
                        AvailablePoints.Add(new Point(CurrPosition.X - 100, CurrPosition.Y - 200)); //
                    if (figures[x - 1, y + 2].site != site)
                        AvailablePoints.Add(new Point(CurrPosition.X - 100, CurrPosition.Y + 200));
                }

                if (x == 6)
                {
                    if (figures[x - 2, y + 1].site != site)
                        AvailablePoints.Add(new Point(CurrPosition.X - 200, CurrPosition.Y + 100));
                    if (figures[x - 2, y - 1].site != site)
                        AvailablePoints.Add(new Point(CurrPosition.X - 200, CurrPosition.Y - 100));
                    if (figures[x - 1, y + 2].site != site)
                        AvailablePoints.Add(new Point(CurrPosition.X - 100, CurrPosition.Y + 200));
                    if (figures[x - 1, y - 2].site != site)
                        AvailablePoints.Add(new Point(CurrPosition.X - 100, CurrPosition.Y - 200));
                    if (figures[x + 1, y - 2].site != site)
                        AvailablePoints.Add(new Point(CurrPosition.X + 100, CurrPosition.Y - 200)); //
                    if (figures[x + 1, y + 2].site != site)
                        AvailablePoints.Add(new Point(CurrPosition.X + 100, CurrPosition.Y + 200));
                }

                if (x >= 2 && x <= 5)
                {
                    if (figures[x + 2, y + 1].site != site)
                        AvailablePoints.Add(new Point(CurrPosition.X + 200, CurrPosition.Y + 100));
                    if (figures[x + 2, y - 1].site != site)
                        AvailablePoints.Add(new Point(CurrPosition.X + 200, CurrPosition.Y - 100));
                    if (figures[x + 1, y + 2].site != site)
                        AvailablePoints.Add(new Point(CurrPosition.X + 100, CurrPosition.Y + 200));
                    if (figures[x + 1, y - 2].site != site)
                        AvailablePoints.Add(new Point(CurrPosition.X + 100, CurrPosition.Y - 200));
                    if (figures[x - 2, y + 1].site != site)
                        AvailablePoints.Add(new Point(CurrPosition.X - 200, CurrPosition.Y + 100));
                    if (figures[x - 2, y - 1].site != site)
                        AvailablePoints.Add(new Point(CurrPosition.X - 200, CurrPosition.Y - 100));
                    if (figures[x - 1, y + 2].site != site)
                        AvailablePoints.Add(new Point(CurrPosition.X - 100, CurrPosition.Y + 200));
                    if (figures[x - 1, y - 2].site != site)
                        AvailablePoints.Add(new Point(CurrPosition.X - 100, CurrPosition.Y - 200));
                }
            }
        }

        public void Image_Click(object sender, EventArgs e)
        {
            if (Controller.IsWhiteTurn && site == Site.White)
            {
                foreach (var element in Controller.FieldOfFigures) element.UpdateList(Controller.FieldOfFigures);
                var rnd = new Random().Next(AvailablePoints.Count);
                if (AvailablePoints.Count != 0)
                {
                    var x = (CurrPosition.X - 550) / 100;
                    var y = (CurrPosition.Y - 130) / 100;
                    var newX = (AvailablePoints[rnd].X - 550) / 100;
                    var newY = (AvailablePoints[rnd].Y - 130) / 100;
                    if (Controller.FieldOfFigures[newX, newY].condition != Condition.NotFigure)
                        Controller.form.Controls.Remove(Controller.FieldOfFigures[newX, newY].image);
                    Controller.form.Controls.Remove(image);
                    Controller.FieldOfFigures[newX, newY] = new Horse(site = site, AvailablePoints[rnd]);
                    Controller.form.Controls.Add(Controller.FieldOfFigures[newX, newY].image);
                    Controller.FieldOfFigures[x, y] = new Figure {condition = Condition.NotFigure};
                    Controller.IsWhiteTurn = false;
                }
            }
            else
            {
                if (Controller.IsWhiteTurn == false && site == Site.Black)
                {
                    foreach (var element in Controller.FieldOfFigures) element.UpdateList(Controller.FieldOfFigures);
                    var rnd = new Random().Next(AvailablePoints.Count);
                    if (AvailablePoints.Count != 0)
                    {
                        var x = (CurrPosition.X - 550) / 100;
                        var y = (CurrPosition.Y - 130) / 100;
                        var newX = (AvailablePoints[rnd].X - 550) / 100;
                        var newY = (AvailablePoints[rnd].Y - 130) / 100;
                        if (Controller.FieldOfFigures[newX, newY].condition != Condition.NotFigure)
                            Controller.form.Controls.Remove(Controller.FieldOfFigures[newX, newY].image);
                        Controller.form.Controls.Remove(image);
                        Controller.FieldOfFigures[newX, newY] = new Horse(site = site, AvailablePoints[rnd]);
                        Controller.form.Controls.Add(Controller.FieldOfFigures[newX, newY].image);
                        Controller.FieldOfFigures[x, y] = new Figure {condition = Condition.NotFigure};
                        Controller.IsWhiteTurn = true;
                    }
                }
            }
        }
    }

    public class Officer : Figure
    {
        public Officer(Site site, Point position)
        {
            condition = Condition.Officer;
            image = new Button();
            CurrPosition = position;
            image.Location = CurrPosition;
            image.Size = Resources.BlackOfficer.Size;
            if (site == Site.Black) image.Image = Resources.BlackOfficer;
            else image.Image = Resources.Officer;
            Controller.list.Add(image);
            this.site = site;
            image.Click += Image_Click;
        }

        public override void UpdateList(Figure[,] figures)
        {
            AvailablePoints.Clear();
            var x = (CurrPosition.X - 550) / 100;
            var y = (CurrPosition.Y - 130) / 100;

            int currPosX = x;
            int currPosY = y;

            int iterations = 0;

            while (currPosX != 7 && currPosY != 7 && figures[currPosX + 1, currPosY + 1].site != site) //в правый нижний угол
            {
                iterations++;
                AvailablePoints.Add(new Point(CurrPosition.X+iterations*100,CurrPosition.Y+iterations*100));
                if (figures[currPosX + 1, currPosY + 1].condition != Condition.NotFigure &&
                    figures[currPosX + 1, currPosY + 1].site != site) break;
                currPosX++;
                currPosY++;
            }

            iterations = 0;
            currPosX = x;
            currPosY = y;

            while (currPosX != 0 && currPosY != 0 && figures[currPosX - 1, currPosY - 1].site != site) //в левый верхний угол
            {
                iterations++;
                AvailablePoints.Add(new Point(CurrPosition.X - iterations * 100, CurrPosition.Y - iterations * 100));
                if (figures[currPosX - 1, currPosY - 1].condition != Condition.NotFigure &&
                    figures[currPosX - 1, currPosY - 1].site != site) break;
                currPosX--;
                currPosY--;
            }

            iterations = 0;
            currPosX = x;
            currPosY = y;

            while (currPosX != 7 && currPosY != 0 && figures[currPosX + 1, currPosY - 1].site != site) //в правый верхний угол
            {
                iterations++;
                AvailablePoints.Add(new Point(CurrPosition.X + iterations * 100, CurrPosition.Y - iterations * 100));
                if (figures[currPosX + 1, currPosY - 1].condition != Condition.NotFigure &&
                    figures[currPosX + 1, currPosY - 1].site != site) break;
                currPosX++;
                currPosY--;
            }
            iterations = 0;
            currPosX = x;
            currPosY = y;

            while (currPosX != 0 && currPosY != 7 && figures[currPosX - 1, currPosY + 1].site != site) //в левый нижний угол
            {
                iterations++;
                AvailablePoints.Add(new Point(CurrPosition.X - iterations * 100, CurrPosition.Y + iterations * 100));
                if (figures[currPosX - 1, currPosY + 1].condition != Condition.NotFigure &&
                    figures[currPosX - 1, currPosY + 1].site != site) break;
                currPosX--;
                currPosY++;
            }
        }

        public void Image_Click(object sender, EventArgs e)
        {
            if (Controller.IsWhiteTurn && site == Site.White)
            {
                foreach (var element in Controller.FieldOfFigures) element.UpdateList(Controller.FieldOfFigures);
                var rnd = new Random().Next(AvailablePoints.Count);
                if (AvailablePoints.Count != 0)
                {
                    var x = (CurrPosition.X - 550) / 100;
                    var y = (CurrPosition.Y - 130) / 100;
                    var newX = (AvailablePoints[rnd].X - 550) / 100;
                    var newY = (AvailablePoints[rnd].Y - 130) / 100;
                    if (Controller.FieldOfFigures[newX, newY].condition != Condition.NotFigure)
                        Controller.form.Controls.Remove(Controller.FieldOfFigures[newX, newY].image);
                    Controller.form.Controls.Remove(image);
                    Controller.FieldOfFigures[newX, newY] = new Officer(site = site, AvailablePoints[rnd]);
                    Controller.form.Controls.Add(Controller.FieldOfFigures[newX, newY].image);
                    Controller.FieldOfFigures[x, y] = new Figure { condition = Condition.NotFigure };
                    Controller.IsWhiteTurn = false;
                }
            }
            else
            {
                if (Controller.IsWhiteTurn == false && site == Site.Black)
                {
                    foreach (var element in Controller.FieldOfFigures) element.UpdateList(Controller.FieldOfFigures);
                    var rnd = new Random().Next(AvailablePoints.Count);
                    if (AvailablePoints.Count != 0)
                    {
                        var x = (CurrPosition.X - 550) / 100;
                        var y = (CurrPosition.Y - 130) / 100;
                        var newX = (AvailablePoints[rnd].X - 550) / 100;
                        var newY = (AvailablePoints[rnd].Y - 130) / 100;
                        if (Controller.FieldOfFigures[newX, newY].condition != Condition.NotFigure)
                            Controller.form.Controls.Remove(Controller.FieldOfFigures[newX, newY].image);
                        Controller.form.Controls.Remove(image);
                        Controller.FieldOfFigures[newX, newY] = new Officer(site = site, AvailablePoints[rnd]);
                        Controller.form.Controls.Add(Controller.FieldOfFigures[newX, newY].image);
                        Controller.FieldOfFigures[x, y] = new Figure { condition = Condition.NotFigure };
                        Controller.IsWhiteTurn = true;
                    }
                }
            }
        }


    }

    public class Rook : Figure
    {
        public Rook(Site site, Point position)
        {
            image = new Button();
            CurrPosition = position;
            image.Location = CurrPosition;
            image.Size = Resources.BlackRook.Size;
            if (site == Site.Black) image.Image = Resources.BlackRook;
            else image.Image = Resources.Rook;
            condition = Condition.Rook;
            Controller.list.Add(image);
            this.site = site;
            image.Click += Image_Click;
        }

        public override void UpdateList(Figure[,] figures)
        {
            AvailablePoints.Clear();
            var x = (CurrPosition.X - 550) / 100;
            var y = (CurrPosition.Y - 130) / 100;

            var currXValue = x;
            var currYValue = y;

            var iterations = 0;

            while (currXValue != 7 && figures[currXValue + 1, y].site != site) //вправо
            {
                iterations++;
                AvailablePoints.Add(new Point(CurrPosition.X + iterations * 100, CurrPosition.Y));
                if (figures[currXValue + 1, y].condition != Condition.NotFigure &&
                    figures[currXValue + 1, y].site != site) break;
                currXValue++;
            }

            currXValue = x;
            iterations = 0;

            while (currXValue != 0 && figures[currXValue - 1, y].site != site) //влево
            {
                iterations++;
                AvailablePoints.Add(new Point(CurrPosition.X - iterations * 100, CurrPosition.Y));
                if (figures[currXValue - 1, y].condition != Condition.NotFigure &&
                    figures[currXValue - 1, y].site != site) break;
                currXValue--;
            }

            currXValue = x;
            iterations = 0;

            while (currYValue != 7 && figures[x, currYValue + 1].site != site) //вниз
            {
                iterations++;
                AvailablePoints.Add(new Point(CurrPosition.X, CurrPosition.Y + iterations * 100));
                if (figures[x, currYValue + 1].condition != Condition.NotFigure &&
                    figures[x, currYValue + 1].site != site) break;
                currYValue++;
            }

            currYValue = y;
            iterations = 0;

            while (currYValue != 0 && figures[x, currYValue - 1].site != site) //вверх
            {
                iterations++;
                AvailablePoints.Add(new Point(CurrPosition.X, CurrPosition.Y - iterations * 100));
                if (figures[x, currYValue - 1].condition != Condition.NotFigure &&
                    figures[x, currYValue - 1].site != site) break;
                currYValue--;
            }
        }

        public void Image_Click(object sender, EventArgs e)
        {
            if (Controller.IsWhiteTurn && site == Site.White)
            {
                foreach (var element in Controller.FieldOfFigures) element.UpdateList(Controller.FieldOfFigures);
                var rnd = new Random().Next(AvailablePoints.Count);
                if (AvailablePoints.Count != 0)
                {
                    var x = (CurrPosition.X - 550) / 100;
                    var y = (CurrPosition.Y - 130) / 100;
                    var newX = (AvailablePoints[rnd].X - 550) / 100;
                    var newY = (AvailablePoints[rnd].Y - 130) / 100;
                    if (Controller.FieldOfFigures[newX, newY].condition != Condition.NotFigure)
                        Controller.form.Controls.Remove(Controller.FieldOfFigures[newX, newY].image);
                    Controller.form.Controls.Remove(image);
                    Controller.FieldOfFigures[newX, newY] = new Rook(site = site, AvailablePoints[rnd]);
                    Controller.form.Controls.Add(Controller.FieldOfFigures[newX, newY].image);
                    Controller.FieldOfFigures[x, y] = new Figure {condition = Condition.NotFigure};
                    Controller.IsWhiteTurn = false;
                }
            }
            else
            {
                if (Controller.IsWhiteTurn == false && site == Site.Black)
                {
                    foreach (var element in Controller.FieldOfFigures) element.UpdateList(Controller.FieldOfFigures);
                    var rnd = new Random().Next(AvailablePoints.Count);
                    if (AvailablePoints.Count != 0)
                    {
                        var x = (CurrPosition.X - 550) / 100;
                        var y = (CurrPosition.Y - 130) / 100;
                        var newX = (AvailablePoints[rnd].X - 550) / 100;
                        var newY = (AvailablePoints[rnd].Y - 130) / 100;
                        if (Controller.FieldOfFigures[newX, newY].condition != Condition.NotFigure)
                            Controller.form.Controls.Remove(Controller.FieldOfFigures[newX, newY].image);
                        Controller.form.Controls.Remove(image);
                        Controller.FieldOfFigures[newX, newY] = new Rook(site = site, AvailablePoints[rnd]);
                        Controller.form.Controls.Add(Controller.FieldOfFigures[newX, newY].image);
                        Controller.FieldOfFigures[x, y] = new Figure {condition = Condition.NotFigure};
                        Controller.IsWhiteTurn = true;
                    }
                }
            }
        }
    }

    public class Queen : Figure
    {
        public Queen(Site site, Point position)
        {
            image = new Button();
            CurrPosition = position;
            image.Location = CurrPosition;
            image.Size = Resources.BlackQueen.Size;
            if (site == Site.Black) image.Image = Resources.BlackQueen;
            else image.Image = Resources.Queen;
            condition = Condition.Queen;
            Controller.list.Add(image);
            this.site = site;
            image.Click += Image_Click;
        }

        public override void UpdateList(Figure[,] figures)
        {
            AvailablePoints.Clear();
            var x = (CurrPosition.X - 550) / 100;
            var y = (CurrPosition.Y - 130) / 100;

            int currPosX = x;
            int currPosY = y;

            int iterations = 0;

            while (currPosX != 7 && currPosY != 7 && figures[currPosX + 1, currPosY + 1].site != site) //в правый нижний угол
            {
                iterations++;
                AvailablePoints.Add(new Point(CurrPosition.X + iterations * 100, CurrPosition.Y + iterations * 100));
                if (figures[currPosX + 1, currPosY + 1].condition != Condition.NotFigure &&
                    figures[currPosX + 1, currPosY + 1].site != site) break;
                currPosX++;
                currPosY++;
            }

            iterations = 0;
            currPosX = x;
            currPosY = y;

            while (currPosX != 0 && currPosY != 0 && figures[currPosX - 1, currPosY - 1].site != site) //в левый верхний угол
            {
                iterations++;
                AvailablePoints.Add(new Point(CurrPosition.X - iterations * 100, CurrPosition.Y - iterations * 100));
                if (figures[currPosX - 1, currPosY - 1].condition != Condition.NotFigure &&
                    figures[currPosX - 1, currPosY - 1].site != site) break;
                currPosX--;
                currPosY--;
            }

            iterations = 0;
            currPosX = x;
            currPosY = y;

            while (currPosX != 7 && currPosY != 0 && figures[currPosX + 1, currPosY - 1].site != site) //в правый верхний угол
            {
                iterations++;
                AvailablePoints.Add(new Point(CurrPosition.X + iterations * 100, CurrPosition.Y - iterations * 100));
                if (figures[currPosX + 1, currPosY - 1].condition != Condition.NotFigure &&
                    figures[currPosX + 1, currPosY - 1].site != site) break;
                currPosX++;
                currPosY--;
            }
            iterations = 0;
            currPosX = x;
            currPosY = y;

            while (currPosX != 0 && currPosY != 7 && figures[currPosX - 1, currPosY + 1].site != site) //в левый нижний угол
            {
                iterations++;
                AvailablePoints.Add(new Point(CurrPosition.X - iterations * 100, CurrPosition.Y + iterations * 100));
                if (figures[currPosX - 1, currPosY + 1].condition != Condition.NotFigure &&
                    figures[currPosX - 1, currPosY + 1].site != site) break;
                currPosX--;
                currPosY++;
            }

            int currXValue = x;
            int currYValue = y;

            iterations = 0;

            while (currXValue != 7 && figures[currXValue + 1, y].site != site) //вправо
            {
                iterations++;
                AvailablePoints.Add(new Point(CurrPosition.X + iterations * 100, CurrPosition.Y));
                if (figures[currXValue + 1, y].condition != Condition.NotFigure &&
                    figures[currXValue + 1, y].site != site) break;
                currXValue++;
            }

            currXValue = x;
            iterations = 0;

            while (currXValue != 0 && figures[currXValue - 1, y].site != site) //влево
            {
                iterations++;
                AvailablePoints.Add(new Point(CurrPosition.X - iterations * 100, CurrPosition.Y));
                if (figures[currXValue - 1, y].condition != Condition.NotFigure &&
                    figures[currXValue - 1, y].site != site) break;
                currXValue--;
            }

            currXValue = x;
            iterations = 0;

            while (currYValue != 7 && figures[x, currYValue + 1].site != site) //вниз
            {
                iterations++;
                AvailablePoints.Add(new Point(CurrPosition.X, CurrPosition.Y + iterations * 100));
                if (figures[x, currYValue + 1].condition != Condition.NotFigure &&
                    figures[x, currYValue + 1].site != site) break;
                currYValue++;
            }

            currYValue = y;
            iterations = 0;

            while (currYValue != 0 && figures[x, currYValue - 1].site != site) //вверх
            {
                iterations++;
                AvailablePoints.Add(new Point(CurrPosition.X, CurrPosition.Y - iterations * 100));
                if (figures[x, currYValue - 1].condition != Condition.NotFigure &&
                    figures[x, currYValue - 1].site != site) break;
                currYValue--;
            }
        }

        public void Image_Click(object sender, EventArgs e)
        {
            if (Controller.IsWhiteTurn && site == Site.White)
            {
                foreach (var element in Controller.FieldOfFigures) element.UpdateList(Controller.FieldOfFigures);
                var rnd = new Random().Next(AvailablePoints.Count);
                if (AvailablePoints.Count != 0)
                {
                    var x = (CurrPosition.X - 550) / 100;
                    var y = (CurrPosition.Y - 130) / 100;
                    var newX = (AvailablePoints[rnd].X - 550) / 100;
                    var newY = (AvailablePoints[rnd].Y - 130) / 100;
                    if (Controller.FieldOfFigures[newX, newY].condition != Condition.NotFigure)
                        Controller.form.Controls.Remove(Controller.FieldOfFigures[newX, newY].image);
                    Controller.form.Controls.Remove(image);
                    Controller.FieldOfFigures[newX, newY] = new Queen(site = site, AvailablePoints[rnd]);
                    Controller.form.Controls.Add(Controller.FieldOfFigures[newX, newY].image);
                    Controller.FieldOfFigures[x, y] = new Figure { condition = Condition.NotFigure };
                    Controller.IsWhiteTurn = false;
                }
            }
            else
            {
                if (Controller.IsWhiteTurn == false && site == Site.Black)
                {
                    foreach (var element in Controller.FieldOfFigures) element.UpdateList(Controller.FieldOfFigures);
                    var rnd = new Random().Next(AvailablePoints.Count);
                    if (AvailablePoints.Count != 0)
                    {
                        var x = (CurrPosition.X - 550) / 100;
                        var y = (CurrPosition.Y - 130) / 100;
                        var newX = (AvailablePoints[rnd].X - 550) / 100;
                        var newY = (AvailablePoints[rnd].Y - 130) / 100;
                        if (Controller.FieldOfFigures[newX, newY].condition != Condition.NotFigure)
                            Controller.form.Controls.Remove(Controller.FieldOfFigures[newX, newY].image);
                        Controller.form.Controls.Remove(image);
                        Controller.FieldOfFigures[newX, newY] = new Queen(site = site, AvailablePoints[rnd]);
                        Controller.form.Controls.Add(Controller.FieldOfFigures[newX, newY].image);
                        Controller.FieldOfFigures[x, y] = new Figure { condition = Condition.NotFigure };
                        Controller.IsWhiteTurn = true;
                    }
                }
            }
        }
    }

    public class King : Figure
    {
        public King(Site site, Point position)
        {
            image = new Button();
            CurrPosition = position;
            image.Location = CurrPosition;
            image.Size = Resources.BlackKing.Size;
            if (site == Site.Black) image.Image = Resources.BlackKing;
            else image.Image = Resources.King;
            condition = Condition.King;
            Controller.list.Add(image);
            this.site = site;
            image.Click += Image_Click;
        }

        public override void UpdateList(Figure[,] figures)
        {
            AvailablePoints.Clear();
            var x = (CurrPosition.X - 550) / 100;
            var y = (CurrPosition.Y - 130) / 100;
            if (site == Site.Black)
            {
                for (var a = 1; a <= 6; a++)
                    if (x == a && y == 0)
                    {
                        if (figures[x - 1, y].site != Site.Black && figures[x - 1, y].condition == Condition.NotFigure)
                            AvailablePoints.Add(new Point(CurrPosition.X - 100, CurrPosition.Y));
                        if (figures[x + 1, y].site != Site.Black && figures[x + 1, y].condition == Condition.NotFigure)
                            AvailablePoints.Add(new Point(CurrPosition.X + 100, CurrPosition.Y));
                        if (figures[x, y + 1].site != Site.Black && figures[x, y + 1].condition == Condition.NotFigure)
                            AvailablePoints.Add(new Point(CurrPosition.X, CurrPosition.Y + 100));
                        if (figures[x - 1, y + 1].site != Site.Black &&
                            figures[x - 1, y + 1].condition == Condition.NotFigure)
                            AvailablePoints.Add(new Point(CurrPosition.X - 100, CurrPosition.Y + 100));
                        if (figures[x + 1, y + 1].site != Site.Black &&
                            figures[x + 1, y + 1].condition == Condition.NotFigure)
                            AvailablePoints.Add(new Point(CurrPosition.X + 100, CurrPosition.Y + 100));
                    }

                for (var a = 1; a <= 6; a++)
                    if (x == 0 && y == a)
                    {
                        if (figures[x, y - 1].site != Site.Black && figures[x, y - 1].condition == Condition.NotFigure)
                            AvailablePoints.Add(new Point(CurrPosition.X, CurrPosition.Y - 100));
                        if (figures[x, y + 1].site != Site.Black && figures[x, y + 1].condition == Condition.NotFigure)
                            AvailablePoints.Add(new Point(CurrPosition.X, CurrPosition.Y + 100));
                        if (figures[x + 1, y - 1].site != Site.Black &&
                            figures[x + 1, y - 1].condition == Condition.NotFigure)
                            AvailablePoints.Add(new Point(CurrPosition.X + 100, CurrPosition.Y - 100));
                        if (figures[x + 1, y + 1].site != Site.Black &&
                            figures[x + 1, y + 1].condition == Condition.NotFigure)
                            AvailablePoints.Add(new Point(CurrPosition.X + 100, CurrPosition.Y + 100));
                        if (figures[x + 1, y].site != Site.Black && figures[x + 1, y].condition == Condition.NotFigure)
                            AvailablePoints.Add(new Point(CurrPosition.X + 100, CurrPosition.Y));
                    }

                for (var a = 1; a <= 6; a++)
                    if (x == a && y == 7)
                    {
                        if (figures[x - 1, y].site != Site.Black && figures[x - 1, y].condition == Condition.NotFigure)
                            AvailablePoints.Add(new Point(CurrPosition.X - 100, CurrPosition.Y));
                        if (figures[x + 1, y].site != Site.Black && figures[x + 1, y].condition == Condition.NotFigure)
                            AvailablePoints.Add(new Point(CurrPosition.X + 100, CurrPosition.Y));
                        if (figures[x, y - 1].site != Site.Black && figures[x, y - 1].condition == Condition.NotFigure)
                            AvailablePoints.Add(new Point(CurrPosition.X, CurrPosition.Y - 100));
                        if (figures[x - 1, y - 1].site != Site.Black &&
                            figures[x - 1, y - 1].condition == Condition.NotFigure)
                            AvailablePoints.Add(new Point(CurrPosition.X - 100, CurrPosition.Y - 100));
                        if (figures[x + 1, y - 1].site != Site.Black &&
                            figures[x + 1, y - 1].condition == Condition.NotFigure)
                            AvailablePoints.Add(new Point(CurrPosition.X + 100, CurrPosition.Y - 100));
                    }

                for (var a = 1; a <= 6; a++)
                    if (x == 7 && y == a)
                    {
                        if (figures[x, y - 1].site != Site.Black && figures[x, y - 1].condition == Condition.NotFigure)
                            AvailablePoints.Add(new Point(CurrPosition.X, CurrPosition.Y - 100));
                        if (figures[x, y + 1].site != Site.Black && figures[x, y + 1].condition == Condition.NotFigure)
                            AvailablePoints.Add(new Point(CurrPosition.X, CurrPosition.Y + 100));
                        if (figures[x - 1, y - 1].site != Site.Black &&
                            figures[x - 1, y - 1].condition == Condition.NotFigure)
                            AvailablePoints.Add(new Point(CurrPosition.X - 100, CurrPosition.Y - 100));
                        if (figures[x - 1, y + 1].site != Site.Black &&
                            figures[x - 1, y + 1].condition == Condition.NotFigure)
                            AvailablePoints.Add(new Point(CurrPosition.X - 100, CurrPosition.Y + 100));
                        if (figures[x - 1, y].site != Site.Black && figures[x - 1, y].condition == Condition.NotFigure)
                            AvailablePoints.Add(new
                                Point(CurrPosition.X - 100, CurrPosition.Y));
                    }

                if (figures[x = 0, y = 0].site != Site.Black)
                {
                    if (figures[x + 1, y].site != Site.Black && figures[x + 1, y].condition == Condition.NotFigure)
                        AvailablePoints.Add(new Point(CurrPosition.X + 100, CurrPosition.Y));
                    if (figures[x + 1, y + 1].site != Site.Black &&
                        figures[x + 1, y + 1].condition == Condition.NotFigure)
                        AvailablePoints.Add(new Point(CurrPosition.X + 100, CurrPosition.Y + 100));
                    if (figures[x, y + 1].site != Site.Black && figures[x, y + 1].condition == Condition.NotFigure)
                        AvailablePoints.Add(new Point(CurrPosition.X, CurrPosition.Y + 100));
                }

                if (figures[x = 7, y = 0].site != Site.Black)
                {
                    if (figures[x - 1, y].site != Site.Black && figures[x - 1, y].condition == Condition.NotFigure)
                        AvailablePoints.Add(new Point(CurrPosition.X - 100, CurrPosition.Y));
                    if (figures[x - 1, y + 1].site != Site.Black &&
                        figures[x - 1, y + 1].condition == Condition.NotFigure)
                        AvailablePoints.Add(new Point(CurrPosition.X - 100, CurrPosition.Y + 100));
                    if (figures[x, y + 1].site != Site.Black && figures[x, y + 1].condition == Condition.NotFigure)
                        AvailablePoints.Add(new Point(CurrPosition.X, CurrPosition.Y + 100));
                }

                if (figures[x = 0, y = 7].site != Site.Black)
                {
                    if (figures[x + 1, y].site != Site.Black && figures[x + 1, y].condition == Condition.NotFigure)
                        AvailablePoints.Add(new Point(CurrPosition.X + 100, CurrPosition.Y));
                    if (figures[x + 1, y - 1].site != Site.Black &&
                        figures[x + 1, y - 1].condition == Condition.NotFigure)
                        AvailablePoints.Add(new Point(CurrPosition.X + 100, CurrPosition.Y - 100));
                    if (figures[x, y - 1].site != Site.Black && figures[x, y - 1].condition == Condition.NotFigure)
                        AvailablePoints.Add(new Point(CurrPosition.X, CurrPosition.Y - 100));
                }

                if (figures[x = 7, y = 7].site != Site.Black)
                {
                    if (figures[x - 1, y].site != Site.Black && figures[x - 1, y].condition == Condition.NotFigure)
                        AvailablePoints.Add(new Point(CurrPosition.X - 100, CurrPosition.Y));
                    if (figures[x - 1, y - 1].site != Site.Black &&
                        figures[x - 1, y - 1].condition == Condition.NotFigure)
                        AvailablePoints.Add(new Point(CurrPosition.X - 100, CurrPosition.Y - 100));
                    if (figures[x, y - 1].site != Site.Black && figures[x, y - 1].condition == Condition.NotFigure)
                        AvailablePoints.Add(new Point(CurrPosition.X, CurrPosition.Y - 100));
                }
            }

            if (site == Site.White)
            {
                for (var a = 1; a <= 6; a++)
                    if (x == a && y == 0)
                    {
                        if (figures[x - 1, y].site != Site.White && figures[x - 1, y].condition == Condition.NotFigure)
                            AvailablePoints.Add(new Point(CurrPosition.X - 100, CurrPosition.Y));
                        if (figures[x + 1, y].site != Site.White && figures[x + 1, y].condition == Condition.NotFigure)
                            AvailablePoints.Add(new Point(CurrPosition.X + 100, CurrPosition.Y));
                        if (figures[x, y + 1].site != Site.White && figures[x, y + 1].condition == Condition.NotFigure)
                            AvailablePoints.Add(new Point(CurrPosition.X, CurrPosition.Y + 100));
                        if (figures[x - 1, y + 1].site != Site.White &&
                            figures[x - 1, y + 1].condition == Condition.NotFigure)
                            AvailablePoints.Add(new Point(CurrPosition.X - 100, CurrPosition.Y + 100));
                        if (figures[x + 1, y + 1].site != Site.White &&
                            figures[x + 1, y + 1].condition == Condition.NotFigure)
                            AvailablePoints.Add(new Point(CurrPosition.X + 100, CurrPosition.Y + 100));
                    }

                for (var a = 1; a <= 6; a++)
                    if (x == 0 && y == a)
                    {
                        if (figures[x, y - 1].site != Site.White && figures[x, y - 1].condition == Condition.NotFigure)
                            AvailablePoints.Add(new Point(CurrPosition.X, CurrPosition.Y - 100));
                        if (figures[x, y + 1].site != Site.White && figures[x, y + 1].condition == Condition.NotFigure)
                            AvailablePoints.Add(new Point(CurrPosition.X, CurrPosition.Y + 100));
                        if (figures[x + 1, y - 1].site != Site.White &&
                            figures[x + 1, y - 1].condition == Condition.NotFigure)
                            AvailablePoints.Add(new Point(CurrPosition.X + 100, CurrPosition.Y -
                                                                                100));
                        if (figures[x + 1, y + 1].site != Site.White &&
                            figures[x + 1, y + 1].condition == Condition.NotFigure)
                            AvailablePoints.Add(new Point(CurrPosition.X + 100, CurrPosition.Y + 100));
                        if (figures[x + 1, y].site != Site.White && figures[x + 1, y].condition == Condition.NotFigure)
                            AvailablePoints.Add(new Point(CurrPosition.X + 100, CurrPosition.Y));
                    }

                for (var a = 1; a <= 6; a++)
                    if (x == a && y == 7)
                    {
                        if (figures[x - 1, y].site != Site.White && figures[x - 1, y].condition == Condition.NotFigure)
                            AvailablePoints.Add(new Point(CurrPosition.X - 100, CurrPosition.Y));
                        if (figures[x + 1, y].site != Site.White && figures[x + 1, y].condition == Condition.NotFigure)
                            AvailablePoints.Add(new Point(CurrPosition.X + 100, CurrPosition.Y));
                        if (figures[x, y - 1].site != Site.White && figures[x, y - 1].condition == Condition.NotFigure)
                            AvailablePoints.Add(new Point(CurrPosition.X, CurrPosition.Y - 100));
                        if (figures[x - 1, y - 1].site != Site.White &&
                            figures[x - 1, y - 1].condition == Condition.NotFigure)
                            AvailablePoints.Add(new Point(CurrPosition.X - 100, CurrPosition.Y - 100));
                        if (figures[x + 1, y - 1].site != Site.White &&
                            figures[x + 1, y - 1].condition == Condition.NotFigure)
                            AvailablePoints.Add(new Point(CurrPosition.X + 100, CurrPosition.Y - 100));
                    }

                for (var a = 1; a <= 6; a++)
                    if (x == 7 && y == a)
                    {
                        if (figures[x, y - 1].site != Site.White && figures[x, y - 1].condition == Condition.NotFigure)
                            AvailablePoints.Add(new Point(CurrPosition.X, CurrPosition.Y - 100));
                        if (figures[x, y + 1].site != Site.White && figures[x, y + 1].condition == Condition.NotFigure)
                            AvailablePoints.Add(new Point(CurrPosition.X, CurrPosition.Y + 100));
                        if (figures[x - 1, y - 1].site != Site.White &&
                            figures[x - 1, y - 1].condition == Condition.NotFigure)
                            AvailablePoints.Add(new Point(CurrPosition.X - 100, CurrPosition.Y - 100));
                        if (figures[x - 1, y + 1].site != Site.White &&
                            figures[x - 1, y + 1].condition == Condition.NotFigure)
                            AvailablePoints.Add(new Point(CurrPosition.X - 100, CurrPosition.Y + 100));
                        if (figures[x - 1, y].site != Site.White && figures[x - 1, y].condition == Condition.NotFigure)
                            AvailablePoints.Add(new Point(CurrPosition.X - 100, CurrPosition.Y));
                    }

                if (figures[x = 0, y = 0].site != Site.White)
                {
                    if (figures[x + 1, y].site != Site.White && figures[x + 1, y].condition == Condition.NotFigure)
                        AvailablePoints.Add(new Point(CurrPosition.X + 100, CurrPosition.Y));
                    if (figures[x + 1, y + 1].site != Site.White &&
                        figures[x + 1, y + 1].condition == Condition.NotFigure)
                        AvailablePoints.Add(new Point(CurrPosition.X + 100, CurrPosition.Y + 100));
                    if (figures[x, y + 1].site != Site.White && figures[x, y + 1].condition == Condition.NotFigure)
                        AvailablePoints.Add(new Point(CurrPosition.X, CurrPosition.Y + 100));
                }

                if (figures[x = 7, y = 0].site != Site.White)
                {
                    if (figures[x - 1, y].site != Site.White && figures[x - 1, y].condition == Condition.NotFigure)
                        AvailablePoints.Add(new Point(CurrPosition.X - 100, CurrPosition.Y));
                    if (figures[x - 1, y + 1].site != Site.White &&
                        figures[x - 1, y + 1].condition == Condition.NotFigure)
                        AvailablePoints.Add(new Point(CurrPosition.X - 100, CurrPosition.Y + 100));
                    if (figures[x, y + 1].site != Site.White && figures[x, y + 1].condition == Condition.NotFigure)
                        AvailablePoints.Add(new Point(CurrPosition.X, CurrPosition.Y + 100));
                }

                if (figures[x = 0, y = 7].site != Site.White)
                {
                    if (figures[x + 1, y].site != Site.White && figures[x + 1, y].condition == Condition.NotFigure)
                        AvailablePoints.Add(new Point(CurrPosition.X + 100, CurrPosition.Y));
                    if (figures[x + 1, y - 1].site != Site.White &&
                        figures[x + 1, y - 1].condition == Condition.NotFigure)
                        AvailablePoints.Add(new Point(CurrPosition.X + 100, CurrPosition.Y - 100));
                    if (figures[x, y - 1].site != Site.White && figures[x, y - 1].condition == Condition.NotFigure)
                        AvailablePoints.Add(new Point(CurrPosition.X, CurrPosition.Y - 100));
                }

                if (figures[x = 7, y = 7].site != Site.White)
                {
                    if (figures[x - 1, y].site != Site.White && figures[x - 1, y].condition == Condition.NotFigure)
                        AvailablePoints.Add(new Point(CurrPosition.X - 100, CurrPosition.Y));
                    if (figures[x - 1, y - 1].site != Site.White &&
                        figures[x - 1, y - 1].condition == Condition.NotFigure)
                        AvailablePoints.Add(new Point(CurrPosition.X - 100, CurrPosition.Y - 100));
                    if (figures[x, y - 1].site != Site.White && figures[x, y - 1].condition == Condition.NotFigure)
                        AvailablePoints.Add(new Point(CurrPosition.X, CurrPosition.Y - 100));
                }
            }
        }

        public void Image_Click(object sender, EventArgs e)
        {
            if (Controller.IsWhiteTurn && site == Site.White)
            {
                foreach (var element in Controller.FieldOfFigures) element.UpdateList(Controller.FieldOfFigures);
                var rnd = new Random().Next(AvailablePoints.Count);
                if (AvailablePoints.Count != 0)
                {
                    var x = (CurrPosition.X - 550) / 100;
                    var y = (CurrPosition.Y - 130) / 100;
                    var newX = (AvailablePoints[rnd].X - 550) / 100;
                    var newY = (AvailablePoints[rnd].Y - 130) / 100;
                    if (Controller.FieldOfFigures[newX, newY].condition != Condition.NotFigure)
                        Controller.form.Controls.Remove(Controller.FieldOfFigures[newX, newY].image);
                    Controller.form.Controls.Remove(image);
                    Controller.FieldOfFigures[newX, newY] = new King(site = site, AvailablePoints[rnd]);
                    Controller.form.Controls.Add(Controller.FieldOfFigures[newX, newY].image);
                    Controller.FieldOfFigures[x, y] = new Figure {condition = Condition.NotFigure};
                    Controller.IsWhiteTurn = false;
                }
            }
            else
            {
                if (Controller.IsWhiteTurn == false && site == Site.Black)
                {
                    foreach (var element in Controller.FieldOfFigures) element.UpdateList(Controller.FieldOfFigures);
                    var rnd = new Random().Next(AvailablePoints.Count);
                    if (AvailablePoints.Count != 0)
                    {
                        var x = (CurrPosition.X - 550) / 100;
                        var y = (CurrPosition.Y - 130) / 100;
                        var newX = (AvailablePoints[rnd].X - 550) / 100;
                        var newY = (AvailablePoints[rnd].Y - 130) / 100;
                        if (Controller.FieldOfFigures[newX, newY].condition != Condition.NotFigure)
                            Controller.form.Controls.Remove(Controller.FieldOfFigures[newX, newY].image);
                        Controller.form.Controls.Remove(image);
                        Controller.FieldOfFigures[newX, newY] = new King(site = site, AvailablePoints[rnd]);
                        Controller.form.Controls.Add(Controller.FieldOfFigures[newX, newY].image);
                        Controller.FieldOfFigures[x, y] = new Figure {condition = Condition.NotFigure};
                        Controller.IsWhiteTurn = true;
                    }
                }
            }
        }
    }

    public class Figure
    {
        public List<Point> AvailablePoints = new List<Point>();
        public Button image;
        public Site site { get; set; }
        public Condition condition { get; set; }
        public Point CurrPosition { get; set; }

        public virtual void UpdateList(Figure[,] figures)
        {
        }
    }
}