﻿using System;
using System.Collections.Generic;
using System.Linq;
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

namespace ARCPSGUI.Controls
{
    /// <summary>
    /// Interaction logic for TimeControl.xaml
    /// </summary>
    public partial class TimeControl : UserControl
    {
        public TimeControl()
        {
            InitializeComponent();
            AddHours();
            AddMinutes();
        }
        public TimeSpan Value
        {
            get { return (TimeSpan)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }
        public static readonly DependencyProperty ValueProperty =
        DependencyProperty.Register("Value", typeof(TimeSpan), typeof(TimeControl),
        new UIPropertyMetadata(DateTime.Now.TimeOfDay, new PropertyChangedCallback(OnValueChanged)));

        public string Hour
        {
            get {

                return  cmbHours.Text;
            }
            set
            {
                cmbHours.Text = value;
            }
        }

        public string Minute
        {
            get
            {

                return cmbMinutes.Text;
            }
            set
            {
                cmbMinutes.Text = value;
            }
        }

       

        private static void OnValueChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            TimeControl control = obj as TimeControl;
            control.Hours = ((TimeSpan)e.NewValue).Hours;
            control.Minutes = ((TimeSpan)e.NewValue).Minutes;
            control.Seconds = ((TimeSpan)e.NewValue).Seconds;

          
        }

        public int Hours
        {
            get { return (int)GetValue(HoursProperty); }
            set { SetValue(HoursProperty, value); }
        }
        public static readonly DependencyProperty HoursProperty =
        DependencyProperty.Register("Hours", typeof(int), typeof(TimeControl),
        new UIPropertyMetadata(0, new PropertyChangedCallback(OnTimeChanged)));

        public int Minutes
        {
            get { return (int)GetValue(MinutesProperty); }
            set { SetValue(MinutesProperty, value); }
        }
        public static readonly DependencyProperty MinutesProperty =
        DependencyProperty.Register("Minutes", typeof(int), typeof(TimeControl),
        new UIPropertyMetadata(0, new PropertyChangedCallback(OnTimeChanged)));

        public int Seconds
        {
            get { return (int)GetValue(SecondsProperty); }
            set { SetValue(SecondsProperty, value); }
        }

        public static readonly DependencyProperty SecondsProperty =
        DependencyProperty.Register("Seconds", typeof(int), typeof(TimeControl),
        new UIPropertyMetadata(0, new PropertyChangedCallback(OnTimeChanged)));


        private static void OnTimeChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            TimeControl control = obj as TimeControl;
            control.Value = new TimeSpan(control.Hours, control.Minutes, control.Seconds);
        }

        private void Down(object sender, KeyEventArgs args)
        {
            switch (((Grid)sender).Name)
            {
                case "sec":
                    if (args.Key == Key.Up)
                        this.Seconds++;
                    if (args.Key == Key.Down)
                        this.Seconds--;
                    break;

                case "min":
                    if (args.Key == Key.Up)
                        this.Minutes++;
                    if (args.Key == Key.Down)
                        this.Minutes--;
                    break;

                case "hour":
                    if (args.Key == Key.Up)
                        this.Hours++;
                    if (args.Key == Key.Down)
                        this.Hours--;
                    break;
            }
        }

        private void mmTxt_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            try
            {
                Convert.ToInt32(e.Text);
            }
            catch
            {
                e.Handled = true;
            }
        }

        private void mmTxt_GotFocus(object sender, RoutedEventArgs e)
        {
            //(sender as TextBox).SelectAll();
        }

        private void ddTxt_GotFocus(object sender, RoutedEventArgs e)
        {
          // (sender as TextBox).SelectAll();
        }

        void AddHours()
        {
            for (int i = 1; i <= 24; i++)
            {
                cmbHours.Items.Add(i);
            }
        }
        void AddMinutes()
        {
            for (int i = 1; i <= 60; i++)
            {
                 cmbMinutes.Items.Add(i);
            }
        }
    }
}