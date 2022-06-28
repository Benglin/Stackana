using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Xleeque.Stackana.Logics;

namespace Stackana
{
    public partial class Page : UserControl
    {
        // Game loop and frame processing data members.
        DateTime last_tick;
        PageSettings page_settings;
        GameScene game_scene;

        delegate void UpdateFrameCallback(double deltaTime);
        UpdateFrameCallback callbacks;

        public Page(IDictionary<string, string> param)
        {
            InitializeComponent();

            page_settings = new PageSettings();
            page_settings.ReadFromParams(param);
            page_settings.ApplyToPage(this);

            Loaded += new RoutedEventHandler(Page_Loaded);
        }

        public void ShutDown()
        {
            if (null != game_scene)
            {
                callbacks -= game_scene.UpdateFrame;
                game_scene.ShutDown();
                game_scene = null;
            }
        }

        void Page_Loaded(object sender, RoutedEventArgs e)
        {
            // Initialize game board with the provided properties.
            gameSurface.UpdateLayout();
            game_scene = new GameScene(gameSurface);
            game_scene.StartUp();
            callbacks += game_scene.UpdateFrame;

            this.KeyUp +=new KeyEventHandler(Page_KeyUp);
            this.KeyDown +=new KeyEventHandler(Page_KeyDown);

            footerBorder.MouseEnter += new MouseEventHandler(footerBorder_MouseEnter);
            footerBorder.MouseLeave += new MouseEventHandler(footerBorder_MouseLeave);

            last_tick = DateTime.Now; // Start the game loop.
            CompositionTarget.Rendering += new EventHandler(CompositionTarget_Rendering);
        }

        void Page_KeyUp(object sender, KeyEventArgs e)
        {
            if (null != game_scene)
                game_scene.ProcessKeyInput(e.Key, false);
        }

        void Page_KeyDown(object sender, KeyEventArgs e)
        {
            if (null != game_scene)
                game_scene.ProcessKeyInput(e.Key, true);
        }

        void footerBorder_MouseEnter(object sender, EventArgs e)
        {
            footerLabel.Foreground = page_settings.FooterTextOver;
        }

        void footerBorder_MouseLeave(object sender, EventArgs e)
        {
            footerLabel.Foreground = page_settings.FooterTextNorm;
        }

        void CompositionTarget_Rendering(object sender, EventArgs e)
        {
            DateTime this_tick = DateTime.Now;
            TimeSpan elapsed = this_tick - last_tick;

            double elapsed_time = elapsed.TotalMilliseconds;
            if (elapsed_time > Configuration.global_frame_time)
                elapsed_time = Configuration.global_frame_time;

            last_tick = this_tick;
            callbacks(elapsed_time);
        }
    }
}
