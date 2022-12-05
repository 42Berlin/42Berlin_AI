from datetime import datetime as dt
import os
import requests
import json
from dotenv import load_dotenv
from geopy.geocoders import Nominatim
from utils_debug import *
from utils_colors import Colors

load_dotenv()
OPEN_WEATHER_API_KEY = os.getenv('OPEN_WEATHER_API_KEY')
print_debug(f"OPEN_WEATHER_API_KEY: {OPEN_WEATHER_API_KEY}", P_CONFIG)

class ServiceOpenWeather:
    def __init__(self):
        pass
    
    def get_location(self, intent):
        geolocator = Nominatim(user_agent="weather-bot")
        items = intent.query_result.parameters.items()
        city = next(item for item in items if item[0] == "geo-city")[1]
        location = geolocator.geocode(city)
        return location

    def get_date(self, intent):
        items = intent.query_result.parameters.items()
        date = next(item for item in items if item[0] == "date-time")[1]
        if not date:
            date = dt.now()
        elif not isinstance(date, str):
            date_item = date.items()
            date = dt.strptime(date_item[0][1], "%Y-%m-%dT%H:%M:%S+02:00")
        else:
            date = dt.strptime(date, "%Y-%m-%dT%H:%M:%S+02:00")
        return date


    def get_weather_by_time(self, wanted_date, report, time):
        dt = wanted_date.timestamp()
        time_report = report[time]
        # print(time_report)
        for entry in time_report:
            if entry["dt"] >= dt:
                return entry

    def get_weather(self, location):
        open_weather_url = "https://api.openweathermap.org/data/2.5/onecall?lat=%s&lon=%s&appid=%s&units=metric" % \
                        (location.latitude, location.longitude, OPEN_WEATHER_API_KEY)
        response = requests.get(open_weather_url)
        report = json.loads(response.text)
        return report
    
    def get_current_weather_berlin():
        open_weather_url = "https://api.openweathermap.org/data/2.5/weather?q=berlin&appid=%s&units=metric" % \
                        (OPEN_WEATHER_API_KEY)
        response = requests.get(open_weather_url)
        report = json.loads(response.text)
        return report

    def format_and_get_response(self, date, delta, report):
        if delta.total_seconds() <= 172800:
            time = "hourly"
        elif delta.total_seconds() <= 691200: # until 8 days
            time = "daily"
        else:
            return "Ask to your phone"
        weather_report = self.get_weather_by_time(date, report, time)
        if (time == "hourly"):
            temp = str(weather_report["temp"])
        else:
            temp = str(weather_report["temp"]["day"])
        return date.strftime("%A %d %B %H hours %M") + ", we will have " + weather_report["weather"][0]["description"] + \
                ", with " + temp + " degrees\n"        

    def process_weather_request(self, intent):
        try:
            location = self.get_location(intent)
            date = self.get_date(intent)
            delta = date - dt.now()
            report = self.get_weather(location)
            return self.format_and_get_response(date, delta, report)
        except Exception as e:
            print_error("Error in process_weather_request: %s. Generating default response." % e)
            return "I don't know."