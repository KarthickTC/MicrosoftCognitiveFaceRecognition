using Microsoft.Azure.CognitiveServices.Vision.Face;
using Microsoft.Azure.CognitiveServices.Vision.Face.Models;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Drawing;
using FaceRecognition.Models;
using Newtonsoft.Json;

namespace FaceRecognition.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }


        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            var filename = Path.GetFileName(file.FileName);
            var filePath = Path.Combine(
                     Directory.GetCurrentDirectory(), "wwwroot","images",
                     filename);

            if (file.Length > 0)
            {
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }
            }

            var ImageData = new FaceRecognitionModel();

            FaceRecognitionProcess oProcess = new FaceRecognitionProcess();
            ImageData = await oProcess.DetectFace(filePath);
            TempData["FaceList"] = JsonConvert.SerializeObject(ImageData.oFaceList.m_faceListmodel);
            

            return View("Index", ImageData.oImageDetail);
        }


        public JsonResult FaceRecognitionResult(string xcoordinate, string ycoordinate)
        {
            IList<DetectedFace> faceList = JsonConvert.DeserializeObject<IList<DetectedFace>>(TempData.Peek("FaceList").ToString());
            String[] faceDescriptions;
            double xcoordinate_val = Convert.ToDouble(xcoordinate);
            double ycoordinate_val = Convert.ToDouble(ycoordinate);
            string face_desc = "";
            if (faceList.Count > 0)
            {
                faceDescriptions = new String[faceList.Count];
                for (int i = 0; i < faceList.Count; ++i)
                {
                    DetectedFace face = faceList[i];
                    FaceRecognitionProcess oProcess = new FaceRecognitionProcess();
                    faceDescriptions[i] = oProcess.FaceDescription(face);

                    // Display the face description if the mouse is over this face rectangle.
                    if (xcoordinate_val >= face.FaceRectangle.Left && xcoordinate_val <= face.FaceRectangle.Left + face.FaceRectangle.Width &&
                        ycoordinate_val >= face.FaceRectangle.Top && ycoordinate_val <= face.FaceRectangle.Top + face.FaceRectangle.Height)
                    {
                        face_desc = faceDescriptions[i];
                        break;
                    }



                }
            }
            return Json(face_desc);
        }
    }
}