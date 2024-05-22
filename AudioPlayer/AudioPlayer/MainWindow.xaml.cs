using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using Microsoft.Win32;

namespace AudioPlayer
{
    public partial class MainWindow : Window
    {
        private MediaPlayer _mediaPlayer;
        private List<string> _tracks;
        private int _currentTrackIndex;

        public MainWindow()
        {
            InitializeComponent();
            _mediaPlayer = new MediaPlayer();
            _tracks = new List<string>();
            _currentTrackIndex = -1;
            _mediaPlayer.MediaOpened += MediaPlayer_MediaOpened;
            _mediaPlayer.MediaEnded += MediaPlayer_MediaEnded;
        }

        private void AddTrack_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Audio Files|*.mp3;*.wav;*.wma;*.aac"
            };
            if (openFileDialog.ShowDialog() == true)
            {
                _tracks.Add(openFileDialog.FileName);
                Playlist.Items.Add(System.IO.Path.GetFileName(openFileDialog.FileName));
            }
        }

        private void DeleteTrack_Click(object sender, RoutedEventArgs e)
        {
            if (Playlist.SelectedIndex >= 0)
            {
                int index = Playlist.SelectedIndex;
                _tracks.RemoveAt(index);
                Playlist.Items.RemoveAt(index);
                if (_currentTrackIndex == index)
                {
                    Stop_Click(sender, e);
                    _currentTrackIndex = -1;
                    TrackInfo.Text = "Track Info";
                }
            }
        }

        private void SavePlaylist_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "Playlist Files|*.playlist"
            };
            if (saveFileDialog.ShowDialog() == true)
            {
                File.WriteAllLines(saveFileDialog.FileName, _tracks);
            }
        }

        private void LoadPlaylist_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Playlist Files|*.playlist"
            };
            if (openFileDialog.ShowDialog() == true)
            {
                _tracks = new List<string>(File.ReadAllLines(openFileDialog.FileName));
                Playlist.Items.Clear();
                foreach (var track in _tracks)
                {
                    Playlist.Items.Add(System.IO.Path.GetFileName(track));
                }
            }
        }

        private void Play_Click(object sender, RoutedEventArgs e)
        {
            if (_currentTrackIndex == -1 && Playlist.Items.Count > 0)
            {
                _currentTrackIndex = 0;
            }

            if (_currentTrackIndex >= 0 && _currentTrackIndex < _tracks.Count)
            {
                _mediaPlayer.Open(new Uri(_tracks[_currentTrackIndex]));
                _mediaPlayer.Play();
                TrackInfo.Text = System.IO.Path.GetFileName(_tracks[_currentTrackIndex]);
            }
        }

        private void Pause_Click(object sender, RoutedEventArgs e)
        {
            _mediaPlayer.Pause();
        }

        private void Stop_Click(object sender, RoutedEventArgs e)
        {
            _mediaPlayer.Stop();
            ProgressBar.Value = 0;
            CurrentTime.Text = "00:00";
        }

        private void Previous_Click(object sender, RoutedEventArgs e)
        {
            if (_currentTrackIndex > 0)
            {
                _currentTrackIndex--;
                Play_Click(sender, e);
            }
        }

        private void Next_Click(object sender, RoutedEventArgs e)
        {
            if (_currentTrackIndex < _tracks.Count - 1)
            {
                _currentTrackIndex++;
                Play_Click(sender, e);
            }
        }

        private void VolumeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            //_mediaPlayer.Volume = VolumeSlider.Value;
        }

        private void MediaPlayer_MediaOpened(object sender, EventArgs e)
        {
            ProgressBar.Maximum = _mediaPlayer.NaturalDuration.TimeSpan.TotalSeconds;
            TotalTime.Text = _mediaPlayer.NaturalDuration.TimeSpan.ToString(@"mm\:ss");
        }

        private void MediaPlayer_MediaEnded(object sender, EventArgs e)
        {
            Next_Click(sender, (RoutedEventArgs)e);
        }
    }
}