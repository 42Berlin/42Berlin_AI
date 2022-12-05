from enum import Enum
from utils_colors import Colors

# Server config
HOST            = 'localhost'
PORT            = 50007
REQ_BUFF_SIZE   = 4096

# AI NAME
AI_NAME = "B4T2"

# SHOULD THE AI GREETS THE PERSON WITH ITS NAME
GREETING_NAME = False

# Printing debug
P_DEFAULT   = "print_debug"
P_SERVER    = "print_server"
P_REQREP    = "print_reqrep"
P_FUNCTION  = "print_function"
P_CONFIG    = "print_config"
P_TIME      = "print_time"
P_HIDDEN    = "print_hidden"
P_FACE      = "print_face"

# Printing
VERBOSE         = True
PRINT_ERROR     = True
PRINT_WARNING   = True

# FACE RECOGNITION
FACE_TMP_PATH  = "data/faces_shots"
FACE_BASENAME   = "img"
FACE_EXTENSION  = "jpg"
FACE_DB_PATH    = "data/faces_db"

# OPEN AI PROMPT
CHAT_BEGINNING  = 12 # change only if we change the chatbot prompt (utils_chat.py)
CHAT_END        = 4

class RequestTypes(Enum):
    START               = 'connect'
    ANSWER              = 'message'
    WEATHER             = 'weather'
    LOOP                = 'loop'
    END                 = 'end'
    ERROR               = 'error'
    IMAGE               = 'image'
    IMG_GEN_1_REQUEST   = 'img_gen_1_request'
    IMG_GEN_2_PROMPT    = 'img_gen_2_prompt'
    REMEMBER_ME         = 'remember_me'
    INTENT_FOUND        = 'intent_found'
    NO_INTENT           = 'no_intent'

class ResponseTypes(Enum):
    MESSAGE             = 'message'
    VIDEO_ONLY          = 'video_only'
    VIDEO_BACKGROUND    = 'video_background'
    PHOTO_BACKGROUND    = 'photo_background'
    ERROR               = 'error'