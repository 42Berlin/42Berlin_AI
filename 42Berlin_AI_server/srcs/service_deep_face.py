import io
from  PIL import Image
from deepface import DeepFace
import os
from utils_constants import *
from utils_debug import *
from utils_colors import Colors

class ServiceDeepFace:
    def __init__(self, frames):
        self.frames = frames
        self.images = None
        self.names = None

    def extract_name_from_path(self, photo_path):
        basename = os.path.basename(photo_path)
        basename_no_extension = os.path.splitext(basename)[0]
        name = ''.join(c for c in basename_no_extension if not c.isdigit())
        return name
        
    def get_name_from_analyze(self, analysis):
        print_debug(f"{__name__}, get_name_from_analyze", P_FUNCTION)
        if analysis.empty:
            print_debug(f"analysis is empty", P_FACE)
            return ""
        score = analysis.head(1)['VGG-Face_cosine'].values[0]
        print_debug(f"score : {score}", P_FACE)
        if score >= 0.3:
            print_debug(f"score >= 0.3", P_FACE)
            print_debug(f"missed name : {analysis.head(1)['identity'].values[0]}", P_FACE)
            return ""
        photo_path = analysis.head(1)['identity'].values[0]
        print_debug(f"photo_path : {photo_path}", P_FACE)
        name = self.extract_name_from_path(photo_path).capitalize() 
        return name

    def analyze_faces(self):
        print_debug(f"{__name__}, analyze_faces", P_FUNCTION)
        print_debug(f"img_path : {self.images}", P_FACE)
        print_debug(f"db_path : {FACE_DB_PATH}", P_FACE)
        analyses = DeepFace.find(img_path=self.images, db_path=FACE_DB_PATH, enforce_detection=False)
        print_debug(f"analyses: {analyses}", P_FACE)
        self.names = []
        if (isinstance(analyses, list)):
            for analysis in analyses:
                name = self.get_name_from_analyze(analysis)
                if name:
                    self.names.append(name)
        else:
            name = self.get_name_from_analyze(analyses)
            if name:
                self.names.append(name)
        if len(self.names) == 0:
            self.names = [""]
        return self.names

    def save_images(self):
        print_debug(f"{__name__}, save_images", P_FUNCTION)
        i = 0
        self.images = []
        for frame in self.frames:
            fbytes = bytes(frame)
            image = Image.open(io.BytesIO(fbytes))
            name = FACE_TMP_PATH + "/" + FACE_BASENAME + str(i) + "." + FACE_EXTENSION
            image.save(name)
            self.images.append(name)
            print_debug(f"One image saved : ", name)
            i+=1

    def delete_images(self):
        print_debug(f"{__name__}, delete_images", P_FUNCTION)
        for img in self.images:
            os.remove(img)

    def face_recognition_engine(self):
        print_debug(f"{__name__}, face_recognition_engine", P_FUNCTION)
        try :
            self.save_images()
            self.analyze_faces()
            # self.delete_images()
            print_debug(f"DeepFace found: {self.names}", P_FACE)
            return self.names
        except Exception as e:
            print_error("DeepFace failed: %s." % e)
            self.names = [""]
            return self.names
        