# import the necessary packages
import time
import os
from threading import Thread
import requests

from config import my_constant

os.add_dll_directory(r'C:\Program Files\VideoLAN\VLC')
import vlc
import argparse
import pickle
import cv2
import imutils
import numpy as np
from helper import my_face_detection, my_face_recognition, recognition_api, my_service

# construct the argument parser and parse the arguments
ap = argparse.ArgumentParser()
ap.add_argument("-i", "--image", default="dataset/gold/20.jpg",
                help="path to input image")
args = vars(ap.parse_args())
# Load arguments
imagePath = args["image"]

recognizer = pickle.loads(open(my_constant.recognizerPath, "rb").read())
le = pickle.loads(open(my_constant.lePath, "rb").read())

while True:
    image = cv2.imread(imagePath)
    image = imutils.resize(image, width=600)
    (h, w) = image.shape[:2]

    result = my_service.recognize_image_after_read(image)

    if result is not None:
        (box, name, proba) = result
        (top, right, bottom, left) = box
        # Show and call API
        # recognition_api.recognize_face_new_thread(name)

        # draw the predicted face name on the image
        text = "{}: {:.2f}%".format(name, proba * 100)

        cv2.rectangle(image, (left, top), (right, bottom), (0, 0, 225), 2)
        y = top - 10 if top - 10 > 10 else top + 10
        cv2.putText(image, text, (left, y), cv2.FONT_HERSHEY_SIMPLEX,
                    0.45, (0, 0, 255), 2)
        # show the output image
        cv2.imshow("Image", image)
        cv2.waitKey(0)

