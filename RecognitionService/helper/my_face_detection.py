import os

import cv2
import face_recognition
import numpy as np
from imutils.face_utils import FaceAligner
from imutils.face_utils import rect_to_bb
import dlib
import imutils

"""
:return a list with multiple tuple, each tuple have size 4 - describe boudary of face
"""


def face_locations(image):
    return _face_locations_hog(image)


def _face_locations_caffe(image):
    faceDetectionModelPath = "model/face_detection_model"
    confidenceThreshold = 0.9
    (h,w) = image.shape[:2]
    # construct a blob from the image
    imageBlob = cv2.dnn.blobFromImage(
        cv2.resize(image, (300, 300)), 1.0, (300, 300),
        (104.0, 177.0, 123.0), swapRB=False, crop=False)
    protoPath = os.path.sep.join([faceDetectionModelPath, "deploy.prototxt"])
    modelPath = os.path.sep.join([faceDetectionModelPath,
                                  "res10_300x300_ssd_iter_140000.caffemodel"])
    detector = cv2.dnn.readNetFromCaffe(protoPath, modelPath)

    # apply OpenCV's deep learning-based face detector to localize
    # faces in the input image
    detector.setInput(imageBlob)
    detections = detector.forward()

    # loop over the detections
    boxes = []
    for i in range(0, detections.shape[2]):
        # extract the confidence (i.e., probability) associated with the
        # prediction
        confidence = detections[0, 0, i, 2]

        # filter out weak detections
        if confidence > confidenceThreshold:
            # compute the (x, y)-coordinates of the bounding box for the
            # face
            box = detections[0, 0, i, 3:7] * np.array([w, h, w, h])
            boxes.append(box)
    for i, box in enumerate(boxes):
        tmp = box[0]
        box[0] = box[1]
        box[1] = box[2]
        box[2] = box[3]
        box[3] = tmp
        boxes[i] = tuple(map(int, box))
    return boxes

def _face_locations_hog(image):
    rgb = cv2.cvtColor(image, cv2.COLOR_BGR2RGB)
    boxes = face_recognition.face_locations(rgb, model="hog")
    return boxes


def _face_locations_cnn(image):
    rgb = cv2.cvtColor(image, cv2.COLOR_BGR2RGB)
    boxes = face_recognition.face_locations(rgb, model="cnn")
    return boxes


def align_face(image, box):
    facialLandmarksModelPath = "model/facial_landmarks_model"
    # create the facial landmark predictor and the face aligner
    predictor_path = os.path.sep.join([facialLandmarksModelPath, "shape_predictor_68_face_landmarks.dat"])
    predictor = dlib.shape_predictor(predictor_path)
    fa = FaceAligner(predictor, desiredFaceWidth=256)

    # resize and convert the image to grayscale
    gray = cv2.cvtColor(image, cv2.COLOR_BGR2GRAY)

    rect = _css_to_rect(box)
    face_aligned = fa.align(image, gray, rect)
    return face_aligned

    # faceLandmarks =  face_recognition.face_landmarks(image, boxes)
    # the implementation of this calls the same 68-point facial landmarks model. We manually fetch our predictor
    # so the FaceAligner class in dlib can automatically predict and align everything for us.


def _css_to_rect(css):
    """
    Convert a tuple in (top, right, bottom, left) order to a dlib `rect` object
    :param css:  plain tuple representation of the rect in (top, right, bottom, left) order
    :return: a dlib `rect` object
    """
    return dlib.rectangle(css[3], css[0], css[1], css[2])

