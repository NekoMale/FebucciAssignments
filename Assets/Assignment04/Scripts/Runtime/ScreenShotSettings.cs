using System;

namespace Core.Assignment04
{
    public static class ScreenShotSettings
    {
        /// <summary>
        /// Screenshots' ratio
        /// </summary>
        public static float Ratio => 16f / 9f;
        /// <summary>
        /// Screenshots' ratio label
        /// </summary>
        public static string RatioDisplay => "16:9";
        /// <summary>
        /// Screenshots saved at path
        /// </summary>
        public static string Path => System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures), "Assignment/");
    }
}