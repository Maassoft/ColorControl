using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace DJ
{
    public static class ScrollingHelper
    {
        public static void ScrollToEnd(this ListView listView)
        {
            var scrollViewer = GetDescendantByType(listView, typeof(ScrollViewer)) as ScrollViewer;
            scrollViewer?.ScrollToEnd();
        }

        public static Visual GetDescendantByType(Visual element, Type type)
        {
            if (element != null)
            {
                if (element.GetType() != type)
                {
                    Visual foundElement = null;
                    (element as FrameworkElement)?.ApplyTemplate();

                    for (var i = 0; i < VisualTreeHelper.GetChildrenCount(element); i++)
                    {
                        var visual = VisualTreeHelper.GetChild(element, i) as Visual;
                        foundElement = GetDescendantByType(visual, type);
                        if (foundElement != null)
                        {
                            break;
                        }
                    }

                    return foundElement;
                }

                return element;
            }

            return null;
        }
    }
}