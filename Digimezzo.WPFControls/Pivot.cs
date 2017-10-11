﻿using Digimezzo.WPFControls.Effects;
using Digimezzo.WPFControls.Utils;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Digimezzo.WPFControls
{
    public enum PivotAnimationType
    {
        Fade = 1,
        Slide = 2
    }

    public class Pivot : TabControl
    {
        private Grid contentPanel;
        private ContentPresenter mainContent;
        private Shape paintArea;
        private int previous = -1;
        private int current = -1;
        private FeatheringEffect effect;
        private bool isFirstEffectDisplay = true;
        private bool isFirstSelectionChange = true;

        public PivotAnimationType AnimationType
        {
            get { return (PivotAnimationType)GetValue(animationTypeProperty); }
            set { SetValue(animationTypeProperty, value); }
        }

        public static readonly DependencyProperty animationTypeProperty =
            DependencyProperty.Register(nameof(AnimationType), typeof(PivotAnimationType), typeof(Pivot), new PropertyMetadata(PivotAnimationType.Fade));

        public double Elevation
        {
            get { return Convert.ToDouble(GetValue(ElevationProperty)); }
            set { SetValue(ElevationProperty, value); }
        }

        public static readonly DependencyProperty ElevationProperty =
          DependencyProperty.Register(nameof(Elevation), typeof(double), typeof(Pivot), new PropertyMetadata(0.0));

        public Brush ElevationBackground
        {
            get { return (Brush)GetValue(ElevationBackgroundProperty); }
            set { SetValue(ElevationBackgroundProperty, value); }
        }

        public static readonly DependencyProperty ElevationBackgroundProperty =
           DependencyProperty.Register(nameof(ElevationBackground), typeof(Brush), typeof(Pivot), new PropertyMetadata(Brushes.Transparent));

        public Brush HeaderBackground
        {
            get { return (Brush)GetValue(HeaderBackgroundProperty); }
            set { SetValue(HeaderBackgroundProperty, value); }
        }

        public static readonly DependencyProperty HeaderBackgroundProperty =
         DependencyProperty.Register(nameof(HeaderBackground), typeof(Brush), typeof(Pivot), new PropertyMetadata(Brushes.Transparent));

        public double FadeDistance
        {
            get { return Convert.ToDouble(GetValue(FadeDistanceProperty)); }
            set { SetValue(FadeDistanceProperty, value); }
        }

        public static readonly DependencyProperty FadeDistanceProperty =
         DependencyProperty.Register(nameof(FadeDistance), typeof(double), typeof(Pivot), new PropertyMetadata(20.0));

        public double Duration
        {
            get { return Convert.ToDouble(GetValue(DurationProperty)); }
            set { SetValue(DurationProperty, value); }
        }

        public static readonly DependencyProperty DurationProperty =
           DependencyProperty.Register(nameof(Duration), typeof(double), typeof(Pivot), new PropertyMetadata(0.5));

        public double EasingAmplitude
        {
            get { return Convert.ToDouble(GetValue(EasingAmplitudeProperty)); }
            set { SetValue(EasingAmplitudeProperty, value); }
        }

        public static readonly DependencyProperty EasingAmplitudeProperty =
            DependencyProperty.Register(nameof(EasingAmplitude), typeof(double), typeof(Pivot), new PropertyMetadata(0.0));

        public double IndicatorHeight
        {
            get { return (double)GetValue(IndicatorHeightProperty); }
            set { SetValue(IndicatorHeightProperty, value); }
        }

        public static readonly DependencyProperty IndicatorHeightProperty =
          DependencyProperty.Register(nameof(IndicatorHeight), typeof(double), typeof(Pivot), new PropertyMetadata(0.0));

        public Brush IndicatorBackground
        {
            get { return (Brush)GetValue(IndicatorBackgroundProperty); }
            set { SetValue(IndicatorBackgroundProperty, value); }
        }

        public static readonly DependencyProperty IndicatorBackgroundProperty =
          DependencyProperty.Register(nameof(IndicatorBackground), typeof(Brush), typeof(Pivot), new PropertyMetadata(Brushes.Transparent));

        public bool DisableTabKey
        {
            get { return (bool)GetValue(DisableTabKeyProperty); }
            set { SetValue(DisableTabKeyProperty, value); }
        }

        public static readonly DependencyProperty DisableTabKeyProperty =
          DependencyProperty.Register(nameof(DisableTabKey), typeof(bool), typeof(Pivot), new PropertyMetadata(false));

        public double FeatheringRadius
        {
            get => (double)GetValue(FeatheringRadiusProperty);
            set => SetValue(FeatheringRadiusProperty, value);
        }

        public static DependencyProperty FeatheringRadiusProperty =
            DependencyProperty.Register(nameof(FeatheringRadius), typeof(double), typeof(Pivot), new PropertyMetadata(0.0));

        public bool SlideFadeIn
        {
            get { return (bool)GetValue(SlideFadeInProperty); }
            set { SetValue(SlideFadeInProperty, value); }
        }

        public static readonly DependencyProperty SlideFadeInProperty =
            DependencyProperty.Register(nameof(SlideFadeIn), typeof(bool), typeof(Pivot), new PropertyMetadata(false));

        public double SlideFadeInDuration
        {
            get { return Convert.ToDouble(GetValue(SlideFadeInDurationProperty)); }
            set { SetValue(SlideFadeInDurationProperty, value); }
        }

        public static readonly DependencyProperty SlideFadeInDurationProperty =
          DependencyProperty.Register(nameof(SlideFadeInDuration), typeof(double), typeof(Pivot), new PropertyMetadata(0.5));

        static Pivot()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Pivot), new FrameworkPropertyMetadata(typeof(Pivot)));
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            this.contentPanel = (Grid)GetTemplateChild("contentPanel");
            this.mainContent = (ContentPresenter)GetTemplateChild("PART_SelectedContentHost");
            this.paintArea = (Shape)GetTemplateChild("PART_PaintArea");
            this.effect = new FeatheringEffect() { FeatheringRadius = this.FeatheringRadius };

            if (this.contentPanel != null)
            {
                this.SelectionChanged += Pivot_SelectionChanged;
            }

            if (this.DisableTabKey)
            {
                this.PreviewKeyDown += Pivot_PreviewKeyDown;
            }
        }

        private void Pivot_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Tab)
            {
                e.Handled = true;
            }
        }

        private void DoFadeAnimation()
        {
            DoubleAnimation slideAnimation = null;
            DoubleAnimation opacityAnimation = new DoubleAnimation() { From = 0.0, To = 1.0, Duration = TimeSpan.FromSeconds(this.Duration * 1.5) };

            if (previous > current)
            {
                slideAnimation = new DoubleAnimation() { From = -this.FadeDistance, To = 0.0, Duration = TimeSpan.FromSeconds(this.Duration) };

            }
            else
            {
                slideAnimation = new DoubleAnimation() { From = this.FadeDistance, To = 0.0, Duration = TimeSpan.FromSeconds(this.Duration) };
            }

            TranslateTransform translateTransform1 = new TranslateTransform();
            var fadeStoryboard = new Storyboard();
            fadeStoryboard.Children.Add(opacityAnimation);
            Storyboard.SetTargetName(contentPanel, contentPanel.Name);
            Storyboard.SetTargetProperty(opacityAnimation, new PropertyPath(UserControl.OpacityProperty));
            fadeStoryboard.Begin(contentPanel);
            translateTransform1.BeginAnimation(TranslateTransform.XProperty, slideAnimation);
            contentPanel.RenderTransform = translateTransform1;
            previous = current;
        }

        private void ApplyEffect()
        {
            // The first time we show the content, don't apply the effect. This prevents side-effects.
            if (this.isFirstEffectDisplay)
            {
                this.isFirstEffectDisplay = false;
            }
            else if (this.FeatheringRadius > 0.0)
            {
                // Only apply the effect when FeatheringRadius is specified.
                this.effect.TexWidth = ActualWidth;
                this.Effect = effect;
            }
        }

        private void ClearEffect()
        {
            this.Effect = null;
        }

        private void DoSlideAnimation()
        {
            this.ApplyEffect();

            if (this.paintArea != null && this.mainContent != null && this.ActualWidth > 0 && this.ActualHeight > 0)
            {
                this.paintArea.Fill = AnimationUtils.CreateBrushFromVisual(this.mainContent, this.ActualWidth, this.ActualHeight);

                var newContentTransform = new TranslateTransform();
                var oldContentTransform = new TranslateTransform();
                this.paintArea.RenderTransform = oldContentTransform;
                this.mainContent.RenderTransform = newContentTransform;
                this.paintArea.Visibility = Visibility.Visible;

                if (previous > current)
                {
                    newContentTransform.BeginAnimation(TranslateTransform.XProperty, AnimationUtils.CreateSlideAnimation(-this.ActualWidth, 0, this.EasingAmplitude, this.Duration));
                    oldContentTransform.BeginAnimation(TranslateTransform.XProperty, AnimationUtils.CreateSlideAnimation(0, this.ActualWidth, this.EasingAmplitude, this.Duration, (s, e) =>
                    {
                        this.paintArea.Visibility = Visibility.Hidden;
                        this.ClearEffect();
                    }));

                }
                else
                {
                    newContentTransform.BeginAnimation(TranslateTransform.XProperty, AnimationUtils.CreateSlideAnimation(this.ActualWidth, 0, this.EasingAmplitude, this.Duration));
                    oldContentTransform.BeginAnimation(TranslateTransform.XProperty, AnimationUtils.CreateSlideAnimation(0, -this.ActualWidth, this.EasingAmplitude, this.Duration, (s, e) =>
                    {
                        this.paintArea.Visibility = Visibility.Hidden;
                        this.ClearEffect();
                    }));
                }

                previous = current;

                if (this.SlideFadeIn)
                {
                    this.mainContent.BeginAnimation(OpacityProperty, AnimationUtils.CreateFadeAnimation(0, 1, this.SlideFadeInDuration));
                    this.paintArea.BeginAnimation(OpacityProperty, AnimationUtils.CreateFadeAnimation(1, 0, this.SlideFadeInDuration));
                }
            }
        }

        private void Pivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Don't animate the first time the control is displayed
            if (this.isFirstSelectionChange)
            {
                this.isFirstSelectionChange = false;
                return;
            }

            current = (sender as TabControl).SelectedIndex;

            if (previous != current)
            {
                if (this.AnimationType == PivotAnimationType.Fade)
                {
                    this.DoFadeAnimation();
                }
                else if (this.AnimationType == PivotAnimationType.Slide)
                {
                    this.DoSlideAnimation();
                }
            }
        }
    }

    public class PivotItem : TabItem
    {
        public double HeaderFontSize
        {
            get { return (double)GetValue(HeaderFontSizeProperty); }
            set { SetValue(HeaderFontSizeProperty, value); }
        }

        public static readonly DependencyProperty HeaderFontSizeProperty =
           DependencyProperty.Register(nameof(HeaderFontSize), typeof(double), typeof(PivotItem), new PropertyMetadata(13.0));

        public Brush SelectedForeground
        {
            get { return (Brush)GetValue(SelectedForegroundProperty); }
            set { SetValue(SelectedForegroundProperty, value); }
        }

        public static readonly DependencyProperty SelectedForegroundProperty =
           DependencyProperty.Register(nameof(SelectedForeground), typeof(Brush), typeof(PivotItem), new PropertyMetadata(Brushes.Black));

        public FontWeight SelectedFontWeight
        {
            get { return (FontWeight)GetValue(SelectedFontWeightProperty); }
            set { SetValue(SelectedFontWeightProperty, value); }
        }

        public static readonly DependencyProperty SelectedFontWeightProperty =
             DependencyProperty.Register(nameof(SelectedFontWeight), typeof(FontWeight), typeof(PivotItem), new PropertyMetadata(FontWeights.Normal));

        static PivotItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PivotItem), new FrameworkPropertyMetadata(typeof(PivotItem)));
        }
    }
}
