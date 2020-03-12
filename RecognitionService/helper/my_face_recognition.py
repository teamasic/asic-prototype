import cv2
import face_recognition

"""
    :return: a list with multiple ndArray, each ndArray has size 128
"""


def face_encodings(image, boxes=None):
    if boxes is None:
        boxes = [(0, 0, 0, 0)] # take the entire image
    return _face_encodings_dlib(image, boxes)


def _face_encodings_dlib(image, boxes=None):
    rgb = cv2.cvtColor(image, cv2.COLOR_BGR2RGB)
    return face_recognition.face_encodings(rgb, boxes)


def _face_encodings_opencv(image, boxes):
    return True
