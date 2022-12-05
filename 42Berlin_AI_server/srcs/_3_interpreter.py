from utils_debug import *
from utils_constants import *
import json
from service_open_ai import ServiceOpenAI
from service_open_weather import ServiceOpenWeather
from service_deep_face import ServiceDeepFace
from utils_colors import Colors
from google.protobuf.json_format import MessageToDict
import os
import shutil

class Interpreter:
    def __init__(self, config):
        self.config = config

    def create_response_as_json(self, response_message, response_type=ResponseTypes.MESSAGE.value, response_author=AI_NAME, payload={}):
        response = {
            "type": response_type,
            "author": response_author,
            "string": response_message,
            "payload": payload
        }
        response_as_json = json.dumps(response)
        return response_as_json

############################### OTHER GENERATION FUNCTIONS ##########################################

    def generate_LOOP(self, request, config):
        print_debug(f"{__name__}, generate_LOOP", P_FUNCTION)
        return self.create_response_as_json("Oops, we are in a loop. I am saving your API calls budget", response_author="Static Bear")

    def generate_END(self, request, config):
        print_debug(f"{__name__}, generate_END", P_FUNCTION)
        return self.create_response_as_json(self.config['chat'].get_random_ending())

    def generate_ERROR(self, request, config):
        print_debug(f"{__name__}, generate_ERROR", P_FUNCTION)
        return self.create_response_as_json("Sorry, I didn't understand", response_type=ResponseTypes.ERROR.value)

############################### GENERATE ANSWER ##########################################

    def generate_ANSWER(self, request, config):
        print_debug(f"{__name__}, generate_ANSWER", P_FUNCTION)
        return self.create_response_as_json(ServiceOpenAI().get_gpt3_answer(config['chat_history']))

############################### GENERATE INTENT_FOUND ##########################################

    def parse_payload(self, intent_raw):
        payload = {}
        intent_dict = MessageToDict(intent_raw)
        for dic in intent_dict['queryResult']['fulfillmentMessages']:
            for k in dic:
                if (k == 'payload'):
                    payload = dic[k]
        payload_sorted = dict(sorted(payload.items()))
        chunks = []
        response_type = ResponseTypes.MESSAGE.value
        for k in payload_sorted :
            if (k.startswith('chunk')):
                chunks.append(payload_sorted[k])
            elif (k == "type"):
                response_type = payload_sorted[k]['value']
        # for chunk in chunks:
            # print_color(chunk, Colors.B_BLUE)
        return chunks, response_type

    def get_full_text(self, chunks):
        text = ""
        for chunk in chunks:
            text += chunk['text'] + " "
        print_color(text, Colors.B_PINK)
        return text

    def generate_media_response(self, chunks, response_type):
        text = self.get_full_text(chunks)
        return self.create_response_as_json(text, response_type=response_type, payload=chunks)

    def generate_INTENT_FOUND(self, request, config):
        print_debug(f"{__name__}, generate_INTENT_FOUND", P_FUNCTION)
        intent = self.config['intent']
        if (intent == None):
            print_warning("intent is None")
            return self.create_response_as_json("Sorry, I didn't understand", response_type=ResponseTypes.ERROR.value)
        text = intent.query_result.fulfillment_text
        chunks, response_type = self.parse_payload(intent)
        if (response_type == ResponseTypes.PHOTO_BACKGROUND.value
         or response_type == ResponseTypes.VIDEO_BACKGROUND.value
         or response_type == ResponseTypes.VIDEO_ONLY.value):
            return self.generate_media_response(chunks, response_type)
        return self.create_response_as_json(text)
    
############################### GENERATE WEATHER ##########################################

    def generate_WEATHER(self, request, config):
        print_debug(f"{__name__}, generate_WEATHER", P_FUNCTION)
        return self.create_response_as_json(ServiceOpenWeather().process_weather_request(config['request_intent']))
    
############################### GENERATE REMEMBER_ME ##########################################

    def extract_name(self, intent):
        intent_dict = MessageToDict(intent)
        name = intent_dict['queryResult']['parameters']['name']['name']
        return name.capitalize()

    def get_next_unique_id(self):
        db_dirs = next(os.walk('./' + FACE_DB_PATH))[1]
        max_id = 0
        for dir in db_dirs:
            if dir.isdigit():
                if int(dir) > max_id:
                    max_id = int(dir)
        return max_id + 1
            
    def save_picture(self, name):
        src_picture = FACE_TMP_PATH + "/" + FACE_BASENAME + str(0) + "." + FACE_EXTENSION
        unique_id = self.get_next_unique_id()
        os.mkdir(os.path.join(".", FACE_DB_PATH, str(unique_id)))
        dst_picture = os.path.join(".", FACE_DB_PATH, str(unique_id), str(name + "0." + FACE_EXTENSION))
        shutil.copy(src_picture, dst_picture)
        return

    def generate_REMEMBER_ME(self, request, config):
        print_debug(f"{__name__}, generate_REMEMBER_ME", P_FUNCTION)
        text = config['request_intent'].query_result.fulfillment_text
        name = self.extract_name(config['request_intent'])
        self.save_picture(name)
        return self.create_response_as_json(text)

############################### GENERATE START ##########################################
    
    def generate_starter(self, names):
        print_debug(f"{__name__}, generate_starter", P_FUNCTION)
        rand_starter = self.config['chat'].get_random_starter()
        rand_greeting = self.config['chat'].get_random_greeting()
        names_string = ""
        for name in names:
            names_string += name + ", "
        starter = rand_greeting + " " + names_string + rand_starter
        return starter

    def get_recognized_faces_names(self, request):
        print_debug(f"{__name__}, get_recognized_faces_names", P_FUNCTION)
        if (GREETING_NAME == True):
            names = ServiceDeepFace(request['string']).face_recognition_engine()
        else:
            names = [""]
        return names

    def generate_START(self, request, config):
        print_debug(f"{__name__}, generate_START", P_FUNCTION)
        names = self.get_recognized_faces_names(request)
        resp = self.generate_starter(names)
        return self.create_response_as_json(resp)

############################### GENERAL GENERATION ##########################################

    def generate_response(self, request, req_type, config):
        print_debug(f"{__name__}, generate_response", P_FUNCTION)
        resp_generators = [
            {
                'type': RequestTypes.START,
                'func': self.generate_START,
            },
            {
                'type': RequestTypes.LOOP,
                'func': self.generate_LOOP,
            },
            {
                'type': RequestTypes.NO_INTENT,
                'func': self.generate_ANSWER,
            },
            {
                'type': RequestTypes.ERROR,
                'func': self.generate_ERROR,
            },
            {
                'type': RequestTypes.WEATHER,
                'func': self.generate_WEATHER,
            },
            {
                'type': RequestTypes.REMEMBER_ME,
                'func': self.generate_REMEMBER_ME,
            },
            {
                'type': RequestTypes.INTENT_FOUND,
                'func': self.generate_INTENT_FOUND,
            },
            {
                'type': RequestTypes.END,
                'func': self.generate_END,
            },
        ]
        for resp_gen in resp_generators:
            if resp_gen['type'] == req_type:
                func = resp_gen['func']
                resp = func(request, config)
                return resp
        print_error("ERROR, reponse type not found")
        return