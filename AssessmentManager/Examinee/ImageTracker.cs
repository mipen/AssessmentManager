using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssessmentManager
{
    public static class ImageTracker
    {
        private static List<ImageDisplay> list = new List<ImageDisplay>();

        public static bool ImageDisplayShown(string questionName)
        {
            return list.Any((ImageDisplay i) => i.Text.Contains(questionName));
        }

        public static ImageDisplay FindImageDisplay(string questionName)
        {
            ImageDisplay d = (from x in list
                              where x.Text.Contains(questionName)
                              select x).First();
            return d;
        }

        public static void RegisterImageDisplay(ImageDisplay d)
        {
            list.Add(d);
        }

        public static void DeregisterImageDisplay(ImageDisplay d)
        {
            list.Remove(d);
        }

        public static void Reset()
        {
            list = new List<ImageDisplay>();
        }
    }
}
