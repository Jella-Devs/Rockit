using Rockit.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMPLib;

namespace Rockit.Services
{
    public static class MusicPlayerService
    {
        public static AxWMPLib.AxWindowsMediaPlayer MediaPlayer { get; set; }

        public static void Inicializar(AxWMPLib.AxWindowsMediaPlayer player)
        {
            MediaPlayer = player;
            MediaPlayer.PlayStateChange += MediaPlayer_PlayStateChange;
        }

        public static void ReproducirSiguienteCancion()
        {
            if (MediaPlayer == null || PlaylistStore.playlist.Count == 0)
                return;

            // Si ya está reproduciendo o en transición, no hagas nada
            //if (MediaPlayer.playState == WMPPlayState.wmppsPlaying ||
            //    MediaPlayer.playState == WMPPlayState.wmppsTransitioning)
            //    return;

            var nextSong = PlaylistStore.playlist[0];

            if (!File.Exists(nextSong.SongPath))
            {
                // Remueve la canción inválida y continúa
                PlaylistStore.playlist.RemoveAt(0);
                ReproducirSiguienteCancion();
                return;
            }

            MediaPlayer.settings.autoStart = true;
            MediaPlayer.URL = nextSong.SongPath;
            Console.WriteLine($"Estado: {MediaPlayer.playState} - Ruta: {nextSong.SongPath}");
            MediaPlayer.Ctlcontrols.play();

        }


        private static void MediaPlayer_PlayStateChange(object sender, AxWMPLib._WMPOCXEvents_PlayStateChangeEvent e)
        {
            // 8 = MediaEnded
            if (e.newState == 8)
            {
                if (PlaylistStore.playlist.Count > 0)
                    PlaylistStore.playlist.RemoveAt(0);

                ReproducirSiguienteCancion();
            }
        }

        public static void AgregarCancionAPlaylist(string nombre, string path)
        {
            PlaylistStore.playlist.Add(new PlayListItem
            {
                SongName = nombre,
                SongPath = path
            });

            // Si no se está reproduciendo nada, iniciar reproducción
            if (MediaPlayer.playState != WMPPlayState.wmppsPlaying)
                ReproducirSiguienteCancion();
        }
    }

}
