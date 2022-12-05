from utils_constants import *
from utils_colors import Colors

print_types = {
    P_DEFAULT : {
        "color": Colors.ORANGE,
        "active": True,     
    },
    P_SERVER : {
        "color": Colors.BLUE,
        "active": True,     
    },
    P_REQREP : {
        "color": Colors.GREEN,
        "active": True,     
    },
    P_FUNCTION : {
        "color": Colors.BROWN,
        "active": False,     
    },
    P_HIDDEN : {
        "color": Colors.GREY,
        "active": True,     
    },
    P_CONFIG : {
        "color": Colors.GREY,
        "active": False,     
    },
    P_TIME : {
        "color": Colors.B_YELLOW,
        "active": False,     
    },
    P_FACE : {
        "color": Colors.B_ORANGE,
        "active": False,     
    },}

def print_color(message, color=Colors.YELLOW):
    if (VERBOSE == True):
        print(color, message, Colors.END)

def print_debug(message, name=P_DEFAULT):
    if (VERBOSE == True):
        for t in print_types.items(): 
            if (t[0] == name):
                if (t[1]['active'] == True):
                    print_color(message, t[1]['color'])

def print_error(message, color=Colors.ERROR):
    if (PRINT_ERROR == True):
        print(color, "Error: " + Colors.B_WHITE + message + Colors.END)

def print_warning(message, color=Colors.WARNING):
    if (PRINT_WARNING == True):
        print(color, "Warning: " + Colors.B_WHITE + message + Colors.END)
