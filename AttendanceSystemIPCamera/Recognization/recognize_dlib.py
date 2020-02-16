# import the necessary packages
import pickle

import cv2
import face_recognition
import imutils
import numpy as np
from flask import jsonify
def recognizer(imagePath):
    recognizer = pickle.loads(open("helper/output_dlib/recognizer.pickle", "rb").read())
    le = pickle.loads(open("helper/output_dlib/le.pickle", "rb").read())

    image = cv2.imread(imagePath)
    image = imutils.resize(image, width=600)
    (h, w) = image.shape[:2]
    rgb = cv2.cvtColor(image, cv2.COLOR_BGR2RGB)

    boxes = detect_image(imagePath)

    vecs = face_recognition.face_encodings(rgb, boxes)

    if (len(vecs) == 1):
        vec = vecs[0]
        preds = recognizer.predict_proba([vec])[0]
        j = np.argmax(preds)
        proba = preds[j]
        name = le.classes_[j]
        return jsonify({"name": name, "probability": proba}), 200
    else:
        return "", 400

def detect_image(imagePath):
    # load the image, resize it to have a width of 600 pixels (while
    # maintaining the aspect ratio), and then grab the image dimensions
    image = cv2.imread(imagePath)
    image = imutils.resize(image, width=600)
    (h, w) = image.shape[:2]

    rgb = cv2.cvtColor(image, cv2.COLOR_BGR2RGB)
    boxes = face_recognition.face_locations(rgb, model="hog")
    return boxes