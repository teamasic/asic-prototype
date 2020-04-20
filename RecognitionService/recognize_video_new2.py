import argparse
import copy
import multiprocessing
import threading
import time
import tkinter as tk
from datetime import datetime
from queue import Queue

import cv2
import imutils
from PIL import Image
from PIL import ImageTk
from imutils.video import FPS

from config import my_constant
from helper import my_service, my_face_detection, stream_video, recognition_api, my_utils, boolean_wrapper
from helper.my_utils import remove_all_files


def recognition_faces(resultFull):
    results = [result[0] for result in resultFull]
    print(results)
    for result in results:
        (box, name, proba) = result
        # Show and call API
        if isForCheckingAttendance:
            if connectQueueApiError.empty() is False:
                if connectQueueApiError.get() is True:
                    if name == "unknown":
                        recognition_api.recognize_unknown_new_thread(name, copy.deepcopy(currentImage), box,
                                                                     connectQueueApiError, sessionId)
                    else:
                        recognition_api.recognize_face_new_thread(name, connectQueueApiError)
                else:
                    cannotCallApi.change(True)
                    return
            else:
                if name == "unknown":
                    recognition_api.recognize_unknown_new_thread(name, copy.deepcopy(currentImage), box,
                                                                 connectQueueApiError, sessionId)
                else:
                    recognition_api.recognize_face_new_thread(name, connectQueueApiError)

    continueDetect.change(True)


def show_frame():
    startTimeMilli = int(round(time.time() * 1000))
    while int(round(time.time() * 1000)) - startTimeMilli < durationForRecognitionMilli:
        if cannotCallApi.isTrue():
            window.quit()
        else:
            frame = vs.read()
            # Resize and cut
            cv2image = imutils.resize(frame, width=my_constant.resizeWidthShow)
            (h, w) = cv2image.shape[:2]
            cv2image = cv2image[0:h, 100:w - 100]


            # Convert to rgb image
            cv2imageBGR = cv2.cvtColor(cv2image, cv2.COLOR_BGR2RGB)

            if continueDetect.isTrue():
                global currentImage
                currentImage = copy.deepcopy(cv2image)
                boxes = my_face_detection.face_locations(cv2imageBGR)
                if 0 < len(boxes) <= maxNumOfPeople:
                    continueDetect.change(False)
                    for box in boxes:
                        (top, right, bottom, left) = box
                        cv2.rectangle(cv2imageBGR, (left, top), (right, bottom), (0, 0, 225), 2)
                        y = top - 10 if top - 10 > 10 else top + 10
                        cv2.putText(cv2imageBGR, "", (left, y), cv2.FONT_HERSHEY_SIMPLEX,
                                    0.45, (0, 0, 255), 2)
                    pool.starmap_async(my_service.get_label_after_detect_multiple,
                                       [(cv2imageBGR, [box]) for box in boxes], callback=recognition_faces)

            img = Image.fromarray(cv2imageBGR)
            imgtk = ImageTk.PhotoImage(image=img)
            fps.update()
            lbImage.configure(image=imgtk)
            lbImage.imgtk = imgtk
            time.sleep(0.05)
    window.quit()


if __name__ == "__main__":
    # get arguments
    ap = argparse.ArgumentParser()
    ap.add_argument("-p", "--rtsp", default="rtsp://192.168.1.4:8554/unicast",
                    help="path to rtsp string")
    ap.add_argument("-n", "--num", default=2,
                    help="num of maximum people to recognize image, recommend 1 for real time with normal cpu")
    ap.add_argument("-t", "--time", default=120000,
                    help="Time for recognition in video in milliseconds")
    ap.add_argument("-a", "--attendance", default=True,
                    help="Open video stream for checking attendance or not")
    ap.add_argument("-s", "--sessionId", default=1,
                    help="Session Id")
    args = vars(ap.parse_args())

    # Load arguments
    rtspString = args["rtsp"]
    maxNumOfPeople = int(args["num"])
    durationForRecognitionMilli = int(args["time"])
    isForCheckingAttendance = (str(args["attendance"]) == "True")
    sessionId = int(args["sessionId"])

    # Setup
    print("[INFO] Warming up...")
    fps = FPS()
    fps.start()
    pool = multiprocessing.Pool()
    cannotCallApi = boolean_wrapper.BooleanWrapper(False)
    continueDetect = boolean_wrapper.BooleanWrapper(True)
    connectQueueApiError = Queue()
    currentImage = None


    # Setup gui
    window = tk.Tk()
    window.wm_title("ASIC checking attendance")
    window.resizable(False, False)

    lbImage = tk.Label(window)
    lbImage.grid(row=0, column=0, padx=10, pady=5)

    btnExit = tk.Button(window, text="Exit", command=window.quit)
    btnExit.grid(row=1, column=0, padx=10, pady=5, sticky='EWNS')

    # Warm up camera
    isOpenStreamOk = False
    countTryOpenStream = 0
    timesTryConnect = 3
    vs = None
    try:
        # Try connect camera 3 times
        while countTryOpenStream < timesTryConnect and isOpenStreamOk is False:
            countTryOpenStream += 1
            if countTryOpenStream == 1:
                httpString = "http://localhost:{}".format(my_constant.portHttpStream)
            else:
                httpString = my_service.transfer_rtsp_to_http(rtspString)
            time.sleep(2.0)
            vs = stream_video.CustomVideoStream(src=httpString)
            if vs.stream.isOpened():
                isOpenStreamOk = True

        # If cannot connect -> raise exception
        if isOpenStreamOk is False:
            raise Exception("Cannot read video stream")

        # If connect OK -> continue work
        vs.start()
        threading.Thread(target=show_frame, daemon=True).start()
        window.mainloop()

        # Check if be able to call api
        if cannotCallApi.isTrue():
            raise Exception("Cannot checking attendance")
        fps.stop()
        print("FPS: ", fps.fps())

    finally:
        # Clean
        cv2.destroyAllWindows()
        vs.stop()
        pool.close()
        remove_all_files(my_constant.unknownDir)
