# USAGE
# python recognize_video.py --detector face_detection_model \
#	--embedding-model openface_nn4.small2.v1.t7 \
#	--recognizer output/recognizer.pickle \
#	--le output/le.pickle

# import the necessary packages
from imutils.video import VideoStream
from imutils.video import FPS
import numpy as np
import argparse
import imutils
import pickle
import time
import cv2
import os
import face_recognition
import requests

# initialize the video stream, then allow the camera sensor to warm up
print("[INFO] starting video stream...")

codes = ["SB12345", "SB12348", "SB12347"]
headers = {'Content-type': 'application/json', 'Accept': 'application/json'}

time.sleep(20)
for code in codes:
	payload = {"code": code}
	r = requests.post("https://localhost:44359/api/record", json=payload, verify=False, headers=headers)
	time.sleep(5)

# do a bit of cleanup
cv2.destroyAllWindows()
vs.stop()