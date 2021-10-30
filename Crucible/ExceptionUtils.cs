namespace RoboPhredDev.PotionCraft.Crucible
{
    using System;
    using System.Text;

    public static class ExceptionUtils
    {
        public static string ToExpandedString(this Exception ex)
        {
            var sb = new StringBuilder();
            while (true)
            {
                sb.AppendLine(ex.Message);
                if (ex.InnerException == null)
                {
                    sb.AppendLine(ex.StackTrace);
                    break;
                }

                ex = ex.InnerException;
            }

            return sb.ToString();
        }
    }
}