# DescribePicture
F# Sample Using Azure Cognitive Services
----------------------------------------
This is a very simple example on how to access Azure Cognitive Services from F#. The REST API calls are made using the Json type provider from FSharp.Data.
  * Learning.fsx shows how to do it interactively from an script file
  * Program.fs shows how to do it from a command-line executable

Both files:
  1. Take an image (.JPG or .PNG file) and submit it to the Vision API in Azure Cognitive Services to get a text description
  2. Submit the text description to the Translator service in Azure Market Place to get an Spanish translation
  3. Submit the translated text to the Text-to-Speech service in Azure Market Plate to get an audio file
  4. Play the audio file
