using LaserwarTest.Core.UI.Popups.Animations;
using System;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;

namespace LaserwarTest.UI.Popups.Animations
{
    public class ScalePopupCloseAnimation : IPopupContentAnimation
    {
        private const double DURATION_SECONDS = 0.25;

        public event EventHandler Completed;

        CompositeTransform Target { set; get; }

        Storyboard Animation { set; get; }

        public void SetTarget(UIElement animatingElement)
        {
            Target = (animatingElement.RenderTransform as CompositeTransform)
                ?? new CompositeTransform();

            animatingElement.RenderTransform = Target;
            animatingElement.RenderTransformOrigin = new Point(0.5, 0.5);
        }

        public void Start()
        {
            if (Animation != null) return;

            Animation = new Storyboard();
            Animation.Completed += (s, args) => Complete();

            DoubleAnimation scaleX = new DoubleAnimation
            {
                Duration = TimeSpan.FromSeconds(DURATION_SECONDS),
                EasingFunction = new ElasticEase() { EasingMode = EasingMode.EaseIn },
                To = 0
            };
            Storyboard.SetTarget(scaleX, Target);
            Storyboard.SetTargetProperty(scaleX, nameof(CompositeTransform.ScaleX));

            DoubleAnimation scaleY = new DoubleAnimation
            {
                Duration = TimeSpan.FromSeconds(DURATION_SECONDS),
                EasingFunction = new ElasticEase() { EasingMode = EasingMode.EaseIn },
                To = 0
            };
            Storyboard.SetTarget(scaleY, Target);
            Storyboard.SetTargetProperty(scaleY, nameof(CompositeTransform.ScaleY));

            Animation.Children.Add(scaleX);
            Animation.Children.Add(scaleY);

            Animation.Begin();
        }

        public void Stop()
        {
            if (Animation == null) return;

            Animation.Stop();
            Complete();
        }

        private void Complete()
        {
            Animation = null;
            Completed?.Invoke(this, EventArgs.Empty);
        }
    }
}
