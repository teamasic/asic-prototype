# import the necessary packages
from __future__ import print_function

import copy
import multiprocessing
import queue

from PIL import Image
from PIL import ImageTk
import tkinter as tki
import threading
import datetime
import imutils
import cv2
import os
import time

from imutils.video import FPS

from config import my_constant
from helper import my_face_detection, my_service, my_utils, recognition_api
from helper.my_utils import remove_all_files


class PhotoBoothApp:
	def __init__(self, vs, pool, isForCheckingAttendance):
		self.pool = pool
		# store the video stream object and output path, then initialize
		# the most recently read frame, thread for reading frames, and
		# the thread stop event
		self.vs = vs
		self.isForCheckingAttendance = isForCheckingAttendance
		self.frame = None
		self.thread = None
		# self.stopEvent = None
		self.isError = queue.Queue()
		self.stopRunVideo = queue.Queue()

		# initialize the root window and image panel
		self.root = tki.Tk()
		self.panel = None

		# create a button, that when pressed, will take the current
		# frame and save it to file
		self.btn = tki.Button(self.root, text="Check attendance",
			command=self.takeSnapshot)
		self.btn.pack(side="bottom", fill="both", expand="yes", padx=10,
			pady=10)

		# start a thread that constantly pools the video sensor for
		# the most recently read frame
		# self.stopEvent = threading.Event()
		self.thread = threading.Thread(target=self.videoLoop, args=(), daemon=True)
		self.thread.start()

		# set a callback to handle when the window is closed
		self.root.wm_title("ASIC checking attendance")
		self.root.wm_protocol("WM_DELETE_WINDOW", self.onClose)

	def videoLoop(self):
		# DISCLAIMER:
		# I'm not a GUI developer, nor do I even pretend to be. This
		# try/except statement is a pretty ugly hack to get around
			# a RunTime error that Tkinter throws due to threading
		try:
			# keep looping over frames until we are instructed to stop
			while True:
				if self.isError.empty() is False:
					self.onClose()
				# Dang chay bt
				if self.stopRunVideo.empty():
					# grab the frame from the video stream and resize it to
					# have a maximum width of 300 pixels
					self.btn["state"] = "normal"
					self.frame = self.vs.read()
					self.frame = imutils.resize(self.frame, width=my_constant.resizeWidthShow)
				# Dang nhan dien
				else:
					self.btn["state"] = "disable"
				# OpenCV represents images in BGR order; however PIL
				# represents images in RGB order, so we need to swap
				# the channels, then convert to PIL and ImageTk format
				image = cv2.cvtColor(self.frame, cv2.COLOR_BGR2RGB)
				image = Image.fromarray(image)
				image = ImageTk.PhotoImage(image)

				# if the panel is not None, we need to initialize it
				if self.panel is None:
					self.panel = tki.Label(image=image)
					self.panel.image = image
					self.panel.pack(side="left", padx=10, pady=10)

				# otherwise, simply update the panel
				else:
					self.panel.configure(image=image)
					self.panel.image = image

		except RuntimeError:
			print("[INFO] caught a RuntimeError")

	def takeSnapshot(self):
		self.stopRunVideo.put(1)
		threading.Thread(target=self.recognition_multiple_new, args=(self.frame.copy(), None, self.isForCheckingAttendance, self.stopRunVideo, self.isError)).start()
	def onClose(self):
		# set the stop event, cleanup the camera, and allow the rest of
		# the quit process to continue
		print("[INFO] closing window...")
		# self.stopEvent.set()
		self.root.quit()

	def 	recognition_multiple_new(self, image, boxes, isForCheckingAttendance, isCheckAttendanceDoneQueue, isErrorQueue):
		startTime = datetime.datetime.now()
		if boxes == None:
			boxes = my_face_detection.face_locations(image)
		resultFull = self.pool.starmap(my_service.get_label_after_detect_multiple,
								  [(copy.deepcopy(image), [copy.deepcopy(box)]) for box in boxes])
		results = [result[0] for result in resultFull]
		print(results)
		print(datetime.datetime.now() - startTime)
		if isForCheckingAttendance:
			codes = []
			unknowns = []
			for result in results:
				(box, name, proba) = result
				if name == "unknown":
					imageName = my_utils.saveImageFunction(image, box)
					unknowns.append(imageName)
				else:
					codes.append(name)
			try:
				recognition_api.recognize_multiple_faces(codes, unknowns)
			except:
				isErrorQueue.put(1)
		isCheckAttendanceDoneQueue.get()
