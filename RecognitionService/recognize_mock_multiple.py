# import the necessary packages
import requests
import logging
import time

def _send(payload):
	headers = {'Content-type': 'application/json', 'Accept': 'application/json'}
	r = requests.post("https://localhost:44359/api/record/endSnapshot", json=payload, verify=False, headers=headers)

_send({
	"codes": ['SE12346', 'SE12348', 'SE62823'],
	"unknowns": ['1.png']
})
time.sleep(10)
_send({
	"codes": ['SE62823', 'SE12351', 'SE1001'],
	"unknowns": ['2.jpg']
})
time.sleep(60)