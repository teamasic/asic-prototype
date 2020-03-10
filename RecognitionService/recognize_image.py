# import the necessary packages
import time
import os
from threading import Thread
import requests
os.add_dll_directory(r'C:\Program Files\VideoLAN\VLC')
import vlc
import argparse
import pickle
import cv2
import imutils
import numpy as np
from helper import my_face_detection, my_face_recognition, recognition_api

# construct the argument parser and parse the arguments
ap = argparse.ArgumentParser()
ap.add_argument("-i", "--image", default="dataset/gold/20.jpg",
                help="path to input image")
ap.add_argument("-r", "--recognizer", default="output_dlib/recognizer.pickle",
                help="path to model trained to recognize faces")
ap.add_argument("-l", "--le", default="output_dlib/le.pickle",
                help="path to label encoder")
args = vars(ap.parse_args())
def _recognize_face(name):
    headers = {'Content-type': 'application/json', 'Accept': 'application/json'}
    payload = {"code": name}
    r = requests.post("https://localhost:44359/api/record", json=payload, verify=False, headers=headers)
    print(r)


def recognize_face_new_thread(name):
    thread = Thread(target=_recognize_face, args=(name,))
    thread.start()
# Load arguments
recognizer = pickle.loads(open(args["recognizer"], "rb").read())
le = pickle.loads(open(args["le"], "rb").read())
snapshotPath = args["image"]

while True:

    # load the image, resize it to have a width of 600 pixels (while
    # maintaining the aspect ratio), and then grab the image dimensions
    image = cv2.imread(args["image"])
    image = imutils.resize(image, width=200)
    (h, w) = image.shape[:2]

    # detect images
    boxes = my_face_detection.face_locations(image)

    # compute the facial embedding for the face
    vecs = my_face_recognition.face_encodings(image, boxes)

    names = []
    probas = []
    for vec in vecs:
        preds = recognizer.predict_proba([vec])[0]
        j = np.argmax(preds)
        proba = preds[j]
        name = le.classes_[j]
        names.append(name)
        probas.append(proba)

    # Show and call API
    for ((top, right, bottom, left), name, proba) in zip(boxes, names, probas):
        text = "{}: {:.2f}%".format(name, proba * 100)

        # draw the predicted face name on the image
        cv2.rectangle(image, (left, top), (right, bottom), (0, 0, 225), 2)
        y = top - 10 if top - 10 > 10 else top + 10
        cv2.putText(image, text, (left, y), cv2.FONT_HERSHEY_SIMPLEX,
                    0.45, (0, 0, 255), 2)

    # show the output image
    cv2.imshow("Image", image)
    cv2.waitKey(1)
