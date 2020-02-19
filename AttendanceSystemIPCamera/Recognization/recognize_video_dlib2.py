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

# construct the argument parser and parse the arguments
ap = argparse.ArgumentParser()
ap.add_argument("-r", "--recognizer", default="output_dlib/recognizer.pickle",
	help="path to model trained to recognize faces")
ap.add_argument("-l", "--le", default="output_dlib/le.pickle",
	help="path to label encoder")
args = vars(ap.parse_args())

# load the actual face recognition model along with the label encoder
recognizer = pickle.loads(open(args["recognizer"], "rb").read())
le = pickle.loads(open(args["le"], "rb").read())

# initialize the video stream, then allow the camera sensor to warm up
print("[INFO] starting video stream...")
vs = VideoStream(src=0).start()
time.sleep(2.0)

# start the FPS throughput estimator
fps = FPS().start()

# loop over frames from the video file stream
while True:
	# grab the frame from the threaded video stream
	frame = vs.read()

	# resize the frame to have a width of 600 pixels (while
	# maintaining the aspect ratio), and then grab the image
	# dimensions
	image = imutils.resize(frame, width=600)
	(h, w) = image.shape[:2]

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

	for ((top, right, bottom, left), name, proba) in zip(boxes, names, probas):
		text = "{}: {:.2f}%".format(name, proba * 100)
		print(text)
		headers = {'Content-type': 'application/json', 'Accept': 'application/json'}
		payload = {"code": name}
		r = requests.post("https://localhost:44359/api/record", json=payload, verify=False, headers=headers)
		print(r.text)
		# draw the predicted face name on the image
		cv2.rectangle(image, (left, top), (right, bottom), (0, 0, 225), 2)
		y = top - 10 if top - 10 > 10 else top + 10
		cv2.putText(image, text, (left, y), cv2.FONT_HERSHEY_SIMPLEX,
					0.45, (0, 0, 255), 2)

	# update the FPS counter
	fps.update()

	# show the output frame
	cv2.imshow("Frame", image)
	key = cv2.waitKey(1) & 0xFF

	# if the `q` key was pressed, break from the loop
	if key == ord("q"):
		break

# stop the timer and display FPS information
fps.stop()
print("[INFO] elasped time: {:.2f}".format(fps.elapsed()))
print("[INFO] approx. FPS: {:.2f}".format(fps.fps()))

# do a bit of cleanup
cv2.destroyAllWindows()
vs.stop()