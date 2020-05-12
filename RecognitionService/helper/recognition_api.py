from os import path
from threading import Thread

import requests
import logging

from config import my_constant
from helper import my_utils


def _recognize_face(name, image, box, connectQueue, sessionId, peopleDictionary, vec):
    try:
        pathPeople = path.join(my_constant.peopleDir, str(sessionId))
        imageName = name + ".jpg"
        imageName, successSave = my_utils.saveImageFunction(image, box, pathPeople, padding=20, imageName=imageName)
        if successSave:
            headers = {'Content-type': 'application/json', 'Accept': 'application/json'}
            payload = {"code": name, "image": imageName}
            print("before", peopleDictionary)
            peopleDictionary.append({
                "image_name": imageName,
                "embedding_value": vec
            })
            print("after", peopleDictionary)
            r = requests.post("https://localhost:44359/api/record", json=payload, verify=False, headers=headers)
    except Exception as e:
        connectQueue.put(False)
        return
    connectQueue.put(True)

def _recognize_unknown(name, image, box, connectQueue, sessionId, unknownDictionary, vec):
    try:
        pathUnknown = path.join(my_constant.unknownDir, str(sessionId))
        imageName, successSave = my_utils.saveImageFunction(image, box, pathUnknown, padding=20)
        print(imageName, successSave, pathUnknown)
        if imageName is None:
            print("None")
        if successSave:
            headers = {'Content-type': 'application/json', 'Accept': 'application/json'}
            payload = {"code": name, "image": imageName}
            print(imageName)
            print("before", unknownDictionary)
            unknownDictionary.append({
                "image_name": imageName,
                "embedding_value": vec
            })
            print("after", unknownDictionary)
            r = requests.post("https://localhost:44359/api/record", json=payload, verify=False, headers=headers)
    except Exception as e:
        print(e)
        connectQueue.put(False)
        return
    connectQueue.put(True)

def recognize_face_new_thread(name, image, box, connectQueue, sessionId, peopleDictionary, vec):
    thread = Thread(target=_recognize_face, args=(name, image, box, connectQueue, sessionId, peopleDictionary, vec), daemon=True)
    thread.start()

def recognize_unknown_new_thread(name, image, box, connectQueue, sessionId, unknownDictionary, vec):
    thread = Thread(target=_recognize_unknown, args=(name, image, box, connectQueue, sessionId, unknownDictionary, vec), daemon=True)
    thread.start()
    return None


def recognize_multiple_faces(codes, unknowns):
    try:
        headers = {'Content-type': 'application/json', 'Accept': 'application/json'}
        payload = {"codes": codes, "unknowns": unknowns}
        r = requests.post("https://localhost:44359/api/record/endSnapshot", json=payload, verify=False, headers=headers)
        print(r)
    except Exception as e:
        print(e)
        raise Exception("Cannot check attendance")
