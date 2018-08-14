using Microsoft.Azure.CognitiveServices.Vision.Face;
using Microsoft.Azure.CognitiveServices.Vision.Face.Models;

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FaceRecognition.Models
{
    public class FaceRecognitionProcess
    {
        //provide your subscription key
        private const string subscriptionKey = "";

        private const string baseUri =
            "https://southeastasia.api.cognitive.microsoft.com/face/v1.0";

        private readonly IFaceClient faceClient = new FaceClient(
    new ApiKeyServiceClientCredentials(subscriptionKey),
    new System.Net.Http.DelegatingHandler[] { });

        IList<DetectedFace> faceList;   // The list of detected faces.
        String[] faceDescriptions;      // The list of descriptions for the detected faces.

        public FaceRecognitionProcess()
        {
            if (Uri.IsWellFormedUriString(baseUri, UriKind.Absolute))
            {
                faceClient.BaseUri = new Uri(baseUri);
            }
            else
            {
                throw new ApplicationException(baseUri + " Invalid URI");
            }
        }

        public async Task<FaceRecognitionModel> DetectFace(string filePath)
        {
            // Detect any faces in the image.
            faceList = await UploadAndDetectFaces(filePath);
          
            string imagefullname = "";
            string imagewithrect = "";
            string imagewithrectPath = "";
            if (faceList.Count > 0)
            {
                faceDescriptions = new String[faceList.Count];
                Image image = Image.FromFile(filePath);
                for (int i = 0; i < faceList.Count; ++i)
                {
                    DetectedFace face = faceList[i];

                    // Store the face description.
                    faceDescriptions[i] = FaceDescription(face);

                    /*****START-----BELOW SECTION JUST CROPS THE FACE, IT IS NOT NEEDED FOR THE GENERAL WORKFLOW
                    var croppedImg = "FullFace_" +  Convert.ToString(Guid.NewGuid()) + ".jpeg" as string;
                    var croppedImgPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", croppedImg as string);
                    var croppedImgFullPath = croppedImgPath as string;
                    Bitmap CroppedFace = null;
                    CroppedFace = CropBitmap(
                                        (Bitmap)Image.FromFile(filePath),
                                        face.FaceRectangle.Left,
                                        face.FaceRectangle.Top,
                                        face.FaceRectangle.Width,
                                        face.FaceRectangle.Height);
                    CroppedFace.Save(croppedImgFullPath, ImageFormat.Jpeg);
                    if (CroppedFace != null)
                        ((IDisposable)CroppedFace).Dispose();
                        ------END******/
                    
                    //draw a rectangle 
                    using (Graphics g = Graphics.FromImage(image))
                    {
                        g.DrawRectangle(new Pen(Brushes.Red, 4), new Rectangle(face.FaceRectangle.Left
                            , face.FaceRectangle.Top, face.FaceRectangle.Width, face.FaceRectangle.Height));
                    }
                    

                }

                imagewithrect = "Rect_" + Convert.ToString(Guid.NewGuid()) + ".jpeg" as string;
                imagewithrectPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", imagewithrect as string);

                image.Save(imagewithrectPath);

                imagefullname = imagewithrect;
            }
            var ImageData = new ImageDetailModel();
            ImageData.m_imageurl = imagefullname;
            ImageData.m_facerecognitionparam = faceDescriptions;

            var FaceListData = new FaceListModel();
            FaceListData.m_faceListmodel = faceList;

            var oFaceRecognitionModel = new FaceRecognitionModel();
            oFaceRecognitionModel.oImageDetail = ImageData;
            oFaceRecognitionModel.oFaceList = FaceListData;
            return oFaceRecognitionModel;
        }

        // Uploads the image file and calls DetectWithStreamAsync.
        private async Task<IList<DetectedFace>> UploadAndDetectFaces(string imageFilePath)
        {
            // The list of Face attributes to return.
            IList<FaceAttributeType> faceAttributes =
                new FaceAttributeType[]
                {
            FaceAttributeType.Gender, FaceAttributeType.Age,
            FaceAttributeType.Smile, FaceAttributeType.Emotion,
            FaceAttributeType.Glasses, FaceAttributeType.Hair
                };

            // Call the Face API.
            try
            {
                using (Stream imageFileStream = File.OpenRead(imageFilePath))
                {
                    // The second argument specifies to return the faceId, while
                    // the third argument specifies not to return face landmarks.
                    IList<DetectedFace> faceList =
                        await faceClient.Face.DetectWithStreamAsync(
                            imageFileStream, true, false, faceAttributes);
                    return faceList;
                }
            }
            // Catch and display Face API errors.
            catch (APIErrorException f)
            {
                return new List<DetectedFace>();
            }
            // Catch and display all other errors.
            catch (Exception e)
            {
                return new List<DetectedFace>();
            }
        }

        // Creates a string out of the attributes describing the face.
        public string FaceDescription(DetectedFace face)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("Face: ");

            // Add the gender, age, and smile.
            sb.Append(face.FaceAttributes.Gender);
            sb.Append(", ");
            sb.Append(face.FaceAttributes.Age);
            sb.Append(", ");
            sb.Append(String.Format("smile {0:F1}%, ", face.FaceAttributes.Smile * 100));
            sb.Append(Environment.NewLine);
            // Add the emotions. Display all emotions over 10%.
            sb.Append("Emotion: ");
            Emotion emotionScores = face.FaceAttributes.Emotion;
            if (emotionScores.Anger >= 0.1f)
                sb.Append(String.Format("anger {0:F1}%, ", emotionScores.Anger * 100));
            if (emotionScores.Contempt >= 0.1f)
                sb.Append(String.Format("contempt {0:F1}%, ", emotionScores.Contempt * 100));
            if (emotionScores.Disgust >= 0.1f)
                sb.Append(String.Format("disgust {0:F1}%, ", emotionScores.Disgust * 100));
            if (emotionScores.Fear >= 0.1f)
                sb.Append(String.Format("fear {0:F1}%, ", emotionScores.Fear * 100));
            if (emotionScores.Happiness >= 0.1f)
                sb.Append(String.Format("happiness {0:F1}%, ", emotionScores.Happiness * 100));
            if (emotionScores.Neutral >= 0.1f)
                sb.Append(String.Format("neutral {0:F1}%, ", emotionScores.Neutral * 100));
            if (emotionScores.Sadness >= 0.1f)
                sb.Append(String.Format("sadness {0:F1}%, ", emotionScores.Sadness * 100));
            if (emotionScores.Surprise >= 0.1f)
                sb.Append(String.Format("surprise {0:F1}%, ", emotionScores.Surprise * 100));

            // Add glasses.
            sb.Append(face.FaceAttributes.Glasses);
            sb.Append(Environment.NewLine);

            // Add hair.
            sb.Append("Hair: ");

            // Display baldness confidence if over 1%.
            if (face.FaceAttributes.Hair.Bald >= 0.01f)
                sb.Append(String.Format("bald {0:F1}% ", face.FaceAttributes.Hair.Bald * 100));

            // Display all hair color attributes over 10%.
            IList<HairColor> hairColors = face.FaceAttributes.Hair.HairColor;
            foreach (HairColor hairColor in hairColors)
            {
                if (hairColor.Confidence >= 0.1f)
                {
                    sb.Append(hairColor.Color.ToString());
                    sb.Append(String.Format(" {0:F1}% ", hairColor.Confidence * 100));
                }
            }
            sb.Append(Environment.NewLine);
            // Return the built string.
            return sb.ToString();
        }
    }
}
