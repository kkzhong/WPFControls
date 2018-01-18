﻿using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Digimezzo.WPFControls
{
    public class MaterialToggleSwitch : CheckBox
    {
        public Brush Accent
        {
            get { return (Brush)GetValue(AccentProperty); }
            set { SetValue(AccentProperty, value); }
        }

        public static readonly DependencyProperty AccentProperty =
            DependencyProperty.Register(nameof(Accent), typeof(Brush), typeof(MaterialToggleSwitch), new PropertyMetadata(new SolidColorBrush(Colors.Blue)));

        public bool UseDarkTheme
        {
            get { return (bool)GetValue(UseDarkThemeProperty); }
            set { SetValue(UseDarkThemeProperty, value); }
        }

        public static readonly DependencyProperty UseDarkThemeProperty =
            DependencyProperty.Register(nameof(UseDarkTheme), typeof(bool), typeof(MaterialToggleSwitch), new PropertyMetadata(false));

        static MaterialToggleSwitch()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MaterialToggleSwitch), new FrameworkPropertyMetadata(typeof(MaterialToggleSwitch)));
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this.Background = this.UseDarkTheme ? new SolidColorBrush(Colors.White) : new SolidColorBrush(Colors.Black);
        }
    }
}
