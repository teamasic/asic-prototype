import argparse
import datetime
import pickle

import cv2
import numpy as np

from helper import my_face_detection, my_face_recognition

# construct the argument parser and parse the arguments
ap = argparse.ArgumentParser()
ap.add_argument("-r", "--recognizer", default="output_dlib/recognizer.pickle",
                help="path to model trained to recognize faces")
ap.add_argument("-l", "--le", default="output_dlib/le.pickle",
                help="path to label encoder")
ap.add_argument("-i", "--imagePath", default="images/thanh1.jpg",
                help="path to label encoder")
args = vars(ap.parse_args())

# Load arguments
recognizer = pickle.loads(open(args["recognizer"], "rb").read())
le = pickle.loads(open(args["le"], "rb").read())
imagePath = args["imagePath"]

# read image
image = cv2.imread(imagePath)

# detect faces
boxes = my_face_detection.face_locations(image)
# check so ensure there is only 1 face
if len(boxes) == 1:
    # compute the facial embedding for the face
    vec = my_face_recognition.face_encodings(image, boxes)[0]
    preds = recognizer.predict_proba([vec])[0]
    j = np.argmax(preds)
    proba = preds[j]
    name = le.classes_[j]

    # Check unknown and weak recognition
    print(name)