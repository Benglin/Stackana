using System;
using System.Collections.Generic;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Stackana;

namespace Xleeque.Stackana.Logics
{
    static class Configuration
    {
        public const int max_column = 10;
        public const int min_column = 4;
        public const int max_row = 20;
        public const int min_row = 10;
        public const double block_gap = 2.0;
        public const double max_canvas_width = 400;
        public const double max_canvas_height = 300;
        public const double global_frame_time = 1000.0 / 30.0;
    }

    public class Utils
    {
        static Random _random;

        static Utils()
        {
            _random = new Random();
        }

        public static int RndGen(int min, int max)
        {
            return _random.Next(min, max);
        }
    }

    public partial class PageSettings
    {
        double page_width = Configuration.max_canvas_width;
        double page_height = Configuration.max_canvas_height;

        Brush header_background = null;
        Brush footer_background = null;
        Brush footer_text_norm = null;
        Brush footer_text_over = null;

        public PageSettings()
        {
        }

        public void ReadFromParams(IDictionary<string, string> param)
        {
            header_background = new SolidColorBrush(Colors.Black);
            footer_background = new SolidColorBrush(Colors.Black);
            footer_text_norm = new SolidColorBrush(Colors.White);
            footer_text_over = new SolidColorBrush(Colors.Orange);
        }

        public bool ApplyToPage(Page page)
        {
            page.Width = page_width;
            page.Height = page_height;

            page.headerBorder.Background = header_background;
            page.footerBorder.Background = footer_background;
            page.footerLabel.Foreground = footer_text_norm;
            return true;
        }

        public double PageWidth
        {
            get { return page_width; }
        }

        public double PageHeight
        {
            get { return page_height; }
        }

        public Brush HeaderBackground
        {
            get { return header_background; }
        }

        public Brush FooterBackground
        {
            get { return footer_background; }
        }

        public Brush FooterTextNorm
        {
            get { return footer_text_norm; }
        }

        public Brush FooterTextOver
        {
            get { return footer_text_over; }
        }
    }
}
