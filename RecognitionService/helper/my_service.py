import pickle

import cv2
import numpy as np

from config import my_constant
from helper import my_face_detection, my_face_recognition


def recognize_image(imagePath, threshold=0):
    image = cv2.imread(imagePath)
    return recognize_image_after_read(image, threshold)


def recognize_image_after_read(image, threshold=0):
    boxes = my_face_detection.face_locations(image)
    if len(boxes) == 1:
        vec = my_face_recognition.face_encodings(image, boxes)[0]
        name, proba = _get_label(vec, threshold)
        return boxes[0], name, proba
    return None


def _get_label(vec, threshold=0):
    recognizer = pickle.loads(open(my_constant.recognizerPath, "rb").read())
    le = pickle.loads(open(my_constant.lePath, "rb").read())
    preds = recognizer.predict_proba([vec])[0]
    j = np.argmax(preds)
    proba = preds[j]
    name = le.classes_[j]
    if (proba > threshold):
        return name, proba
    return "Unknown", None
