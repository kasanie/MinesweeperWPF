using System;
using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace MinesweeperWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            InitializeField();
        }
        BitmapImage imgMine = new BitmapImage(new Uri(@"Images/Mine.png", UriKind.Relative));
        BitmapImage imgSign = new BitmapImage(new Uri(@"Images/Sign.jpg", UriKind.Relative));
        const int fieldSize = 15; //размеры квадратного поля
        const int minesCount = 40; //количество мин
        static int stepCount = 0;
        BitArray fieldArray = new BitArray(fieldSize * fieldSize);
        Button[] buttonArray = new Button[fieldSize * fieldSize];
        Random rnd = new Random();

        void InitializeField(bool stop = false, int count = 0)
        {
            for (int i = 0; i < fieldArray.Length; i++)
            {
                if (count == minesCount)
                {
                    break;
                }
                if (fieldArray[i] == false)
                {
                    fieldArray[i] = rnd.Next(0, 100) > 79;
                }
                else continue;
                if (fieldArray[i])
                {
                    count++;
                }
            }

            if (count < minesCount) InitializeField(true, count);
            if (stop) return;

            this.Width = (playField.Width = 23 * fieldSize) + 30;
            this.Height = (playField.Height = 23 * fieldSize) + 60;
            CreateButton(fieldSize * fieldSize);

        }

        void CreateButton(int count)
        {
            for (int i = 0; i < count; i++)
            {
                Button button = new Button()
                {
                    Width = 23,
                    Height = 23,
                    Tag = i
                };
                // if (fieldArray[i]) button.Content = i; //Проверка расположения мин
                button.Click += new RoutedEventHandler(b_Click);
                button.MouseRightButtonUp += new MouseButtonEventHandler(b_MouseRightButtonUp);
                playField.Children.Add(button);
                buttonArray[i] = button;
            }
        }

        void b_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            Button button = (Button)sender;
            int index = (int)button.Tag;
            if (button.Content == null) SetButton(button, TypeButton.Sign);
            else button.Content = null;
        }

        void b_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            int index = (int)button.Tag;
            if (fieldArray[index])
            {
                for (int i = 0; i < fieldArray.Length; i++)
                    if (fieldArray[i]) SetButton(buttonArray[i], TypeButton.Mine);
                MessageBox.Show("You lost");
            }
            else
            {
                Step(index);
                if (stepCount == fieldSize * fieldSize - minesCount)
                    MessageBox.Show("You win");
            }
        }

        void Step(int index)
        {
            int[] t = new int[8];
            int row = index / fieldSize;
            int column = row * fieldSize - index;

            t[0] = fieldSize * (row - 1) + (index % fieldSize);
            t[1] = fieldSize * (row + 1) + (index % fieldSize);
            t[2] = t[0] + 1;
            t[3] = t[0] - 1;
            t[4] = t[1] + 1;
            t[5] = t[1] - 1;
            t[6] = index + 1;
            t[7] = index - 1;

            int count = 0;
            for (int i = 0; i < t.Length; i++)
                if (ContainsIndex(t[i], index, row, column))
                    if (fieldArray[t[i]])
                        count++;

            if (count == 0)
            {
                buttonArray[index].IsEnabled = false;
                stepCount += 1;
                for (int i = 0; i < t.Length; i++)
                    if (ContainsIndex(t[i], index, row, column))
                        if (buttonArray[t[i]].IsEnabled)
                            Step(t[i]);
            }
            else
            {
                SetButton(buttonArray[index], TypeButton.Number, count);
                buttonArray[index].Foreground = Brushes.Blue;
                stepCount += 1;
            }
        }

        bool ContainsIndex(int index, int baseInd, int baseRow, int baseCol)
        {
            if (index >= 0 && index < fieldArray.Length)
            {
                int row = index / fieldSize;
                int column = row * fieldSize - index;
                if (Math.Abs(baseRow - row) > 1) return false;
                if (Math.Abs(baseCol - column) > 1) return false;
                return true;
            }
            else return false;
        }

        void SetButton(Button button, TypeButton type, int number = 0)
        {
            BitmapImage bitmap = new BitmapImage();

            if (type == TypeButton.None)
            {
                button.IsEnabled = false;
                return;
            }

            if (type == TypeButton.Mine)
            {
                bitmap = imgMine;
                button.IsEnabled = false;
            }

            if (type == TypeButton.Sign)
                bitmap = imgSign;

            if (type != TypeButton.Number)
            {
                Image img = new Image() { Source = bitmap };
                Grid grid = new Grid();
                grid.Children.Add(img);
                button.Content = grid;
            }

            if (type == TypeButton.Number)
            {
                button.Content = number;
                button.IsEnabled = false;
            }
        }
        public enum TypeButton
        {
            Mine,
            Sign,
            Number,
            None
        }

    }
}
