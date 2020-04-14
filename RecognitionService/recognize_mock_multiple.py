# import the necessary packages
import requests
import logging
import time
import random

def _send(payload):
	headers = {'Content-type': 'application/json', 'Accept': 'application/json'}
	r = requests.post("https://localhost:44359/api/record/endSnapshot", json=payload, verify=False, headers=headers)

# PREPARE MOCK DATA SECTION
def _get_code(num):
	return 'SE' + "{:03d}".format(num)

def _sample(codes):
	max_sample_size = (len(codes) - 1) // 3
	sample_size = random.randrange(1, max_sample_size)
	return random.sample(codes, sample_size)

max = 37
codes = [ _get_code(code) for code in range(1, max + 1)]
# END PREPARE MOCK DATA SECTION

for i in range(5):
	_send({
		"codes": _sample(codes),
		"unknowns": ['1.png']
	})
	time.sleep(5)
time.sleep(100)