# import the necessary packages
import argparse
import copy
import multiprocessing
import time

import cv2
import imutils
from imutils.video import FPS

from config import my_constant
from helper import stream_video, my_service, recognition_api, my_face_detection, my_utils
from helper.my_utils import remove_all_files


def recognitionProcess(imageProcessQueue, isForCheckingAttendance, pRegError):
    while True:
        if imageProcessQueue.empty() is False:
            image, boxes = imageProcessQueue.get()
            if boxes == None:
                boxes = my_face_detection.face_locations(image)
            results = my_service.get_label_after_detect_multiple(image, boxes)
            print(results)
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
                    pRegError.value = 1


if __name__ == "__main__":

    ap = argparse.ArgumentParser()
    ap.add_argument("-p", "--rtsp", default="rtsp://192.168.1.4:8554/unicast",
                    help="path to rtsp string")
    ap.add_argument("-a", "--attendance", default=False,
                    help="Open video stream for checking attendance or not")
    ap.add_argument("-b", "--box", default=False,
                    help="Show box in video")
    args = vars(ap.parse_args())

    # Load arguments
    rtspString = args["rtsp"]
    isForCheckingAttendance = (str(args["attendance"]) == "True")
    isShowBox = (str(args["box"]) == "True")

    # transfer rtsp to http
    httpString = my_service.transfer_rtsp_to_http(rtspString)
    time.sleep(2)

    # Process for recognition
    imageProcessQueue = multiprocessing.Manager().Queue()
    pRegError = multiprocessing.Value("i", 0)
    pReg = multiprocessing.Process(target=recognitionProcess,
                                   args=(imageProcessQueue, isForCheckingAttendance, pRegError,))
    pReg.daemon = True
    pReg.start()

    # initialize the video stream, then allow the camera sensor to warm up
    print("[INFO] starting video stream...")
    vs = stream_video.CustomVideoStream(src=httpString)
    if vs.stream.isOpened() is False:
        print("Cannot read video stream")
        raise Exception("Cannot read video stream")
    else:
        # start the FPS throughput estimator
        vs = vs.start()
        fps = FPS().start()
        startTimeMilli = int(round(time.time() * 1000))
        while True:
            # Check sub process error
            if pRegError.value == 1:
                break
            # retrieve the frame from the threaded video stream
            image = vs.read()
            # resize and crop image
            image = imutils.resize(image, width=my_constant.resizeWidthRecognize)
            (h, w) = image.shape[:2]
            image = image[my_constant.cropTop:h - my_constant.cropBottom,
                          my_constant.cropleft: w - my_constant.cropRight]
            boxes = None
            if isShowBox:
                boxes = my_face_detection.face_locations(image)
                if len(boxes) > 0:
                    for box in boxes:
                        (top, right, bottom, left) = box
                        cv2.rectangle(image, (left, top), (right, bottom), (0, 0, 225), 2)
                        y = top - 10 if top - 10 > 10 else top + 10
                        cv2.putText(image, "", (left, y), cv2.FONT_HERSHEY_SIMPLEX,
                                    0.45, (0, 0, 255), 2)
                # show the output image
            fps.update()
            cv2.imshow("Image", image)
            k = cv2.waitKey(1)
            if k == ord("q"):
                break
            elif k % 256 == 32:
                if imageProcessQueue.empty():
                    imageProcessQueue.put((image, boxes))
                    print("Snapshot saved")
        fps.stop()
        print("FPS: {}".format(fps.fps()))
        print("Time elapsed: {}".format(fps.elapsed()))
        cv2.destroyAllWindows()
        vs.stop()
        remove_all_files(my_constant.unknownDir)
        if pRegError.value == 1:
            raise Exception("Cannot check attendance")
    print("Terminating...")
