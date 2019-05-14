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
                    if (figures[x, y + 2].condition == Condition.NotFigure)
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
                    if (figures[x, y - 2].condition == Condition.NotFigure)
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
                        Controller.FieldOfFigures[newX, newY] = new Soldier(site = site, AvailablePoints[rnd]);
                        Controller.form.Controls.Add(Controller.FieldOfFigures[newX, newY].image);
                        Controller.FieldOfFigures[x, y] = new Figure { condition = Condition.NotFigure };
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
        }

        public override void UpdateList(Figure[,] figures)
        {
            var x = (CurrPosition.X - 550) / 100;
            var y = (CurrPosition.Y - 130) / 100;

            if (y == 0) //для самой верхней строки
            {
                if (x == 0)
                {
                    AvailablePoints.Add(new Point(2,1));
                    AvailablePoints.Add(new Point(1,2));
                }

                if (x == 7)
                {
                    AvailablePoints.Add(new Point(5, 1));
                    AvailablePoints.Add(new Point(6, 2));
                }

                if (x == 1)
                {
                    AvailablePoints.Add(new Point(0,2));
                    AvailablePoints.Add(new Point(2, 2));
                    AvailablePoints.Add(new Point(3, 1));
                }

                if (x == 6)
                {
                    AvailablePoints.Add(new Point(7, 2));
                    AvailablePoints.Add(new Point(5, 2));
                    AvailablePoints.Add(new Point(4, 1));
                }

                if (x != 0 && x != 1 && x != 6 && x != 7)
                {
                    AvailablePoints.Add(new Point(x-2, y+1));
                    AvailablePoints.Add(new Point(x+2, y+1));
                    AvailablePoints.Add(new Point(x+1, y+2));
                    AvailablePoints.Add(new Point(x-1, y+2));
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
        }
    }

    public class Figure
    {
        public Site site { get; set; }
        public Condition condition { get; set; }
        public Point CurrPosition { get; set; }
        public List<Point> AvailablePoints = new List<Point>();
        public Button image;

        public virtual void UpdateList(Figure[,] figures)
        {
        }
    }
}