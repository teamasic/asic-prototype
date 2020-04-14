from threading import Thread

import requests
import logging

from helper import my_utils


def _recognize_face(name, connectQueue):
    try:
        headers = {'Content-type': 'application/json', 'Accept': 'application/json'}
        payload = {"code": name}
        r = requests.post("https://localhost:44359/api/record", json=payload, verify=False, headers=headers)
    except Exception as e:
        connectQueue.put(False)
        return
    connectQueue.put(True)

def _recognize_unknown(name, image, box, connectQueue):
    try:
        imageName = my_utils.saveImageFunction(image, box)
        headers = {'Content-type': 'application/json', 'Accept': 'application/json'}
        payload = {"code": name, "image": imageName}
        r = requests.post("https://localhost:44359/api/record", json=payload, verify=False, headers=headers)
    except Exception as e:
        connectQueue.put(False)
        return
    connectQueue.put(True)

def recognize_face_new_thread(name, connectQueue):
    thread = Thread(target=_recognize_face, args=(name, connectQueue, ), daemon=True)
    thread.start()

def recognize_unknown_new_thread(name, image, box, connectQueue):
    thread = Thread(target=_recognize_unknown, args=(name, image, box, connectQueue,), daemon=True)
    thread.start()
    return None


def recognize_multiple_faces(codes, unknowns):
    try:
        headers = {'Content-type': 'application/json', 'Accept': 'application/json'}
        payload = {"codes": codes, "unknowns": unknowns}
        r = requests.post("https://localhost:44359/api/record/endSnapshot", json=payload, verify=False, headers=headers)
        print(r)
    except Exception as e:
        raise Exception("Cannot check attendance")
