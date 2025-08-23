from google import genai
from google.genai import types
from PIL import Image
from io import BytesIO
import sys
import requests
import warnings
warnings.simplefilter('ignore')


imagePath = sys.argv[1]
prompt = sys.argv[2]
callBack = sys.argv[3]
import PIL.Image
# C:\\Users\\ovram\\Downloads\\WhatsApp Image 2025-07-12 at 15.43.32_0281aba3.jpg
# C:\Users\ovram\Downloads\WhatsApp_Image_2025-07-12_at_15.43.32_0281aba3.jpg
# "C:\Users\ovram\Downloads\garage.jpg"
image = PIL.Image.open(imagePath)

client = genai.Client(api_key="AIzaSyC0syEHyOybeM-TC6Nrg2AP-cFqtZ7NXDU")

text_input = (prompt)

response = client.models.generate_content(
    model="gemini-2.0-flash-preview-image-generation",
    contents=[text_input, image],
    config=types.GenerateContentConfig(
      response_modalities=['TEXT', 'IMAGE']
    )
)

for part in response.candidates[0].content.parts:
  if part.text is not None:
    print(part.text)
  elif part.inline_data is not None:
    image = Image.open(BytesIO((part.inline_data.data)))
    image.save(imagePath)
    #C:\\Users\\ovram\\Downloads\\output2.jpg


    result = {
    "status": "success",
    "instruction": prompt,
    "input_image": imagePath
}

    files = {"file": open(imagePath, "rb")}
    data = {"status": "success", "instruction": prompt}

resp = requests.post(callBack, files=files, data=data)
print(resp.status_code, resp.text)
