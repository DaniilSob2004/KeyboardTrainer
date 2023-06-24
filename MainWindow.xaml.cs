using System;
using System.Collections.Generic;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace KeyboardTrainer
{
    public partial class MainWindow : Window
    {
        static int MaxSeconds = 30;
        static string symbols = "QWERTYUIOPASDFGHJKLZXCVBNM[];'`1234567890-+./";  // символы которые рандомно раставляются
        static Random r = new Random();
        static Timer t = new Timer();
        static Brush brush = new SolidColorBrush(Color.FromArgb(0xFF, 0xDD, 0xDD, 0xDD));  // цвет кнопки по умолчанию

        bool isEnd = false;  // закончился ли ввод символов
        string nameKey = "";  // название нажатой кнопки
        string lastClick = "";  // название последней нажатой кнопки
        int indInputSymbols = 0;  // индекс символа который сейчас вводит пользователь
        int amountWords = 0;  // кол-во символов
        int amountFails = 0;  // кол-во фейлов
        int totalSec = 0;  // кол-во секунд ввода пользователя
       
        List<Button> buttons = new List<Button>();  // список всех кнопок клавиатуры
        List<Button> clickBtns = new List<Button>();  // список нажатых кнопок


        public MainWindow()
        {
            InitializeComponent();
            GetAllButtons();
            StartSettingsTimer();
        }


        private void StartSettingsTimer()
        {
            t.Interval = 1000;
            t.Elapsed += T_Elapsed;
        }

        private void T_Elapsed(object? sender, ElapsedEventArgs e)
        {
            if ((totalSec++) >= MaxSeconds - 1)  // если прошло MaxSeconds секунд
            {
                CalculateSymbolsPerMinute();  // считаем кол-во символов в минуту
                UserMessage("Время вышло(");  // выводим сообщение
            }
        }


        private void GetAllButtons()
        {
            object obj = FindName("allBtn");
            try
            {
                if (obj != null)
                {
                    foreach (UIElement panel in ((Grid)obj).Children)  // получаем общий контейнер Grid
                    {
                        foreach (UIElement grid in ((StackPanel)panel).Children)  // получаем StackPanel в котором находится один Grid
                        {
                            foreach (UIElement btn in ((Grid)grid).Children)  // получаем Grid в котором находятся кнопки
                            {
                                buttons.Add((Button)btn);  // получаем кнопку, и добавляем в список
                            }
                        }
                    }
                }
            }
            catch (Exception) { }
        }

        private bool GetNameKey(KeyEventArgs e)
        {
            // получаем название кнопки и если необходимо, меняем(настраиваем) под нужный контент
            nameKey = e.Key + "";

            if (nameKey == "Back") nameKey = "Backspace";
            else if (nameKey == "Capital") nameKey = "Caps Lock";
            else if (nameKey == "Oem3") nameKey = "`";
            else if (nameKey == "OemMinus") nameKey = "-";
            else if (nameKey == "OemPlus") nameKey = "+";
            else if (nameKey == "Oem6") nameKey = "]";
            else if (nameKey == "Oem1") nameKey = ";";
            else if (nameKey == "OemOpenBrackets") nameKey = "[";
            else if (nameKey == "OemQuotes") nameKey = "'";
            else if (nameKey == "OemQuestion") nameKey = "/";
            else if (nameKey == "OemComma") nameKey = ",";
            else if (nameKey == "OemPeriod") nameKey = ".";
            else if (nameKey == "LWin") nameKey = "Win";
            else if (nameKey == "Return") nameKey = "Enter";
            else if (nameKey.Length == 2 && nameKey[0] == 'D') nameKey = nameKey.Replace("D", "");
            else if (nameKey == "LeftCtrl")
            {
                ClickBtn((Button)FindName("leftCtrl"));
                return false;
            }
            else if (nameKey == "RightCtrl")
            {
                ClickBtn((Button)FindName("rightCtrl"));
                return false;
            }
            else if (nameKey == "RightShift")
            {
                ClickBtn((Button)FindName("rightShift"));
                return false;
            }
            else if (nameKey == "LeftShift")
            {
                ClickBtn((Button)FindName("leftShift"));
                return false;
            }
            else if (Keyboard.IsKeyDown(Key.LeftAlt))
            {
                ClickBtn((Button)FindName("leftAlt"));
                return false;
            }
            else if (Keyboard.IsKeyDown(Key.RightAlt))
            {
                ClickBtn((Button)FindName("rightAlt"));
                return false;
            }
            return true;  // если true, значит будем искать подходящую кнопку, иначе кнопка уже найдена
        }


        private void ClickBtn(Button btn)
        {
            lastClick = btn.Content + "";  // запоминаем название
            clickBtns.Add(btn);  // добавляем в список нажатых кнопок
            btn.Background = Brushes.Cyan;  // меняем цвет (для эффекта нажатой кнопки)
        }

        private void UserInput()
        {
            try
            {
                WrapPanel panelBtn = (WrapPanel)FindName("panelBtn");  // находим контейнер, где находятся кнопки для ввода текста пользователем
                Button b = (Button)panelBtn.Children[indInputSymbols];  // получаем кнопку, которую должен ввести пользователь

                if (nameKey == b.Content.ToString())  // если правильно ввёл символ
                {
                    indInputSymbols++;  // двигаем индекс, для доступа к след символу
                    b.Background = Brushes.Green;
                    if (indInputSymbols == panelBtn.Children.Count)  // если это последний символ
                    {
                        isEnd = true;  // для того чтобы изменить эффект последней нажатой кнопки
                        CalculateSymbolsPerMinute();  // считаем кол-во символов в минуту
                    }
                }
                else  // если не правильно
                {
                    UpdateFails(++amountFails);  // добавляем +1 к фейлу
                }
            }
            catch (Exception) {}
        }

        private void CreateBtn(string text)
        {
            try
            {
                Button btn = new Button() { Content = text, Padding = new Thickness(8) };
                ((WrapPanel)FindName("panelBtn")).Children.Add(btn);
            }
            catch (Exception) {}
        }

        private void GenerateText()
        {
            int rInd;
            for (int i = 0; i < amountWords; i++)
            {
                rInd = r.Next(0, symbols.Length);
                CreateBtn(symbols[rInd].ToString());  // создаём кнопку которую должен вводить пользователь
            }
            indInputSymbols = 0;
        }

        private void ClearPanel()
        {
            // для выполнения кода обновления пользовательского интерфейса на UI-потоке
            Application.Current.Dispatcher.Invoke(() =>
            {
                // обновление интерфейса и обнуление настроек
                try
                {
                    t.Stop();
                    ((WrapPanel)FindName("panelBtn")).Children.Clear();  // удаляем все кнопки для ввода
                    UpdateFails(0);  // обнуление
                    indInputSymbols = totalSec = 0;
                    isEnd = false;
                }
                catch (Exception) {}
            });
        }

        private void UserMessage(string message)
        {
            ClearPanel();  // очищаем панель
            MessageBox.Show(message, "Message", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void UpdateFails(int value)
        {
            try
            {
                // обновляем данные о фейлах
                ((TextBlock)FindName("amountFail")).Text = $"{value}";
            }
            catch (Exception) {}
        }

        private void CalculateSymbolsPerMinute()
        {
            // для выполнения кода обновления пользовательского интерфейса на UI-потоке
            Application.Current.Dispatcher.Invoke(() =>
            {
                try
                {
                    // обновление интерфейса
                    float value = (float)((60 / (float)totalSec) * (indInputSymbols));  // подсчёт кол-ва символов в минуту
                    ((TextBlock)FindName("amountSybmPerSec")).Text = $"{(int)value}";
                }
                catch (Exception) {}
            });
        }


        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (GetNameKey(e))  // получаем название кнопки
            {
                if (lastClick == nameKey) return;  // если есть залипание (кнопка постоянно нажата), то выходим
                foreach (Button btn in buttons)
                {
                    if (btn.Content.ToString() == nameKey)  // если нашли кнопку
                    {
                        ClickBtn(btn);
                        UserInput();
                    }
                }
            }
        }

        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            if (isEnd)  // если пользователь правильно ввёл все символы
                UserMessage("Поздравляем, вы справились!)");

            foreach (var btn in clickBtns) btn.Background = brush;  // возвращаем цвета отпущенных кнопок
            clickBtns.Clear();  // очищаем
            lastClick = "";
        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            try
            {
                // обновляем данные о кол-ве генерации символов
                ((TextBlock)FindName("amountSybms")).Text = $"{amountWords = (int)e.NewValue}";
            }
            catch (Exception) {}
        }

        private void ButtonStart_Click(object sender, RoutedEventArgs e)
        {
            if (!t.Enabled)
            {
                GenerateText();
                ((Button)sender).Focusable = false;  // убираем фокус с кнопки
                t.Start();
            }
        }

        private void ButtonClose_Click(object sender, RoutedEventArgs e)
        {
            if (t.Enabled)
            {
                t.Stop();
                ClearPanel();
                ((Button)sender).Focusable = false;  // убираем фокус с кнопки
            }
        }
    }
}
