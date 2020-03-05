from threading import Thread

import requests


def _recognize_face(name):
    headers = {'Content-type': 'application/json', 'Accept': 'application/json'}
    payload = {"code": name}
    r = requests.post("https://localhost:44359/api/record", json=payload, verify=False, headers=headers)
    print(r)


def recognize_face_new_thread(name):
    thread = Thread(target=_recognize_face, args=(name,))
    thread.start()
