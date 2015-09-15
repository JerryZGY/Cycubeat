using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Cycubeat.Controls
{
    public partial class StartControl : UserControl
    {
        private double resWidth = SystemParameters.FullPrimaryScreenWidth;

        private double resHeight = SystemParameters.FullPrimaryScreenHeight;

        private const double size = 100;

        private const double distance = 50;

        private static double[] scaleMap = { 1, .8, .8, .8, .8, .64, .64, .64, .64 };

        private static string[] colorsMap =
        {
            "#FF696969",
            "#FFA7A7A7","#FFA7A7A7","#FFA7A7A7","#FFA7A7A7",
            "#FFFFFFFF", "#FFFFFFFF", "#FFFFFFFF", "#FFFFFFFF"
        };

        private static Point[] offsetMap =
        {
            new Point(0, 0), //Center
            new Point(-(size * scaleMap[1] + distance), 0), //Left
            new Point(size * scaleMap[2] + distance, 0), //Right
            new Point(0, -(size * scaleMap[3] + distance)), //Top
            new Point(0, size * scaleMap[4] + distance), //Down
            new Point(-(size * scaleMap[5] + distance), -(size * scaleMap[5] + distance)), //TopLeft
            new Point(size * scaleMap[6] + distance, -(size * scaleMap[6] + distance)), //TopRight
            new Point(-(size * scaleMap[7] + distance), size * scaleMap[7] + distance), //DownLeft
            new Point(size * scaleMap[8] + distance, size * scaleMap[8] + distance), //DownRight
        };

        private Rectangle[] rectangles = new Rectangle[9];

        public StartControl()
        {
            InitializeComponent();
        }

        private void initRect()
        {
            for (int i = 0; i < rectangles.Length; i++)
            {
                rectangles[i] = new Rectangle();
                rectangles[i].Opacity = 0;
                rectangles[i].Fill = (SolidColorBrush)(new BrushConverter().ConvertFromString(colorsMap[i]));
                rectangles[i].Width = rectangles[i].Height = size * scaleMap[i];
                setRectPosition(rectangles[i], offsetMap[i].X, offsetMap[i].Y);
                cnv_Title.Children.Add(rectangles[i]);
            }
        }

        private void setRectPosition(Rectangle element, double offsetX, double offsetY)
        {
            Canvas.SetLeft(element, (resWidth - element.Width) / 2 + offsetX);
            Canvas.SetTop(element, (resHeight - element.Height) / 2 + offsetY);
        }

        private void startStory()
        {
            DoubleAnimation fadeIn = new DoubleAnimation()
            {
                From = 0,
                To = 1,
                Duration = TimeSpan.FromSeconds(.5),
                EasingFunction = new QuarticEase() { EasingMode = EasingMode.EaseOut }
            };
            rectangles[0].BeginAnimation(OpacityProperty, fadeIn);
            fadeIn.BeginTime = TimeSpan.FromSeconds(.5);
            rectangles[1].BeginAnimation(OpacityProperty, fadeIn);
            rectangles[2].BeginAnimation(OpacityProperty, fadeIn);
            rectangles[3].BeginAnimation(OpacityProperty, fadeIn);
            rectangles[4].BeginAnimation(OpacityProperty, fadeIn);
            fadeIn.BeginTime = TimeSpan.FromSeconds(1);
            rectangles[5].BeginAnimation(OpacityProperty, fadeIn);
            rectangles[6].BeginAnimation(OpacityProperty, fadeIn);
            rectangles[7].BeginAnimation(OpacityProperty, fadeIn);
            rectangles[8].BeginAnimation(OpacityProperty, fadeIn);
            DoubleAnimation rotate = new DoubleAnimation()
            {
                From = 0,
                To = 45,
                Duration = TimeSpan.FromSeconds(.5),
                EasingFunction = new QuarticEase() { EasingMode = EasingMode.EaseOut },
                BeginTime = TimeSpan.FromSeconds(1.5)
            };
            cnv_Title.RenderTransform.BeginAnimation(RotateTransform.AngleProperty, rotate);
            fadeIn.BeginTime = TimeSpan.FromSeconds(2.5);
            Img_Title.BeginAnimation(OpacityProperty, fadeIn);

            DoubleAnimation fadeInEffect = new DoubleAnimation()
            {
                From = 0,
                To = 0.5,
                Duration = TimeSpan.FromSeconds(.5),
                EasingFunction = new QuarticEase() { EasingMode = EasingMode.EaseOut },
            };
            fadeInEffect.BeginTime = TimeSpan.FromSeconds(3);
            Img_TitleEffect.BeginAnimation(OpacityProperty, fadeInEffect);
            DoubleAnimation scaleEffect = new DoubleAnimation()
            {
                From = 1,
                To = 1.2,
                Duration = TimeSpan.FromSeconds(.5),
                EasingFunction = new QuarticEase() { EasingMode = EasingMode.EaseOut }
            };
            scaleEffect.BeginTime = TimeSpan.FromSeconds(3);
            Img_TitleEffect.RenderTransform.BeginAnimation(ScaleTransform.ScaleXProperty, scaleEffect);
            Img_TitleEffect.RenderTransform.BeginAnimation(ScaleTransform.ScaleYProperty, scaleEffect);
            DoubleAnimation fadeOutEffect = new DoubleAnimation()
            {
                From = 0.5,
                To = 0,
                Duration = TimeSpan.FromSeconds(2),
                EasingFunction = new QuarticEase() { EasingMode = EasingMode.EaseOut },
            };
            fadeOutEffect.BeginTime = TimeSpan.FromSeconds(3.5);
            Img_TitleEffect.BeginAnimation(OpacityProperty, fadeOutEffect);
            fadeIn.BeginTime = TimeSpan.FromSeconds(4);
            Tbx_Copyright.BeginAnimation(OpacityProperty, fadeIn);
            DoubleAnimation shine = new DoubleAnimation()
            {
                From = 0,
                To = 0.4,
                Duration = TimeSpan.FromSeconds(.5),
                AutoReverse = true,
                RepeatBehavior = RepeatBehavior.Forever
            };
            shine.BeginTime = TimeSpan.FromSeconds(5.5);
            Img_TitleShine.BeginAnimation(OpacityProperty, shine);
        }

        private void StartControl_Loaded(object sender, RoutedEventArgs e)
        {
            Storyboard storyBoard = ((Storyboard)this.Resources["BackgroundEffect"]);
            storyBoard.Begin();
            cnv_Title.Width = resWidth;
            cnv_Title.Height = resHeight;
            initRect();
            startStory();
        }
    }
}
