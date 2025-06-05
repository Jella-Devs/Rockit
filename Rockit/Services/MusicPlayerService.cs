using Rockit.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace Rockit.Services
{
    public class MusicPlayerService
    {
        private Form1 mainForm;
        public WaveOutEvent outputDevice;
        public bool isPlaying = false;
        private int currentIndex = 0;
        private AudioFileReader audioFile;
        private static MusicPlayerService instance;
        private bool skipPlaybackStopped = false;

        private string artistkey;
        public static MusicPlayerService Instance
        {
            get
            {
                if (instance == null)
                    instance = new MusicPlayerService();
                return instance;
            }
        }
        private MusicPlayerService()
        {
            // Constructor privado para evitar múltiples instancias
        }
        public void SetMainForm(Form1 form)
        {
            mainForm = form;
        }
        public void Play()
        {
            if (PlaylistStore.playlist.Count == 0) return;
            try
            {
                audioFile = new AudioFileReader(PlaylistStore.playlist[currentIndex].SongPath);
                outputDevice = new WaveOutEvent();
                outputDevice.Init(audioFile);
                outputDevice.PlaybackStopped += OnPlaybackStopped;
                outputDevice.Play();
                isPlaying = true;
                mainForm.StatusPlayerinLabels();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al reproducir: " + ex.Message);
            }
        }
        private void OnPlaybackStopped(object sender, StoppedEventArgs e)
        {
            if (skipPlaybackStopped)
            {
                skipPlaybackStopped = false;
                return; // Evita doble eliminación
            }

            audioFile?.Dispose();
            outputDevice?.Dispose();

            if (currentIndex < PlaylistStore.playlist.Count)
            {
                PlaylistStore.playlist.RemoveAt(currentIndex);
            }

            if (PlaylistStore.playlist.Count == 0)
            {
                currentIndex = 0;
                isPlaying = false;
                mainForm.StatusPlayerinLabels();
                return;
            }

            if (currentIndex >= PlaylistStore.playlist.Count)
                currentIndex = 0;

            Play();
        }
        public void Stop()
        {
            if (outputDevice != null)
            {
                skipPlaybackStopped = true; // Ignora el evento en este caso
                outputDevice.Stop();
            }

            audioFile?.Dispose();
            outputDevice?.Dispose();
            
        }
        public void ClearPlaylist()
        {
            Stop();
            PlaylistStore.playlist.Clear();
            isPlaying = false;
            mainForm.StatusPlayerinLabels();
        }
        public void Skip()
        {
            Stop();

            if (PlaylistStore.playlist.Count == 0)
                return;

            // Eliminar la canción actual solo si está dentro del rango
            if (currentIndex >= 0 && currentIndex < PlaylistStore.playlist.Count)
            {
                PlaylistStore.playlist.RemoveAt(currentIndex);
            }

            // Si la playlist ya no tiene canciones, salir
            if (PlaylistStore.playlist.Count == 0)
            {
                currentIndex = 0;
                isPlaying = false;
                mainForm.StatusPlayerinLabels();
                return;
            }

            // Si currentIndex ya está fuera de rango por la eliminación, ajustarlo
            if (currentIndex >= PlaylistStore.playlist.Count)
            {
                currentIndex = 0;
            }
            mainForm.StatusPlayerinLabels();
            Play();
        }
        public void AgregarCancionAPlaylist(string nombre, string path)
        {
            PlaylistStore.playlist.Add(new PlayListItem
            {
                SongName = nombre,
                SongPath = path
            });
            // Solo iniciar la reproducción si no hay nada en reproducción o aún no se ha empezado
            if (!isPlaying)
            {
                currentIndex = 0;
                Play();
            }
        }
        private void RemoveCurrentSong()
        {
            if (PlaylistStore.playlist.Count > 0 && currentIndex < PlaylistStore.playlist.Count)
            {
                PlaylistStore.playlist.RemoveAt(currentIndex);
            }
            if (currentIndex >= PlaylistStore.playlist.Count)
            {
                currentIndex = 0;
                isPlaying = false;
            }
        }
    }

}
