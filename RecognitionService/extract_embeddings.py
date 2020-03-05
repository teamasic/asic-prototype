import argparse
import os
import pickle

import cv2
import face_recognition
import imutils
# import the necessary packages
from imutils import paths

# construct the argument parser and parse the arguments
from helper import my_face_detection, my_face_recognition

ap = argparse.ArgumentParser()
ap.add_argument("-i", "--dataset", default="dataset",
                help="path to input directory of faces + images")
ap.add_argument("-e", "--embeddings", default="output_dlib/embeddings.pickle",
                help="path to output serialized db of facial embeddings")

args = vars(ap.parse_args())

# grab the paths to the input images in our dataset
print("[INFO] quantifying faces...")
imagePaths = list(paths.list_images(args["dataset"]))

# initialize our lists of extracted facial embeddings and
# corresponding people names
knownEmbeddings = []
knownNames = []

# initialize the total number of faces processed
total = 0

# loop over the image paths
for (i, imagePath) in enumerate(imagePaths):
    # extract the person name from the image path
    print("[INFO] processing image {}/{}".format(i + 1,
                                                 len(imagePaths)))

    name = imagePath.split(os.path.sep)[-2]

    # load the image, resize it to have a width of 600 pixels (while
    # maintaining the aspect ratio), and then grab the image
    # dimensions
    image = cv2.imread(imagePath)
    image = imutils.resize(image, width=600)
    (h, w) = image.shape[:2]

    rgb = cv2.cvtColor(image, cv2.COLOR_BGR2RGB)
    boxes = my_face_detection.face_locations(image)
    if len(boxes) > 1:
        print(imagePath, "> 1")
    if (len(boxes) == 0):
        print(imagePath, "= 0")
    # compute the facial embedding for the face
    vecs = my_face_recognition.face_encodings(image, boxes)
    for vec in vecs:
        knownEmbeddings.append(vec.flatten())
        knownNames.append(name)
        total += 1

# add the name of the person + corresponding face
# embedding to their respective lists


# dump the facial embeddings + names to disk
print("[INFO] serializing {} encodings...".format(total))
data = {"embeddings": knownEmbeddings, "names": knownNames}
f = open(args["embeddings"], "wb")
f.write(pickle.dumps(data))
f.close()
