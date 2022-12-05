# 42 Berlin AI - Quickstart

The goal of this project is to explore, test and understand the future of human-AI interactions, combining multiple sensory inputs and incorporating the latest technologies in generative AI and the visual arts.
We make our developments openly available, to build a community of practice with fellows and students to shape and research the future of AI.

### Prerequisites
- [Unity v2021.3.1f1](https://unity3d.com/get-unity/download/archive)
- [Tensorflow](https://developer.apple.com/metal/tensorflow-plugin/)
- [Python](https://www.python.org/downloads/)

### Requirements
- API Key for [OpenAI](https://beta.openai.com/account/api-keys)
- API Key for [Google Cloud](https://cloud.google.com/docs/authentication/api-keys#creating_an_api_key), allowing Speech-to-text and Text-to-speech
- API Key for [Dialogflow](https://cloud.google.com/dialogflow/es/docs/quick/build-agent)
- API Key for [OpenWeatherMap](https://home.openweathermap.org/api_keys)
- [OpenCV for Unity](https://assetstore.unity.com/packages/tools/integration/opencv-for-unity-21088) (you can get a free demo)

### Setup

#### Installations
1. Clone this repository

1. Create a new virtual environment from the server folder and activate it
   ```bash
   cd 42BearPlayground/42Berlin_AI_server;
   python -m venv venv;
   . venv/bin/activate;
   ```
2. Install the requirements, making sure you uncomment the correct lines according to your operating system
   ```bash
   pip install -r requirements.txt 
   ```

3. For MacOs only : install deepface
   ```bash
   pip install deepface --no-deps
   ```

#### Environment variables

1. For both folders (42Berlin_AI and 42Berlin_AI_server), make a copy of the example .env file and fill it with your secret keys

   ```bash
   cp .env.example .env
   ```
2. Add your Google cloud JSON file for Dialogflow as `42Berlin_AI_server/private_key.json`

3. Add your Google cloud JSON file for Speech-to-text and Text-to-speech in `42Berlin_AI/Assets/Resources/private_key.json`

4. Add a file containing some speech context (one sentence per line) as `42Berlin_AI/Assets/FrostweepGames/StreamingSpeechRecognition/speechContext.txt`

#### Launch

1. In 42Berlin_AI_server/, run the server

   ```bash
   sh launch.sh
   ```

2. Open and run the 42Berlin_AI project on Unity

# Useful links
- [OpenAI website](https://openai.com/) / [How GPT-3 works](https://jalammar.github.io/how-gpt3-works-visualizations-animations/)
- [Deepface repository](https://github.com/serengil/deepface) / [How faces recognition works using Deepface](https://sefiks.com/2020/05/01/a-gentle-introduction-to-face-recognition-in-deep-learning/)
