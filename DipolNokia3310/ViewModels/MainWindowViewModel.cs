using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia.Threading;

namespace DipolNokia3310.ViewModels
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private string _displayText = string.Empty;
        private string _resultText = string.Empty;
        private string _inputSequence = string.Empty;

        private readonly Dictionary<string, string[]> _keyMappings;

        // Последнее нажатие и таймер
        private string _lastPressedKey = null;
        private DateTime _lastPressTime = DateTime.MinValue;
        private Dictionary<string, int> _pressCount;
        private readonly DispatcherTimer _timeoutTimer;

        public MainWindowViewModel()
        {
            // Инициализируем маппинг клавиш
            _keyMappings = new Dictionary<string, string[]>
            {
                {"1", new[] {"1"}},
                {"2", new[] {"A", "B", "C", "2"}},
                {"3", new[] {"D", "E", "F", "3"}},
                {"4", new[] {"G", "H", "I", "4"}},
                {"5", new[] {"J", "K", "L", "5"}},
                {"6", new[] {"M", "N", "O", "6"}},
                {"7", new[] {"P", "Q", "R", "S", "7"}},
                {"8", new[] {"T", "U", "V", "8"}},
                {"9", new[] {"W", "X", "Y", "Z", "9"}},
                {"0", new[] {" ", "0"}},
                {"*", new[] {"*"}}
            };

            // Счетчик нажатий для каждой клавиши
            _pressCount = new Dictionary<string, int>();
            foreach (var key in _keyMappings.Keys)
            {
                _pressCount[key] = 0;
            }

            // Инициализируем таймер
            _timeoutTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(1000)
            };
            _timeoutTimer.Tick += (s, e) =>
            {
                _lastPressedKey = null;
                _timeoutTimer.Stop();
            };

            // Создаем команды
            KeyPressCommand = new DelegateCommand<string>(OnKeyPress);
            ClearCommand = new DelegateCommand(OnClear);
            SendCommand = new DelegateCommand(OnSend);
            BackspaceCommand = new DelegateCommand(OnBackspace);
        }

        // Свойства с уведомлением об изменении
        public string DisplayText
        {
            get => _displayText;
            set
            {
                if (_displayText != value)
                {
                    _displayText = value;
                    OnPropertyChanged();
                }
            }
        }

        public string ResultText
        {
            get => _resultText;
            set
            {
                if (_resultText != value)
                {
                    _resultText = value;
                    OnPropertyChanged();
                }
            }
        }

        public string InputSequence
        {
            get => _inputSequence;
            set
            {
                if (_inputSequence != value)
                {
                    _inputSequence = value;
                    OnPropertyChanged();
                }
            }
        }

        // Команды
        public ICommand KeyPressCommand { get; }
        public ICommand ClearCommand { get; }
        public ICommand SendCommand { get; }
        public ICommand BackspaceCommand { get; }

        // Обработка нажатия клавиши
        private void OnKeyPress(string key)
        {
            // Проверяем, есть ли клавиша в словаре
            if (!_keyMappings.ContainsKey(key))
                return;

            // Текущее время
            DateTime now = DateTime.Now;

            // Если нажата та же клавиша и прошло меньше 1 секунды с последнего нажатия
            if (key == _lastPressedKey && (now - _lastPressTime).TotalMilliseconds < 1000)
            {
                // Увеличиваем счетчик нажатий
                _pressCount[key]++;

                // Вычисляем индекс символа в массиве возможных символов
                int charIndex = (_pressCount[key] - 1) % _keyMappings[key].Length;

                // Удаляем последний символ и добавляем новый
                Dispatcher.UIThread.Post(() =>
                {
                    if (DisplayText.Length > 0)
                        DisplayText = DisplayText.Substring(0, DisplayText.Length - 1);

                    DisplayText += _keyMappings[key][charIndex];
                });
            }
            else
            {
                // Новая клавиша или прошло больше 1 секунды
                _pressCount[key] = 1;

                // Добавляем первый символ из возможных для этой клавиши
                Dispatcher.UIThread.Post(() =>
                {
                    DisplayText += _keyMappings[key][0];
                    InputSequence += key;
                });
            }

            // Обновляем последнюю нажатую клавишу и время
            _lastPressedKey = key;
            _lastPressTime = now;

            // Перезапускаем таймер
            Dispatcher.UIThread.Post(() =>
            {
                _timeoutTimer.Stop();
                _timeoutTimer.Start();
            });
        }

        // Очистка всех полей
        private void OnClear()
        {
            // Выполняем операцию в UI-потоке через Dispatcher
            Dispatcher.UIThread.Post(() =>
            {
                // Очищаем все текстовые поля
                DisplayText = string.Empty;
                InputSequence = string.Empty;
                ResultText = string.Empty;

                // Сбрасываем состояние ввода
                _lastPressedKey = null;
                _lastPressTime = DateTime.MinValue;
                _timeoutTimer.Stop();

                // Сбрасываем счетчики нажатий для всех клавиш
                foreach (var key in _keyMappings.Keys)
                {
                    _pressCount[key] = 0;
                }
            });
        }

        // Удаление последнего символа
        private void OnBackspace()
        {
            Dispatcher.UIThread.Post(() =>
            {
                if (DisplayText.Length > 0)
                {
                    DisplayText = DisplayText.Substring(0, DisplayText.Length - 1);

                    if (InputSequence.Length > 0)
                    {
                        InputSequence = InputSequence.Substring(0, InputSequence.Length - 1);
                    }
                }
            });
        }

        // Отправка текста
        private void OnSend()
        {
            Dispatcher.UIThread.Post(() =>
            {
                if (string.IsNullOrEmpty(DisplayText))
                    return;

                ResultText = DisplayText;
                InputSequence += "#";
                DisplayText = string.Empty;
                _lastPressedKey = null;
                _timeoutTimer.Stop();

                // Сбрасываем счетчики нажатий
                foreach (var key in _keyMappings.Keys)
                {
                    _pressCount[key] = 0;
                }
            });
        }

        // Метод для уведомления об изменении свойства
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    // Простая реализация ICommand для кнопок
    public class DelegateCommand : ICommand
    {
        private readonly Action _execute;
        private readonly Func<bool> _canExecute;

        public event EventHandler CanExecuteChanged;

        public DelegateCommand(Action execute, Func<bool> canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute == null || _canExecute();
        }

        public void Execute(object parameter)
        {
            _execute();
        }

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    // Реализация ICommand с параметром
    public class DelegateCommand<T> : ICommand
    {
        private readonly Action<T> _execute;
        private readonly Func<T, bool> _canExecute;

        public event EventHandler CanExecuteChanged;

        public DelegateCommand(Action<T> execute, Func<T, bool> canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            if (parameter == null && typeof(T).IsValueType)
                return false;

            return _canExecute == null || _canExecute((T)parameter);
        }

        public void Execute(object parameter)
        {
            _execute((T)parameter);
        }

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}