# import the necessary packages
import argparse
import copy
import time
from multiprocessing import Process
from queue import Queue
from threading import Thread
import multiprocessing
import cv2
import imutils
import requests
from imutils import paths
from imutils.video import FPS

from config import my_constant
from helper import stream_video, my_service, recognition_api, my_face_detection, my_utils
from helper.my_utils import remove_all_files


def recognition_multiple(image, isProcess, isForCheckingAttendance):
    print("Start recognition")
    imageResize = imutils.resize(image, width=my_constant.resizeWidthRecognize)
    (h, w) = imageResize.shape[:2]
    imageResize = imageResize[my_constant.cropTop:h - my_constant.cropBottom,
                  my_constant.cropleft: w - my_constant.cropRight]
    results = my_service.recognize_image_after_read_multiple(imageResize)
    codes = []
    unknowns = []
    for result in results:
        (box, name, proba) = result
        # Show and call API
        if isForCheckingAttendance:
            if name == "unknown":
                imageName = my_utils.saveImageFunction(image, box)
                unknowns.append(imageName)
            else:
                codes.append(name)
    print(codes)
    print(unknowns)
    print(results)
    recognition_api.recognize_multiple_faces(codes, unknowns)
    isProcess.value = 0


if __name__ == "__main__":

    ap = argparse.ArgumentParser()
    ap.add_argument("-p", "--rtsp", default="rtsp://192.168.1.4:8554/unicast",
                    help="path to rtsp string")
    ap.add_argument("-a", "--attendance", default=True,
                    help="Open video stream for checking attendance or not")
    args = vars(ap.parse_args())

    # Load arguments
    rtspString = args["rtsp"]
    isForCheckingAttendance = (str(args["attendance"]) == "True")

    # transfer rtsp to http
    httpString = my_service.transfer_rtsp_to_http(rtspString)

    # flag to handle api exeption
    isProcess = multiprocessing.Value('i', 0)

    # Queue to save unknown images
    imageUnknowns = Queue()

    # initialize the video stream, then allow the camera sensor to warm up
    print("[INFO] starting video stream...")
    vs = stream_video.CustomVideoStream(src=httpString)
    if vs.stream.isOpened() is False:
        print("Cannot read video stream")
        raise Exception("Cannot read video stream")
    else:
        # start thread save face
        # Thread(target=saveUnknownImage, args=(imageUnknowns,), daemon=True).start()

        # start the FPS throughput estimator
        vs = vs.start()
        fps = FPS().start()
        while True:
            # retrieve the frame from the threaded video stream
            image = vs.read()
            fps.update()
            cv2.imshow("Image", imutils.resize(image, width=my_constant.resizeWidthShow))
            k = cv2.waitKey(1)
            if k == ord("q"):
                break
            elif k % 256 == 32:
                if isProcess.value == 0:
                    isProcess.value = 1
                    p = Process(target=recognition_multiple, args=(image, isProcess, isForCheckingAttendance))
                    p.start()
        fps.stop()
        print("FPS: {}".format(fps.fps()))
        print("Time elapsed: {}".format(fps.elapsed()))
        cv2.destroyAllWindows()
        vs.stop()
        remove_all_files(my_constant.unknownDir)
