using Microsoft.AspNetCore.Identity;
using Microsoft.Azure.CognitiveServices.Vision.Face.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FaceRecognition.Models
{
    public class FaceRecognitionModel 
    {
       public ImageDetailModel oImageDetail { get; set; }
       public FaceListModel oFaceList { get; set; }
    }

    public class ImageDetailModel
    {
        public string m_imageurl { get; set; }
        public string[] m_facerecognitionparam { get; set; }
    }

    public class FaceListModel
    {
        public IList<DetectedFace> m_faceListmodel { get; set; }
    }
}
