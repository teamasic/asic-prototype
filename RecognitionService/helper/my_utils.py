import json
import os
import uuid
from multiprocessing.context import Process
from os import path
from pathlib import Path
from threading import Thread
import numpy as np

import cv2
import imutils

from config import my_constant


def saveImageFunction(image, box, dir, padding=20, imageName = None, isOverwrite=False):
    Path(dir).mkdir(exist_ok=True)
    (top, right, bottom, left) = box
    (h, w) = image.shape[:2]
    cv2.rectangle(image, (left, top), (right, bottom), (0, 0, 225), 1)
    newTop = 0 if top - padding < 0 else top - padding
    newBottom = h if bottom + padding > h else bottom + padding
    newLeft = 0 if left - padding < 0 else left - padding
    newRight = w if right + padding > w else right + padding

    crop_image = image[newTop:newBottom, newLeft:newRight]
    crop_image = cv2.resize(crop_image, (150, 150), interpolation=cv2.INTER_AREA)
    if imageName is None:
        imageName = "{}.jpg".format(str(uuid.uuid4()))
    fullPath = os.path.join(dir, imageName)
    successSave = False
    if (path.exists(fullPath) is False) or (path.exists(fullPath) is True and isOverwrite):
        successSave = cv2.imwrite(fullPath, crop_image)
    return imageName, successSave


def remove_all_files(folder):
    if os.path.exists(folder):
        for filename in os.listdir(folder):
            file_path = os.path.join(folder, filename)
            try:
                if os.path.isfile(file_path) or os.path.islink(file_path):
                    os.unlink(file_path)
            except Exception as e:
                pass
    else:
        raise Exception("Folder {} does not exist".format(folder))


def getUnknownDictionary(sessionId):
    pathUnknown = path.join(my_constant.unknownDir, str(sessionId))
    Path(pathUnknown).mkdir(exist_ok=True)
    jsonFilePath = path.join(pathUnknown, "unknown.json")
    return getImageDictionary(jsonFilePath)

def getImageDictionary(jsonFilePath):
    result = []
    if os.path.exists(jsonFilePath):
        with open(jsonFilePath) as json_file:
            try:
                data = json.load(json_file)
                embeddingList = data['embeddings']
                for i, value in enumerate(embeddingList):
                    embedding_value = np.asarray(list(value["embedding_value"]))
                    newDic = {
                        "image_name": value["image_name"],
                        "embedding_value": embedding_value
                    }
                    result.append(newDic)
            except Exception as ex:
                print(ex)
                print("Read json error")
    else:
        data = {"embeddings": []}
        with open(jsonFilePath, "w", encoding='utf-8') as json_file:
            json.dump(data, json_file)
    return result


def getPeopleDictionary(sessionId):
    pathPeople = path.join(my_constant.peopleDir, str(sessionId))
    Path(pathPeople).mkdir(exist_ok=True)
    jsonFilePath = path.join(pathPeople, "people.json")
    return getImageDictionary(jsonFilePath)


def saveUnknownEmbeddings(unknownDictionary, sessionId):
    pathUnknown = path.join(my_constant.unknownDir, str(sessionId))
    jsonFilePath = path.join(pathUnknown, "unknown.json")
    for unkownEmbedding in unknownDictionary:
        unkownEmbedding["embedding_value"] = unkownEmbedding["embedding_value"].tolist()
    data = {"embeddings": unknownDictionary}
    with open(jsonFilePath, "w", encoding='utf-8') as json_file:
        json.dump(data, json_file)


def savePeopleEmbeddings(peopleDictionary, sessionId):
    pathUnknown = path.join(my_constant.peopleDir, str(sessionId))
    jsonFilePath = path.join(pathUnknown, "people.json")
    for peopleEmbedding in peopleDictionary:
        peopleEmbedding["embedding_value"] = peopleEmbedding["embedding_value"].tolist()
    data = {"embeddings": peopleDictionary}
    with open(jsonFilePath, "w", encoding='utf-8') as json_file:
        json.dump(data, json_file)