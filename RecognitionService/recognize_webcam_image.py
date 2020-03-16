import argparse
import datetime
import pickle

import cv2
import numpy as np

from config import my_constant
from helper import my_face_detection, my_face_recognition, my_service

# construct the argument parser and parse the arguments
ap = argparse.ArgumentParser()
ap.add_argument("-i", "--imagePath", default="images/thanh1.jpg",
                help="path to label encoder")
args = vars(ap.parse_args())

# Load argument
imagePath = args["imagePath"]
result = my_service.recognize_image(imagePath)
if result is None:
    print("")
else:
    (box, name, proba) = result
    print(name)
