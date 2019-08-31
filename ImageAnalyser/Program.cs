using Amazon;
using Amazon.Rekognition;
using Amazon.Rekognition.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace ImageAnalyser
{
    class Program
    {
        static AmazonRekognitionClient _client;

        static void Main(string[] args)
        {
            try
            {
                //Replace YOUR-AWS-ACCESS-KEY, YOUR-AWS-SECRET-ACCESS-KEY and RegionEndpoint with your AWS Credentials and Region
                _client = new AmazonRekognitionClient("YOUR-AWS-ACCESS-KEY", "YOUR-AWS-SECRET-ACCESS-KEY", RegionEndpoint.CNNorth1);
                string picPath = string.Empty;
                bool running = true;

                while (running)
                {
                    Console.Clear();
                    Console.WriteLine("Type the desired option's number:");
                    Console.WriteLine("1 - Detect Faces");
                    Console.WriteLine("2 - Detect Objects and Scenes");
                    Console.WriteLine("3 - Detect Text");
                    Console.WriteLine("4 - Start Detecting Objects and Scenes on video");
                    Console.WriteLine("5 - Get Detected Objects and Scenes on video");
                    var input = Console.ReadLine();

                    if (input == "4")
                        DetectLabelsVideo();
                    else if (input == "5")
                    {
                        Console.WriteLine("Please provide JobId");
                        GetDetectedLabelsVideo(Console.ReadLine());
                    }
                    else
                    {
                        Console.WriteLine("Please provide picture's full path");
                        picPath = Console.ReadLine();
                        byte[] image = File.ReadAllBytes(picPath);
                        using (var stream = new MemoryStream(image))
                        {
                            if (int.TryParse(input, out int result))
                            {
                                switch (result)
                                {
                                    case 1:
                                        DetectFaces(stream);
                                        break;
                                    case 2:
                                        DetectLabels(stream);
                                        break;
                                    case 3:
                                        DetectText(stream);
                                        break;
                                    default:
                                        Console.WriteLine("Invalid input, Bye");
                                        break;
                                }
                            }
                            else
                                Console.WriteLine("Invalid input, Bye");
                        }
                    }
                    Console.WriteLine();
                    Console.WriteLine("Run again? Y/N");
                    var input2 = Console.ReadLine();
                    running = input2.ToLower() == "y" ? true : false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        static void DetectFaces(MemoryStream stream)
        {
            var response = _client.DetectFacesAsync(new DetectFacesRequest
            {
                Attributes = { "ALL" },
                Image = new Image { Bytes = stream }
            }).Result;

            Console.WriteLine($"Faces found: {response.FaceDetails.Count}");
            Console.WriteLine();
            int faceCounter = 1;
            foreach (var faceDetail in response.FaceDetails)
            {
                float emotionConfidence = 0;
                string emotionName = string.Empty;
                //Determines dominant emotion
                foreach (var emotion in faceDetail.Emotions)
                {
                    if (emotion.Confidence > emotionConfidence)
                    {
                        emotionConfidence = emotion.Confidence;
                        emotionName = emotion.Type;
                    }
                }

                Console.WriteLine($"Face {faceCounter}:");
                Console.WriteLine($"");
                Console.WriteLine($"Appears to be {faceDetail.Gender.Value}       {faceDetail.Gender.Confidence} %");
                if (faceDetail.Beard.Value)
                    Console.WriteLine($"Appears to have a beard                       {faceDetail.Beard.Confidence} %");
                if (faceDetail.Eyeglasses.Value)
                    Console.WriteLine($"Wearing glasses                               {faceDetail.Eyeglasses.Confidence} %");
                if (faceDetail.Sunglasses.Value)
                    Console.WriteLine($"Wearing Sunglasses                            {faceDetail.Sunglasses.Confidence} %");
                Console.WriteLine($"Age Range                                     {faceDetail.AgeRange.Low} - {faceDetail.AgeRange.High} old");
                Console.WriteLine($"Appears to be {emotionName}                   {emotionConfidence} %");

                Console.WriteLine();
                Console.WriteLine();

                faceCounter++;
            }

            LogResponse(GetIndentedJson(response), "DetectLabels");
        }
        static void DetectLabels(MemoryStream stream)
        {
            Console.WriteLine("Minimum confidence level? (0 - 100)");
            var minConfidence = float.Parse(Console.ReadLine());

            Stopwatch watch = new Stopwatch();
            watch.Start();
            var response = _client.DetectLabelsAsync(new DetectLabelsRequest
            {
                MinConfidence = minConfidence,
                MaxLabels = 100,
                Image = new Image
                {
                    Bytes = stream
                }
            }).Result;
            watch.Stop();
            Console.WriteLine($"Elapsed Time: { watch.Elapsed }");
            Console.WriteLine($"Objects and Scenes Found: {response.Labels.Count}:");
            Console.WriteLine();

            foreach (var label in response.Labels)
            {
                Console.WriteLine($"{label.Name}                 {label.Confidence} %");
            }

            LogResponse(GetIndentedJson(response), "DetectLabels");
        }
        static void DetectLabelsVideo()
        {
            Console.WriteLine("Minimum confidence level? (0 - 100)");
            var minConfidence = float.Parse(Console.ReadLine());

            Console.WriteLine("Bucket name:");
            var bucketName = Console.ReadLine();

            Console.WriteLine("File name:");
            var fileName = Console.ReadLine();

            var response = _client.StartLabelDetectionAsync(new StartLabelDetectionRequest
            {
                MinConfidence = minConfidence,
                Video = new Video
                {
                    S3Object = new S3Object
                    {
                        Bucket = bucketName,
                        Name = fileName
                    }
                }
            }).Result;

            Console.WriteLine($"JobId: {response.JobId}");

            LogResponse(GetIndentedJson(response), "DetectLabelsVideo");
        }
        static void GetDetectedLabelsVideo(string jobId)
        {
            var response = _client.GetLabelDetectionAsync(new GetLabelDetectionRequest
            {
                JobId = jobId
            }).Result;

            Console.WriteLine($"Job Status: {response.JobStatus}");
            Console.WriteLine($"Objects and Scenes Found: {response.Labels.Count}");
            Console.WriteLine();

            foreach (var label in response.Labels)
            {
                Console.WriteLine($"{label.Label.Name} at {label.Timestamp}        {label.Label.Confidence} %");
            }

            LogResponse(GetIndentedJson(response), "GetDetectedLabelsVideo");
        }
        static void DetectText(MemoryStream stream)
        {
            DetectTextRequest detectTextRequest = new DetectTextRequest()
            {
                Image = new Image()
                {
                    Bytes = stream
                }
            };

            DetectTextResponse response = _client.DetectTextAsync(detectTextRequest).Result;
            Console.WriteLine($"Texts Found: {response.TextDetections.Count}");
            Console.WriteLine();

            foreach (TextDetection text in response.TextDetections)
            {
                Console.WriteLine("text: " + text.DetectedText);
                Console.WriteLine("Confidence: " + text.Confidence);
                Console.WriteLine("Type: " + text.Type);
                Console.WriteLine();
            }

            Console.WriteLine();
            Console.WriteLine("JSON response:");
            Console.WriteLine();
            LogResponse(JsonConvert.SerializeObject(response, Formatting.Indented), "DetectText");

        }
        static void LogResponse(string text, string method)
        {
            string path = $@"..\..\..\..\{method}.txt";

            StreamWriter sw;

            if (!File.Exists(path))
                sw = File.CreateText(path);
            else
                sw = new StreamWriter(path);
            
            using (sw)
            {
                sw.WriteLine(text);
            }
        }

        static string GetIndentedJson(object response)
        {
            return JsonConvert.SerializeObject(response, Formatting.Indented);
        }
    }
}
