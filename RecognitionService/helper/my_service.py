import pickle

import cv2
import numpy as np
from imutils import paths
from sklearn import svm
from sklearn.preprocessing import LabelEncoder

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


def generate_more_embeddings(datasetPath):
    imagePaths = paths.list_images(datasetPath)
    data = pickle.loads(open(my_constant.embeddingsPath, "rb").read())
    knownEmbeddings = data["embeddings"]
    knownNames = data["knownNames"]
    totalAdded = 0
    for (i, imagePath) in enumerate(imagePaths):
        # extract the person name from the image path
        print("[INFO] processing image {}/{}".format(i + 1,
                                                     len(imagePaths)))

        name = imagePath.split(cv2.os.path.sep)[-2]

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
        totalAdded += 1

    # dump the facial embeddings + names to disk
    print("[INFO] serializing more {} encodings...".format(totalAdded))
    print("[INFO] serializing total {} encodings...".format(len(knownEmbeddings)))
    data = {"embeddings": knownEmbeddings, "names": knownNames}
    f = open(my_constant.embeddingsPath, "wb")
    f.write(pickle.dumps(data))
    f.close()


def generate_embeddings(datasetPath):
    imagePaths = paths.list_images(datasetPath)
    knownEmbeddings = []
    knownNames = []
    totalAdded = 0
    for (i, imagePath) in enumerate(imagePaths):
        # extract the person name from the image path
        print("[INFO] processing image {}/{}".format(i + 1,
                                                     len(imagePaths)))

        name = imagePath.split(cv2.os.path.sep)[-2]

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
        totalAdded += 1

    # dump the facial embeddings + names to disk
    print("[INFO] serializing total {} encodings...".format(totalAdded))
    data = {"embeddings": knownEmbeddings, "names": knownNames}
    f = open(my_constant.embeddingsPath, "wb")
    f.write(pickle.dumps(data))
    f.close()


def generate_train_model():
    print("[INFO] loading face embeddings...")
    data = pickle.loads(open(my_constant.embeddingsPath, "rb").read())

    # encode the labels
    print("[INFO] encoding labels...")
    le = LabelEncoder()
    labels = le.fit_transform(data["names"])

    # train the model used to accept the 128-d embeddings of the face and
    # then produce the actual face recognition
    print("[INFO] training model...")
    recognizer = svm.SVC(C=1.0, kernel="linear", probability=True)
    recognizer.fit(data["embeddings"], labels)

    # write the actual face recognition model to disk
    f = open(my_constant.recognizerPath, "wb")
    f.write(pickle.dumps(recognizer))
    f.close()

    # write the label encoder to disk
    f = open(my_constant.lePath, "wb")
    f.write(pickle.dumps(le))
    f.close()
