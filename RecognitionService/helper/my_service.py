import pickle

import cv2
import face_recognition
import imutils
import numpy as np

from helper import my_face_detection, my_face_recognition


def recognize_image(imagePath, threshold = 0.4):
    recognizer = pickle.loads(open("output_dlib/recognizer.pickle", "rb").read())
    le = pickle.loads(open("output_dlib/le.pickle", "rb").read())

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
        print(proba)
        return name
    return None