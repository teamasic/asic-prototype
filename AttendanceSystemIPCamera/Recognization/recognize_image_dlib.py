# import the necessary packages
import time
import os

import requests

os.add_dll_directory(r'C:\Program Files\VideoLAN\VLC')
import vlc
import argparse
import pickle
import cv2
import face_recognition
import imutils
import numpy as np

# construct the argument parser and parse the arguments
ap = argparse.ArgumentParser()
ap.add_argument("-i", "--image", default="snapshot/snapshot.jpg",
                help="path to input image")
ap.add_argument("-r", "--recognizer", default="output_dlib/recognizer.pickle",
                help="path to model trained to recognize faces")
ap.add_argument("-l", "--le", default="output_dlib/le.pickle",
                help="path to label encoder")
ap.add_argument("-p", "--rtsp", default="rtsp://192.168.1.7:8554/unicast",
                help="path to rtsp string")
args = vars(ap.parse_args())

# Load arguments
recognizer = pickle.loads(open(args["recognizer"], "rb").read())
le = pickle.loads(open(args["le"], "rb").read())
rtspString = args["rtsp"]
snapshotPath = args["image"]

# Play the video stream
player=vlc.MediaPlayer(rtspString)
player.play()
while 1:
    time.sleep(1)
    player.video_take_snapshot(0, snapshotPath, 0, 0)

    # load the image, resize it to have a width of 600 pixels (while
    # maintaining the aspect ratio), and then grab the image dimensions
    image = cv2.imread(args["image"])
    image = imutils.resize(image, width=600)
    (h, w) = image.shape[:2]

    # detect images
    rgb = cv2.cvtColor(image, cv2.COLOR_BGR2RGB)
    boxes = face_recognition.face_locations(rgb, model="hog")

    # compute the facial embedding for the face
    vecs = face_recognition.face_encodings(rgb, boxes)
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
        headers = {'Content-type': 'application/json', 'Accept': 'application/json'}
        payload = {"code": name}
        r = requests.post("https://localhost:44359/api/record", json=payload, verify=False, headers=headers)
        print(r.text)
        # draw the predicted face name on the image
        cv2.rectangle(image, (left, top), (right, bottom), (0, 0, 225), 2)
        y = top - 10 if top - 10 > 10 else top + 10
        cv2.putText(image, text, (left, y), cv2.FONT_HERSHEY_SIMPLEX,
                    0.45, (0, 0, 255), 2)

    # show the output image
    cv2.imshow("Image", image)
    cv2.waitKey(1)
