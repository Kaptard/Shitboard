using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Text.RegularExpressions;
using System.ComponentModel;

namespace Shitboard
{
    class Memes : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private String _staticPath = System.AppDomain.CurrentDomain.BaseDirectory;
        private String _staticFile = @"memes.txt";
        private String _staticText = "";
        private String _currentMeme = "";
        private Random rand = new Random();
        private enum Source
        {
            STATIC,
            REDDIT
        }
        private Source CurrentSource = Source.STATIC;

        public void init()
        {
            loop();
            monitor();
            setStaticText();
            setCurrentMeme();
        }

        private void OnPropertyChanged(String property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }

        public String CurrentMeme
        {
            get
            {
                return _currentMeme;
            }
            set
            {
                _currentMeme = value;
                OnPropertyChanged("CurrentMeme");
            }
        }

        private void loop()
        {
            DispatcherTimer t1 = new DispatcherTimer();
            t1.Tick += refresh;
            t1.Interval = new TimeSpan(0, 0, 1);
            t1.Start();
        }

        private void refresh(object sender, EventArgs e)
        {
            setCurrentMeme();
            Clipboard.SetText(getCurrentMeme());
        }

        private void monitor()
        {
            var watcher = new FileSystemWatcher();
            watcher.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.DirectoryName;
            watcher.Path = _staticPath;
            watcher.Filter = _staticFile;
            watcher.Changed += setStaticText;
            watcher.Created += setStaticText;
            watcher.Deleted += setStaticText;
            watcher.Renamed += setStaticText;
            watcher.EnableRaisingEvents = true;
        }

        private void setStaticText(object source, FileSystemEventArgs e)
        {
            setStaticText();
        }

        private void setStaticText()
        {
            try
            {
                _staticText = File.ReadAllText(_staticPath + _staticFile);
            }
            catch (FileNotFoundException)
            {
                _staticText = "Couldn't find memes.txt, pls make one in the same folder as this program";
            }
            catch (FileLoadException)
            {
                _staticText = "memes.txt failed to load";
            }
            catch (IOException)
            {
                _staticText = "memes.txt I/O Error";
            }
            catch (Exception err)
            {
                _staticText = err.Message;
            }
        }

        private String setCurrentMeme()
        {
            if (CurrentSource == Source.STATIC)
            {
                string[] staticMemes = Regex.Split(_staticText, "[\r\n]{2,}");
                _currentMeme = staticMemes[rand.Next(0, staticMemes.Length - 1)];
                OnPropertyChanged("CurrentMeme");
                return _currentMeme;
            }
            else
            {
                OnPropertyChanged("CurrentMeme");
                return "";
            }
        }

        private String getCurrentMeme()
        {
            return _currentMeme;
        }
    }
}
