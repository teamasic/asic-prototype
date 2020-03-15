# import the necessary packages
import argparse
import pickle
from threading import Thread

import cv2
import face_recognition
import imutils
import numpy as np
import requests
from imutils.video import FPS

from config import my_constant
from helper import stream_video, my_service, recognition_api

# construct the argument parser and parse the arguments
ap = argparse.ArgumentParser()
ap.add_argument("-p", "--rtsp", default="rtsp://192.168.1.4:8554/unicast",
                help="path to rtsp string")
args = vars(ap.parse_args())

# Load arguments
rtspString = args["rtsp"]

recognizer = pickle.loads(open(my_constant.recognizerPath, "rb").read())
le = pickle.loads(open(my_constant.lePath, "rb").read())

# initialize the video stream, then allow the camera sensor to warm up
print("[INFO] starting video stream...")
vs = stream_video.CustomVideoStream(src=0).start()

# start the FPS throughput estimator
fps = FPS().start()

while True:
    # retrieve the frame from the threaded video stream
    image = vs.read()
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
    cv2.waitKey(1000)