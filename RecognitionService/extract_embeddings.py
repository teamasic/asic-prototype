import argparse
import os
import pickle

import cv2
from imutils import paths

from config import my_constant
from helper import my_face_detection, my_face_recognition

ap = argparse.ArgumentParser()
ap.add_argument("-i", "--dataset", default="dataset",
                help="path to input directory of faces + images")
args = vars(ap.parse_args())

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

    # load the image
    image = cv2.imread(imagePath)
    boxes = my_face_detection.face_locations(image)
    if len(boxes) > 1:
        print(imagePath, "> 1")
        print(len(boxes))
        continue
    if len(boxes) == 0:
        print(imagePath, "= 0")
        print(len(boxes))
        continue
    vecs = my_face_recognition.face_encodings(image, boxes)
    vec = vecs[0]
    knownEmbeddings.append(vec.flatten())
    knownNames.append(name)
    total += 1

# dump the facial embeddings + names to disk
print("[INFO] serializing {} encodings...".format(total))
data = {"embeddings": knownEmbeddings, "names": knownNames}
f = open(my_constant.embeddingsPath, "wb")
f.write(pickle.dumps(data))
f.close()
