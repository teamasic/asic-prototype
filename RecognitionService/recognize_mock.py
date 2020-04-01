# import the necessary packages
import requests
import logging
import time

def _send(payload):
	headers = {'Content-type': 'application/json', 'Accept': 'application/json'}
	r = requests.post("https://localhost:44359/api/record", json=payload, verify=False, headers=headers)

def _recognize_face(code):
    payload = {"code": code}
    _send(payload)

def _recognize_unknown(image):
    payload = {"code": "Unknown", "avatar": image}
    _send(payload)

def _send_recognized():
	recognized = ['SE62823', 'SE12349', 'SE12345', 'SE12348']
	for code in recognized:
		_recognize_face(code)
		time.sleep(5)   # Delays for 5 seconds. You can also use a float value.

def _send_unknown():
	unknown = ['1.jpg', '3.jpg', '2.jpg']
	for image in unknown:
		_recognize_unknown(image)
		time.sleep(5)   # Delays for 5 seconds. You can also use a float value.

_send_unknown()