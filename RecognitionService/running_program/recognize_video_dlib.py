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

import WebcamVideoStream

# construct the argument parser and parse the arguments
ap = argparse.ArgumentParser()
ap.add_argument("-r", "--recognizer", default="output_dlib/recognizer.pickle",
                help="path to model trained to recognize faces")
ap.add_argument("-l", "--le", default="output_dlib/le.pickle",
                help="path to label encoder")
ap.add_argument("-p", "--rtsp", default="rtsp://192.168.1.4:8554/unicast",
                help="path to rtsp string")
args = vars(ap.parse_args())

# Load arguments
recognizer = pickle.loads(open(args["recognizer"], "rb").read())
le = pickle.loads(open(args["le"], "rb").read())
rtspString = args["rtsp"]

# initialize the video stream, then allow the camera sensor to warm up
print("[INFO] starting video stream...")
vs = WebcamVideoStream.WebcamVideoStream(src=rtspString).start()

# start the FPS throughput estimator
fps = FPS().start()


# loop over frames from the video file stream
def threaded_request(name):
    headers = {'Content-type': 'application/json', 'Accept': 'application/json'}
    payload = {"code": name}
    r = requests.post("https://localhost:44359/api/record", json=payload, verify=False, headers=headers)
    print(r)

while True:
    # retrieve the frame from the threaded video stream
    frame = vs.read()

    # resize the frame to have a width of 600 pixels (while
    # maintaining the aspect ratio), and then grab the image
    # dimensions
    image = imutils.resize(frame, width=600)
    (h, w) = image.shape[:2]

    # detect faces
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

    # show and call API
    for ((top, right, bottom, left), name, proba) in zip(boxes, names, probas):
        text = "{}: {:.2f}%".format(name, proba * 100)
        print(text)
        thread = Thread(target=threaded_request, args=(name, ))
        thread.start()
        cv2.rectangle(image, (left, top), (right, bottom), (0, 0, 225), 2)
        y = top - 10 if top - 10 > 10 else top + 10
        cv2.putText(image, text, (left, y), cv2.FONT_HERSHEY_SIMPLEX,
                    0.45, (0, 0, 255), 2)

    # # update the FPS counter
    fps.update()

    # show the output frame
    cv2.imshow("Frame", image)
    key = cv2.waitKey(1000) & 0xFF

    # if the `q` key was pressed, break from the loop
    if key == ord("q"):
        break

# stop the timer and display FPS information
fps.stop()
print("[INFO] elasped time: {:.2f}".format(fps.elapsed()))
print("[INFO] approx. FPS: {:.2f}".format(fps.fps()))

# do a bit of cleanup
vs.stop()
cv2.destroyAllWindows()