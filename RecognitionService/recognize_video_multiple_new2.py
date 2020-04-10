# USAGE
# python photo_booth.py --output output

# import the necessary packages
from __future__ import print_function

import multiprocessing

from config import my_constant
from helper import my_service, stream_video
from helper.my_utils import remove_all_files
from helper.video_gui_multiple import PhotoBoothApp
from imutils.video import VideoStream
import argparse
import time
if __name__ == "__main__":
	pool = multiprocessing.Pool()

	# construct the argument parse and parse the arguments
	ap = argparse.ArgumentParser()
	ap.add_argument("-p", "--rtsp", default="rtsp://192.168.1.4:8554/unicast",
					help="path to rtsp string")
	ap.add_argument("-a", "--attendance", default=False,
					help="Open video stream for checking attendance or not")
	ap.add_argument("-b", "--box", default=False,
					help="Show box in video")
	args = vars(ap.parse_args())

	# initialize the video stream and allow the camera sensor to warmup
	print("[INFO] warming up camera...")
	# Load arguments
	rtspString = args["rtsp"]
	isForCheckingAttendance = (str(args["attendance"]) == "True")
	isShowBox = (str(args["box"]) == "True")

	# transfer rtsp to http
	# httpString = my_service.transfer_rtsp_to_http(rtspString)
	httpString = "http://localhost:{}".format(my_constant.portHttpStream)
	vs = stream_video.CustomVideoStream(src=httpString)
	time.sleep(2.0)
	try:
		if vs.stream.isOpened() is False:
			print("Cannot read video stream")
			raise Exception("Cannot read video stream")
		else:
			vs.start()
			pba = PhotoBoothApp(vs, pool, isForCheckingAttendance)
			pba.root.mainloop()
			if pba.isError.empty() is False:
				raise Exception("Cannot check attendance")
	finally:
		pool.close()
		vs.stop()
		remove_all_files(my_constant.unknownDir)
