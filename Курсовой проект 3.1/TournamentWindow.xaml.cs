using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Курсовой_проект_3._1
{
    /// <summary>
    /// Логика взаимодействия для TournamentWindow.xaml
    /// </summary>
    public partial class TournamentWindow : Window
    {
        public TournamentWindow()
        {
            InitializeComponent();
            MainGrid.Children.Add(GenerateSingleEliminationBracket(16));
        }

        public Grid GenerateSingleEliminationBracket(int teamCount)
        {
            Grid grid = new Grid();
            grid.ShowGridLines = false;

            // Создание Grid'ов ///
            // Высчитывание размеров сетки
            int roundCount = (int)Math.Log(teamCount, 2);  // количество раундов
            int gridColumnCount = roundCount + roundCount - 1 + 2;  // количество колонок в grid
            int gridRowCount = teamCount + (teamCount - 2) + 4;  // количество строк в grid

            // Добавление колонок
            for (int i = 0; i < gridColumnCount; i++)
            {
                grid.ColumnDefinitions.Add(new ColumnDefinition());
            }

            // Добавление строк
            for (int j = 0; j < gridRowCount; j++)
            {
                grid.RowDefinitions.Add(new RowDefinition());
            }

            // Всё остальное
            int roundNum = 1;  // номер раунда
            int skipStart = 2;  // сколько пропустить строк с начала
            int skipBetween = 2;  // сколько пропускать строк между играми

            // Наполнение сетки
            for (int i = 1; i < gridColumnCount - 1; i++)
            {
                if (i % 2 != 0)  // раунды через одну колонку
                {
                    int counter = 0;
                    for (int j = skipStart; j < gridRowCount - 2; j++)
                    {
                        // создание grid'a с двумя колонками разной ширины (НАЗВАНИЕ КОМАНДЫ и ОЧКИ)
                        Grid gridChild = new Grid();

                        ColumnDefinition wideColumn = new ColumnDefinition();
                        wideColumn.Width = new GridLength(0.8, GridUnitType.Star);
                        ColumnDefinition smallColumn = new ColumnDefinition();
                        smallColumn.Width = new GridLength(0.2, GridUnitType.Star);

                        gridChild.ColumnDefinitions.Add(wideColumn);
                        gridChild.ColumnDefinitions.Add(smallColumn);

                        // текст бокс с названием команды
                        TextBox tbName = new TextBox();
                        Grid.SetColumn(tbName, 0);
                        gridChild.Children.Add(tbName);

                        // текст бокс со счетом
                        TextBox tbScore = new TextBox();
                        tbScore.MaxLength = 2;
                        tbScore.HorizontalContentAlignment = HorizontalAlignment.Center;
                        Grid.SetColumn(tbScore, 1);
                        gridChild.Children.Add(tbScore);

                        // добавление мини-grid'a к большому папочке
                        Grid.SetColumn(gridChild, i);
                        Grid.SetRow(gridChild, j);
                        grid.Children.Add(gridChild);

                        // после каждыой игры нужен пропуск (skipBetween) до следующей игры
                        counter++;
                        if (counter == 2)
                        {
                            j += skipBetween;
                            counter = 0;
                        }
                    }
                    // пересчет значений для следующего раунда
                    roundNum++;
                    int gamesInRoundCount = (int)(teamCount / Math.Pow(2, roundNum));
                    skipStart *= 2;
                    skipBetween = roundNum == roundCount ? skipStart : (gridRowCount - (skipStart * 2) - (gamesInRoundCount * 2)) / (gamesInRoundCount - 1);
                }
                else  // соединяющие линии между раундами
                {
                    if (i == 2)
                    {
                        for (int j = 4; j < gridRowCount; j++)
                        {
                            TextBox linetb1 = new TextBox();
                            linetb1.BorderBrush = Brushes.Black;
                            linetb1.IsReadOnly = true;
                            var bc = new BrushConverter();
                            linetb1.Background = (Brush)bc.ConvertFrom("#fffff0");
                            linetb1.BorderThickness = new Thickness(2, 0, 0, 2);

                            Grid.SetColumn(linetb1, i);
                            Grid.SetRow(linetb1, j);
                            grid.Children.Add(linetb1);

                            TextBox linetb2 = new TextBox();
                            linetb2.BorderBrush = Brushes.Black;
                            linetb2.IsReadOnly = true;
                            linetb2.Background = (Brush)bc.ConvertFrom("#fffff0");
                            linetb2.BorderThickness = new Thickness(2, 2, 0, 0);

                            Grid.SetColumn(linetb2, i);
                            Grid.SetRow(linetb2, j + 1);
                            grid.Children.Add(linetb2);

                            j += 7;
                        }
                    }
                    else
                    {
                        int drawStatus = 0; // 0 - не рисовать, 1 - leftbottom, 2 - lefttop, 3 - leftonly
                        int leftCounter = 0;
                        int rightCounter = 0;

                        for (int j = 4; j < gridRowCount; j++)
                        {
                            // тут рисовка
                            if (drawStatus == 3)
                            {
                                TextBox linetb3 = new TextBox();
                                linetb3.BorderBrush = Brushes.Black;
                                linetb3.IsReadOnly = true;
                                var bc = new BrushConverter();
                                linetb3.Background = (Brush)bc.ConvertFrom("#fffff0");
                                linetb3.BorderThickness = new Thickness(2, 0, 0, 0);

                                Grid.SetColumn(linetb3, i);
                                Grid.SetRow(linetb3, j);
                                grid.Children.Add(linetb3);
                            }
                            else if (drawStatus == 2)
                            {
                                TextBox linetb2 = new TextBox();
                                linetb2.BorderBrush = Brushes.Black;
                                linetb2.IsReadOnly = true;
                                var bc = new BrushConverter();
                                linetb2.Background = (Brush)bc.ConvertFrom("#fffff0");
                                linetb2.BorderThickness = new Thickness(2, 2, 0, 0);

                                Grid.SetColumn(linetb2, i);
                                Grid.SetRow(linetb2, j - 1);
                                grid.Children.Add(linetb2);
                            }
                            else if (drawStatus == 1)
                            {
                                TextBox linetb1 = new TextBox();
                                linetb1.BorderBrush = Brushes.Black;
                                linetb1.IsReadOnly = true;
                                var bc = new BrushConverter();
                                linetb1.Background = (Brush)bc.ConvertFrom("#fffff0");
                                linetb1.BorderThickness = new Thickness(2, 0, 0, 2);

                                Grid.SetColumn(linetb1, i);
                                Grid.SetRow(linetb1, j - 1);
                                grid.Children.Add(linetb1);

                                drawStatus = 3;
                            }

                            // тут проверка
                            if (GetElementInGridPosition(i - 1, j, grid) is Grid)
                            {
                                leftCounter++;
                                drawStatus = 0;

                                if (leftCounter == 2)
                                {
                                    drawStatus = 3;
                                    leftCounter = 0;
                                }
                            }
                            else if (GetElementInGridPosition(i + 1, j, grid) is Grid)
                            {
                                rightCounter++;

                                if (rightCounter == 1)
                                {
                                    drawStatus = 1;
                                }
                                else
                                {
                                    drawStatus = 2;
                                    rightCounter = 0;
                                }
                            }
                        }
                    }
                }
            }
            return grid;
        }

        private UIElement GetElementInGridPosition(int column, int row, Grid RootGrid)
        {
            foreach (UIElement element in RootGrid.Children)
            {
                if (Grid.GetColumn(element) == column && Grid.GetRow(element) == row)
                    return element;
            }

            return null;
        }
    }
}
