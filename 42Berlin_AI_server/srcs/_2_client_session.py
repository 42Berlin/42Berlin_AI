from utils_debug import *
from utils_constants import *
from threading import Lock
from _3_interpreter import Interpreter
from dotenv import load_dotenv
import dialogflow
from google.api_core.exceptions import InvalidArgument
from utils_chat import Chat
import os
import json
from json import JSONDecodeError
from utils_exceptions import *
import time
from utils_timer import Timer
from utils_colors import Colors

load_dotenv()
os.environ["GOOGLE_APPLICATION_CREDENTIALS"] = 'private_key.json'
DIALOGFLOW_PROJECT_ID = os.getenv('DIALOGFLOW_PROJECT_ID')
DIALOGFLOW_LANGUAGE_CODE = os.getenv('DIALOGFLOW_LANGUAGE_CODE')
SESSION_ID = os.getenv('SESSION_ID')
GENERATE_IMAGE_FOLLOWUP="projects/weather-bot-gsvl/agent/sessions/me/contexts/generate-image-followup"
print_debug(f"GOOGLE_APPLICATION_CREDENTIALS: {os.environ['GOOGLE_APPLICATION_CREDENTIALS']}", P_CONFIG)
print_debug(f"DIALOGFLOW_PROJECT_ID: {DIALOGFLOW_PROJECT_ID}", P_CONFIG)
print_debug(f"DIALOGFLOW_LANGUAGE_CODE: {DIALOGFLOW_LANGUAGE_CODE}", P_CONFIG)
print_debug(f"SESSION_ID: {SESSION_ID}", P_CONFIG)
print_debug(f"GENERATE_IMAGE_FOLLOWUP: {GENERATE_IMAGE_FOLLOWUP}", P_CONFIG)

class ClientSession:
    def __init__(self, connection, address):
        self.config = {
            "connection": connection,
            "address": address,
            "dialogflow_session": dialogflow.SessionsClient(),
            "lock": Lock(),
            "chat": Chat(),
            "chat_history": None,
            "request_intent": None,
            "timer": Timer(),
            "intent": None,
        }
        self.config['chat_history'] = self.config['chat'].initial_feeding
        self.response = None
        self.interpreter = Interpreter(self.config)

    def send_response(self, response):
        print_debug(f"{__name__}, send_response", P_FUNCTION)
        self.config['lock'].acquire()
        self.config['connection'].sendall(response.encode())
        self.config['lock'].release()

    def close_socket(self):
        print_debug(f"{__name__}, close_socket", P_FUNCTION)
        if self.config['connection']:
            print_debug("[CLIENT SESSION]: closing socket", P_SERVER)
            self.config['connection'].close()

############################### UPDATE CHAT_HISTORY ##################################

    def add_req_to_chat_history(self, chat, message):
        self.config['chat_history'] = self.config['chat_history'] + chat.start_text_human + message

    def add_resp_to_chat_history(self, chat, message):
        self.config['chat_history'] = self.config['chat_history'] + chat.start_text_bear + message

############################### GET_REQUEST ##########################################

    def get_request(self):
        print_debug(f"{__name__}, get_request", P_FUNCTION)
        request = b''
        while True:
            data_chunk = self.config['connection'].recv(REQ_BUFF_SIZE)
            request += data_chunk
            if len(data_chunk) < REQ_BUFF_SIZE: # either 0 or end of data
                break
        if not request:
            raise ClientFatalException("the client is not reachable (he probably disconnected).")
        formatted_request = json.loads(request)
        self.check_request_format(formatted_request)
        self.monitor_speed(formatted_request)
        return formatted_request

############################### FIND REQUEST TYPE ##########################################

    def get_dialogflow_intent(self, request):
        print_debug(f"{__name__}, get_dialogflow_intent", P_FUNCTION)
        try:
            session_path = self.config['dialogflow_session'].session_path(DIALOGFLOW_PROJECT_ID, SESSION_ID)
            text_input = dialogflow.types.TextInput(text=request['string'], language_code=DIALOGFLOW_LANGUAGE_CODE)
            query_input = dialogflow.types.QueryInput(text=text_input)
            intent = self.config['dialogflow_session'].detect_intent(session=session_path, query_input=query_input)
        except InvalidArgument:
            raise RequestException("in dialogflow intent (request message string may be empty)")
        except Exception as e:
            raise RequestException("in dialogflow intent (" + e + ")")
        return intent

    def get_answer_type(self, request):
        print_debug(f"{__name__}, get_answer_type", P_FUNCTION)
        intent = self.get_dialogflow_intent(request)
        print_debug(f"{intent}, get_answer_type", P_HIDDEN)
        intent_name = intent.query_result.intent.display_name
        intent_confidence = intent.query_result.intent_detection_confidence
        intent_contexts = intent.query_result.output_contexts
        self.config['request_intent'] = intent

        contexts_nbr = 0
        for context in intent_contexts:
            contexts_nbr += 1
        if intent_name == "Default Fallback Intent" and contexts_nbr < 2:
            return RequestTypes.NO_INTENT
        elif intent_name == "get-weather" and intent_confidence >= 0.9:
            return RequestTypes.WEATHER
        elif intent_name == "generate-image" and intent_confidence >= 0.9:
            return RequestTypes.IMG_GEN_1_REQUEST
        elif intent_name == "Default Fallback Intent" and intent_contexts and contexts_nbr == 2 and (intent_contexts[0].name == GENERATE_IMAGE_FOLLOWUP or intent_contexts[1].name == GENERATE_IMAGE_FOLLOWUP) :
            return RequestTypes.IMG_GEN_2_PROMPT
        elif intent_name == "remember-me-followup" and intent_confidence >= 0.8:
            return RequestTypes.REMEMBER_ME
        elif intent_confidence >= 0.8:
            self.config['intent'] = intent
            return RequestTypes.INTENT_FOUND
        else:
            return RequestTypes.ERROR

    def check_request_format(self, request):
        print_debug (f"{__name__}, check_request_format", P_FUNCTION)
        try:
            request['author']
            request['type']
            request['string']
        except KeyError:
            raise RequestException("(request message is empty or missing fields)")
            
    def monitor_speed(self, request):
        print_debug(f"{__name__}, monitor_speed", P_FUNCTION)
        try:
            print_debug(f"Ideal end-of-speech {request['EOS_ideal_ts']}", P_TIME)
            print_debug(f"Final end-of-speech {request['EOS_final_ts']}", P_TIME)
            ideal_ts = self.config["timer"].convert_str_to_datetime(request['EOS_ideal_ts'])
            final_ts = self.config["timer"].convert_str_to_datetime(request['EOS_final_ts'])
            self.config["timer"].set_ideal_EOS(ideal_ts)
            self.config["timer"].set_final_EOS(final_ts)            
            print_debug(f"Difference :  {final_ts - ideal_ts}")
        except Exception as e:
            print_debug(f"End-of-speech not received : {e}", P_TIME)
            pass
        
    def find_request_type(self, request):
        print_debug(f"{__name__}, get_response_type", P_FUNCTION)
        self.config['request_intent'] = None
        if (request['author'] == 'Client_loop'):
            time.sleep(1.0) # To recreate openai delay
            return RequestTypes.LOOP
        elif (request['type'] == RequestTypes.START.value):
            self.config['chat_history'] = self.config['chat'].initial_feeding
            return RequestTypes.START
        elif (request['type'] == RequestTypes.ANSWER.value):
            self.add_req_to_chat_history(self.config['chat'], request['string'])
            return self.get_answer_type(request)
        elif (request['type'] == RequestTypes.END.value):
            return RequestTypes.END
        else:
            print_warning(f"client request type not recognized. Generating a default response.")
            return RequestTypes.ERROR

############################### PRINT_DEBUG ##########################################

    def print_discussion(self, request, req_type, response):
        print_debug(f"Request type is {req_type.name}", P_HIDDEN)
        if (req_type is not RequestTypes.START):
            print_debug(f"Request : {request}", P_REQREP)
        else:
            print_debug(f"Request : first connection (image)", P_REQREP)
        print_debug(f"Response : {response}", P_REQREP)


############################### MAIN ACTIONS ##########################################
    def interact(self):
        print_debug(f"{__name__}, interact", P_FUNCTION)
        while True:
            try:
                print_debug("[CLIENT SESSION]: waiting for request", P_SERVER)
                request  = self.get_request()
                req_type = self.find_request_type(request)
                response = self.interpreter.generate_response(request, req_type, self.config)
                self.print_discussion(request, req_type, response)
                self.add_resp_to_chat_history(self.config['chat'], json.loads(response)['string'])
                self.send_response(response)
            except JSONDecodeError as e:
                print_error(f"request cannot be decoded as JSON.")
            except RequestException as e:
                print_error(f"request not understood {e}")
            except ClientException as e:
                print_error(f"in client session. {e}")
            except TimerException as e:
                print_warning(f"in timer. {e}")
            except OSError as e:
                print_error(f"client socket. Closing client. {str(e)}")
                self.close_socket()
                break
