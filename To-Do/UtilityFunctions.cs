using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace To_Do
{
    /// <summary>
    /// Provides useful static functions to be used in any other class
    /// </summary>
    public static class UtilityFunctions
    {
        public static T FindControl<T>(UIElement parent, Type targetType, string ControlName) where T : FrameworkElement
        {

            if (parent == null) return null;

            if (parent.GetType() == targetType && ((T)parent).Name == ControlName)
            {
                return (T)parent;
            }
            T result = null;
            int count = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < count; i++)
            {
                UIElement child = (UIElement)VisualTreeHelper.GetChild(parent, i);

                if (FindControl<T>(child, targetType, ControlName) != null)
                {
                    result = FindControl<T>(child, targetType, ControlName);
                    break;
                }
            }
            return result;
        }

        public static T FindParent<T>(DependencyObject child, string parentName)
            where T : DependencyObject
        {
            if (child == null) return null;

            T foundParent = null;
            var currentParent = VisualTreeHelper.GetParent(child);

            do
            {
                var frameworkElement = currentParent as FrameworkElement;
                if (frameworkElement.Name == parentName && frameworkElement is T)
                {
                    foundParent = (T)currentParent;
                    break;
                }

                currentParent = VisualTreeHelper.GetParent(currentParent);

            } while (currentParent != null);

            return foundParent;
        }

        public static Color ChangeColorBrightness(Color c, bool isContent)
        {
            float r, g, b;
            if (isContent)
            {
                if (ThemeHelper.IsDarkTheme())
                {

                    r = Lerp(c.R, 125f, 0.2f);
                    g = Lerp(c.G, 125f, 0.2f);
                    b = Lerp(c.B, 125f, 0.2f);

                }
                else
                {
                    r = Lerp(c.R, 255f, 0.7f);
                    g = Lerp(c.G, 255f, 0.7f);
                    b = Lerp(c.B, 255f, 0.7f);
                }
            }
            else
            {
                if (ThemeHelper.IsDarkTheme())
                {

                    r = Lerp(c.R, 255f, 0.1f);
                    g = Lerp(c.G, 255f, 0.1f);
                    b = Lerp(c.B, 255f, 0.1f);

                }
                else
                {
                    r = Lerp(c.R, 255f, 0.8f);
                    g = Lerp(c.G, 255f, 0.8f);
                    b = Lerp(c.B, 255f, 0.8f);
                }
            }


            return Color.FromArgb(c.A, Convert.ToByte(r), Convert.ToByte(g), Convert.ToByte(b));
        }

        public static float Lerp(float a, float b, float f)
        {
            return (float)((a * (1.0 - f)) + (b * f));
        }
    }
}
