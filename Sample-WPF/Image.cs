using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisionAPI_WPF_Samples
{
    public class ImageData
    {
        public ImageData()
        {
            confidenceByTag = new Dictionary<string, double>();
        }
        public string imageId;
        public string imageUrl;
        public Dictionary<string, double> confidenceByTag;
        public double score;
    }
}
