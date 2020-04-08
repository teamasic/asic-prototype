# import the necessary packages
import argparse
import copy
import multiprocessing
import time

import cv2
import imutils
from imutils.video import FPS

from config import my_constant
from helper import stream_video, my_service, recognition_api, my_face_detection
from helper.my_utils import remove_all_files


def recognitionProcess(imageProcessQueue, isForCheckingAttendance, pRegError):
    connectQueueApiError = multiprocessing.Queue()
    while True:
        if imageProcessQueue.empty() is False:
            image, boxes = imageProcessQueue.get()
            results = my_service.get_label_after_detect_multiple(image, boxes)
            print(results)
            for result in results:
                (box, name, proba) = result
                # Show and call API
                if isForCheckingAttendance:
                    if connectQueueApiError.empty() is False:
                        if connectQueueApiError.get() is True:
                            if name == "unknown":
                                recognition_api.recognize_unknown_new_thread(name, copy.deepcopy(image), box,
                                                                             connectQueueApiError)
                            else:
                                recognition_api.recognize_face_new_thread(name, connectQueueApiError)
                        else:
                            pRegError.value = 1
                            return
                    else:
                        recognition_api.recognize_face_new_thread(name, connectQueueApiError)


if __name__ == "__main__":

    ap = argparse.ArgumentParser()
    ap.add_argument("-p", "--rtsp", default="rtsp://192.168.1.4:8554/unicast",
                    help="path to rtsp string")
    ap.add_argument("-n", "--num", default=2,
                    help="num of maximum people to recognize image, recommend 1 for real time with normal cpu")
    ap.add_argument("-t", "--time", default=120000,
                    help="Time for recognition in video in milliseconds")
    ap.add_argument("-a", "--attendance", default=True,
                    help="Open video stream for checking attendance or not")
    args = vars(ap.parse_args())

    # Load arguments
    rtspString = args["rtsp"]
    maxNumOfPeople = int(args["num"])
    durationForRecognitionMilli = int(args["time"])
    isForCheckingAttendance = (str(args["attendance"]) == "True")

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
        while int(round(time.time() * 1000)) - startTimeMilli < durationForRecognitionMilli:
            # Check sub process error
            if pRegError.value == 1:
                break
            # retrieve the frame from the threaded video stream
            image = vs.read()
            image = imutils.resize(image, width=600)
            (h, w) = image.shape[:2]
            image = image[0:h, 100:w - 100]
            if imageProcessQueue.empty():
                boxes = my_face_detection.face_locations(image)
                if 0 < len(boxes) <= maxNumOfPeople:
                    for box in boxes:
                        (top, right, bottom, left) = box
                        cv2.rectangle(image, (left, top), (right, bottom), (0, 0, 225), 2)
                        y = top - 10 if top - 10 > 10 else top + 10
                        cv2.putText(image, "", (left, y), cv2.FONT_HERSHEY_SIMPLEX,
                                    0.45, (0, 0, 255), 2)
                    imageProcessQueue.put((image, boxes))
                # show the output image
            fps.update()
            cv2.imshow("Image", image)
            k = cv2.waitKey(100)
            if k == ord("q"):
                break
        fps.stop()
        print("FPS: {}".format(fps.fps()))
        print("Time elapsed: {}".format(fps.elapsed()))
        cv2.destroyAllWindows()
        vs.stop()
        remove_all_files(my_constant.unknownDir)
        if pRegError.value == 1:
            raise Exception("Cannot check attendance")
    print("Terminating...")
