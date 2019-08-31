# Image Analyser


[![Build Status](https://travis-ci.org/joemccann/dillinger.svg?branch=master)](https://travis-ci.org/joemccann/dillinger)

Image Analyser is test case app that shows how to integrate with AWS Rekognition service using .net core framework.
This app shows how to:

  - Detect faces in an image
  - Detect objects and scenes in an image
  - Detect Text in an image
  - Detect objects and scenes in an video

It does also:
  - Export a log file with the raw response for the selected function

### Tech

Image Analyser uses the following tech:

* [.Net Core] - Your beloved server side framework provided by Microsoft and the open source community.
* [AWS Amazon Rekognition] - Awesome image/video recognition service provided by AWS!
* [Newtonsoft Json] - Popular high-performance JSON framework for .NET.

### Installation

Image Analyser requires [.Net Core 2.1] to run.
Also you'll need an [AWS account set up] with permission to use Rekognition service.

Inside Program.cs file on line 22 replace the following with your AWS Credentials and region:
- YOUR-AWS-ACCESS-KEY
- YOUR-AWS-SECRET-ACCESS-KEY
- RegionEndpoint.CNNorth1

Install the dependencies and build the app.
```sh
$ cd amazon-rekognition-test-case
$ dotnet build
```

Finally, run the app

```sh
$ dotnet run
```

### Usage

The Console app will prompt the following options:

1 - Detect Faces
2 - Detect Objects and Scenes
3 - Detect Text
4 - Start Detecting Objects and Scenes on video
5 - Get Detected Objects and Scenes on video

Just type the number of the desired option.

Options 1-3 will require you to provide the image's full path in order to process.
Option 4 will require the S3 bucket name and file name to generate a JobId wich should be used when requesting for Option 5.

### Issues/Questions
I Hope this project can help Software Engineers that need to quickly setup integrations with Rekognition.
Feel free to create issues with problems or questions you may have with the app or send me a direct message.

[//]: # (These are reference links used in the body of this note and get stripped out when the markdown processor does its job. There is no need to format nicely because it shouldn't be seen. Thanks SO - http://stackoverflow.com/questions/4823468/store-comments-in-markdown-syntax)


   [.Net Core]: <https://dotnet.microsoft.com/download/dotnet-core>
   [AWS Amazon Rekognition]: <https://aws.amazon.com/rekognition>
   [Newtonsoft Json]: <https://www.newtonsoft.com/json>
   [.Net Core 2.1]: <https://dotnet.microsoft.com/download/dotnet-core/2.1>
   [AWS account set up]: <https://aws.amazon.com/premiumsupport/knowledge-center/create-and-activate-aws-account/>
   
