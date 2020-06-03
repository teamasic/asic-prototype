# import the necessary packages
import requests
import logging
import time
import random

def _send(payload):
	headers = {'Content-type': 'application/json', 'Accept': 'application/json'}
	r = requests.post("https://localhost:44359/api/record", json=payload, verify=False, headers=headers)

def _recognize_face(code):
    payload = {"code": code}
    _send(payload)

def _recognize_unknown(image):
    payload = {"code": "unknown", "avatar": image}
    _send(payload)

def _send_recognized(array):
	recognized = array
	for code in recognized:
		_recognize_face(code)

def _send_unknown():
	unknown = ['1.png', '3.jpg', '2.jpg']
	for image in unknown:
		_recognize_unknown(image)
		time.sleep(1)

def _get_code(num):
	return 'SE' + "{:03d}".format(num)

def _sample(codes):
	max_sample_size = (len(codes) - 1) // 3
	sample_size = random.randrange(1, max_sample_size)
	return random.sample(codes, sample_size)

max = 37
codes = [ _get_code(code) for code in range(1, max + 1)]

_send_unknown()
_send_recognized(['SB12348', 'SB12349', 'SB12350', 'SB12345'])

for i in range(4):
	sample = _sample(codes)
	_send_recognized(sample)

time.sleep(100)