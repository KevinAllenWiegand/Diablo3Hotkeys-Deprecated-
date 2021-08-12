using System.Windows;
using System.Windows.Controls;

namespace DiabloIIIHotkeys.Behaviors
{
    internal class ScrollViewerBehavior
    {
        public static bool GetAutoScrollToBottom(DependencyObject obj)
        {
            return (bool)obj.GetValue(AutoScrollToBottomProperty);
        }

        public static void SetAutoScrollToBottom(DependencyObject obj, bool value)
        {
            obj.SetValue(AutoScrollToBottomProperty, value);
        }

        public static readonly DependencyProperty AutoScrollToBottomProperty =
            DependencyProperty.RegisterAttached("AutoScrollToBottom", typeof(bool), typeof(ScrollViewerBehavior), new PropertyMetadata(false, (obj, e) =>
            {
                if (!(obj is ScrollViewer scrollViewer))
                {
                    return;
                }

                if ((bool)e.NewValue)
                {
                    scrollViewer.ScrollToBottom();
                    SetAutoScrollToBottom(obj, false);
                }
            }));
    }
}
