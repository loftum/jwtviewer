using System.Collections.Generic;

namespace JwtViewer.IO
{
    public class Settings
    {
        public List<string> Authorities { get; set; }

        public Settings()
        {
            Authorities = new List<string>
            {
            };
        }
    }
}