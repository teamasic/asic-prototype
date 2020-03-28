from threading import Thread

import requests
import logging


def _recognize_face(name, connectQueue):
    headers = {'Content-type': 'application/json', 'Accept': 'application/json'}
    payload = {"code": name}
    try:
        r = requests.post("https://localhost:44359/api/record", json=payload, verify=False, headers=headers)
    except Exception as e:
        connectQueue.put(False)
        return
    connectQueue.put(True)

def recognize_face_new_thread(name, connectQueue):
    thread = Thread(target=_recognize_face, args=(name, connectQueue, ), daemon=True)
    thread.start()
