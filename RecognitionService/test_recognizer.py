import argparse
import pickle
import cv2
import imutils
import numpy as np
from helper import my_face_detection, my_face_recognition, recognition_api

# imageUrl = "J:\\dataset\\VN-celeb\\58\\2.png";
imageUrl = "E:\\\icloud\\IMG_1214.JPG"

recognizer = pickle.loads(open("output_dlib/recognizer.pickle", "rb").read())
le = pickle.loads(open("output_dlib/le.pickle", "rb").read())

# load the image, resize it to have a width of 600 pixels (while
# maintaining the aspect ratio), and then grab the image dimensions
image = cv2.imread(imageUrl)
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
cv2.waitKey(5*1000)
