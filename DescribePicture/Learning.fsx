#r @"C:\EdgarProjects\FSharp\DescribePicture\packages\FSharp.Data.2.3.0\lib\net40\FSharp.Data.dll"

open FSharp.Data
open FSharp.Data.HttpRequestHeaders
open FSharp.Data.HttpContentTypes
open System.IO
open System.Media

type VisionApi = JsonProvider< @"C:\EdgarProjects\FSharp\DescribePicture\DescribePicture\visionApiResult.json" >

let visionApiRequest = "https://api.projectoxford.ai/vision/v1.0/analyze?visualFeatures=Description,Tags"

let res1 = 
    Http.RequestString
        (visionApiRequest, 
         headers = [ "ocp-apim-subscription-key", "PUT-HERE-YOUR-VISION-API-KEY"
                     ContentType Json ], 
         body = HttpRequestBody.TextRequest 
                    """{ "url": "https://turismomaso.files.wordpress.com/2014/07/quito-ecuador.jpg" }""")

let jsonObject1 = VisionApi.Parse res1

jsonObject1.Description.Tags

let bytes = File.ReadAllBytes @"PUT-HERE-THE-PATH-TO-AN-IMAGE-FILE"

let res2 = 
    Http.RequestString(visionApiRequest, 
                       headers = [ "ocp-apim-subscription-key", "PUT-HERE-YOUR-VISION-API-KEY"
                                   ContentType Binary ],
                       body = HttpRequestBody.BinaryUpload bytes)

let jsonObject2 = VisionApi.Parse res2

let pictureDescription = jsonObject2.Description.Captions.[0].Text

let res3 = 
    Http.RequestString("https://datamarket.accesscontrol.windows.net/v2/OAuth2-13", 
                       body = HttpRequestBody.FormValues [ "client_id", "PUT-HERE-YOUR-DATAMARKET-APPNAME"
                                                           "client_secret", "PUT-HERE-YOUR-TRANSLATOR-API-KEY"
                                                           "scope", "http://api.microsofttranslator.com"
                                                           "grant_type", "client_credentials" ])

type AuthToken = JsonProvider< @"C:\EdgarProjects\FSharp\VisionApi00\VisionApi00\translateTokenResult.json" >

let jsonToken = AuthToken.Parse res3

let translatedXml = 
    Http.RequestString("http://api.microsofttranslator.com/v2/Http.svc/Translate", 
                       query = [ "text", pictureDescription
                                 "from", "en"
                                 "to", "es" ],
                       headers = [ "Authorization", "Bearer " + jsonToken.AccessToken ])

type TranslationXml = XmlProvider< """<string xmlns="http://schemas.microsoft.com/2003/10/Serialization/">¡Hola mundo!</string>""" >

let translatedText = TranslationXml.Parse translatedXml

let res4 = 
    Http.Request("http://api.microsofttranslator.com/V2/Http.svc/Speak", 
                 query = [ "text", translatedText
                           "language", "es" ],
                 headers = [ "Authorization", "Bearer " + jsonToken.AccessToken ])

match res4.Body with
| HttpResponseBody.Text text -> printfn "Algo anda mal"
| HttpResponseBody.Binary bytes -> 
    use stream = new MemoryStream(bytes)
    use player = new SoundPlayer(stream)
    player.PlaySync()
