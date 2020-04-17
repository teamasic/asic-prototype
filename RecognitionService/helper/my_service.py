import copy
import os
import pickle
import shutil
from datetime import datetime
from pathlib import Path

import cv2
import imutils
import numpy as np
from imutils import paths
from sklearn import svm, linear_model
from sklearn.preprocessing import LabelEncoder

from config import my_constant
from helper import my_face_detection, my_face_recognition, my_face_generator, my_utils


def recognize_image(imagePath):
    image = cv2.imread(imagePath)
    return recognize_image_after_read(image)


def recognize_image_after_read(image, alignFace=False, numberOfTimesToUpSample=1):
    boxes = my_face_detection.face_locations(image)
    if len(boxes) == 1:
        if (alignFace == True):
            aligned_image = my_face_detection.align_face(image, boxes[0])
            vecs = my_face_recognition.face_encodings(aligned_image)
        else:
            vecs = my_face_recognition.face_encodings(image, boxes)
        vec = vecs[0]
        name, proba = _get_label(vec)
        return boxes[0], name, proba
    else:
        print("len: {}".format(len(boxes)))
    return None


def _get_label(vec):
    recognizer_model = pickle.loads(open(my_constant.recognizerModelPath, "rb").read())
    recognizer = recognizer_model["recognizer"]
    le = recognizer_model["le"]

    preds = recognizer.predict_proba([vec])[0]
    j = np.argmax(preds)
    proba = preds[j]
    name = le.classes_[j]
    if proba > my_constant.threshold:
        return name, proba
    return "unknown", 0


def recognize_image_after_read_multiple(image, numberOfTimesToUpSample=1):
    boxes = my_face_detection.face_locations(image, numberOfTimesToUpSample)
    return get_label_after_detect_multiple(image, boxes)


def get_label_after_detect_multiple(image, boxes):
    print(os.getpid())
    startitme = datetime.now()
    vecs = my_face_recognition.face_encodings(image, boxes)
    results = []
    for i, vec in enumerate(vecs):
        name, proba = _get_label(vec)
        results.append((boxes[i], name, proba))
    print(datetime.now() - startitme)
    return results


def generate_more_embeddings(datasetPath, alignFace=False):
    imagePaths = list(paths.list_images(datasetPath))
    data = pickle.loads(open(my_constant.embeddingsPath, "rb").read())
    knownEmbeddings = data["embeddings"]
    knownNames = data["names"]
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
        if (alignFace == True):
            aligned_image = my_face_detection.align_face(image, boxes[0])
            vecs = my_face_recognition.face_encodings(aligned_image)
        else:
            vecs = my_face_recognition.face_encodings(image, boxes)
        vec = vecs[0]
        knownEmbeddings.append(vec.flatten())
        knownNames.append(name)
        totalAdded += 1

    # dump the facial embeddings + names to disk
    print("[INFO] serializing more {} encodings...".format(totalAdded))
    print("[INFO] serializing total {} encodings...".format(len(knownEmbeddings)))
    data = {"embeddings": knownEmbeddings, "names": knownNames}
    f = open(my_constant.embeddingsPath, "wb+")
    f.write(pickle.dumps(data))
    f.close()


def generate_embeddings(datasetPath, alignFace=False):
    detected_dir = r"C:\Users\thanh\Desktop\cropimage"
    imagePaths = list(paths.list_images(datasetPath))
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
        image = imutils.resize(image, width=400)
        imageRaw = copy.deepcopy(image)
        image = cv2.cvtColor(image, cv2.COLOR_BGR2RGB)
        boxes = my_face_detection.face_locations(image)
        if len(boxes) > 1:
            print(imagePath, "> 1")
            print(len(boxes))
            continue
        if len(boxes) == 0:
            print(imagePath, "= 0")
            print(len(boxes))
            continue
        pathimage = os.path.join(detected_dir, name)
        Path(pathimage).mkdir(parents=True, exist_ok=True)
        my_utils.saveImageFunction(imageRaw, boxes[0], pathimage, 40)
        if (alignFace == True):
            aligned_image = my_face_detection.align_face(image, boxes[0])
            vecs = my_face_recognition.face_encodings(aligned_image)
        else:
            vecs = my_face_recognition.face_encodings(image, boxes)
        vec = vecs[0]

        knownEmbeddings.append(vec.flatten())
        knownNames.append(name)
        totalAdded += 1

    # dump the facial embeddings + names to disk
    print("[INFO] serializing total {} encodings...".format(totalAdded))
    data = {"embeddings": knownEmbeddings, "names": knownNames}
    f = open(my_constant.embeddingsPath, "wb+")
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

    recognizer_model = {"recognizer": recognizer, "le": le}
    f = open(my_constant.recognizerModelPath, "wb+")
    f.write(pickle.dumps(recognizer_model))
    f.close()


def generate_train_model_softmax():
    print("[INFO] loading face embeddings...")
    data = pickle.loads(open(my_constant.embeddingsPath, "rb").read())

    le = LabelEncoder()
    labels = le.fit_transform(data["names"])

    print("[INFO] training model...")
    logreg = linear_model.LogisticRegression(C=1e5,
                                             solver='lbfgs', multi_class='multinomial')
    logreg.fit(data["embeddings"], labels)

    recognizer_model_softmax = {"recognizer": logreg, "le": le}
    f = open("output_dlib/recognizer_model.pickle", "wb+")
    f.write(pickle.dumps(recognizer_model_softmax))
    f.close()


def augment_images(datasetDir, augmentedDir, genImageNum=4):
    # grab the paths to the input images in our dataset
    print("[INFO] quantifying faces...")
    imagePaths = list(paths.list_images(datasetDir))
    augmented_path = augmentedDir

    unknown_batch = []
    original_batch = []
    name_batch = []
    count = genImageNum  # generate 4 fake images for 1 raw image

    # loop over the image paths
    for (i, imagePath) in enumerate(imagePaths):
        # extract the person name from the image path
        print("[INFO] processing image {}/{}".format(i + 1,
                                                     len(imagePaths)))

        name = imagePath.split(os.path.sep)[-2]

        # load the image, resize it to have a width of 400 pixels (while
        # maintaining the aspect ratio)
        image = cv2.imread(imagePath)
        image = imutils.resize(image, width=400)
        if name == "unknown":
            unknown_batch.append(image)
            pass
        else:
            original_batch.append(image)
            name_batch.append(name)

    augmented_batch = my_face_generator.face_generate(original_batch, count)
    name_batch = name_batch * count

    # add all augmented images into a dictionary
    name_image_dict = dict()
    for name, image in zip(name_batch, augmented_batch):
        if name in name_image_dict:
            name_image_dict[name].append(image)
        else:
            name_image_dict[name] = []
    # add all unknown images into the dictionary
    name_image_dict["unknown"] = unknown_batch

    if os.path.exists(augmented_path):
        shutil.rmtree(augmented_path)
    os.mkdir(augmented_path)
    for name in name_image_dict.keys():
        os.mkdir(os.path.sep.join([augmented_path, name]))

    # write each augmented image into their respective folder
    for name, images in name_image_dict.items():
        for i, image in enumerate(images):
            full_file_name = os.path.sep.join([augmented_path, name, str(i + 1) + ".jpg"])
            cv2.imwrite(full_file_name, image)


def transfer_rtsp_to_http(rtspString):
    command = "{} {} {}".format(my_constant.transferToHttpBatchPath, rtspString, my_constant.portHttpStream)
    os.system(command)
    return "http://localhost:{}".format(my_constant.portHttpStream)
